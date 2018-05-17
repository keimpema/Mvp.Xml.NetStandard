<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:math="http://exslt.org/math" exclude-result-prefixes="math">
  <xsl:output omit-xml-declaration="yes" encoding="ascii"/>
  <xsl:template match="/">
    <out><xsl:value-of select="math:sqrt(9)"/></out>
  </xsl:template>
</xsl:stylesheet>