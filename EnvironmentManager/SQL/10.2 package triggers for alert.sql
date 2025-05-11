CREATE TRIGGER trg_AfterInsert_WaterQuality
ON [dbo].[Archive_Water_Quality]
AFTER INSERT
AS
BEGIN
EXEC [dbo].[CheckWaterQualityAnomalies];
END;

