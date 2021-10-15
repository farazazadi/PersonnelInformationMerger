using System;
using Microsoft.Extensions.Configuration;

namespace PersonnelInformationMerger.Console
{
    public static class Config
    {
        private static readonly IConfiguration Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        
        
        public static class AzureAd
        {
            public static string UsersEndpoint => Configuration["AzureAd:UsersEndpoint"];
            public static double AutoCheckInterval => double.Parse(Configuration["AzureAd:AutoCheckInterval"]);

            public static class Identity
            {
                public static string ClientId => Configuration["AzureAd:Identity:ClientId"];
                public static string ClientSecret => Configuration["AzureAd:Identity:ClientSecret"];
                public static string AuthorityUrl => Configuration["AzureAd:Identity:AuthorityUrl"];
            
            }
            
        }
        

        public static class PayrollSystem
        {
            public static string ExportedFilePath => Configuration["PayrollSystem:ExportedFilePath"];
            public static double AutoCheckInterval => Double.Parse(Configuration["PayrollSystem:AutoCheckInterval"]);
        }
        
        
        public static class Export
        {
            public static class JsonSavingStrategy
            {
                public static string ExportFilePath => Configuration["Export:JsonSavingStrategy:ExportFilePath"];
            }
        }

    }
}