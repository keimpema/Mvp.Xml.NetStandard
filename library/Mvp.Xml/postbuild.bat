set targetDir=%~1
set targetName=%~2

echo TARGETDIR=%targetDir%
echo TARGETNAME=%targetName%

cd "%targetDir%"
<NUL set /p="PWD="
cd

set root=..\..\..\..\..
echo ROOT=%root%

set tools=%root%\artifacts\buildtools
echo TOOLSDIR=%tools%

echo INIT DEV ENV
rem source: https://renenyffenegger.ch/notes/Windows/development/Visual-Studio/environment-variables/index
for /f "usebackq delims=#" %%a in (`"%programfiles(x86)%\Microsoft Visual Studio\Installer\vswhere" -latest -property installationPath`) do call "%%a\Common7\Tools\VsDevCmd.bat"

echo ILDASM
"%tools%\ildasm" %targetName%.dll /out=%targetName%.il /linenum

echo RENAME METHODS
dotnet "%tools%\MethodRenamer.dll" RenameMappings.json %targetName%.il %targetName%.fixed.il

del %targetName%.dll 
rem del %targetName%.pdb

echo ILASM
rem need to specify /pdb but it does not yet create a pdb, so skip for now
rem probably related to https://github.com/dotnet/coreclr/issues/15299
"%tools%\ilasm" /KEY=%root%/mvp-xml-public.snk %targetName%.fixed.il /OUTPUT=%targetName%.dll /dll

echo SIGN
sn -R %targetName%.dll %root%/mvp-xml-keypair.snk

del %targetName%.il %targetName%.fixed.il del %targetName%.res *.resources
