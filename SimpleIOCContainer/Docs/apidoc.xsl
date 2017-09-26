<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:output method="html" encoding="utf-8" indent="yes" xml:space="default"/>

    <xsl:template match="/doc">
        <h:html xmlns:h="http://www.w3.org/1999/xhtml">
            <header>
                <meta charset="utf-8"/>
                <meta http-equiv="cache-control" content="max-age=0"/>
                <meta http-equiv="cache-control" content="no-cache"/>
                <meta http-equiv="expires" content="0"/>
                <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT"/>
                <meta http-equiv="pragma" content="no-cache"/>
                <title>SimpleIOCContainer API</title>
                <link type="text/png" rel="icon" href="/glyphicons_free/glyphicons/png/glyphicons-30-notes-2.png"/>
              <!--<link type="text/css" href="/Volumes/[C] Windows 10/projects/SimpleIOCContainer/Simple/Content/bootstrap-3.3.7/css/bootstrap.css" rel="stylesheet"/>-->
              <link type="text/css" href="/projects/SimpleIOCContainer/Simple/Content/bootstrap-3.3.7/css/bootstrap.css" rel="stylesheet"/>
            </header>
            <body>
                <div class="container">
                    <div class="col-sm-1"/>
                    <div class="col-sm-10">
                        <h1>SimpleIOCContainer</h1>
                        <xsl:apply-templates/>
                    </div>
                    <div class="col-sm-1"/>
                </div>
            </body>
        </h:html>
    </xsl:template>
    <xsl:template match="assembly">
        <div class="row">
            <div class="col-sm-4">
                Assembly:
            </div>
            <div class="col-sm-8">
                <xsl:value-of select="name"/>
            </div>
        </div>
    </xsl:template>
    <xsl:template match="member">
        <div class="row">
            <div class="col-sm-1"/>
            <div class="col-sm-11">
                <xsl:value-of select="@name"/><br/>
                <xsl:apply-templates/>
            </div>
        </div>
    </xsl:template>
    <xsl:template match="summary">
        <div class="row">
            <div class="col-sm-2"/>
            <div class="col-sm-8">
                <xsl:value-of select="."/>
            </div>
        </div>
    </xsl:template>
    <xsl:template match="param">
        <div class="row">
            <div class="col-sm-4">
                <xsl:value-of select="@name"/>
            </div>
            <div class="col-sm-8">
                <xsl:value-of select="."/>
            </div>
        </div>
    </xsl:template>
    <xsl:template match="text()"/>
</xsl:stylesheet>
