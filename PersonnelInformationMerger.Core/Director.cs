using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PersonnelInformationMerger.Core.MergeStrategies;
using PersonnelInformationMerger.Core.Models;
using PersonnelInformationMerger.Core.Providers;
using PersonnelInformationMerger.Core.Providers.EventArgs;
using PersonnelInformationMerger.Core.SavingStrategies;

namespace PersonnelInformationMerger.Core
{
    public class Director
    {

        #region Fields

        private readonly ILogger<Director> _logger;

        private readonly List<PersonStandardModel> _standardModels = new();
        
        private readonly List<BaseProvider> _providers = new();

        private IMergeStrategy _mergingStrategy;

        private ISavingStrategy _savingStrategy;


        #endregion



        #region Constructor And Fulent Interface

        public Director(ILogger<Director> logger)
        {
            _logger = logger;
        }


        public Director WithProviders(params BaseProvider[] providers)
        {
            AddProviders(providers);
            SubscribeProvidersOnPersonnelListChanged(providers);
            return this;
        }


        public Director WithMergingStrategy(IMergeStrategy strategy)
        {
            _mergingStrategy = strategy;
            return this;
        }


        public Director WithSavingStrategy(ISavingStrategy strategy)
        {
            _savingStrategy = strategy;
            return this;
        }


        #endregion



        #region Methods

        private void AddProviders(params BaseProvider[] providers)
        {
            foreach (var provider in providers)
                _providers.Add(provider);
        }
        
        private void SubscribeProvidersOnPersonnelListChanged(params BaseProvider[] providers)
        {
            foreach (var provider in providers)
                provider.PersonnelListChanged+= ProviderOnPersonnelListChanged;
        }
        
        private async Task ProcessProvidersAsync()
        {
            foreach (var provider in _providers)
                await provider.Process();
        }
        

        public async Task<IReadOnlyList<PersonStandardModel>> MergeAsync()
        {
            try
            {
                await ProcessProvidersAsync();

                var result = _providers[0].PersonnelList;

                for (var i = 1; i < _providers.Count; i++)
                    result = _mergingStrategy.Merge(result, _providers[i].PersonnelList);


                _standardModels.Clear();
                _standardModels.AddRange(result);

                return _standardModels;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

        }

        public void Save()
        {
            try
            {
                _savingStrategy.Save(_standardModels);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        #endregion



        #region Event Handlers

        private void ProviderOnPersonnelListChanged(object sender, PersonnelListChangedEventArgs e)
        {
            _logger.LogInformation($"The list of users in ({sender}) has changed.");

            Task.Run(async () =>
            {
                await MergeAsync();
                Save();
            });
        }
        
        #endregion

        
    }
}
