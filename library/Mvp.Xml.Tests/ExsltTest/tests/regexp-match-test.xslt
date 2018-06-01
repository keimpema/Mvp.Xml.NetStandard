<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:regexp="http://exslt.org/regular-expressions">   
  <xsl:template match="/">
--------------------------------------------------------------------

  Test 1, expected results:
  
  Part 1 = http://www.bayes.co.uk/xml/index.xml?/xml/utils/rechecker.xml
  Part 2 = http
  Part 3 = www.bayes.co.uk
  Part 4 =
  Part 5 = /xml/index.xml?/xml/utils/rechecker.xml
  
  Results:
  <xsl:for-each select="regexp:match('http://www.bayes.co.uk/xml/index.xml?/xml/utils/rechecker.xml', 
                                   '(\w+):\/\/([^/:]+)(:\d*)?([^# ]*)')">
  Part <xsl:value-of select="position()" /> = <xsl:value-of select="." />
  </xsl:for-each>
--------------------------------------------------------------------
  Test 2, expected results:
  
  Part 1 = This
  Part 2 = is
  Part 3 = a
  Part 4 = test
  Part 5 = string
  
  Results:
  <xsl:for-each select="regexp:match('This is a test string', '(\w+)', 'g')">
  Part <xsl:value-of select="position()" /> = <xsl:value-of select="." />
</xsl:for-each>
--------------------------------------------------------------------
  Test 3, expected results:

  Part 1 = his
  Part 2 = is
  Part 3 = a
  Part 4 = test
  
  Results:
  <xsl:for-each select="regexp:match('This is a test string', '([a-z])+ ', 'g')">
  Part <xsl:value-of select="position()" /> = <xsl:value-of select="." />
</xsl:for-each>
--------------------------------------------------------------------
  Test 4, expected results:
  
  Part 1 = This
  Part 2 = is
  Part 3 = a
  Part 4 = test
  
  Results:
  <xsl:for-each select="regexp:match('This is a test string', '([a-z])+ ', 'gi')">
  Part <xsl:value-of select="position()" /> = <xsl:value-of select="." />         
</xsl:for-each>
--------------------------------------------------------------------
  Test 5, expected results:
  
  Part 1 = 22/12/2003    21:00    AcmeService    DB updated
  Part 2 = 22/12/2003
  Part 3 = 21:00
  Part 4 = AcmeService
  Part 5 = DB updated

  Results:
  <xsl:for-each select="regexp:match
     ('22/12/2003    21:00    AcmeService    DB updated', '(\d{1,2}/\d{1,2}/\d{4})\s+(\d{2}:\d{2})\s+(\w*)\s+(.*)')">
  Part <xsl:value-of select="position()" /> = <xsl:value-of select="." />         
</xsl:for-each>
  </xsl:template>
</xsl:stylesheet> 