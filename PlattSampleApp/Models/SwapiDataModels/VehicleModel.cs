using Newtonsoft.Json;

namespace PlattSampleApp.Models
{
    public class VehicleModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("cost_in_credits")]
        public string Cost { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("vehicle_class")]
        public string Class { get; set; }

        [JsonProperty("length")]
        public string Length { get; set; }

        [JsonProperty("crew")]
        public string CrewSize { get; set; }

        [JsonProperty("passengers")]
        public string PassengerCapacity { get; set; }

        [JsonProperty("max_atmosphering_speed")]
        public string MaxAtmosphereSpeed { get; set; }

        [JsonProperty("cargo_capacity")]
        public string CargoCapacity { get; set; }

        [JsonProperty("consumables")]
        public string ConsumablesTimeLimit { get; set; }

    }
}