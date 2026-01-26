using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.OpenIddict;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Alerts;
using Volo.Abp.Validation;
using System;
using Volo.Saas.Tenants;
using Volo.Abp.Account.Public.Web.Pages.Account;
using Volo.Abp.Account.Public.Web;
using Volo.Abp.Account.ExternalProviders;
using Owl.reCAPTCHA;
using Volo.Abp.Account.Security.Recaptcha;
using Microsoft.AspNetCore.Http;

namespace MainPlatform.Blazor.Pages.Account
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(LoginModel), typeof(OpenIddictSupportedLoginModel), typeof(CustomLoginModel))]
    public class CustomLoginModel : OpenIddictSupportedLoginModel
    {
        [BindProperty]
        public string? TenantName { get; set; }

        protected ITenantRepository TenantRepository { get; }

        public CustomLoginModel(
            IAuthenticationSchemeProvider schemeProvider,
            IOptions<AbpAccountOptions> accountOptions,
            IAbpRecaptchaValidatorFactory recaptchaValidatorFactory,
            IAccountExternalProviderAppService accountExternalProviderAppService,
            ICurrentPrincipalAccessor currentPrincipalAccessor,
            IOptions<IdentityOptions> identityOptions,
            IOptionsSnapshot<reCAPTCHAOptions> reCaptchaOptions,
            AbpOpenIddictRequestHelper abpOpenIddictRequestHelper,
            ITenantRepository tenantRepository)
            : base(
                schemeProvider, 
                accountOptions, 
                recaptchaValidatorFactory, 
                accountExternalProviderAppService, 
                currentPrincipalAccessor, 
                identityOptions, 
                reCaptchaOptions, 
                abpOpenIddictRequestHelper)
        {
            TenantRepository = tenantRepository;
        }

        public AlertList GetAlerts() => Alerts;

        public override async Task<IActionResult> OnPostAsync(string? action = null)
        {
            if (action == "SwitchTenant")
            {
                return await OnPostSwitchTenantAsync(TenantName);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            IActionResult result;

            try
            {
                result = await base.OnPostAsync(action);
            }
            catch (AbpValidationException)
            {
                return Page();
            }

            // If login failed (we are still on the page), provide more specific feedback
            if (!ModelState.IsValid && LoginInput != null)
            {
                var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) 
                        ?? await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
                
                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        Alerts.Danger("هذا الحساب معطل. يرجى مراجعة مدير النظام.");
                    }
                    else if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now)
                    {
                        Alerts.Danger("هذا الحساب مغلق حالياً بسبب كثرة المحاولات الخاطئة. يرجى المحاولة لاحقاً.");
                    }
                }
            }

            return result;
        }

        protected override void ValidateModel()
        {
            // Do nothing to prevent AbpValidationException
        }

        public virtual async Task<IActionResult> OnPostSwitchTenantAsync(string tenantName)
        {
            // Clear ModelState to avoid validation errors from LoginInput
            ModelState.Clear();
            
            Guid? tenantId = null;

            if (!string.IsNullOrWhiteSpace(tenantName))
            {
                var tenant = await TenantRepository.FindByNameAsync(tenantName);
                if (tenant == null)
                {
                    Alerts.Danger("Tenant not found: " + tenantName);
                    return Page();
                }
                tenantId = tenant.Id;
            }

            // Set tenant cookie
            Response.Cookies.Append(
                "__tenant",
                tenantId?.ToString() ?? "",
                new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Path = "/",
                    HttpOnly = false,
                    Expires = tenantId.HasValue ? System.DateTimeOffset.UtcNow.AddYears(1) : System.DateTimeOffset.UtcNow.AddDays(-1)
                }
            );

            return RedirectToPage("/Account/Login", new { returnUrl = ReturnUrl, returnUrlHash = ReturnUrlHash });
        }

        public virtual IActionResult OnGetSwitchLanguage(string culture, string uiCulture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, uiCulture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    Path = "/",
                    HttpOnly = false
                }
            );

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToPage("/Account/Login");
        }
    }
}
