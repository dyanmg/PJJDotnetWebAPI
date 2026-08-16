$env:ASPNETCORE_ENVIRONMENT="Staging"
$env:DisableGlobalAuthorize="true"
Set-Location ./publish
dotnet ./Day1WebApi.dll
