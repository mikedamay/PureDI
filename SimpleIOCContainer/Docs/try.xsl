<?xml version="1.0" encoding="us-ascii" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output encoding="us-ascii" indent="yes"
    />
    <!-- this is bad.  It will only show the first child -->
    <xsl:template match="//*">
        <xsl:value-of select="mychild"/>
    </xsl:template>
</xsl:stylesheet>