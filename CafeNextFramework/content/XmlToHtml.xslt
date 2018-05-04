<xsl:stylesheet version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <head>
        <meta charset="utf-8" />
        <meta http-equiv="Content- Type" content="text / html; charset = UTF-8" />
        <style>
          body {
          background-color: #FFFFCC;
          }

          table {
          background-color: #DCDCDC;
          text-align: center;
          }

          th {
          background-color: #003399;
          text-align: center;
          color: #FFFFFF;
          font-family: Candara, Calibri, Segoe, 'Segoe UI', Optima, Arial, sans-serif;
          font-size: 15px;
          }

          tr {
          background-color: #E6E6E6;
          color: #1F1F7A;
          font-family: Candara, Calibri, Segoe, 'Segoe UI', Optima, Arial, sans-serif;
          font-size: 13px;
          }

          tr.d0 td {
          background-color: #01A9DB;
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

          <title>
            <xsl:value-of select="substring-before(TestCycle/@Name,'_')" />
          </title>
        </style>
      </head>
      <body>
        <h1>
          <u>TEST CASE REPORT</u>
        </h1>

        <table align="center" border="3" width="100%">
          <th colspan="7">TEST CASE SUMMARY</th>
          <tr class="d0">
            <td>
              <b>TestCase Name</b>
            </td>
            <td>
              <b>Status</b>
            </td>
          </tr>
          <tr>
            <td>
              <xsl:value-of select="substring-before(substring-after(TestCycle/@Name,'_'),'_')" />
            </td>
            <td>
              <xsl:choose>
                <xsl:when test="TestCycle/TestCases/TestCase/@Status = 1">
                  Passed
                </xsl:when>
                <xsl:otherwise>
                  Failed
                </xsl:otherwise>
              </xsl:choose>
            </td>
          </tr>

          <table align="center" border="3" width="100%">
            <br />
            <br />
            <br />
            <th colspan="10">TEST CASE DETAILS</th>
            <tr class="d0">
              <td>
                <b>Step</b>
              </td>
              <td>
                <b>Result</b>
              </td>
              <td>
                <b>Expected Result</b>
              </td>
              <td>
                <b>Actual Result</b>
              </td>
            </tr>
            <xsl:for-each select="TestCycle/TestCases/TestCase/TestSteps/TestStep">
              <xsl:variable name="ResultImage" select="Attachments/Attachment/@path" />
              <tr>
                <td>
                  <xsl:value-of select="TestData" />
                </td>
                <td>
                  <xsl:choose>
                    <xsl:when test="@status = 1">
                      Passed
                    </xsl:when>
                    <xsl:otherwise>
                      Failed
                    </xsl:otherwise>
                  </xsl:choose>
                </td>
                <td>
                  <xsl:value-of select="Expected" />
                </td>
                <td>
                  <xsl:choose>
                    <xsl:when test="Attachments/Attachment/@path">
                      <a href="{$ResultImage}">
                        <xsl:value-of select="Actual" />
                      </a>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="Actual" />
                    </xsl:otherwise>
                  </xsl:choose>
                </td>
              </tr>
            </xsl:for-each>
          </table>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>