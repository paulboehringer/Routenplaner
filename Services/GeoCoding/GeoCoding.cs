﻿using Geocoding;
using Geocoding.Google;
using Services.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.GeoCoding
{
    public class GeoCoding : IGeoCoding
    {
        private const string apiKey = "AIzaSyD8NRnffjViotGemHdsVfW3S4i3sbrWbPk";
        private IEnumerable<Address> addresses;
        private IGeocoder geocoder;

        public GeoCoding()
        {
            geocoder = new GoogleGeocoder() { ApiKey = apiKey };
        }

        public async Task<IEnumerable<LocationDto>> GetAdresses(string data)
        {
            try
            {
                addresses = await geocoder.GeocodeAsync(data);
            }
            catch (Exception ex)
            {
                var errorLocations = new List<LocationDto>()
                {
                    {
                        new LocationDto()
                        {
                            Error = ex.Message
                        }
                    }
                };
                return errorLocations;
            }
            return GetLocations(addresses);
        }

        private IEnumerable<LocationDto> GetLocations(IEnumerable<Address> addresses)
        {
            foreach (var item in addresses)
            {
                yield return new LocationDto()
                {
                    Longitude = item.Coordinates.Longitude,
                    Latitude = item.Coordinates.Latitude,
                    FormatedAddress = item.FormattedAddress
                };
            }
        }
    }
}

