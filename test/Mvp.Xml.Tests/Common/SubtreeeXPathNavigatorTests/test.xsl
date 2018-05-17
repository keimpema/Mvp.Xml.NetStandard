<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output indent="yes"/>
	<xsl:template match="/">
		<!-- <xsl:copy-of select="." /> -->
		<xsl:apply-templates select="//name" />
		<!-- <xsl:apply-templates /> -->
	</xsl:template>

    <xsl:template match="name">
		<person><xsl:value-of select="." /></person>
Out of scope ancestor: <xsl:copy-of select="../../../../*[1]" /><xsl:text>
</xsl:text>
    </xsl:template>
</xsl:stylesheet>
