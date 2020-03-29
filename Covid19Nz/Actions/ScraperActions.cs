using Covid19Nz.Models;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Covid19Nz.Actions
{
    public class ScraperActions : IScraperActions
    {
        private readonly Sources _sources;
        private readonly HtmlWeb _htmlWeb;

        public ScraperActions(Sources sources)
        {
            _sources = sources;
            _htmlWeb = new HtmlWeb();
        }

        public JArray GetRegionDetails()
        {
            var document = _htmlWeb.Load(_sources.RegionDetailsUrl);
            var regionDetails = GetContentDynamically(document, _sources.RegionDetailsXpath);

            return regionDetails;
        }

        public JArray GetConfirmedCaseDetails()
        {
            var document = _htmlWeb.Load(_sources.CaseDetailsUrl);
            var caseDetails = GetContentDynamically(document, _sources.ConfirmedCaseDetailsXpath);

            return caseDetails;
        }

        public JArray GetProbableCaseDetails()
        {
            var document = _htmlWeb.Load(_sources.CaseDetailsUrl);
            var caseDetails = GetContentDynamically(document, _sources.ProbableCaseDetailsXpath);

            return caseDetails;
        }

        public JArray GetSummary()
        {
            var document = _htmlWeb.Load(_sources.RegionDetailsUrl);
            var summary = GetContentDynamically(document, _sources.SummaryXpath);

            return summary;
        }

        private JArray GetContentDynamically(HtmlDocument document, string xpath)
        {
            var table = document.DocumentNode.SelectSingleNode(xpath);
            var propertyNames = table.ChildNodes.First(cn => cn.Name == "thead")
                .ChildNodes.First(cn => cn.Name == "tr")
                .ChildNodes.Where(cn => cn.Name == "th")
                .Select(th => HttpUtility.HtmlDecode(th.InnerText).Trim())
                .ToList();

            var tableBody = table.ChildNodes.First(cn => cn.Name == "tbody");
            var tableRows = tableBody.ChildNodes.Where(n => n.Name == "tr");

            var tableData = tableRows.Select(r => r.ChildNodes.Where(d => d.Name == "td"));

            var details = new JArray();
            foreach (var data in tableData)
            {
                var detail = new JObject();
                for (var index = 0; index < propertyNames.Count; index++)
                {
                    var propertyName = propertyNames[index];
                    var content = FormatString(data.ElementAt(index).InnerText);

                    JProperty property;
                    if (Int32.TryParse(content, out var number))
                    {
                        property = new JProperty(propertyName, number);
                    }
                    else
                    {
                        property = new JProperty(propertyName, content);
                    }

                    detail.Add(property);
                }

                details.Add(detail);
            }

            return details;
        }

        private List<CaseDetails> ToCaseDetails(HtmlDocument document, string xpath)
        {
            var tableBody = document.DocumentNode.SelectSingleNode(xpath);
            var tableRows = tableBody.ChildNodes.Where(n => n.Name == "tr");

            var caseDetails = tableRows.Select(r => r.ChildNodes.Where(d => d.Name == "td"))
                .Select(c => new CaseDetails
                {
                    ReportDate = FormatString(c.ElementAt(0).InnerText),
                    Sex = FormatString(c.ElementAt(1).InnerText),
                    AgeGroup = FormatString(c.ElementAt(2).InnerText),
                    DHB = FormatString(c.ElementAt(3).InnerText),
                    Overseas = FormatString(c.ElementAt(4).InnerText),
                    LastCityBeforeNZ = FormatString(c.ElementAt(5).InnerText),
                    FlightNo = FormatString(c.ElementAt(6).InnerText),
                    DepartureDate = FormatString(c.ElementAt(7).InnerText),
                    ArrivalDate = FormatString(c.ElementAt(8).InnerText)
                }).ToList();

            return caseDetails;
        }

        private string FormatString(string value)
        {
            return HttpUtility.HtmlDecode(value).Trim().Replace("\t", string.Empty);
        }
    }
}
