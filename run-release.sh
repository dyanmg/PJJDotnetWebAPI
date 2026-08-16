#!/usr/bin/env sh

# export ASPNETCORE_URLS=http://*:8080
# export ASPNETCORE_HTTP_PORTS=8080
export ASPNETCORE_ENVIRONMENT=Staging
# export DisableGlobalAuthorize=true
cd ./publish
dotnet ./Day1WebApi.dll
