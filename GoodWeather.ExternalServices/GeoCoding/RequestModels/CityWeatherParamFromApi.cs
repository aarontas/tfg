namespace GoodWeather.ExternalServices.GeoCoding.RequestModels;

public class CityWeatherParamFromApi
{
    public List<CityParamFromApi> results { get; set; }
    public double generationtime_ms { get; set; }
}

public class CityParamFromApi
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    public double elevation { get; set; }
    public string feature_code { get; set; }
    public string country_code { get; set; }
    public int admin1_id { get; set; }
    public int admin2_id { get; set; }
    public int admin3_id { get; set; }
    public string timezone { get; set; }
    public int population { get; set; }
    public List<string> postcodes { get; set; }
    public int country_id { get; set; }
    public string country { get; set; }
    public string admin1 { get; set; }
    public string admin2 { get; set; }
    public string admin3 { get; set; }
}