<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:exsl="http://exslt.org/common" 
xmlns:date="http://exslt.org/dates-and-times"
xmlns:math="http://exslt.org/math"
xmlns:random="http://exslt.org/random"
xmlns:regexp="http://exslt.org/regular-expressions"
xmlns:set="http://exslt.org/sets"
xmlns:str="http://exslt.org/strings"
xmlns:date2="http://gotdotnet.com/exslt/dates-and-times"
xmlns:math2="http://gotdotnet.com/exslt/math"
xmlns:regexp2="http://gotdotnet.com/exslt/regular-expressions"
xmlns:set2="http://gotdotnet.com/exslt/sets"
xmlns:str2="http://gotdotnet.com/exslt/strings"
xmlns:dyn2="http://gotdotnet.com/exslt/dynamic"
exclude-result-prefixes="exsl date math regexp set str date2 math2 regexp2 set2 random dyn2">
    <xsl:template match="/">
        <html>
            <head>
                <title>EXSLT.NET regression test - functions availability</title>
            </head>
            <body>                        
                <h3>EXSLT.NET regression test - functions availability</h3>
                <xsl:apply-templates select="document('../../../doc/Functions.xml')/*/module"/>
            </body>
        </html>      
    </xsl:template>
    <xsl:template match="module">
        <p>
        <h4><xsl:if test="@is-exslt-module='yes'">EXSLT: </xsl:if>
        <xsl:value-of select="@name"/></h4>
        <table border="1" width="100%">
            <tr>
                <th>Function name</th>
                <th>Availability</th>
            </tr>
            <xsl:apply-templates select="function"/>
        </table>
        </p>
    </xsl:template>
    <xsl:template match="function">
        <tr>
            <xsl:variable name="fn" select="concat(../@prefix, ':', @name)"/>
            <td><xsl:value-of select="$fn"/></td>
            <td align="center">
                <xsl:choose>
                    <xsl:when test="function-available($fn)">
                        <span style="color:green">Ok</span>
                    </xsl:when>
                    <xsl:otherwise>
                        <span style="color:red">Not available</span>
                    </xsl:otherwise>
                </xsl:choose>
            </td>
        </tr>
    </xsl:template>
</xsl:stylesheet>  