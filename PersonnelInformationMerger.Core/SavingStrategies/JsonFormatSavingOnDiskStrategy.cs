using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PersonnelInformationMerger.Core.Helpers;
using PersonnelInformationMerger.Core.Models;

namespace PersonnelInformationMerger.Core.SavingStrategies
{
    public class JsonFormatSavingOnDiskStrategy : ISavingStrategy
    {
        #region Fields

        private readonly ILogger<JsonFormatSavingOnDiskStrategy> _logger;
        private readonly string _savingPath;

        #endregion


        #region Constructors

        public JsonFormatSavingOnDiskStrategy(ILogger<JsonFormatSavingOnDiskStrategy> logger, string filePath)
        {
            _logger = logger;
            _savingPath = filePath;
        }

        #endregion


        #region Methods

        public void Save(List<PersonStandardModel> personnelList)
        {
            try
            {

                var json = JsonHelper.Serialize(personnelList);

                FileHelper.WriteAllText(_savingPath, json);

                _logger.LogInformation($"Result exported: {_savingPath}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

        }

        #endregion
    }
}
