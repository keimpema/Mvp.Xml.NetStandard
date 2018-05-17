<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:exsl="http://exslt.org/common" xmlns:str="http://exslt.org/strings" exclude-result-prefixes="exsl str">
  <xsl:template match="testsuite">
    <!--<xsl:apply-templates select="testcases">
        <xsl:with-param name="streaming" select="false()"/>
    </xsl:apply-templates> -->
    <xsl:apply-templates select="testcases">
        <xsl:with-param name="streaming" select="true()"/>
    </xsl:apply-templates>
  </xsl:template> 
  <xsl:template match="testcases">
    <xsl:param name="streaming" select="false()"/>
    <xsl:variable name="creator" select="translate(@creator, ' ,', '_')"/>
    <xsl:variable name="filename">
        <xsl:value-of select="$creator"/>
        <xsl:text>Tests</xsl:text>
        <xsl:if test="not($streaming)">_NS</xsl:if>        
    </xsl:variable>
    <exsl:document href="{$filename}.cs" method="text" encoding="UTF-8">using System;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;

using Mvp.Xml.XInclude;
using NUnit.Framework;

namespace Mvp.Xml.XInclude.Test
{
	/// &lt;summary>
	/// Edinburgh University test cases from the XInclude Test suite.
	/// &lt;/summary>
	[TestFixture]
	public class <xsl:value-of select="$filename"/>
	{
		public <xsl:value-of select="$filename"/>()
		{
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Error));
		}

        /// &lt;summary>
        /// Utility method for running tests.
        /// &lt;/summary>        
        public static void RunAndCompare(string source, string result) 
        {
            XIncludeReaderTests.RunAndCompare(
                "../../XInclude-Test-Suite/<xsl:value-of select="@basedir"/>/" + source, 
                "../../XInclude-Test-Suite/<xsl:value-of select="@basedir"/>/" + result<!--, false, <xsl:choose><xsl:when test="$streaming">true</xsl:when><xsl:otherwise>false</xsl:otherwise></xsl:choose>-->);
        }
        <xsl:apply-templates/>
	}
}
</exsl:document>
  </xsl:template>
  <xsl:template match="testcase">
        /// &lt;summary><xsl:for-each select="str:split(description, '&#xA;')">
        /// <xsl:value-of select="normalize-space(.)"/></xsl:for-each>        
        /// &lt;/summary>
        [Test]<xsl:if test="@type='error'">
        [ExpectedException(typeof(Exception))]</xsl:if>
        public void <xsl:value-of select="translate(@id, '-', '_')"/>() 
        {
            RunAndCompare("<xsl:value-of select="@href"/>", "<xsl:value-of select="output"/>");            
        }  
  </xsl:template>
</xsl:stylesheet>

  