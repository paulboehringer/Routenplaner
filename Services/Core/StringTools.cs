namespace Services.Core
{
    public class StringTools
    {
 
        private string GetCoordinates(LocationDto location)
        {
            return location.Latitude.ToString().Replace(',', '.') + "," + location.Longitude.ToString().Replace(',', '.');
        }

    }
}
