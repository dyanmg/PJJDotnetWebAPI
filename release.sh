#!/usr/bin/env sh

rm -rf publish
dotnet clean Day1WebApi
dotnet build -c Release Day1WebApi
dotnet publish -c Release -o publish Day1WebApi
dotnet ef migrations script -o publish/migrations.sql --project Day1WebApi --configuration Release
# dotnet ef migrations bundle --self-contained -o ./publish/efbundle --project Day1WebApi --configuration Release
