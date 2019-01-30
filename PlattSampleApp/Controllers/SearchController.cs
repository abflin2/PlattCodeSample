using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlattSampleApp.Models;


namespace PlattSampleApp.Controllers
{
    public class SearchController : Controller
    {
        private SwapiRepo repo = new SwapiRepo();

        public ActionResult Search()
        {
            var model = new SearchViewModel();

            return View(model);
        }

        public ActionResult DoSearch(string query, Enums.SearchType searchType)
        {
            switch (searchType)
            {
                case Enums.SearchType.Character:
                    return View("CharacterSearchResults", 
                        repo.GetCharacterSearchResults(query));
                case Enums.SearchType.Planet:
                    return View("PlanetSearchResults",
                        repo.GetPlanetSearchResults(query));
                case Enums.SearchType.Species:
                    return View("SpeciesSearchResults",
                        repo.GetSpeciesSearchResults(query));
                case Enums.SearchType.Vehicle:
                    return View("VehicleSearchResults",
                        repo.GetVehicleSearchResults(query));
            }

            return View("Error");
        }
    }
}