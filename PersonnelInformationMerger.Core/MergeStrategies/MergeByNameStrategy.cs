using Microsoft.Extensions.Logging;
using PersonnelInformationMerger.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonnelInformationMerger.Core.MergeStrategies
{
    public class MergeByNameStrategy : IMergeStrategy
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructors

        public MergeByNameStrategy(ILogger<MergeByNameStrategy> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        public List<PersonStandardModel> Merge(IReadOnlyList<PersonStandardModel> firstPersonnelList, IReadOnlyList<PersonStandardModel> secondPersonnelList)
        {
            try
            {
                _logger.LogInformation("Merging process started...");

                var joinedPersonnelList =
                    (from f in firstPersonnelList
                     join s in secondPersonnelList
                         on f.FullName equals s.FullName
                     select new PersonStandardModel
                     {
                         Id = f.Id ?? s.Id,
                         EmployeeNumber = f.EmployeeNumber ?? s.EmployeeNumber,
                         FullName = f.FullName ?? s.FullName,
                         Mobile = f.Mobile ?? s.Mobile,
                         Email = f.Email ?? s.Email,
                         Title = f.Title ?? s.Title,
                         Address = f.Address ?? s.Address,
                         City = f.City ?? s.City
                     }).ToList();

                _logger.LogInformation("Merging process is finished.");

                return joinedPersonnelList;
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
