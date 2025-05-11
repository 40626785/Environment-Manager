CREATE TRIGGER trg_after_insert_weather_data
ON weather_data
AFTER INSERT
AS
BEGIN
    EXEC pkg_archive_weather_data;
END;

