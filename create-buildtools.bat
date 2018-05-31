@echo off

rem set working folder
cd %~dp0src\mvp.xml.methodrenamer

call dotnet restore
if not "%errorlevel%"=="0" goto failure

call dotnet publish --configuration Release --framework netcoreapp2.0 -r win-x64 --no-restore --output ..\..\artifacts\buildtools
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1
