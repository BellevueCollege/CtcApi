@echo off
MSBUILD.exe /t:Build,Package /p:Configuration=Release CtcApi.csproj