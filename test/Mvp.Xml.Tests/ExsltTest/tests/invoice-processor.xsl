<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:exsl="http://exslt.org/common" exclude-result-prefixes="exsl">
    <xsl:template match="/">
        <!-- Main result document - confirmation -->
        <html>
            <head>
                <title>Thank you for purchasing!</title>
            </head>
            <body>
                <h2>Thank you for purchasing at fabrikam.com!</h2>
            </body>                        
            <xsl:apply-templates mode="order"/>
        </html>
    </xsl:template>
    <xsl:template match="invoice" mode="order">
        <!-- Additional result document - SOAP message for fabrikam.com
        order processing web service -->
        <exsl:document href="soap/order.xml" indent="yes">
            <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
                <soap:Body>                    
                    <ns:Order xmlns:ns="urn:fabrikam-com:orders">
                        <xsl:apply-templates mode="order"/>
                    </ns:Order>
                </soap:Body>
            </soap:Envelope>
        </exsl:document>
    </xsl:template>
    <xsl:template match="item" mode="order">
        <xsl:copy-of select="."/>
    </xsl:template>
</xsl:stylesheet>
