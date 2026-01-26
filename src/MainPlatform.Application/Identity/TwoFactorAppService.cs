using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using QRCoder;

namespace MainPlatform.Identity;

[Authorize]
public class TwoFactorAppService : MainPlatformAppService, ITwoFactorAppService
{
    private readonly IdentityUserManager _userManager;

    public TwoFactorAppService(IdentityUserManager userManager)
    {
        _userManager = userManager;
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
