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

public interface ITwoFactorAppService : IApplicationService
{
    Task<TwoFactorStatusDto> GetStatusAsync();
    Task<SetupTwoFactorDto> SetupAsync();
    Task<TwoFactorRecoveryCodesDto> VerifyAndEnableAsync(VerifyTwoFactorDto input);
    Task DisableAsync();
}
