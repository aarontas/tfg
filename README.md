# GoodWeather

GoodWeatheris a .Net API REST made as part of the final degree project.

## Author
[LinkedIn](https://www.linkedin.com/in/aaron-sanchez-torres-gc8/)

## Architecture
This solution follows clean architecture and CQRS pattern.

## Use case
This repository aims to return a predetermined list of cities with the temperature.
With a city, a start date, and an end date, return the average temperature in the last 5 years and a score depends on the temperature.

To achieve it, the API uses two external APIs to get the geolocalization of the city and with this geocoding, get the historic temperature.

Later, get the average and generate the score related to this temperature.

