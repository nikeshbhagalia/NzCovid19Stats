using Covid19Nz.Models;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Covid19Nz.Actions
{
    public class ScraperActions : IScraperActions
    {
        private const string RegionDetailsCacheKey = "RegionDetails";
        private const string ConfirmedCaseDetailsCacheKey = "ConfirmedCaseDetails";
        private const string ProbableCaseDetailsCacheKey = "ProbableCaseDetails";
        private const string SummaryCacheKey = "Summary";

        private readonly Sources _sources;
        private readonly HtmlWeb _htmlWeb;

        public ScraperActions(Sources sources)
        {
            _sources = sources;
            _htmlWeb = new HtmlWeb();
        }

        public JArray GetRegionDetails()
        {
            if (RegionDetails is null)
            {
                var document = _htmlWeb.Load(_sources.RegionDetailsUrl);
                RegionDetails = GetContentDynamically(document, _sources.RegionDetailsXpath);
            }

            return RegionDetails;
        }

        public JArray GetConfirmedCaseDetails()
        {
            if (ConfirmedCaseDetails is null)
            {
                var document = _htmlWeb.Load(_sources.CaseDetailsUrl);
                ConfirmedCaseDetails = GetContentDynamically(document, _sources.ConfirmedCaseDetailsXpath);
            }

            return ConfirmedCaseDetails;
        }

        public JArray GetProbableCaseDetails()
        {
            if (ProbableCaseDetails is null)
            {
                var document = _htmlWeb.Load(_sources.CaseDetailsUrl);
                ProbableCaseDetails = GetContentDynamically(document, _sources.ProbableCaseDetailsXpath);
            }

            return ProbableCaseDetails;
        }

        public JArray GetSummary()
        {
            if (Summary is null)
            {
                var document = _htmlWeb.Load(_sources.RegionDetailsUrl);
                Summary = GetContentDynamically(document, _sources.SummaryXpath, "title");
            }

            return Summary;
        }

        private JArray GetContentDynamically(HtmlDocument document, string xpath, string emptyKeyReplacement = "")
        {
            var table = document.DocumentNode.SelectSingleNode(xpath);
            var propertyNames = table.ChildNodes.First(cn => cn.Name == "thead")
                .ChildNodes.First(cn => cn.Name == "tr")
                .ChildNodes.Where(cn => cn.Name == "th")
                .Select(th => HttpUtility.HtmlDecode(th.InnerText).Trim())
                .Select(pn => string.IsNullOrEmpty(pn) ? emptyKeyReplacement : pn)
                .ToList();

            var tableBody = table.ChildNodes.First(cn => cn.Name == "tbody");
            var tableRows = tableBody.ChildNodes.Where(n => n.Name == "tr");

            var tableData = tableRows.Select(r => r.ChildNodes.Where(d => d.Name == "td" || d.Name == "th"));

            var details = new JArray();
            foreach (var data in tableData)
            {
                if (data.Count() < propertyNames.Count)
                {
                    continue;
                }

                var detail = new JObject();
                for (var index = 0; index < propertyNames.Count; index++)
                {
                    var propertyName = propertyNames[index];
                    var content = FormatString(data.ElementAt(index).InnerText);

                    JProperty property;
                    if (Int32.TryParse(content.Replace(",", ""), out var number))
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

        internal JArray RegionDetails
        {
            get => MemoryCache.Default.Get(RegionDetailsCacheKey) as JArray;
            set
            {
                MemoryCache.Default.Remove(RegionDetailsCacheKey);
                if (value is null)
                {
                    return;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1)
                };

                MemoryCache.Default.Add(RegionDetailsCacheKey, value, cacheItemPolicy);
            }
        }

        internal JArray ConfirmedCaseDetails
        {
            get => MemoryCache.Default.Get(ConfirmedCaseDetailsCacheKey) as JArray;
            set
            {
                MemoryCache.Default.Remove(ConfirmedCaseDetailsCacheKey);
                if (value is null)
                {
                    return;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1)
                };

                MemoryCache.Default.Add(ConfirmedCaseDetailsCacheKey, value, cacheItemPolicy);
            }
        }

        internal JArray ProbableCaseDetails
        {
            get => MemoryCache.Default.Get(ProbableCaseDetailsCacheKey) as JArray;
            set
            {
                MemoryCache.Default.Remove(ProbableCaseDetailsCacheKey);
                if (value is null)
                {
                    return;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1)
                };

                MemoryCache.Default.Add(ProbableCaseDetailsCacheKey, value, cacheItemPolicy);
            }
        }

        internal JArray Summary
        {
            get => MemoryCache.Default.Get(SummaryCacheKey) as JArray;
            set
            {
                MemoryCache.Default.Remove(SummaryCacheKey);
                if (value is null)
                {
                    return;
                }

                var cacheItemPolicy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1)
                };

                MemoryCache.Default.Add(SummaryCacheKey, value, cacheItemPolicy);
            }
        }
    }
}
