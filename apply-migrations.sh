#!/usr/bin/env sh

cd publish
# export ASPNETCORE_ENVIRONMENT=Production
# ./efbundle
sqlite3 Day1WebApi-release.db < migrations.sql
