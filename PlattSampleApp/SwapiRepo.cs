using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PlattSampleApp.Models;
using PlattSampleApp.DAL;

namespace PlattSampleApp
{
    

    public interface IRepository
    {
        AllPlanetsViewModel GetAllPlanets();
        SinglePlanetViewModel GetPlanet(int planetId);
        PlanetResidentsViewModel GetResidentsOfPlanet(string planet);
    }

    public class SwapiRepo : IRepository
    {
        private SwapiAccessors swapiAccessors = new SwapiAccessors();

        public AllPlanetsViewModel GetAllPlanets()
        {
            List<Planet> result = swapiAccessors.RequestAllPlanets();

            var allPlanets = new AllPlanetsViewModel();
            allPlanets.Planets.AddRange(result.Select(p => new PlanetDetailsViewModel()
            {
                Name = p.Name,
                Population = p.Population,
                Diameter = p.Diameter,
                Terrain = p.Terrain,
                LengthOfYear = p.OrbitalPeriod
            }).OrderByDescending(o => o.NumberDiameter));

            allPlanets.AverageDiameter = allPlanets.Planets
                .Select(n => n.NumberDiameter)
                .Where(a => a > 0).Average();

            return allPlanets;
        }

        public SinglePlanetViewModel GetPlanet(int planetId)
        {
            Planet p = swapiAccessors.RequestPlanetById(planetId);

            SinglePlanetViewModel planet = p == null ? null : 
                new SinglePlanetViewModel()
                {
                    Name = p.Name,
                    LengthOfDay = p.RotationPeriod,
                    LengthOfYear = p.OrbitalPeriod,
                    Diameter = p.Diameter,
                    Climate = p.Climate,
                    Gravity = p.Gravity,
                    SurfaceWaterPercentage = p.SurfaceWater,
                    Population = p.Population
                };

            return planet;
        }

        public PlanetResidentsViewModel GetResidentsOfPlanet(string planet)
        {
            PlanetResidentsViewModel residents = new PlanetResidentsViewModel();

            List<Person> results = swapiAccessors.RequestResidentsOfPlanet(planet);

            if(results != null)
            {
                residents.Residents.AddRange(results.Select(r => new ResidentSummary()
                {
                    Name = r.Name,
                    Height = r.Height,
                    Weight = r.Mass,
                    Gender = r.Gender,
                    HairColor = r.HairColor,
                    EyeColor = r.EyeColor,
                    SkinColor = r.SkinColor
                }).OrderBy(o => o.Name));
            }

            return residents;

        }

        public VehicleSummaryViewModel GetVehiclesSummary()
        {
            VehicleSummaryViewModel vehiclesSummary = new VehicleSummaryViewModel();

            List<Vehicle> vehicles = swapiAccessors.RequestAllVehicles();

            List<Vehicle> vehiclesWithCost = vehicles.Where(v => v.Cost != "unknown").ToList();
            var query = vehiclesWithCost
                .GroupBy(v => v.Manufacturer,
                    (man) => new VehicleStatsViewModel()
                    {
                        ManufacturerName = man.Manufacturer,
                        VehicleCount = vehiclesWithCost.Where(count => count.Manufacturer == man.Manufacturer).Count(),
                        AverageCost =
                            vehiclesWithCost
                                .Where(c => (c.Manufacturer == man.Manufacturer ))
                                .Average(a => Convert.ToDouble(a.Cost))
                    });

            vehiclesSummary.Details.AddRange(query.Where(m => !m.Key.Equals("Unknown")).Select(k => k.First<VehicleStatsViewModel>()).ToList().OrderByDescending(x => x.VehicleCount).ThenByDescending(y => y.AverageCost));
            vehiclesSummary.ManufacturerCount = vehiclesSummary.Details.Count();
            vehiclesSummary.VehicleCount = vehicles.Where(c => c.Cost != "unknown").Count();
            return vehiclesSummary;
        }
    }
}