﻿using Covid19Nz.Models;
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

        private JArray GetContentDynamically(HtmlDocument document, string tableXPath, string emptyKeyReplacement = "")
        {
            var tableRows = document.DocumentNode.SelectNodes($"{tableXPath}//tr");

            var propertyNames = tableRows.First()
                .ChildNodes.Where(cn => cn.Name.StartsWith("t"))
                .Select(th => HttpUtility.HtmlDecode(th.InnerText).Trim())
                .Select(pn => string.IsNullOrEmpty(pn) ? emptyKeyReplacement : pn)
                .ToList();

            var tableData = tableRows
                .Skip(1)
                .Select(r => r.ChildNodes
                    .Where(cn => cn.Name.StartsWith("t"))
                    .ToList());
                
            var details = new JArray();
            foreach (var data in tableData)
            {
                if (data.Count() < propertyNames.Count)
                {
                    for (var index = 0; index < data.Count(); index++)
                    {
                        var hasColspan = Int32.TryParse(data[index].Attributes.SingleOrDefault(a => a.Name == "colspan")?.Value, out var colspanValue);
                        if (hasColspan && colspanValue > 1)
                        {
                            var htmlNode = data[index].NextSibling.NextSibling.Clone();
                            
                            for (var colspan = 1; colspan < colspanValue; colspan++)
                            {
                                data.Insert(index + colspan, htmlNode);
                            }
                        }
                    }
                }

                var detail = new JObject();
                for (var index = 0; index < propertyNames.Count; index++)
                {
                    var propertyName = propertyNames[index];
                    var content = FormatString(data[index].InnerText);

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
