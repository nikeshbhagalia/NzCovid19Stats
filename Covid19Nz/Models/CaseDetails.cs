using Newtonsoft.Json;

namespace Covid19Nz.Models
{
    public class CaseDetails
    {
        [JsonProperty("Report Date")]
        public string ReportDate { get; set; }

        [JsonProperty(nameof(Sex))]
        public string Sex { get; set; }

        [JsonProperty("Age group")]
        public string AgeGroup { get; set; }

        [JsonProperty(nameof(DHB))]
        public string DHB { get; set; }

        [JsonProperty(nameof(Overseas))]
        public string Overseas { get; set; }

        [JsonProperty("Last City before NZ")]
        public string LastCityBeforeNZ { get; set; }

        [JsonProperty("Flight No")]
        public string FlightNo { get; set; }

        [JsonProperty("Departure date")]
        public string DepartureDate { get; set; }

        [JsonProperty("Arrival date")]
        public string ArrivalDate { get; set; }
    }
}