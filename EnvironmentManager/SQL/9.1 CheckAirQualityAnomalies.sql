CREATE PROCEDURE [dbo].[CheckAirQualityAnomalies]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @v_procname NVARCHAR(100) = 'CheckAirQualityAnomalies',
        @LogMessage NVARCHAR(MAX),
        @ErrorMessage NVARCHAR(MAX),
        @LocationId INT,
        @Yesterday DATE = CAST(GETDATE() - 1 AS DATE);

    BEGIN TRY
        -- Log start
        SET @LogMessage = 'Starting procedure ' + @v_procname;
        EXEC dbo.LogMessage @LogMessage;

        -- 1. Check for missing 24 hourly inserts per location yesterday
        IF EXISTS (
            SELECT LocationId
            FROM Air_Quality
            WHERE [Date] = @Yesterday
            GROUP BY LocationId
            HAVING COUNT(*) <> 24
        )
        BEGIN
            INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Message)
            SELECT 
                LocationId,
                GETDATE(),
                'AirQualityDataCheck',
                COUNT(*),
                'Less than 24 hourly air quality records for yesterday'
            FROM Air_Quality
            WHERE [Date] = @Yesterday
            GROUP BY LocationId
            HAVING COUNT(*) <> 24;
        END

        -- 2. Check for NULL values in key fields
        IF EXISTS (
            SELECT 1
            FROM Air_Quality
            WHERE Nitrogen_dioxide IS NULL
               OR Sulphur_dioxide IS NULL
               OR PM2_5_particulate_matter IS NULL
               OR PM10_particulate_matter IS NULL
               OR [Date] IS NULL
               OR [Time] IS NULL
               OR LocationId IS NULL
        )
        BEGIN
            INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Message)
            SELECT 
                ISNULL(LocationId, -1),
                GETDATE(),
                'AirQualityNullCheck',
                NULL,
                'One or more NULL values found in Air_Quality'
            FROM Air_Quality
            WHERE Nitrogen_dioxide IS NULL
               OR Sulphur_dioxide IS NULL
               OR PM2_5_particulate_matter IS NULL
               OR PM10_particulate_matter IS NULL
               OR [Date] IS NULL
               OR [Time] IS NULL
               OR LocationId IS NULL;
        END

        -- 3. Loop through each location
        DECLARE LocationCursor CURSOR FOR
        SELECT DISTINCT LocationId FROM Air_Quality;

        OPEN LocationCursor;
        FETCH NEXT FROM LocationCursor INTO @LocationId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- CTE: last 1440 records for this location
            ;WITH Last1440 AS (
                SELECT TOP 1440 *
                FROM Air_Quality
                WHERE LocationId = @LocationId
                ORDER BY ID DESC
            ),
            Stats AS (
                SELECT
                    STDEV(Nitrogen_dioxide) AS StdNO2,
                    STDEV(Sulphur_dioxide) AS StdSO2,
                    STDEV(PM2_5_particulate_matter) AS StdPM25,
                    STDEV(PM10_particulate_matter) AS StdPM10,
                    AVG(Nitrogen_dioxide) AS AvgNO2,
                    AVG(Sulphur_dioxide) AS AvgSO2,
                    AVG(PM2_5_particulate_matter) AS AvgPM25,
                    AVG(PM10_particulate_matter) AS AvgPM10
                FROM Last1440
            )
            SELECT * INTO #TempStats FROM Stats;

            DECLARE @LatestId INT = (
                SELECT MAX(ID) FROM Air_Quality WHERE LocationId = @LocationId
            );

            DECLARE
                @NO2 FLOAT,
                @SO2 FLOAT,
                @PM25 FLOAT,
                @PM10 FLOAT,
                @DT DATETIME;

            SELECT
                @NO2 = Nitrogen_dioxide,
                @SO2 = Sulphur_dioxide,
                @PM25 = PM2_5_particulate_matter,
                @PM10 = PM10_particulate_matter,
                @DT = CAST([Date] AS DATETIME) + CAST([Time] AS DATETIME)
            FROM Air_Quality
            WHERE ID = @LatestId;

            -- Check deviations
            IF ABS(@NO2 - (SELECT AvgNO2 FROM #TempStats)) > (SELECT StdNO2 FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'Nitrogen_dioxide', @NO2, (SELECT StdNO2 FROM #TempStats), 'NO2 outside standard deviation');

            IF ABS(@SO2 - (SELECT AvgSO2 FROM #TempStats)) > (SELECT StdSO2 FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'Sulphur_dioxide', @SO2, (SELECT StdSO2 FROM #TempStats), 'SO2 outside standard deviation');

            IF ABS(@PM25 - (SELECT AvgPM25 FROM #TempStats)) > (SELECT StdPM25 FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'PM2_5_particulate_matter', @PM25, (SELECT StdPM25 FROM #TempStats), 'PM2.5 outside standard deviation');

            IF ABS(@PM10 - (SELECT AvgPM10 FROM #TempStats)) > (SELECT StdPM10 FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'PM10_particulate_matter', @PM10, (SELECT StdPM10 FROM #TempStats), 'PM10 outside standard deviation');

            DROP TABLE #TempStats;

            FETCH NEXT FROM LocationCursor INTO @LocationId;
        END;

        CLOSE LocationCursor;
        DEALLOCATE LocationCursor;

        -- Log end
        SET @LogMessage = 'Finished procedure ' + @v_procname;
        EXEC dbo.LogMessage @LogMessage;
    END TRY

    BEGIN CATCH
        SET @ErrorMessage = 'Error in procedure ' + @v_procname + ': ' + ERROR_MESSAGE();
        EXEC dbo.LogError @ErrorMessage;
    END CATCH
END;
