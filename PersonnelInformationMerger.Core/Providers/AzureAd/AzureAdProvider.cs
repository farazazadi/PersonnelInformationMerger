using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using PersonnelInformationMerger.Core.Helpers;
using PersonnelInformationMerger.Core.Models;

namespace PersonnelInformationMerger.Core.Providers.AzureAd
{
    public class AzureAdProvider : BaseProvider
    {

        #region Fields

        private readonly ILogger<AzureAdProvider> _logger;

        private string _lastAzureAdResponse;

        #endregion

        #region Properties

        public string UsersEndpoint { get; init; }
        public string ClientId { get; init; }
        public string ClientSecret { get; init; }
        public string AuthorityUrl { get; init; }

        #endregion

        #region Constructors

        protected AzureAdProvider() : base()
        {
            
        }

        public AzureAdProvider(ILogger<AzureAdProvider> logger, double autoCheckInterval) : base(autoCheckInterval)
        {
            _logger = logger;
        }

        #endregion


        #region Methods

        public override async Task Process()
        {
            try
            {
                var azureAdUsersJson = await GetAzureAdUsersAsync();

                var isResponseChanged = IsAzureAdResponseChanged(azureAdUsersJson);

                _lastAzureAdResponse = azureAdUsersJson;

                var azureAdUserModelList = MapToAzureAdUserModelList(azureAdUsersJson);

                var personStandardModels = MapToPersonStandardModel(azureAdUserModelList);

                if (isResponseChanged)
                    base.OnPersonnelListChanged(personStandardModels, Personnel);

                Personnel.Clear();
                Personnel.AddRange(personStandardModels);

            }
            catch (Exception e)
            {
                DisableAutoCheckTimer();
                _logger.LogError(e.Message, e);
                throw;
            }

        }

        private async Task<string> GetAzureAdUsersAsync()
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(ClientId)
                .WithClientSecret(ClientSecret)
                .WithAuthority(new Uri(AuthorityUrl))
                .Build();

            var token = await confidentialClientApplication.AcquireTokenForClient(new[] { ".default" }).ExecuteAsync();


            var request = new HttpRequestMessage
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken) },
                RequestUri = new Uri(UsersEndpoint),
            };

            using var client = new HttpClient();

            using var response = await client.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var normalizedJsonResponse = RemoveOdataAttributes(jsonResponse);

            return normalizedJsonResponse;
        }

        private bool IsAzureAdResponseChanged(string azureAdResponse)
        {
            if (string.IsNullOrEmpty(_lastAzureAdResponse))
                return false;

            return _lastAzureAdResponse != azureAdResponse;
        }

        private static List<AzureAdUserModel> MapToAzureAdUserModelList(string jsonContent)
        {
            var azureAdUserModels = JsonHelper.Deserialize<List<AzureAdUserModel>>(jsonContent);

            return azureAdUserModels;
        }

        private static List<PersonStandardModel> MapToPersonStandardModel(List<AzureAdUserModel> azureAdUserModelList)
        {
            var personStandardModels = (azureAdUserModelList ?? throw new NullReferenceException())
                .Select(p => new PersonStandardModel
                {
                    Id = p.Id,
                    Title = p.JobTitle,
                    FullName = $"{p.GivenName} {p.Surname}",
                    Mobile = p.MobilePhone,
                    Email = p.Mail,
                    Address = p.OfficeLocation,
                }).ToList();

            return personStandardModels;
        }
        
        private string RemoveOdataAttributes(string jsonString)
        {
            var temp = jsonString.Substring(77);
            temp = temp.Substring(0, temp.Length - 1);

            return temp;
        }

        #endregion


        #region Event Handlers

        protected override void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () => await Process());
        }

        #endregion

    }
}
