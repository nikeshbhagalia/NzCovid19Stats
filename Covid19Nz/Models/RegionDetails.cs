using Newtonsoft.Json;

namespace Covid19Nz.Models
{
    public class RegionDetails
    {
        [JsonProperty(nameof(DHB))]
        public string DHB { get; set; }

        [JsonProperty("Total Cases")]
        public int TotalCases { get; set; }
    }
}
