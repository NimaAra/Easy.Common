@echo off
set version=%1

dotnet restore .\Easy.Common
dotnet pack .\Easy.Common\Easy.Common.csproj -o ..\nupkgs -c Release /p:PackageVersion=%version% --include-symbols --include-source
