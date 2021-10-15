using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PersonnelInformationMerger.Core.MergeStrategies;
using PersonnelInformationMerger.Core.Models;
using Xunit;

namespace PersonnelInformationMerger.Core.UnitTests.MergeStrategies
{
    public class MergeByNameStrategyTest
    {

        [Fact]
        public void MergeMethod_ShouldReturnExpectedResult_WhenCalled()
        {
            // Arrange

            var id = Guid.NewGuid().ToString();

            var firstPersonnelList = new List<PersonStandardModel>
            {
                new PersonStandardModel
                {
                    Id = id,
                    FullName = "John Smith",
                    Email = "john@test.com"
                }
            };

            var secondPersonnelList = new List<PersonStandardModel>
            {
                new PersonStandardModel
                {
                    EmployeeNumber = 1,
                    Title = "CTO",
                    FullName = "John Smith",
                    Mobile = "+123456789",
                    City = "New York City",
                    Address = "Test Address"
                }
            };

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


            var mergingStrategy = new MergeByNameStrategy(new NullLogger<MergeByNameStrategy>());


            // Act
            var result = mergingStrategy.Merge(firstPersonnelList, secondPersonnelList);


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
    }
}
