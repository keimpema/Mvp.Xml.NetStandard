@echo Off

set config=%1
if "%config%" == "" (
    set config=Release
)

set version=
if not "%BuildCounter%" == "" (
   set packversionsuffix=--version-suffix ci-%BuildCounter%
)

rem set working folder
set home=%~dp0
set home=%home:~0,-1%
echo HOME=%home%
cd %home%

rem restore
call dotnet restore

rem build/publish methodrenamer and get platform specific versions of ildasm and ilasm
cd %home%\src\mvp.xml.methodrenamer
if not "%errorlevel%"=="0" goto failure
dotnet publish --configuration %config% --framework netcoreapp2.0 -r win-x64 --no-restore --output %home%\artifacts\buildtools
if not "%errorlevel%"=="0" goto failure

rem build library and run unit tests
cd %home%\src\mvp.xml
if not "%errorlevel%"=="0" goto failure
call dotnet build --configuration %config% --no-restore
if not "%errorlevel%"=="0" goto failure
call dotnet test %home%\test\mvp.xml.tests\mvp.xml.tests.csproj --configuration %config% --no-restore --verbosity=normal
if not "%errorlevel%"=="0" goto failure

rem create nuget package in artifacts folder
call dotnet pack mvp.xml.csproj --configuration %config% %packversionsuffix% --output %home%\artifacts --no-build --no-restore
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1
