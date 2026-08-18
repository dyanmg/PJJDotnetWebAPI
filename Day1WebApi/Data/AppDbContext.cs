using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;

namespace Day1WebApi.Data
{
    public class AppDbContext : IdentityDbContext<Pegawai>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccesor) : base(options)
        {
            SavingChanges += SavingChangesEvent;
            SavedChanges += SavedChangesEvent;
            _httpContextAccessor = httpContextAccesor;
        }

        public DbSet<Aset> Aset => Set<Aset>();
        public DbSet<Kategori> Kategori => Set<Kategori>();
        public DbSet<Pegawai> Pegawai => Set<Pegawai>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Aset>(e =>
            {
                e.ToTable("aset");
                e.Property(x => x.Nama).IsRequired().HasMaxLength(120);
                e.Property(x => x.Nilai).IsRequired().HasColumnName("nilai");
                e.HasQueryFilter(x => x.DeletedAt == null);
            });

            modelBuilder.Entity<Kategori>(e =>
            {
                e.HasMany(x => x.Aset).WithOne(x => x.Kategori).HasForeignKey(x => x.KategoriId);
            });
            modelBuilder.Entity<Pegawai>(e =>
            {
                e.HasQueryFilter(x => x.DeletedAt == null);
            });
            base.OnModelCreating(modelBuilder);
        }
        
        private void SavingChangesEvent(object? sender, SavingChangesEventArgs args)
        {
            foreach(var entry in ChangeTracker.Entries())
            {
                if(entry.Entity is BaseModel model)
                {       
                    if(entry.State == EntityState.Deleted)
                    {
                        model.DeletedAt = DateTime.Now;
                        entry.State = EntityState.Modified;
                        foreach (var property in Entry(entry.Entity).Properties)
                        {
                            if (property.Metadata.Name != "DeletedAt")
                            {
                                property.IsModified = false;
                            }
                        }
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        model.UpdatedAt = DateTime.Now;
                        // Di isi nama user yang melakukan update dari token
                        model.UpdateBy = _httpContextAccessor.HttpContext?.User?.FindFirst("nama")?.Value ?? "Unknown";
                    }
                   
                }

                if (entry.Entity is BaseIdentityModel identityModel)
                {       
                    if(entry.State == EntityState.Deleted)
                    {
                        identityModel.DeletedAt = DateTime.Now;
                        entry.State = EntityState.Modified;
                        foreach (var property in Entry(entry.Entity).Properties)
                        {
                            if (property.Metadata.Name != "DeletedAt")
                            {
                                property.IsModified = false;
                            }
                        }
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        identityModel.UpdatedAt = DateTime.Now;
                        // Di isi nama user yang melakukan update dari token
                        identityModel.UpdateBy = _httpContextAccessor.HttpContext?.User?.FindFirst("nama")?.Value ?? "Unknown";
                    }
                   
                }
            }
        }


        private void SavedChangesEvent(object? sender, SavedChangesEventArgs args)
        {
            Console.WriteLine("setelah data disimpan");
        }


        public void PartialUpdate<TParamEntity, TDestEntity>(TParamEntity source, TDestEntity entity)
        {
            var reflection = typeof(TParamEntity);

            foreach (var property in Entry(entity).Properties)
            {

                var sourceValue = reflection.GetProperty(property.Metadata.Name)?.GetValue(source);
                if(property.Metadata.Name == "UpdatedAt")
                {
                    property.IsModified = true;
                    continue;
                }
                if (sourceValue == null)
                {
                    property.IsModified = false;
                }
                else
                {
                    property.CurrentValue = sourceValue;
                }
            }
        }
    }
}
