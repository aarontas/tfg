namespace ExternalServices.Weather.RequestModels;

public record CityParameters(
    double Latitude,
    double Longitude,
    DateTime StartDate,
    DateTime EndDate,
    string Hourly);
