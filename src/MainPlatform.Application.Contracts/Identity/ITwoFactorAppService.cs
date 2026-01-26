using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MainPlatform.Identity;

public class TwoFactorStatusDto
{
    public bool IsEnabled { get; set; }
}

public class SetupTwoFactorDto
{
    public string SharedKey { get; set; } = string.Empty;
    public string QrCodeUri { get; set; } = string.Empty;
}

public class VerifyTwoFactorDto
{
    public string Code { get; set; } = string.Empty;
}

public class TwoFactorRecoveryCodesDto
{
    public IEnumerable<string> RecoveryCodes { get; set; } = new List<string>();
}

public class TwoFactorSmsStatusDto
{
    public string? PhoneNumber { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }
}

public class SendSmsCodeDto
{
    public string PhoneNumber { get; set; } = string.Empty;
}

public class VerifySmsCodeDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class TwoFactorEmailStatusDto
{
    public string? Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
}

public class SendEmailCodeDto
{
    public string Email { get; set; } = string.Empty;
}

public class VerifyEmailCodeDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public interface ITwoFactorAppService : IApplicationService
{
    Task<TwoFactorStatusDto> GetStatusAsync();
    Task<SetupTwoFactorDto> SetupAsync();
    Task<TwoFactorRecoveryCodesDto> VerifyAndEnableAsync(VerifyTwoFactorDto input);
    Task DisableAsync();
    Task<bool> CheckPasswordAsync(string password);

    Task<TwoFactorSmsStatusDto> GetSmsStatusAsync();
    Task SendSmsCodeAsync(SendSmsCodeDto input);
    Task<TwoFactorRecoveryCodesDto> VerifySmsAndEnableAsync(VerifySmsCodeDto input);

    Task<TwoFactorEmailStatusDto> GetEmailStatusAsync();
    Task SendEmailCodeAsync(SendEmailCodeDto input);
    Task<TwoFactorRecoveryCodesDto> VerifyEmailAndEnableAsync(VerifyEmailCodeDto input);
}
