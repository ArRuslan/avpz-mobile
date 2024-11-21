using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Services.ReCaptcha
{
    internal class ReCaptchaService
    {
        public const string siteKey = "6LcxkvApAAAAADd6pVenLzYl-99zkvfqq7VhTm6f";
        public const string baseUrl = "https://hhb-testing.ruslan.page";
        public const string captchaHtml = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <script src='https://www.google.com/recaptcha/api.js' async defer></script>
                    <script>
                        function onSubmit(token) {{
                            window.location.href = '{baseUrl}/?token=' + token;
                        }}
                    </script>
                    <style>
                            form {{
                                display: flex;
                                flex-direction: column;
                                align-items: center;
                                justify-content: center;
                                margin: 0; /* Видаляє зовнішні відступи */
                            }}
                    </style>
                </head>
                <body>
                    <form action='?' method='POST'>
                        <div class='g-recaptcha' data-sitekey='{siteKey}' data-callback='onSubmit'></div>
                        <br/>
                    </form>
                </body>
                </html>";

        public static async Task<string> HandleCaptchaNavigation(WebNavigatedEventArgs e)
        {
            try
            {
                if (e.Url.StartsWith($"{baseUrl}/?token="))
                {
                    var uri = new Uri(e.Url);
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    string? token = query.Get("token");

                    if (!string.IsNullOrEmpty(token))
                    {
                        return token;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating reCAPTCHA: {ex.Message}");
            }

            return string.Empty;
        }
    }
}
