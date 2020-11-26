using Covid19Nz.Actions;
using Covid19Nz.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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

        [HttpGet("regiondetails")]
        public ActionResult<JArray> GetRegionDetails()
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

        [HttpGet("confirmedcasedetails")]
        public ActionResult<JArray> GetConfirmedCaseDetails()
        {
            try
            {
                return new JArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("probablecasedetails")]
        public ActionResult<JArray> GetProbableCaseDetails()
        {
            try
            {
                return _scraperActions.GetProbableCaseDetails();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("summary")]
        public ActionResult<JArray> GetSummary()
        {
            try
            {
                return _scraperActions.GetSummary();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
