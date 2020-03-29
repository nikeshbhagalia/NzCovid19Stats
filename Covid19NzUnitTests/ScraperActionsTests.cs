using Covid19Nz.Actions;
using Covid19Nz.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

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
        public void GetConfirmedCaseDetailsShouldNotBeEmpty()
        {
            var caseDetails = _scraperActions.GetConfirmedCaseDetails();

            Assert.IsNotNull(caseDetails);
            Assert.IsNotEmpty(caseDetails);
        }

        [Test]
        public void GetProbableCaseDetailsShouldNotBeEmpty()
        {
            var caseDetails = _scraperActions.GetProbableCaseDetails();

            Assert.IsNotNull(caseDetails);
            Assert.IsNotEmpty(caseDetails);
        }

        [Test]
        public void GetSummaryShouldNotBeEmpty()
        {
            var summary = _scraperActions.GetSummary();

            Assert.IsNotNull(summary);
            Assert.IsNotEmpty(summary);
        }
    }
}