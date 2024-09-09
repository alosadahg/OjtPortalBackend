namespace OjtPortal.EmailTemplates
{
    public class EmailComponent
    {

        public static string title(string title)
        {
            return $@"
                <!-- start hero -->
                <tr>
                  <td align=""center"">
                    <!--[if (gte mso 9)|(IE)]>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                    <tr>
                    <td align=""center"" valign=""top"" width=""600"">
                    <![endif]-->
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
                      <tr>
                        <td align=""left"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
                          <h1 style=""margin: 0; font-size: 30px; font-weight: 700; text-align: center; letter-spacing: -1px; line-height: 48px;"">{title}</h1>
                        </td>
                      </tr>
                    </table>
                    <!--[if (gte mso 9)|(IE)]>
                    </td>
                    </tr>
                    </table>
                    <![endif]-->
                  </td>
                </tr>
                <!-- end hero -->
            ";
        }

        public static string otp(string otp)
        {
            return $@"
                <!-- start hero -->
                <tr>
                  <td align=""center"">
                    <!--[if (gte mso 9)|(IE)]>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                    <tr>
                    <td align=""center"" valign=""top"" width=""600"">
                    <![endif]-->
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
                      <tr>
                        <td align=""left"" bgcolor=""#ffffff"" style=""font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif;"">
                          <h1 style=""margin: 0; font-size: 32px; font-weight: 700; text-align: center; letter-spacing: -1px; line-height: 48px;  color: #FF6B6B"">{otp}</h1>
                        </td>
                      </tr>
                    </table>
                    <!--[if (gte mso 9)|(IE)]>
                    </td>
                    </tr>
                    </table>
                    <![endif]-->
                  </td>
                </tr>
                <!-- end hero -->
            ";
        }

        public static string description(string description)
        {
            return $@"
                <!-- start copy -->
                      <tr>
                        <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                          <p style=""margin: 0;"">{description}</p>
                        </td>
                      </tr>
                <!-- end copy -->
            ";
        }

        public static string button(string link, String buttonText)
        {
            return $@"
                <!-- start button -->
                      <tr>
                        <td align=""left"" bgcolor=""#ffffff"">
                          <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                            <tr>
                              <td align=""center"" bgcolor=""#ffffff"" style=""padding: 12px;"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                                  <tr>
                                    <td align=""center"" bgcolor=""#FF6B6B"" style=""border-radius: 6px;"">
                                      <a href='{link}' target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">{buttonText}</a>
                                    </td>
                                  </tr>
                                </table>
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                      <!-- end button -->
            ";
        }

        public static string footer(string footnote)
        {
            return $@"
                <!-- start permission -->
                <tr>
                  <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
                    <p style=""margin: 0;"">You received this email because we received a request {footnote} of your account.</p>
                  </td>
                </tr>
                <!-- end permission -->
            ";
        }
    }
}
