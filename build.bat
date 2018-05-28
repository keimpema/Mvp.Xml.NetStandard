@echo Off

set config=%1
if "%config%" == "" (
    set config=Release
)

set version=
if not "%BuildCounter%" == "" (
   set packversionsuffix=--version-suffix ci-%BuildCounter%
)

rem cd src

rem Restore
call dotnet restore
if not "%errorlevel%"=="0" goto failure

rem build all
call dotnet build --configuration %config% --no-restore
if not "%errorlevel%"=="0" goto failure

rem cd test

rem test
call dotnet test test\mvp.xml.tests\mvp.xml.tests.csproj --configuration %config% --no-build --no-restore --verbosity=detailed

rem package
mkdir artifacts
call dotnet pack src\mvp.xml\mvp.xml.csproj --configuration %config% %packversionsuffix% --output ..\..\artifacts --no-build --no-restore
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1
