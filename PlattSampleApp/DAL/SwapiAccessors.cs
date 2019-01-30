using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using PlattSampleApp.Models;
using Newtonsoft.Json.Linq;

namespace PlattSampleApp.DAL
{
    public interface ISwapiAccessors
    {
        List<PlanetModel> RequestAllPlanets();
        PlanetModel RequestPlanetById(int id);
        List<PersonModel> RequestResidentsOfPlanet(string planet);
        List<VehicleModel> RequestAllVehicles();
        List<PersonModel> PeopleSearch(string searchTerm);
        List<PlanetModel> PlanetSearch(string searchTerm);
        List<SpeciesModel> SpeciesSearch(string searchTerm);
        List<VehicleModel> VehicleSearch(string searchTerm);
    }

    public class SwapiAccessors : ISwapiAccessors
    {
        const string SWAPI_URL = "https://swapi.co/api/";
        const string GET_REQUEST_METHOD = "GET";

        public List<PlanetModel> RequestAllPlanets()
        {
            List<PlanetModel> planets = new List<PlanetModel>();

            string requestUrl = $"{SWAPI_URL}planets";

            string response = Request(requestUrl, GET_REQUEST_METHOD);

            planets = GetResultList<PlanetModel>(response);

            return planets;
        }

        public PlanetModel RequestPlanetById(int id)
        {
            PlanetModel planet = null;
            string requestUrl = $"{SWAPI_URL}planets/{id}";

            string response = Request(requestUrl, GET_REQUEST_METHOD);

            if (!string.IsNullOrWhiteSpace(response))
            {
                planet = JsonConvert.DeserializeObject<PlanetModel>(response);
            }

            return planet;
        }

        public List<PersonModel> RequestResidentsOfPlanet(string planet)
        {
            List<PersonModel> residents = null;

            string planetSearchUrl = $"{SWAPI_URL}planets/?search={planet}";
 
            string homeworld = Request(planetSearchUrl, GET_REQUEST_METHOD);

            if(!string.IsNullOrWhiteSpace(homeworld))
            {
                JObject planetJson = JObject.Parse(homeworld);
                List<JToken> residentUrls = planetJson["results"][0]["residents"].ToList();
                List<PersonModel> people = new List<PersonModel>();


                foreach(var url in residentUrls)
                {
                    string personResponse = Request(url.ToObject<string>(), GET_REQUEST_METHOD);
                    people.Add(JsonConvert.DeserializeObject<PersonModel>(personResponse));
                }

                if ( people.Count > 0)
                {
                    residents = people;
                }
            }

            return residents;

        }

        public List<VehicleModel> RequestAllVehicles()
        {
            List<VehicleModel> vehicles = new List<VehicleModel>();

            string requestUrl = $"{SWAPI_URL}vehicles";

            string response = Request(requestUrl, GET_REQUEST_METHOD);

            vehicles = GetResultList<VehicleModel>(response);

            return vehicles;
        }

        public List<PersonModel> PeopleSearch(string searchTerm)
        {
            
            string url = $"{SWAPI_URL}people/?search={searchTerm}";

            string response = Request(url, GET_REQUEST_METHOD);

            return GetResultList<PersonModel>(response);

        }

        public List<PlanetModel> PlanetSearch(string searchTerm)
        {
            string url = $"{SWAPI_URL}planets/?search={searchTerm}";

            string response = Request(url, GET_REQUEST_METHOD);

            return GetResultList<PlanetModel>(response);
        }

        public List<VehicleModel> VehicleSearch(string searchTerm)
        {
            string url = $"{SWAPI_URL}vehicles/?search={searchTerm}";

            string response = Request(url, GET_REQUEST_METHOD);

            return GetResultList<VehicleModel>(response);
        }

        public List<SpeciesModel> SpeciesSearch(string searchTerm)
        {
            string url = $"{SWAPI_URL}species/?search={searchTerm}";

            string response = Request(url, GET_REQUEST_METHOD);

            return GetResultList<SpeciesModel>(response);
        }

        private string Request(string url, string httpMethod)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = httpMethod;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            string responseString;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                responseString = reader.ReadToEnd();
            }

            return responseString;
        }

        private List<T> GetResultList<T>(string response) 
        {
            List<T> resultList = new List<T>();
            JObject jsonResponse = JObject.Parse(response);

            string next = jsonResponse["next"].ToString();
            List<JToken> results = jsonResponse["results"].Children().ToList();

            while (!string.IsNullOrWhiteSpace(next) && !next.Equals("none"))
            {
                response = Request(next, GET_REQUEST_METHOD);
                jsonResponse = JObject.Parse(response);

                next = jsonResponse["next"].ToString();
                results.AddRange(jsonResponse["results"].Children().ToList());
            }

            foreach (var result in results)
            {
                resultList.Add(result.ToObject<T>());
            }

            return resultList;
        }

    }
}