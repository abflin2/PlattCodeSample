using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlattSampleApp.Models
{
    public class PlanetDetailsViewModel
    {
        public string Name { get; set; }

        public string Population { get; set; }

        public string Diameter { get; set; }

        public string Terrain { get; set; }

        public string LengthOfYear { get; set; }

        public string FormattedPopulation => Population == "unknown" ? "unknown" : long.Parse(Population).ToString("N0");

        public double NumberDiameter => Diameter == "unknown" ? -1 : Convert.ToDouble(Diameter);
    }
}