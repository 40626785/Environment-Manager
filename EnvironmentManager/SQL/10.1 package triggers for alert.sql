CREATE TRIGGER trg_AfterInsert_WeatherData
ON [dbo].[weather_data]
AFTER INSERT
AS
BEGIN
EXEC [dbo].[CheckWeatherDataAnomalies];
END;

