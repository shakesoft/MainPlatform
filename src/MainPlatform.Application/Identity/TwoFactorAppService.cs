using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using QRCoder;
using Volo.Abp.Sms;
using Volo.Abp.Emailing;
using Volo.Abp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace MainPlatform.Identity;

[Authorize]
public class TwoFactorAppService : MainPlatformAppService, ITwoFactorAppService
{
    private readonly IdentityUserManager _userManager;
    private readonly ISmsSender _smsSender;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public TwoFactorAppService(
        IdentityUserManager userManager, 
        ISmsSender smsSender,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _smsSender = smsSender;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<TwoFactorStatusDto> GetStatusAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        return new TwoFactorStatusDto
        {
            IsEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
        };
    }

    public async Task<SetupTwoFactorDto> SetupAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        await _userManager.ResetAuthenticatorKeyAsync(user);
        
        var sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(sharedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var email = await _userManager.GetEmailAsync(user);
        var qrCodeUri = GenerateQrCodeUri(email!, sharedKey!);

        return new SetupTwoFactorDto
        {
            SharedKey = sharedKey!,
            QrCodeUri = qrCodeUri
        };
    }

    public async Task<TwoFactorRecoveryCodesDto> VerifyAndEnableAsync(VerifyTwoFactorDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, 
            _userManager.Options.Tokens.AuthenticatorTokenProvider, 
            input.Code);

        if (!isValid)
        {
            throw new UserFriendlyException("رمز التحقق غير صحيح");
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        return new TwoFactorRecoveryCodesDto
        {
            RecoveryCodes = recoveryCodes.ToList()
        };
    }

