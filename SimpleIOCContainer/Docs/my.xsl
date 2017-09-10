<?xml version="1.0" encoding="ascii" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output encoding="ascii" indent="yes" method="xml"/>
 <xsl:template match="/">
     <xsl:value-of select="diagnosticSchema/group/causeCode[text() = 'MissingBean']/following-sibling::intro"/>
     <xsl:text>&#10;&#13;</xsl:text>
 </xsl:template>
 <xsl:template match="text()"/>
</xsl:stylesheet>