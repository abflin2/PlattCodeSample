using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlattSampleApp.Models;

namespace PlattSampleApp.Controllers
{
    public class HomeController : Controller
    {
        private SwapiRepo repo = new SwapiRepo(); 

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllPlanets()
        {
            var model = repo.GetAllPlanets();

            return View(model);
        }

        public ActionResult GetPlanetTwentyTwo(int planetid)
        {
            var model = repo.GetPlanet(planetid);

            return View(model);
        }

        public ActionResult GetResidentsOfPlanetNaboo(string planetname)
        {
            var model = repo.GetResidentsOfPlanet(planetname);

            return View(model);
        }

        public ActionResult VehicleSummary()
        {
            var model = repo.GetVehiclesSummary();

            return View(model);
        }
    }
}