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

        public SendSecurityCodeModel(
            SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
            IdentityUserManager userManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl });
            }

            var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
            Providers = providers.Select(p => new SelectListItem { Value = p, Text = p }).ToList();

            // 1. Try to find Authenticator in the list (case-insensitive)
            var authenticatorInList = providers.FirstOrDefault(p => p.Equals("Authenticator", StringComparison.OrdinalIgnoreCase)) 
                                   ?? providers.FirstOrDefault(p => p.Contains("Authenticator", StringComparison.OrdinalIgnoreCase));

            // 2. If not in list, check if user actually has a key (double check)
            if (authenticatorInList == null)
            {
                var key = await UserManager.GetAuthenticatorKeyAsync(user);
                if (!string.IsNullOrEmpty(key))
                {
                    authenticatorInList = "Authenticator";
                }
            }

            // 3. If found or forced, use it
            if (authenticatorInList != null)
            {
                SelectedProvider = "Authenticator"; // Normalize name
                return await OnPostAsync();
            }

            // 4. Fallback to first available
            if (providers.Count > 0)
            {
                SelectedProvider = providers[0];
                return await OnPostAsync();
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

            // If it's NOT Authenticator, we might need to send a code
            if (SelectedProvider != "Authenticator")
            {
                // In a real ABP app, this is handled by sending an event or using IEmailSender
                // For now, let's just trigger the token generation which often triggers the sending if integrated
                await UserManager.GenerateTwoFactorTokenAsync(user, SelectedProvider);
            }

            return RedirectToPage("/Account/VerifySecurityCode", new
            {
                provider = SelectedProvider,
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash,
                rememberMe = RememberMe ?? false
            });
        }
    }
}
