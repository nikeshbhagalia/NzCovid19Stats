using Covid19Nz.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Covid19Nz.Actions
{
    public interface IScraperActions
    {
        JArray GetRegionDetails();

        JArray GetConfirmedCaseDetails();

        JArray GetProbableCaseDetails();

        JArray GetSummary();
    }
}
