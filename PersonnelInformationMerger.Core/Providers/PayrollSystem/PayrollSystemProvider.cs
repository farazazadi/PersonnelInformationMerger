using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using PersonnelInformationMerger.Core.Helpers;
using PersonnelInformationMerger.Core.Models;

namespace PersonnelInformationMerger.Core.Providers.PayrollSystem
{
    public class PayrollSystemProvider : BaseProvider
    {
        #region Fields

        private readonly ILogger<PayrollSystemProvider> _logger;
        private readonly string _xmlFilePath;

        private string _lastPayrollPersonnelXmlContent;

        #endregion


        #region Constructors

        public PayrollSystemProvider() : base()
        {
            
        }

        public PayrollSystemProvider(ILogger<PayrollSystemProvider> logger,string xmlFilePath, double autoCheckInterval)
            : base(autoCheckInterval)
        {
            _logger = logger;
            _xmlFilePath = xmlFilePath;
        }

        #endregion


        #region Methods

        public override Task Process()
        {
            try
            {
                
                var xmlString = FileHelper.ReadAllText(_xmlFilePath);

                var isXmlFileContentChanged = IsPayrollXmlFileContentChanged(xmlString);

                _lastPayrollPersonnelXmlContent = xmlString;

                var personStandardModels = MapToPersonStandardModel(_xmlFilePath);

                if (isXmlFileContentChanged)
                    base.OnPersonnelListChanged(personStandardModels, Personnel);

                Personnel.Clear();
                Personnel.AddRange(personStandardModels);


                return Task.CompletedTask;

            }
            catch (Exception e)
            {
                DisableAutoCheckTimer();
                _logger.LogError(e.Message, e);
                throw;
            }

        }

        private bool IsPayrollXmlFileContentChanged(string xmlString)
        {
            if (string.IsNullOrEmpty(_lastPayrollPersonnelXmlContent))
                return false;

            return _lastPayrollPersonnelXmlContent != xmlString;
        }

        private static List<PersonStandardModel> MapToPersonStandardModel(string filePath)
        {
            var parsedPersonsList = XmlHelper.Deserialize<PayrollSystemEmployeeModel>(filePath)
                .PersonnelList
                .Select(person =>
                {

                    int? employeeNumber = person.EmployeeNumber == null ? null : int.Parse(person.EmployeeNumber);

                    return new PersonStandardModel
                    {
                        EmployeeNumber = employeeNumber,
                        FullName = person.Name,
                        Email = person.Email,
                        Mobile = person.Mobile,
                        Title = person.Title,
                        Address = person.Address,
                        City = person.City
                    };
                }).ToList();



            return parsedPersonsList;
        }

        #endregion


        #region Event Handlers

        protected override void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Process();
        }

        #endregion

    }
}
