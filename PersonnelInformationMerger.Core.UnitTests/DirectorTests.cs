using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PersonnelInformationMerger.Core.MergeStrategies;
using PersonnelInformationMerger.Core.Models;
using PersonnelInformationMerger.Core.Providers.AzureAd;
using PersonnelInformationMerger.Core.Providers.PayrollSystem;
using PersonnelInformationMerger.Core.SavingStrategies;
using Xunit;

namespace PersonnelInformationMerger.Core.UnitTests
{
    public class DirectorTests
    {

        private readonly Mock<AzureAdProvider> _azureAdProviderMock = new();
        private readonly Mock<PayrollSystemProvider> _payrollSystemProviderMock = new();
        private readonly Mock<ISavingStrategy> _savingStrategy = new();


        [Fact]
        public async Task MergeAsyncMethod_ShouldReturnExpectedResult_WhenInvoked()
        {
            // Arrange

            var id = Guid.NewGuid().ToString();

            _azureAdProviderMock.Setup(a => a.PersonnelList).Returns(() => new List<PersonStandardModel>
            {
                new()
                {
                    Id = id,
                    FullName = "John Smith",
                    Email = "john@test.com"
                }
            });


            _payrollSystemProviderMock.Setup(a => a.PersonnelList).Returns(() => new List<PersonStandardModel>
            {
                new()
                {
                    EmployeeNumber = 1,
                    Title = "CTO",
                    FullName = "John Smith",
                    Mobile = "+123456789",
                    City = "New York City",
                    Address = "Test Address"
                }
            });


            var mergeByNameStrategy = new MergeByNameStrategy(new NullLogger<MergeByNameStrategy>());


            var expectedResult = new PersonStandardModel
            {
                Id = id,
                EmployeeNumber = 1,
                Title = "CTO",
                FullName = "John Smith",
                Mobile = "+123456789",
                Email = "john@test.com",
                City = "New York City",
                Address = "Test Address"
            };



            var director = new Director(new NullLogger<Director>())
                .WithProviders(_azureAdProviderMock.Object, _payrollSystemProviderMock.Object)
                .WithMergingStrategy(mergeByNameStrategy)
                .WithSavingStrategy(_savingStrategy.Object);


            // Act

            var result = await director.MergeAsync();


            // Assert 

            var firstItem = result.First();

            firstItem.Id.Should().Be(expectedResult.Id);
            firstItem.EmployeeNumber.Should().Be(expectedResult.EmployeeNumber);
            firstItem.Title.Should().Be(expectedResult.Title);
            firstItem.FullName.Should().Be(expectedResult.FullName);
            firstItem.Mobile.Should().Be(expectedResult.Mobile);
            firstItem.Email.Should().Be(expectedResult.Email);
            firstItem.City.Should().Be(expectedResult.City);
            firstItem.Address.Should().Be(expectedResult.Address);
        }



        [Fact]
        public async Task SaveMethod_ShouldCallSaveStrategy_WhenInvoked()
        {
            // Arrange

            var id = Guid.NewGuid().ToString();

            _azureAdProviderMock.Setup(a => a.PersonnelList).Returns(() => new List<PersonStandardModel>
            {
                new()
                {
                    Id = id,
                    FullName = "John Smith",
                    Email = "john@test.com"
                }
            });


            _payrollSystemProviderMock.Setup(a => a.PersonnelList).Returns(() => new List<PersonStandardModel>
            {
                new()
                {
                    EmployeeNumber = 1,
                    Title = "CTO",
                    FullName = "John Smith",
                    Mobile = "+123456789",
                    City = "New York City",
                    Address = "Test Address"
                }
            });


            var mergeByNameStrategy = new MergeByNameStrategy(new NullLogger<MergeByNameStrategy>());

            
            var director = new Director(new NullLogger<Director>())
                .WithProviders(_azureAdProviderMock.Object, _payrollSystemProviderMock.Object)
                .WithMergingStrategy(mergeByNameStrategy)
                .WithSavingStrategy(_savingStrategy.Object);


            // Act

            var result = await director.MergeAsync();

            director.Save();



            // Assert 

            _savingStrategy.Verify(s => s.Save(It.IsAny<List<PersonStandardModel>>()), Times.Once);
        }




    }
}
