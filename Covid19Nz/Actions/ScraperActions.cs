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

            var regionDetails = tableRows.Select(r => r.ChildNodes.Where(d => d.Name == "td"))
                .Select(c => new RegionDetails
                {
                    DHB = FormatString(c.ElementAt(0).InnerText),
                    TotalCases = Int32.Parse(FormatString(c.ElementAt(1).InnerText))
                }).ToList();

            return regionDetails;
        }

        public List<CaseDetails> GetCaseDetails()
        {
            var document = _htmlWeb.Load(_sources.CaseDetailsUrl);
            var tableBody = document.DocumentNode.SelectSingleNode(CaseDetailsXpath);
            var tableRows = tableBody.ChildNodes.Where(n => n.Name == "tr");

            var caseDetails = tableRows.Select(r => r.ChildNodes.Where(d => d.Name == "td"))
                .Select(c => new CaseDetails
                {
                    Case = Int32.Parse(FormatString(c.ElementAt(0).InnerText)),
                    DHB = FormatString(c.ElementAt(1).InnerText),
                    Age = FormatString(c.ElementAt(2).InnerText),
                    Gender = FormatString(c.ElementAt(3).InnerText),
                    Details = FormatString(c.ElementAt(4).InnerText)
                }).ToList();

            return caseDetails;
        }

        private string FormatString(string value)
        {
            return HttpUtility.HtmlDecode(value).Trim();
        }
    }
}
