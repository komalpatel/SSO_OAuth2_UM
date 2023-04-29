<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:msxsl="urn:schemas-microsoft-com:xslt"
				version="1.0">
	<!--xmlns:extension="urn:extension"-->
	<xsl:output method="xml" indent="yes"/>
	<xsl:decimal-format name="de" decimal-separator="," grouping-separator="." />
	<xsl:variable name="lightFont" select="'Open Sans'"/>
	<xsl:variable name="boldFont" select="'Open Sans'"/>
	<xsl:variable name="boldWeight" select="'bold'"/>
	<xsl:variable name="regularFont" select="'Open Sans'"/>
	<xsl:variable name="regularWeight" select="'light'"/>
	<xsl:variable name="fontSize" select="'12pt'"/>
	<xsl:variable name="fontSizeSmall" select="'9pt'"/>
	<xsl:variable name="accentColor" select="'#333333'"/>

	<xsl:param name="textColor" select="'#000000'"></xsl:param>
	<xsl:param name="scheme" select="'w'"></xsl:param>


	<xsl:template match="/">
		<root>
			<lightFont>
				<xsl:value-of select="$lightFont"/>
			</lightFont>
			<boldFont>
				<xsl:value-of select="$boldFont"/>
			</boldFont>
			<boldWeight>
				<xsl:value-of select="$boldWeight"/>
			</boldWeight>
			<regularFont>
				<xsl:value-of select="$regularFont"/>
			</regularFont>
			<regularFont>
				<xsl:value-of select="$regularFont"/>
			</regularFont>
			<accentColor>
				<xsl:value-of select="$accentColor"/>
			</accentColor>
			<fontSize>
				<xsl:value-of select="$fontSize"/>
			</fontSize>
			<!--<footerHeight>40pt</footerHeight>-->
			<partner>
				<header>
					<!--<p  backgroundcolor="white"  text-align="end" padding-bottom="-1cm">
						<img src="Logo-Charge-at-Friends.png" content-height="2.0in" content-width="4.22in" scaling="non-uniform" height="0.60in" backgroundcolor="white" />
					</p>-->
					<p text-align="center" padding-top="0.1cm" font-size="10pt" font-family="{$boldFont}" font-weight="800" color="{$textColor}">
						Charge@Friends
					</p>
				</header>


				<body font-family="{$lightFont}" font-size="{$fontSize}" text-align="left">
					<page>
						<table left="2.0cm" padding-top="0.1cm" padding-left="2cm" padding-bottom="0.0cm" font-family="{$lightFont}">
							<col width="1.2cm"/>
							<col width="3.2cm"/>
							<col width="3.5cm"/>
							<tbody>
								<tr>
									<td></td>
									<td>
										<img src="sticker_{$scheme}@1024px.png" content-height="3.2cm" content-width="3.2cm" scaling="non-uniform" height="2.4cm" />
									</td>
									<td>
										<img src="{//ChargePoint/Guid}.png" content-height="3.2cm" content-width="3.2cm" scaling="non-uniform" height="2.4cm"  />
									</td>
								</tr>

							</tbody>
						</table>
						<p padding-top="0.1cm" padding-bottom="0.0cm" text-align="center" font-size="5pt" font-weight="600" color="{$textColor}">
							<xsl:value-of select="//ChargePoint/Country"/>*<xsl:value-of select="//ChargePoint/ZipCode"/>*<xsl:value-of select="//ChargePoint/City"/>*<xsl:value-of select="//ChargePoint/Street"/>*<xsl:value-of select="//ChargePoint/StreetNumber"/>*<xsl:value-of select="//ChargePoint/CpName"/>*<xsl:value-of select="//ChargePoint/Name"/>
						</p>

						<!--<p padding-bottom="0.8cm">

						</p>
						<p padding-bottom="0.8cm" align="center">
							Datum: <xsl:value-of select="extension:FormatDate(//Booking/EndTime)"/>
						</p>
						<p padding-bottom="0.8cm" align="center">
							Uhrzeit: <xsl:value-of select="extension:FormatTime(//Booking/EndTime)"/>
						</p>
						<p padding-bottom="2cm" align="center">
							Transaktions-Nr.: <xsl:value-of select="format-number(number(//Customer/OperatorId),'0000','de')"/>-<xsl:value-of select="format-number(number(//Booking/InvoiceId),'0000','de')"/>
						</p>-->
						<!--<p font-size="8pt" text-align="center" left="2cm">
							<xsl:if test="//Customer/IsSmallBusiness='true'">
								<span font-size="$fontSizeSmall">* gemäß Kleinunternehmerregelung §19 Abs. 1 UstG</span>
							</xsl:if>
						</p>-->
					</page>
				</body>

			</partner>
		</root>
	</xsl:template>


</xsl:stylesheet>