using Covid19Nz.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Covid19Nz.Actions
{
    public class ScraperActions : IScraperActions
    {
        private const string RegionDetailsXpath = "//caption[contains(.,'Total cases by DHB')]/..//tbody";
        private const string CaseDetailsXpath = "//caption[contains(.,'Confirmed COVID-19 cases')]/..//tbody";

        private readonly Sources _sources;
        private readonly HtmlWeb _htmlWeb;

        public ScraperActions(Sources sources)
        {
            _sources = sources;
            _htmlWeb = new HtmlWeb();
        }

        public List<RegionDetails> GetRegionDetails()
        {
            var document = _htmlWeb.Load(_sources.RegionDetailsUrl);
            var tableBody = document.DocumentNode.SelectSingleNode(RegionDetailsXpath);
            var tableRows = tableBody.ChildNodes.Where(n => n.Name == "tr");

            var regionDetails = tableRows.Select(r => r.InnerText.Trim().Replace("\t", "")
                    .Split(new[] { "\n" }, StringSplitOptions.None))
                .Select(c => new RegionDetails
                {
                    DHB = HttpUtility.HtmlDecode(c[0]),
                    TotalCases = Int32.Parse(c[1])
                }).ToList();

            return regionDetails;
        }

        public List<CaseDetails> GetCaseDetails()
        {
            var document = _htmlWeb.Load(_sources.CaseDetailsUrl);
            var tableBody = document.DocumentNode.SelectSingleNode(CaseDetailsXpath);
            var tableRows = tableBody.ChildNodes.Where(n => n.Name == "tr");

            var caseDetails = tableRows.Select(r => r.InnerText.Trim().Replace("\t", "")
                    .Split(new[] { "\n" }, StringSplitOptions.None))
                .Select(c => new CaseDetails
                {
                    Case = Int32.Parse(c[0]),
                    DHB = HttpUtility.HtmlDecode(c[1]),
                    Age = c[2],
                    Gender = c[3],
                    Details = HttpUtility.HtmlDecode(c[4])
                }).ToList();

            return caseDetails;
        }
    }
}
