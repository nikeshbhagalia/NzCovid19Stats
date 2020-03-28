using Covid19Nz.Actions;
using Covid19Nz.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Covid19Nz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IScraperActions _scraperActions;

        public StatsController(IScraperActions scraperActions)
        {
            _scraperActions = scraperActions;
        }

        [HttpGet("regionDetails")]
        public ActionResult<ICollection<RegionDetails>> GetRegionDetails()
        {
            try
            {
                return _scraperActions.GetRegionDetails();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("caseDetails")]
        public ActionResult<ICollection<CaseDetails>> GetCaseDetails()
        {
            try
            {
                return _scraperActions.GetConfirmedCaseDetails();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
