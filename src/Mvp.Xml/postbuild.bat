set targetDir=%~1
set targetName=%~2

echo TARGETDIR=%targetDir%
echo TARGETNAME=%targetName%

cd "%targetDir%"
<NUL set /p="PWD="
cd

set tools=..\..\..\..\..\artifacts\buildtools
echo TOOLSDIR=%tools%

"%tools%\ildasm" %targetName%.dll /out=%targetName%.il /linenum

dotnet "%tools%\MethodRenamer.dll" RenameMappings.json %targetName%.il %targetName%.fixed.il

del %targetName%.dll 
rem del %targetName%.pdb

rem need to specify /pdb but it does not yet create a pdb, so skip for now
rem probably related to https://github.com/dotnet/coreclr/issues/15299
"%tools%\ilasm" %targetName%.fixed.il /OUTPUT=%targetName%.dll /dll

del %targetName%.il %targetName%.fixed.il del %targetName%.res *.resources
