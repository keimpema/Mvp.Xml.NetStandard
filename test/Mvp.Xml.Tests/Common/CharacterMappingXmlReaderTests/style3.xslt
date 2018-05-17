<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:nxslt="http://www.xmllab.net/nxslt">
    <nxslt:output use-character-maps="testmap"/>
    <nxslt:character-map name="testmap" use-character-maps="testmap2">
        <nxslt:output-character character="&#160;" string="&amp;nbsp;" />
    </nxslt:character-map>
    <nxslt:character-map name="testmap2">
        <nxslt:output-character character="&#161;" string="161" />
        <nxslt:output-character character="&#162;" string="162" />
    </nxslt:character-map>
</xsl:stylesheet>