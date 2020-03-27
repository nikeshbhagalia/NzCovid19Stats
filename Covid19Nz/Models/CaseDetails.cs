using Newtonsoft.Json;

namespace Covid19Nz.Models
{
    public class CaseDetails
    {
        [JsonProperty(nameof(Case))]
        public int Case { get; set; }

        [JsonProperty(nameof(DHB))]
        public string DHB { get; set; }

        [JsonProperty(nameof(Age))]
        public string Age { get; set; }

        [JsonProperty(nameof(Gender))]
        public string Gender { get; set; }

        [JsonProperty(nameof(Details))]
        public string Details { get; set; }
    }
}