using Newtonsoft.Json.Linq;
using Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Services.Directions
{
    public class Directions: IDirections, IDisposable
    {
        private const string baseUrl = "https://maps.googleapis.com/maps/api/directions/json?key=AIzaSyD8Lo3lGuAO3TBML-LPZWjA7T4nAA4eB_k";

        readonly HttpClient client;

        public Directions()
        {
            client = new HttpClient();
        }

        public async Task<DirectionDto> GetDirections(IEnumerable<LocationDto> locations)
        {
            var url = GenerateUrl(locations);
            if (url == null)
            {
                return new DirectionDto()
                {
                    Directions = string.Empty,
                    Error = "Es muss mindestens eines der Felder Von, Via oder Nach ausgewählt werden!"
                };
            }

            string jsonResult = null;

            try
            {
                jsonResult = await client.GetStringAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                return new DirectionDto()
                {
                    Directions = string.Empty,
                    Error = ex.Message
                };
            }

            return ParseJason(jsonResult);
        }

        private DirectionDto ParseJason(string jsonResult)
        {
            var dto = new DirectionDto();
            try
            {
                dto.Parsed = JObject.Parse(jsonResult);
                JavaScriptSerializer parser = new JavaScriptSerializer();
                var responseData = parser.Deserialize<RootObject>(dto.Parsed.ToString());
                dto.Routes = responseData;
                return dto;
            }
            catch (Exception ex) 
            {
                dto.Error = ex.Message;
                return dto;
            }
        }

        /// <summary>
        /// Generates the needed url depending on values delivered from UI
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        private string GenerateUrl(IEnumerable<LocationDto> locations)
        {
            if (locations == null || locations.Count() < 2) { return null; }

            QueryStringBuilder qsb = new QueryStringBuilder();
            qsb.Append(baseUrl);

            var origin = locations.Where(x => x.LocationType == LocationType.Origin).FirstOrDefault();
            qsb.Append("origin", origin.FormatedLatLon);

            var destination = locations.Where(x => x.LocationType == LocationType.Destination).FirstOrDefault();
            qsb.Append("destination", destination.FormatedLatLon);

            var waypoints = locations.Where(x => x.LocationType == LocationType.WayPoint);
            qsb.Append("waypoints", GetWayPoints(waypoints));

            return qsb.ToString(); 
        }

        private string GetWayPoints(IEnumerable<LocationDto> locations)
        {
            var wpString = "via:{0}";
            var waypoints = string.Empty;
            var counter = 0;
            foreach (var item in locations)
            {
                counter++;
                var wp = string.Format(wpString, item.FormatedLatLon);
                if (counter != locations.Count())
                {
                    wp += "|";
                }
                waypoints += wp;
            }

            return waypoints; 
        }

        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(client);
        }
    }
}
