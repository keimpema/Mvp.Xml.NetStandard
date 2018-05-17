<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:nxslt="http://www.xmllab.net/nxslt">
    <nxslt:output use-character-maps="testmap"/>
    <nxslt:character-map name="testmap">
        <nxslt:output-character character="&#160;" string="&amp;nbsp;" />
    </nxslt:character-map>
    <xsl:template match="/">
        <out>data</out>
    </xsl:template>
</xsl:stylesheet>