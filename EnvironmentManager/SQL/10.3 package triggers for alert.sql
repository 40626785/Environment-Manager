CREATE TRIGGER trg_AfterInsert_AirQuality
ON [dbo].[Air_Quality]
AFTER INSERT
AS
BEGIN
    EXEC [dbo].[CheckAirQualityAnomalies];
END;
