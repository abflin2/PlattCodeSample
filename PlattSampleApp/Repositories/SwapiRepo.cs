using System;
using System.Collections.Generic;
using System.Linq;
using PlattSampleApp.Models;
using PlattSampleApp.DAL;

namespace PlattSampleApp
{
    
    public interface IRepository
    {
        AllPlanetsViewModel GetAllPlanets();
        SinglePlanetViewModel GetPlanet(int planetId);
        PlanetResidentsViewModel GetResidentsOfPlanet(string planet);
        VehicleSummaryViewModel GetVehiclesSummary();
        SearchResultsViewModel<PersonModel> GetCharacterSearchResults(string searchTerm);
        SearchResultsViewModel<PlanetModel> GetPlanetSearchResults(string searchTerm);
        SearchResultsViewModel<SpeciesModel> GetSpeciesSearchResults(string searchTerm);
        SearchResultsViewModel<VehicleModel> GetVehicleSearchResults(string searchTerm);
    }

    public class SwapiRepo : IRepository
    {
        private ISwapiAccessors swapiAccessors;

        public SwapiRepo(ISwapiAccessors accessors = null)
        {
            swapiAccessors = accessors ?? new SwapiAccessors();
        }

        public AllPlanetsViewModel GetAllPlanets()
        {
            List<PlanetModel> result = swapiAccessors.RequestAllPlanets();

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
            PlanetModel p = swapiAccessors.RequestPlanetById(planetId);

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

            List<PersonModel> results = swapiAccessors.RequestResidentsOfPlanet(planet);

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

            List<VehicleModel> vehicles = swapiAccessors.RequestAllVehicles();

            List<VehicleModel> vehiclesWithCost = vehicles.Where(v => v.Cost != "unknown").ToList();
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

        public SearchResultsViewModel<PersonModel> GetCharacterSearchResults(string searchTerm)
        {

            var searchResults = swapiAccessors.PeopleSearch(searchTerm);

            return new SearchResultsViewModel<PersonModel>()
            {
                SearchResults = searchResults.OrderBy(x => x.Name).ToList(),
                OriginalSearchTerm = searchTerm
            };
        }

        public SearchResultsViewModel<PlanetModel> GetPlanetSearchResults(string searchTerm)
        {
            var searchResults = swapiAccessors.PlanetSearch(searchTerm);

            return new SearchResultsViewModel<PlanetModel>()
            {
                SearchResults = searchResults.OrderBy(x => x.Name).ToList(),
                OriginalSearchTerm = searchTerm
            };
        }

        public SearchResultsViewModel<SpeciesModel> GetSpeciesSearchResults(string searchTerm)
        {
            var searchResults = swapiAccessors.SpeciesSearch(searchTerm);

            return new SearchResultsViewModel<SpeciesModel>()
            {
                SearchResults = searchResults.OrderBy(x => x.Name).ToList(),
                OriginalSearchTerm = searchTerm
            };
        }

        public SearchResultsViewModel<VehicleModel> GetVehicleSearchResults(string searchTerm)
        {
            var searchResults = swapiAccessors.VehicleSearch(searchTerm);

            return new SearchResultsViewModel<VehicleModel>()
            {
                SearchResults = searchResults.OrderBy(x => x.Name).ToList(),
                OriginalSearchTerm = searchTerm
            };
        }
    }
}