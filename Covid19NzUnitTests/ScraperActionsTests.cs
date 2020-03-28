using Covid19Nz.Actions;
using Covid19Nz.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Linq;

namespace Covid19NzUnitTests
{
    public class ScraperActionsTests
    {
        private IScraperActions _scraperActions;

        [OneTimeSetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.dev.json", optional: false, reloadOnChange: true)
                .Build();

            var sources = configuration.GetSection(nameof(Sources)).Get<Sources>();

            _scraperActions = new ScraperActions(sources);
        }

        [Test]
        public void GetRegionDetailsShouldNotBeEmpty()
        {
            var regionDetails = _scraperActions.GetRegionDetails();

            Assert.IsNotNull(regionDetails);
            Assert.IsNotEmpty(regionDetails);
        }

        [Test]
        public void GetCaseDetailsShouldNotBeEmpty()
        {
            var caseDetails = _scraperActions.GetCaseDetails();

            Assert.IsNotNull(caseDetails);
            Assert.IsNotEmpty(caseDetails);
        }
    }
}