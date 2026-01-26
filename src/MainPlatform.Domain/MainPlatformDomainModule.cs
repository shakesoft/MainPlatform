using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MainPlatform.Localization;
using MainPlatform.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.LanguageManagement;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement;
using Volo.Abp.TextTemplateManagement;
using Volo.Saas;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.Caching;
using Volo.Abp.Commercial.SuiteTemplates;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.Sms;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using OPAC;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Collaboration;

namespace MainPlatform;

[DependsOn(
    typeof(MainPlatformDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpCachingModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityProDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpOpenIddictProDomainModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(SaasDomainModule),
    typeof(TextTemplateManagementDomainModule),
    typeof(LanguageManagementDomainModule),
    typeof(VoloAbpCommercialSuiteTemplatesModule),
    typeof(AbpEmailingModule),
    typeof(AbpSmsModule),
    typeof(AbpGdprDomainModule),
    typeof(BlobStoringDatabaseDomainModule),
    typeof(OPACDomainModule)
    )]
[DependsOn(typeof(CollaborationDomainModule))]
    public class MainPlatformDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
        });


#if DEBUG
        // context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
        // context.Services.Replace(ServiceDescriptor.Singleton<ISmsSender, DebugSmsSender>());
        context.Services.AddTransient<ISmsSender, TwilioSmsSender>();
#else
        context.Services.AddTransient<ISmsSender, TwilioSmsSender>();
#endif
    }
}

public class DebugSmsSender : ISmsSender
{
    public Task SendAsync(SmsMessage smsMessage)
    {
        // For development: Log the code to the debug output
        Console.WriteLine("---------------- SMS SENT ----------------");
        Console.WriteLine($"TO: {smsMessage.PhoneNumber}");
        Console.WriteLine($"TEXT: {smsMessage.Text}");
        Console.WriteLine("------------------------------------------");
        return Task.CompletedTask;
    }
}

public class TwilioSmsSender : ISmsSender, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public TwilioSmsSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(SmsMessage smsMessage)
    {
        var accountSid = _configuration["Sms:Twilio:AccountSid"];
        var authToken = _configuration["Sms:Twilio:AuthToken"];
        var fromNumber = _configuration["Sms:Twilio:FromNumber"];

        if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
        {
            // Fallback to console if config is missing
            Console.WriteLine("TWILIO CONFIG MISSING. Falling back to console.");
            Console.WriteLine($"TO: {smsMessage.PhoneNumber} TEXT: {smsMessage.Text}");
            return;
        }

        TwilioClient.Init(accountSid, authToken);

        await MessageResource.CreateAsync(
            body: smsMessage.Text,
            from: new Twilio.Types.PhoneNumber(fromNumber),
            to: new Twilio.Types.PhoneNumber(smsMessage.PhoneNumber)
        );
    }
}