    public async Task DisableAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        await _userManager.SetTwoFactorEnabledAsync(user, false);
    }

    public async Task<bool> CheckPasswordAsync(string password)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<TwoFactorSmsStatusDto> GetSmsStatusAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        return new TwoFactorSmsStatusDto
        {
            PhoneNumber = user.PhoneNumber,
            IsPhoneNumberConfirmed = user.PhoneNumberConfirmed
        };
    }

    public async Task SendSmsCodeAsync(SendSmsCodeDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        
        if (string.IsNullOrEmpty(input.PhoneNumber))
        {
            throw new UserFriendlyException("يرجى إدخال رقم الهاتف");
        }

        // Generate token for changing/confirming phone number
        var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, input.PhoneNumber);
        
        // Send SMS
        await _smsSender.SendAsync(input.PhoneNumber, $"رمز التحقق الخاص بك هو: {token}");
    }

    public async Task<TwoFactorRecoveryCodesDto> VerifySmsAndEnableAsync(VerifySmsCodeDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        
        var result = await _userManager.ChangePhoneNumberAsync(user, input.PhoneNumber, input.Code);
        if (!result.Succeeded)
        {
            throw new UserFriendlyException("رمز التحقق غير صحيح أو قد انتهت صلاحيته");
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        return new TwoFactorRecoveryCodesDto
        {
            RecoveryCodes = recoveryCodes.ToList()
        };
    }

    public async Task<TwoFactorEmailStatusDto> GetEmailStatusAsync()
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        
        // Get email from ePlatform
        var ePlatformEmail = await GetEmailFromEPlatformAsync(user.Id);
        
        return new TwoFactorEmailStatusDto
        {
            Email = ePlatformEmail ?? user.Email,
            IsEmailConfirmed = user.EmailConfirmed
        };
    }

    public async Task SendEmailCodeAsync(SendEmailCodeDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        
        var email = input.Email;
        if (string.IsNullOrEmpty(email))
        {
            email = await GetEmailFromEPlatformAsync(user.Id);
        }

        if (string.IsNullOrEmpty(email))
        {
            email = user.Email;
        }

        if (string.IsNullOrEmpty(email))
        {
            throw new UserFriendlyException("يرجى إدخال البريد الإلكتروني");
        }

        // Generate a 6-digit random code
        var code = new Random().Next(100000, 999999).ToString();
        
        // Store the code in user's extra properties to verify it later
        user.SetProperty("EmailVerificationCode", code);
        await _userManager.UpdateAsync(user);
        
        // Send Email with 6-digit code
        await SendEmailInternalAsync(email, "رمز التحقق الخاص بك", code);
    }

    private async Task SendEmailInternalAsync(string emailTo, string subject, string code)
    {
        try
        {
            var host = _configuration["Settings:Abp.Email.Smtp.Host"];
            var portStr = _configuration["Settings:Abp.Email.Smtp.Port"];
            var emailSender = _configuration["Settings:Abp.Email.Smtp.UserName"];
            var password = _configuration["Settings:Abp.Email.Smtp.Password"];
            var enableSsl = _configuration.GetValue<bool>("Settings:Abp.Email.Smtp.EnableSsl", true);

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(emailSender) || string.IsNullOrEmpty(password))
            {
                // Fallback to ABP EmailSender if custom settings not found (though they should be there)
                await _emailSender.SendAsync(emailTo, subject, $"رمز التحقق الخاص بك هو: {code}");
                return;
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
                        <p>لقد طلبت الحصول على رمز أمان للمصادقة الثنائية. يرجى استخدام الرمز التالي لإكمال عملية التحقق:</p>
                        
                        <div class=""code-box"">
                            <span class=""code"">{code}</span>
                        </div>
                        
                        <p>هذا الرمز صالح لفترة محدودة. إذا لم تطلب هذا الرمز، يرجى تجاهل هذا البريد الإلكتروني.</p>
                    </div>
                    <div class=""footer"">
                        <p>© {DateTime.Now.Year} Main Platform - جميع الحقوق محفوظة</p>
                        <p>هذه رسالة تلقائية، يرجى عدم الرد عليها.</p>
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
            // If manual sending fails, try ABP EmailSender as last resort
            try { await _emailSender.SendAsync(emailTo, subject, $"رمز التحقق الخاص بك هو: {code}"); } catch { }
            Console.WriteLine($"Error sending email via SmtpClient: {ex.Message}");
        }
    }

    public async Task<TwoFactorRecoveryCodesDto> VerifyEmailAndEnableAsync(VerifyEmailCodeDto input)
    {
        var user = await _userManager.GetByIdAsync(CurrentUser.GetId());
        
        var storedCode = user.GetProperty<string>("EmailVerificationCode");
        if (string.IsNullOrEmpty(storedCode) || input.Code != storedCode)
        {
            throw new UserFriendlyException("رمز التحقق غير صحيح أو قد انتهت صلاحيته");
        }

        // Clear the verification code
        user.SetProperty("EmailVerificationCode", null);
        
        // Update user email and confirm it
        if (!string.Equals(user.Email, input.Email, StringComparison.OrdinalIgnoreCase))
        {
            await _userManager.SetEmailAsync(user, input.Email);
        }
        
        user.SetEmailConfirmed(true);
        await _userManager.UpdateAsync(user);

        // Enable Two-Factor
        await _userManager.SetTwoFactorEnabledAsync(user, true);
        
        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        return new TwoFactorRecoveryCodesDto
        {
            RecoveryCodes = recoveryCodes.ToList()
        };
    }

    private async Task<string?> GetEmailFromEPlatformAsync(Guid userId)
    {
        var connectionString = _configuration.GetConnectionString("Eplatform");
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

    private string GenerateQrCodeUri(string email, string key)
    {
        const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        var appName = "MainPlatform";
        var unformattedKey = key.Replace(" ", string.Empty);
        
        var totpUri = string.Format(
            AuthenticatorUriFormat,
            Uri.EscapeDataString(appName),
            Uri.EscapeDataString(email),
            unformattedKey);

        using (var qrGenerator = new QRCodeGenerator())
        {
            using (var qrCodeData = qrGenerator.CreateQrCode(totpUri, QRCodeGenerator.ECCLevel.Q))
            {
                using (var qrCode = new PngByteQRCode(qrCodeData))
                {
                    var qrCodeImage = qrCode.GetGraphic(20);
                    return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
                }
            }
        }
    }
}
