using Services.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.StaticMaps
{
    public class StaticMaps: IStaticMaps, IDisposable
    {
        private const string baseUrl = "https://maps.googleapis.com/maps/api/staticmap?&key=AIzaSyD8Lo3lGuAO3TBML-LPZWjA7T4nAA4eB_k&format=gif&size=800x500&maptype=roadmap";
        readonly HttpClient client;
        
        public StaticMaps()
        {
            client = new HttpClient();
        }

        public async Task<StreamDto> GetMap(IEnumerable<LocationDto> locations)
        {
            var url = GenerateUrl(locations);
            if (url == null)
            {
                return new StreamDto()
                {
                    Stream = null,
                    Error = "Es muss mindestens eines der Felder Von, Via oder Nach ausgewählt werden!"
                };
            }

            Stream data = null;

            try
            {
                data = await client.GetStreamAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                return new StreamDto()
                {
                    Stream = null,
                    Error = ex.Message
                };
            }

            return new StreamDto()
            {
                Stream = data,
                Error = string.Empty
            };
        }


        /// <summary>
        /// Generates the needed url depending on values delivered from UI
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        private string GenerateUrl(IEnumerable<LocationDto> locations)
        {
            if (locations == null || locations.Count() == 0) { return null; }

            QueryStringBuilder qsb = new QueryStringBuilder();
            qsb.Append(baseUrl);

            // if only one location is selected, center the location and zoom to factor 15
            if (locations.Count() == 1)
            {
                qsb.Append("zoom", "15");
                qsb.Append("center", locations.First().FormatedLatLon);
            }
               
            int counter = 0;
            var color = "red";
            var label = "V";
            foreach (var item in locations)
            {
                if (counter == 0) { color = "blue"; label = "S";  }
                else if (counter == locations.Count()-1) { color = "green"; label = "Z";  }
                else if (counter > 0 && counter < locations.Count() - 1) { color = "red"; label = "V"; }
                qsb.Append("markers", GetMarker(item, color, label));
                counter++;
            }
            return qsb.ToString();
        }

        private string GetMarker(LocationDto location, string color, string label)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("color:");
            sb.Append(color);
            sb.Append("%7Clabel:");
            sb.Append(label);
            sb.Append("%7C");
            sb.Append(location.FormatedLatLon);
            return sb.ToString(); 
        }

        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(client);
        }
    }
}
