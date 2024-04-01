using System.Globalization;

namespace GoodWeather.ExternalServices.Weather.RequestModels;

public class CityParameters
{
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Hourly { get; set; }

    public CityParameters(double latitude, double longitude, DateTime startDate, DateTime endDate, string hourly)
    {
        Latitude = Math.Round(latitude, 2).ToString(CultureInfo.InvariantCulture);
        Longitude = Math.Round(longitude, 2).ToString(CultureInfo.InvariantCulture);
        StartDate = startDate.ToString("yyyy-MM-dd");
        EndDate = endDate.ToString("yyyy-MM-dd");
        Hourly = hourly;
    }
}



