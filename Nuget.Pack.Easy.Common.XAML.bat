@echo off
set version=%1

dotnet restore .\Easy.Common.XAML
dotnet pack .\Easy.Common.XAML\Easy.Common.XAML.csproj -o .\nupkgs -c Release /p:PackageVersion=%version% --include-symbols --include-source
