using Covid19Nz.Models;
using System.Collections.Generic;

namespace Covid19Nz.Actions
{
    public interface IScraperActions
    {
        List<RegionDetails> GetRegionDetails();

        List<CaseDetails> GetConfirmedCaseDetails();
    }
}
