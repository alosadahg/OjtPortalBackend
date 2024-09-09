namespace OjtPortal.EmailTemplates
{
    public static class EmailTemplate
    {
        public static string ActivationEmailTemplate(string email, string password, string activationLink)
        {
            var preheader = "Activate your account";
            string defaultCredentials = $"<span style=\"display: block; text-align: center;\">Email: {email}<br>Password: {password}</span>";
            var body = EmailComponent.title("Confirm Your Email Address")
                + EmailComponent.description("We are excited to welcome you in the Ojt Management Portal! These are your default login credentials.<br>" + defaultCredentials + "<br> Please activate your account to login. Just click the button below to activate.")
                + EmailComponent.button(activationLink, "Activate your account");
            var footnote = EmailComponent.footer("for the activation");
            return baseTemplate(preheader, body, footnote);
        }

        public static string ActivationEmailTemplate(string activationLink)
        {
            var preheader = "Activate your account";
            var body = EmailComponent.title("Confirm Your Email Address")  
                + EmailComponent.description("We are excited to welcome you in Ojt Management Portal! First, you need to confirm your account. Just click the button below.") 
                + EmailComponent.button(activationLink,"Activate your account");
            var footnote = EmailComponent.footer("for the activation");
            return baseTemplate(preheader, body, footnote);
        }

        public static string OTPTemplate(string otp)
        {
            var preheader = "Forgot Password OTP";
            var body = EmailComponent.title("Forgot Password OTP")
                + EmailComponent.description("Please use this OTP to reset your password.") + EmailComponent.otp(otp);
            var footnote = EmailComponent.footer("to forget the password");
            return baseTemplate(preheader, body, footnote);
        }

        private static string baseTemplate(string preheader, string body, string footer)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>

              <meta charset=""utf-8"">
              <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
              <title>Email Confirmation</title>
              <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
              <style type=""text/css"">
              /**
               * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
               */
              @media screen {{
                @font-face {{
                  font-family: 'Source Sans Pro';
                  font-style: normal;
                  font-weight: 400;
                  src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
                }}
                @font-face {{
                  font-family: 'Source Sans Pro';
                  font-style: normal;
                  font-weight: 700;
                  src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
                }}
              }}
              /**
               * Avoid browser level font resizing.
               * 1. Windows Mobile
               * 2. iOS / OSX
               */
              body,
              table,
              td,
              a {{
                -ms-text-size-adjust: 100%; /* 1 */
                -webkit-text-size-adjust: 100%; /* 2 */
              }}
              /**
               * Remove extra space added to tables and cells in Outlook.
               */
              table,
              td {{
                mso-table-rspace: 0pt;
                mso-table-lspace: 0pt;
              }}
              /**
               * Better fluid images in Internet Explorer.
               */
              img {{
                -ms-interpolation-mode: bicubic;
              }}
              /**
               * Remove blue links for iOS devices.
               */
              a[x-apple-data-detectors] {{
                font-family: inherit !important;
                font-size: inherit !important;
                font-weight: inherit !important;
                line-height: inherit !important;
                color: inherit !important;
                text-decoration: none !important;
              }}
              /**
               * Fix centering issues in Android 4.4.
               */
              div[style*=""margin: 16px 0;""] {{
                margin: 0 !important;
              }}
              body {{
                width: 100% !important;
                height: 100% !important;
                padding: 0 !important;
                margin: 0 !important;
              }}
              /**
               * Collapse table borders to avoid space between cells.
               */
              table {{
                border-collapse: collapse !important;
              }}
              a {{
                color: #FF6B6B;
              }}
              img {{
                height: auto;
                line-height: 100%;
                text-decoration: none;
                border: 0;
                outline: none;
              }}
              </style>

            </head>
            <body style=""background-color: #f3f4f6;"">

              <!-- start preheader -->
              <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
               {preheader}
              </div>
              <!-- end preheader -->

              <!-- start body -->
              <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

                <!-- start logo -->
                <tr>
                  <td align=""center"" bgcolor=""#f3f4f6"">
                    <!--[if (gte mso 9)|(IE)]>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                    <tr>
                    <td align=""center"" valign=""top"" width=""600"">
                    <![endif]-->
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
                      <tr>
                        <td align=""center"" valign=""top"" style=""padding: 20px 10px;"">
                          <!-- Logo url is inactive in supabase s3 bucket, please replace -->
                          <a href=""https://ascnakawytsqigjunyor.supabase.co/storage/v1/object/public/Photos/Logo/Ojt%20Portal%20Logo%20Landscape.png"" target=""_blank"" style=""display: inline-block;""> 
                            <img src=""https://ascnakawytsqigjunyor.supabase.co/storage/v1/object/public/Photos/Logo/Ojt%20Portal%20Logo%20Landscape.png"" alt=""Logo"" border=""0"" style=""display: block; width: 500px; min-width: 48px;""> 
                          </a> 
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
                <!-- end logo -->

                

                <!-- start copy block -->
                <tr>
                  <td align=""center"" bgcolor=""#f3f4f6"">
                    <!--[if (gte mso 9)|(IE)]>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                    <tr>
                    <td align=""center"" valign=""top"" width=""600"">
                    <![endif]-->
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

                      {body}

                      <!-- start copy -->
                      <tr>
                        <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                          <p style=""margin: 0;"">This email is used for notification purposes only. Please do not reply to this email.</p>
                        </td>
                      </tr>
                      <!-- end copy -->

                      <!-- start copy -->
                      <tr>
                        <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
                          <p style=""margin: 0;"">Cheers,<br> OJT Management Portal Devs </p>
                        </td>
                      </tr>
                      <!-- end copy -->

                    </table>
                    <!--[if (gte mso 9)|(IE)]>
                    </td>
                    </tr>
                    </table>
                    <![endif]-->
                  </td>
                </tr>
                <!-- end copy block -->

                <!-- start footer -->
                <tr>
                  <td align=""center"" bgcolor=""#f3f4f6"" style=""padding: 24px;"">
                    <!--[if (gte mso 9)|(IE)]>
                    <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
                    <tr>
                    <td align=""center"" valign=""top"" width=""600"">
                    <![endif]-->
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

                      {footer}

                    </table>
                    <!--[if (gte mso 9)|(IE)]>
                    </td>
                    </tr>
                    </table>
                    <![endif]-->
                  </td>
                </tr>
                <!-- end footer -->

              </table>
              <!-- end body -->

            </body>
            </html>
            ";
        }
    }
}
