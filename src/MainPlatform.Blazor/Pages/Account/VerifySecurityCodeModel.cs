using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Sms;
using Microsoft.Extensions.Localization;
using MainPlatform.Localization;
using System.Collections.Generic;

namespace MainPlatform.Blazor.Pages.Account
{
    public class VerifySecurityCodeModel : AccountPageModel
    {
        private readonly IdentityUserManager _userManager;
        private readonly ISmsSender _smsSender;
        private readonly IStringLocalizer<MainPlatformResource> _L;

        [BindProperty]
        [Required]
        public string Code { get; set; }

        [BindProperty]
        public bool RememberBrowser { get; set; }

        [BindProperty]
        public bool UseRecoveryCode { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrlHash { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public bool? RememberMe { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string? Provider { get; set; }

        public List<string> AvailableProviders { get; set; } = new();

        protected new SignInManager<Volo.Abp.Identity.IdentityUser> SignInManager { get; }

        public VerifySecurityCodeModel(
            SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
            IdentityUserManager userManager,
            ISmsSender smsSender,
            IStringLocalizer<MainPlatformResource> L)
        {
            SignInManager = signInManager;
            _userManager = userManager;
            _smsSender = smsSender;
            _L = L;
            Code = string.Empty;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            ModelState.Clear();
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl });
            }

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            AvailableProviders = providers.ToList();

            if (string.IsNullOrEmpty(Provider))
            {
                // Prioritize Phone if available, otherwise pick the first one
                Provider = AvailableProviders.Contains("Phone") ? "Phone" : AvailableProviders.FirstOrDefault();
            }

            if (Provider == "Phone")
            {
                // Send SMS token
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
                await _smsSender.SendAsync(user.PhoneNumber!, _L["SmsTwoFactorMessage", token]);
            }

            return Page();
        }

        public string GetProviderDisplayName(string provider)
        {
            return provider switch
            {
                "Authenticator" => _L["AuthenticatorApp"],
                "Phone" => _L["SMSTextOrWhatsApp"],
                "Email" => _L["Email"],
                _ => provider
            };
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            // Remove validation errors for optional hidden fields if they are empty
            ModelState.Remove(nameof(ReturnUrl));
            ModelState.Remove(nameof(ReturnUrlHash));
            ModelState.Remove(nameof(Provider));
            ModelState.Remove(nameof(RememberMe));

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Microsoft.AspNetCore.Identity.SignInResult result;

            if (UseRecoveryCode)
            {
                result = await SignInManager.TwoFactorRecoveryCodeSignInAsync(Code.Trim());
            }
            else
            {
                result = await SignInManager.TwoFactorSignInAsync(
                    Provider ?? string.Empty,
                    Code.Replace(" ", string.Empty).Replace("-", string.Empty),
                    RememberMe ?? false,
                    RememberBrowser);
            }

            if (result.Succeeded)
            {
                var redirectUrl = ReturnUrl;
                if (!string.IsNullOrEmpty(ReturnUrlHash))
                {
                    redirectUrl += ReturnUrlHash;
                }
                return Redirect(redirectUrl ?? "/");
            }

            if (result.IsLockedOut)
            {
                Alerts.Danger("User locked out.");
                return RedirectToPage("/Account/Login");
            }

            string errorMessage;
            if (UseRecoveryCode)
            {
                errorMessage = "Invalid recovery code. Please try again.";
            }
            else if (Provider == "Authenticator")
            {
                errorMessage = "Invalid code. Please check your authenticator app.";
            }
            else if (Provider == "Phone")
            {
                errorMessage = "Invalid SMS code. Please try again.";
            }
            else
            {
                errorMessage = "Invalid code or session expired.";
            }

            ModelState.AddModelError(string.Empty, errorMessage);
            return Page();
        }
    }
}
