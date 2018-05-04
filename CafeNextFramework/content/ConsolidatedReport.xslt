<xsl:stylesheet version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <head>
        <meta charset="utf-8" />
        <meta http-equiv="Content- Type" content="text / html; charset = UTF-8" />
        <style>
          body {
          background-color: white;
          }

          table {
          background-color: #DCDCDC;
          text-align: center;
          border-style : none;
          }

          th {
          background-color: #dcd470;
          text-align: center;
          color: black;
          font-family: Candara, Calibri, Segoe, 'Segoe UI', Optima, Arial, sans-serif;
          font-size: 17px;
          }

          tr {
          background-color: #E6E6E6;
          color: #1F1F7A;
          font-family: Candara, Calibri, Segoe, 'Segoe UI', Optima, Arial, sans-serif;
          font-size: 15px;
          }

          tr.d0 td {
          background-color: #ace7f9;
          }

          h1 {
          align =center;
          text-align: center;
          color: #003399;
          font-family: Candara, Calibri, Segoe, 'Segoe UI', Optima, Arial, sans-serif;
          font-size: 25px;
          }

          h4 {
          text-align =right;
          font-family: Candara, Calibri, Segoe, 'Segoe UI', Optima, Arial, sans-serif;
          font-size: 13px;
          }

          /*Tabs*/
          /*----- Tabs -----*/
          .tabs {
          width:100%;
          display:inline-block;
          }
          /*----- Tab Links -----*/
          /* Clearfix */
          .tab-links {
          margin-left : 30px;
          margin-right : 30px;
          -webkit-padding-start : 0px;
          }
          .tab-links:after {
          display:block;
          clear:both;
          content:'';
          }
          .tab-links li {
          margin:0px 5px;
          float:left;
          list-style:none;
          width : 110px;
          }
          .tab-links a {
          padding:9px 15px;
          display:inline-block;
          border-radius:3px 3px 0px 0px;
          background:#dcd470;
          font-size:16px;
          font-weight:600;
          color:#4c4c4c;
          transition:all linear 0.15s;
          }
          .tab-links a:hover {
          background:#dcd470;
          text-decoration:none;
          }
          li.active a, li.active a:hover {
          background:#7FB5DA;
          color:#4c4c4c;
          }
          /*----- Content of Tabs -----*/
          .tab-content {
          border-radius:3px;
          box-shadow:-1px 1px 1px rgba(0,0,0,0.15);
          margin-left : 30px;
          margin-right : 30px;
          margin-top: -13px;
          }
          .tab {
          display:none;
          }
          .tab.active {
          display:block;
          }


        </style>
        <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"/>
        <!--<script type="text/javascript" language ="javascript" >
	  <xsl:comment>
        <![CDATA[	
		$(document).ready(function() {
		$('.tabs .tab-links a').on('click', function(e)  {
		var currentAttrValue = $(this).attr('href');
		
		// Show/Hide Tabs
		$('.tabs ' + currentAttrValue).show().siblings().hide();
		
		// Change/remove current tab to active
		$(this).parent('li').addClass('active').siblings().removeClass('active');
		
		e.preventDefault();
		});
		});
		   ]]>
      </xsl:comment>
		</script>-->
        <script type="text/javascript" language ="javascript" >
          <xsl:comment>
            <![CDATA[	
		$(function() {
		$('.tabs .tab-links a').on('click', function(e)  {
		var currentAttrValue = $(this).attr('href');
		
		// Show/Hide Tabs
		$('.tabs ' + currentAttrValue).show().siblings().hide();
		
		// Change/remove current tab to active
		$(this).parent('li').addClass('active').siblings().removeClass('active');
		
		e.preventDefault();
		});
		});
		   ]]>
          </xsl:comment>
        </script>
      </head>
      <body>
        <h1>
          <u>Consolidated Report</u>
        </h1>
        <div class="tabs">
          <ul class="tab-links">
            <xsl:for-each select="ConsolidatedReport/Market">
              <xsl:variable name="anchorId" select="concat('#tab',position())" />
              <xsl:choose>
                <xsl:when test="position()= 1" >
                  <li class="active">
                    <a href="{$anchorId}">
                      <xsl:value-of select="@marketName"/>
                    </a>
                  </li>
                </xsl:when>
                <xsl:otherwise>
                  <li>
                    <a href="{$anchorId}">
                      <xsl:value-of select="@marketName"/>
                    </a>
                  </li>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>
          </ul>
          <div class="tab-content">
            <xsl:for-each select="ConsolidatedReport/Market">
              <xsl:variable name="MarketName" select="@marketName" />
              <xsl:variable name="divId" select="concat('tab',position())" />

              <xsl:variable name="tableContainer">
                <table align="center" border="3" width="100%">

                  <th colspan="7">
                    Test Execution Report for Market - <xsl:value-of select="$MarketName"/>
                  </th>
                  <tr class="d0">
                    <td>
                      <b>TestCase Name</b>
                    </td>
                    <td>
                      <b>CHROME</b>
                    </td>
                    <td>
                      <b>IE</b>
                    </td>
                    <td>
                      <b>FIREFOX</b>
                    </td>
                    <td>
                      <b>PHANTOMJS</b>
                    </td>
                  </tr>
                  <xsl:for-each select="Testcase">
                    <tr>
                      <td>
                        <xsl:variable name="TestcaseName" select="@testcaseName" />
                        <xsl:value-of select="$TestcaseName"/>
                      </td>
                      <xsl:variable name="NotApplicable" select ="'NA'"/>
                      <xsl:variable name="Empty_string" select = "''"/>

                      <xsl:variable name="ChromeResult">
                        <xsl:for-each select="Browser">
                          <xsl:variable name="BrowserName" select="@browserName" />
                          <xsl:variable name="ChromeReportPath" select="HtmlReportPath" />
                          <xsl:choose>
                            <xsl:when test="$BrowserName = 'chrome'" >
                              <a href="{$ChromeReportPath}">
                                <xsl:value-of select="Result"/>
                              </a>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:value-of select="$Empty_string"/>
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:for-each>
                      </xsl:variable>
                      <xsl:variable name="IEResult">
                        <xsl:for-each select="Browser">
                          <xsl:variable name="BrowserName" select="@browserName" />
                          <xsl:variable name="IEReportPath" select="HtmlReportPath" />
                          <xsl:choose>
                            <xsl:when test="$BrowserName = 'internet explorer'">
                              <a href="{$IEReportPath}">
                                <xsl:value-of select="Result"/>
                              </a>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:value-of select="$Empty_string"/>
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:for-each>
                      </xsl:variable>
                      <xsl:variable name="FirefoxResult">
                        <xsl:for-each select="Browser">
                          <xsl:variable name="BrowserName" select="@browserName" />
                          <xsl:variable name="FirefoxReportpath" select="HtmlReportPath" />
                          <xsl:choose>
                            <xsl:when test="$BrowserName = 'firefox'">
                              <a href="{$FirefoxReportpath}">
                                <xsl:value-of select="Result"/>
                              </a>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:value-of select="$Empty_string"/>
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:for-each>
                      </xsl:variable>
                      <xsl:variable name="PhantomJsResult">
                        <xsl:for-each select="Browser">
                          <xsl:variable name="BrowserName" select="@browserName" />
                          <xsl:variable name="PhantomJsReportpath" select="HtmlReportPath" />
                          <xsl:choose>
                            <xsl:when test="$BrowserName = 'phantomjs'">
                              <a href="{$PhantomJsReportpath}">
                                <xsl:value-of select="Result"/>
                              </a>
                            </xsl:when>
                            <xsl:otherwise>
                              <xsl:value-of select="$Empty_string"/>
                            </xsl:otherwise>
                          </xsl:choose>
                        </xsl:for-each>
                      </xsl:variable>

                      <td>
                        <xsl:choose>
                          <xsl:when test="($ChromeResult) and ($ChromeResult != '') ">
                            <xsl:copy-of select="$ChromeResult"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$NotApplicable"/>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <td>
                        <xsl:choose>
                          <xsl:when test="($IEResult) and ($IEResult != '')">
                            <xsl:copy-of select="$IEResult"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$NotApplicable"/>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <td>
                        <xsl:choose>
                          <xsl:when test="($FirefoxResult) and ($FirefoxResult != '')">
                            <xsl:copy-of select="$FirefoxResult"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$NotApplicable"/>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <td>
                        <xsl:choose>
                          <xsl:when test="($PhantomJsResult) and ($PhantomJsResult != '')">
                            <xsl:copy-of select="$PhantomJsResult"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:value-of select="$NotApplicable"/>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                    </tr>
                  </xsl:for-each>
                </table>
              </xsl:variable>

              <xsl:variable name="divContainer">
                <xsl:choose>
                  <xsl:when test="position()= 1" >
                    <div id="{$divId}" class="tab active">
                      <xsl:copy-of select="$tableContainer"/>
                    </div>
                  </xsl:when>
                  <xsl:otherwise>
                    <div id="{$divId}" class="tab">
                      <xsl:copy-of select="$tableContainer"/>
                    </div>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:variable>

              <xsl:copy-of select="$divContainer"/>


            </xsl:for-each>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>