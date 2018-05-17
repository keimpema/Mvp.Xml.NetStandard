<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:nxslt="http://www.xmllab.net/nxslt" exclude-result-prefixes="nxslt">
    <xsl:output encoding="iso-8859-1" omit-xml-declaration="yes" />
    <nxslt:output use-character-maps="testmap"/>
    <nxslt:character-map name="testmap" use-character-maps="testmap2">
        <nxslt:output-character character="&#160;" string="&amp;nbsp;" />
    </nxslt:character-map>
    <nxslt:character-map name="testmap2">
        <nxslt:output-character character="&#161;" string="161" />
        <nxslt:output-character character="&#162;" string="162" />
        <nxslt:output-character character="{" string="&lt;" />
        <nxslt:output-character character="}" string="&gt;" />
    </nxslt:character-map>
    <xsl:template match="/">
        <out attr="a&#xA0;b">
            <text>Some&#xA0;text, now ASP.NET {%# Eval("foo") %} and more&#xA0;text.</text>
            <xsl:copy-of select="//foo"/>
        </out>
    </xsl:template>
</xsl:stylesheet>