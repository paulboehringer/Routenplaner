namespace Services.Core
{
    /// <summary>
    /// Class for data exchange between Services and Routenplaner
    /// </summary>
    public class LocationDto
    {
        private double latitude;
        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
                GetFormatedLatLon();
            }
        }

        private double longitude;
        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
                GetFormatedLatLon();
            }
        }
        public string FormatedAddress { get; set; }
        public LocationType LocationType { get; set; }
        public string FormatedLatLon { get; private set; }
        public string Error { get; set; }

        private void GetFormatedLatLon()
        {
            FormatedLatLon = Latitude.ToString().Replace(',', '.') + "," + Longitude.ToString().Replace(',', '.');
        }
    }
}
