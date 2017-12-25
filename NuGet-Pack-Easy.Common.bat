@echo off
set releaseVersion=%1

dotnet restore .\Easy.Common
dotnet pack .\Easy.Common\Easy.Common.csproj -o ..\nupkgs -c Release /p:Version=%releaseVersion% --include-symbols --include-source
