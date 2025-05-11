CREATE TRIGGER after_insert_air_quality
ON Air_Quality
AFTER INSERT
AS
BEGIN
    EXEC pkg_archive_air_quality;
END;
