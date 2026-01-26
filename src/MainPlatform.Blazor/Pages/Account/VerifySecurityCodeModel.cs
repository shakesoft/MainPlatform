using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Identity;

namespace MainPlatform.Blazor.Pages.Account
{
    public class VerifySecurityCodeModel : AccountPageModel
    {
        [BindProperty]
        [Required]
        public string Code { get; set; }

        [BindProperty]
        public bool RememberBrowser { get; set; }

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

        protected new SignInManager<Volo.Abp.Identity.IdentityUser> SignInManager { get; }

        public VerifySecurityCodeModel(SignInManager<Volo.Abp.Identity.IdentityUser> signInManager)
        {
            SignInManager = signInManager;
            Code = string.Empty; // Initialize to avoid warning but [Required] will handle validation
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            ModelState.Clear();
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl });
            }

            return Page();
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

            var result = await SignInManager.TwoFactorSignInAsync(
                Provider ?? string.Empty,
                Code.Replace(" ", string.Empty).Replace("-", string.Empty),
                RememberMe ?? false,
                RememberBrowser);

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

            ModelState.AddModelError(string.Empty, "Invalid code or session expired. Please check your authenticator app.");
            return Page();
        }
    }
}
