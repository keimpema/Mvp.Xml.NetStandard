<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output encoding="utf-8"/>
    <xsl:param name="prm1" select="'default value'"/>
    <xsl:template match="/">
        <out xmlns="foo" bar="" baz="a&amp;b" xmlns:foo="http://schemas.microsoft.com/xsd/catalog">
            <xsl:comment> @#$$ comment</xsl:comment>
            <xsl:processing-instruction name="pi">ghghghgh gh gh"" ''</xsl:processing-instruction>
            <prm1>
              <xsl:value-of select="$prm1" />
            </prm1>
            <xsl:copy-of select="/" />
        </out>
    </xsl:template>
</xsl:stylesheet>