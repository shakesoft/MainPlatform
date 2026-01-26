using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Identity;
using Volo.Abp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace MainPlatform.Blazor.Pages.Account
{
    public class SendSecurityCodeModel : AccountPageModel
    {
        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrlHash { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public bool? RememberMe { get; set; }

        [BindProperty]
        public string? SelectedProvider { get; set; }

        public IList<SelectListItem>? Providers { get; set; }

        protected new SignInManager<Volo.Abp.Identity.IdentityUser> SignInManager { get; }
        protected new IdentityUserManager UserManager { get; }
        protected Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        public SendSecurityCodeModel(
            SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
            IdentityUserManager userManager,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            Configuration = configuration;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl });
            }

            var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
            Providers = providers.Select(p => new SelectListItem 
            { 
                Value = p, 
                Text = p == "Authenticator" ? L["AuthenticatorApp"].Value : 
                       p == "Email" ? L["Email"].Value : 
                       (p == "Phone" || p == "SMS") ? L["SMSTextOrWhatsApp"].Value : p 
            }).ToList();

            // Force add Authenticator if user has a key but it's not in the valid providers list
            if (!providers.Any(p => p.Equals("Authenticator", StringComparison.OrdinalIgnoreCase)))
            {
                var key = await UserManager.GetAuthenticatorKeyAsync(user);
                if (!string.IsNullOrEmpty(key))
                {
                    Providers.Add(new SelectListItem 
                    { 
                        Value = "Authenticator", 
                        Text = L["AuthenticatorApp"].Value 
                    });
                }
            }

            // 3. Set default selection
            if (Providers.Any(p => p.Value == "Authenticator"))
            {
                SelectedProvider = "Authenticator";
            }
            else if (Providers.Count > 0)
            {
                SelectedProvider = Providers[0].Value;
            }

            return Page();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl });
            }

            if (string.IsNullOrWhiteSpace(SelectedProvider))
            {
                var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
                Providers = providers.Select(p => new SelectListItem { Value = p, Text = p }).ToList();
                return Page();
            }

            // Sync email from ePlatform if provider is Email
            if (SelectedProvider == "Email")
            {
                var ePlatformEmail = await GetEmailFromEPlatformAsync(user.Id);
                if (!string.IsNullOrEmpty(ePlatformEmail) && !string.Equals(user.Email, ePlatformEmail, StringComparison.OrdinalIgnoreCase))
                {
                    await UserManager.SetEmailAsync(user, ePlatformEmail);
                    
                    // IMPORTANT: Set confirmed to true so 2FA token generation works correctly
                    user.SetEmailConfirmed(true);
                    await UserManager.UpdateAsync(user);
                }
            }

            // If it's NOT Authenticator, we need to generate and send a code
            if (SelectedProvider != "Authenticator")
            {
                // Identity usually expects "Email" or "Phone"
                var code = await UserManager.GenerateTwoFactorTokenAsync(user, SelectedProvider);
                
                if (SelectedProvider == "Email" && !string.IsNullOrEmpty(user.Email))
                {
                    await SendEmailInternalAsync(user.Email, "رمز أمان المصادقة الثنائية", code);
                }
            }

            return RedirectToPage("/Account/VerifySecurityCode", new
            {
                provider = SelectedProvider,
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash,
                rememberMe = RememberMe ?? false
            });
        }

        private async Task SendEmailInternalAsync(string emailTo, string subject, string code)
        {
            try
            {
                var host = Configuration["Settings:Abp.Email.Smtp.Host"];
                var portStr = Configuration["Settings:Abp.Email.Smtp.Port"];
                var emailSender = Configuration["Settings:Abp.Email.Smtp.UserName"];
                var password = Configuration["Settings:Abp.Email.Smtp.Password"];
                var enableSsl = Configuration.GetValue<bool>("Settings:Abp.Email.Smtp.EnableSsl", true);

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(emailSender) || string.IsNullOrEmpty(password))
                {
                    throw new Volo.Abp.UserFriendlyException("إعدادات البريد الإلكتروني غير مكتملة في النظام.");
                }

                int port = 587;
                int.TryParse(portStr, out port);

                var body = $@"
                <!DOCTYPE html>
                <html lang=""ar"" dir=""rtl"">
                <head>
                    <meta charset=""UTF-8"">
                    <style>
                        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; direction: rtl; text-align: right; background-color: #f4f7f9; padding: 20px; }}
                        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 12px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }}
                        .header {{ text-align: center; margin-bottom: 30px; border-bottom: 2px solid #f0f0f0; padding-bottom: 20px; }}
                        .logo {{ color: #4F46E5; font-size: 28px; font-weight: bold; }}
                        .content {{ line-height: 1.8; color: #333; font-size: 16px; }}
                        .code-box {{ text-align: center; margin: 30px 0; padding: 20px; background-color: #f8faff; border: 2px dashed #4F46E5; border-radius: 8px; }}
                        .code {{ font-size: 32px; font-weight: bold; color: #4F46E5; letter-spacing: 8px; }}
                        .footer {{ font-size: 13px; color: #888; margin-top: 30px; text-align: center; border-top: 1px solid #eee; padding-top: 20px; }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""header"">
                            <div class=""logo"">Main Platform</div>
                        </div>
                        <div class=""content"">
                            <p>مرحباً،</p>
                            <p>لتسجيل الدخول إلى حسابك، يرجى استخدام رمز الأمان التالي:</p>
                            
                            <div class=""code-box"">
                                <span class=""code"">{code}</span>
                            </div>
                            
                            <p>هذا الرمز صالح لفترة محدودة. إذا لم تقم بمحاولة تسجيل الدخول، يرجى تأمين حسابك فوراً.</p>
                        </div>
                        <div class=""footer"">
                            <p>© {DateTime.Now.Year} Main Platform - جميع الحقوق محفوظة</p>
                        </div>
                    </div>
                </body>
                </html>";

                using (var client = new System.Net.Mail.SmtpClient(host, port))
                {
                    client.Credentials = new System.Net.NetworkCredential(emailSender, password);
                    client.EnableSsl = enableSsl;

                    var mail = new System.Net.Mail.MailMessage
                    {
                        From = new System.Net.Mail.MailAddress(emailSender, "Main Platform"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                        SubjectEncoding = System.Text.Encoding.UTF8,
                        BodyEncoding = System.Text.Encoding.UTF8
                    };

                    mail.To.Add(emailTo);
                    await client.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw new Volo.Abp.UserFriendlyException("فشل إرسال البريد الإلكتروني: " + ex.Message);
            }
        }

        private async Task<string?> GetEmailFromEPlatformAsync(Guid userId)
        {
            var connectionString = Configuration.GetConnectionString("Eplatform");
            if (string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT Email FROM AbpUsers WHERE Id = @UserId", 
                        connection);
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                // Log error or handle gracefully
                Console.WriteLine($"Error fetching email from ePlatform: {ex.Message}");
                return null;
            }
        }
    }
}
