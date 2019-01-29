using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        List<Planet> RequestAllPlanets();
        Planet RequestPlanetById(int id);
        List<Person> RequestResidentsOfPlanet(string planet);
        List<Vehicle> RequestAllVehicles();
    }

    public class SwapiAccessors : ISwapiAccessors
    {
        const string SWAPI_URL = "https://swapi.co/api/";
        const string GET_REQUEST_METHOD = "GET";

        public List<Planet> RequestAllPlanets()
        {
            List<Planet> planets = new List<Planet>();

            string requestUrl = $"{SWAPI_URL}planets";

            string response = Request(requestUrl, GET_REQUEST_METHOD);

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

            foreach(var result in results)
            {
                planets.Add(result.ToObject<Planet>());
            }

            return planets;
        }

        public Planet RequestPlanetById(int id)
        {
            Planet planet = null;
            string requestUrl = $"{SWAPI_URL}planets/{id}";

            string response = Request(requestUrl, GET_REQUEST_METHOD);

            if (!string.IsNullOrWhiteSpace(response))
            {
                planet = JsonConvert.DeserializeObject<Planet>(response);
            }

            return planet;
        }

        public List<Person> RequestResidentsOfPlanet(string planet)
        {
            List<Person> residents = null;

            string planetSearchUrl = $"{SWAPI_URL}planets/?search={planet}";
 
            string homeworld = Request(planetSearchUrl, GET_REQUEST_METHOD);

            if(!string.IsNullOrWhiteSpace(homeworld))
            {
                JObject planetJson = JObject.Parse(homeworld);
                List<JToken> residentUrls = planetJson["results"][0]["residents"].ToList();
                List<Person> people = new List<Person>();


                foreach(var url in residentUrls)
                {
                    string personResponse = Request(url.ToObject<string>(), GET_REQUEST_METHOD);
                    people.Add(JsonConvert.DeserializeObject<Person>(personResponse));
                }

                if ( people.Count > 0)
                {
                    residents = people;
                }
            }

            return residents;

        }

        public List<Vehicle> RequestAllVehicles()
        {
            List<Vehicle> vehicles = new List<Vehicle>();

            string requestUrl = $"{SWAPI_URL}vehicles";

            string response = Request(requestUrl, GET_REQUEST_METHOD);

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
                vehicles.Add(result.ToObject<Vehicle>());
            }

            return vehicles;
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
    }
}