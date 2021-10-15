using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonnelInformationMerger.Core;
using PersonnelInformationMerger.Core.MergeStrategies;
using PersonnelInformationMerger.Core.Providers.AzureAd;
using PersonnelInformationMerger.Core.Providers.PayrollSystem;
using PersonnelInformationMerger.Core.SavingStrategies;

namespace PersonnelInformationMerger.Console
{
    class Program
    {
        
        static async Task Main(string[] args)
        {

            System.Console.Title = "Personnel Information Merger";
                
            try
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.WriteLine("Please make sure the configuration (appsettings.json) is set correctly.");

                await RunAsync();

                System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                System.Console.WriteLine("The application is waiting to detect any changes in resources, Press any key to stop...");

                System.Console.ResetColor();

                System.Console.ReadKey();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }

        }


        private static async Task RunAsync()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            await using var serviceProvider = services.BuildServiceProvider();

            var payrollSystemProvider = serviceProvider.GetService<PayrollSystemProvider>();
            var azureAdProvider = serviceProvider.GetService<AzureAdProvider>();
            var mergingStrategy = serviceProvider.GetService<MergeByNameStrategy>();
            var savingStrategy = serviceProvider.GetService<JsonFormatSavingOnDiskStrategy>();

            var director = serviceProvider.GetService<Director>()?
                .WithProviders(payrollSystemProvider, azureAdProvider)
                .WithMergingStrategy(mergingStrategy)
                .WithSavingStrategy(savingStrategy);


            if (director == null)
                return;


            await director.MergeAsync();

            director.Save();
        }


        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure =>
                {
                    configure
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole();
                })

                .AddTransient<IMergeStrategy, MergeByNameStrategy>()
                .AddTransient<MergeByNameStrategy>()
                .AddTransient<Director>();



            services.AddTransient(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<PayrollSystemProvider>>();

                return new PayrollSystemProvider(logger, Config.PayrollSystem.ExportedFilePath,
                    Config.PayrollSystem.AutoCheckInterval);
            });



            services.AddTransient(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<AzureAdProvider>>();

                return new AzureAdProvider(logger, Config.AzureAd.AutoCheckInterval)
                {
                    AuthorityUrl = Config.AzureAd.Identity.AuthorityUrl,
                    ClientId = Config.AzureAd.Identity.ClientId,
                    ClientSecret = Config.AzureAd.Identity.ClientSecret,
                    UsersEndpoint = Config.AzureAd.UsersEndpoint
                };
            });

            services.AddTransient(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<JsonFormatSavingOnDiskStrategy>>();

                return new JsonFormatSavingOnDiskStrategy(logger, Config.Export.JsonSavingStrategy.ExportFilePath);
            });


        }

    }
    
}