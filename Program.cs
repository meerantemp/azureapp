using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FunctionAppTest;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Azure.Data.AppConfiguration;
using AzureFunction.Isolated.HostConfigurator;

//[assembly: HostConfiguratorAttribute(typeof(StartupExtension))]

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureLogging(logging =>
    {
        logging.AddConsole(); // Add Console logging
        logging.SetMinimumLevel(LogLevel.Information); // Set minimum log level
    })
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
    
        string? environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        configBuilder
            .AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.{environment}.json"), optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        IConfiguration config = configBuilder.Build();
        string? keyVaultURL = config["keyVault"];

        if (!string.IsNullOrEmpty(keyVaultURL))
        {
            SecretClient secretClient = new SecretClient(new Uri(keyVaultURL), new DefaultAzureCredential());
            configBuilder.AddAzureKeyVault(
                new Uri(keyVaultURL),
                new DefaultAzureCredential(),
                new AzureKeyVaultConfigurationOptions
                {
                    ReloadInterval = TimeSpan.FromMinutes(10)
                });
        }

        config = configBuilder.Build();

       
    })
    .ConfigureServices((context, services) =>
    {
        
        services.AddSingleton<Function1>();
    })
    .Build();

host.Run();
