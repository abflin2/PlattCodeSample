using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PlattSampleApp.Models
{
    public class Vehicle
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("cost_in_credits")]
        public string Cost { get; set; }
    }
}