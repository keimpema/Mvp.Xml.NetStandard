all:            
	@echo off
        @echo ########### Builds Exslt library...
        cd ..\Exslt
        nmake
        @echo ########### Builds ExsltXPathTest utility...
        cd ..\ExsltXPathTest
        csc /r:..\Exslt\GotDotNet.Exslt.dll /out:ExsltXPathTest.exe /target:exe *.cs /debug- /optimize+
        copy ..\Exslt\GotDotNet.Exslt.dll GotDotNet.Exslt.dll