CREATE TRIGGER trg_after_insert_water_quality
ON Water_Quality
AFTER INSERT
AS
BEGIN
    EXEC pkg_archive_water_quality;
END;

