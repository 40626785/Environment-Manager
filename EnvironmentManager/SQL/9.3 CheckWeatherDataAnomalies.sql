CREATE PROCEDURE [dbo].[CheckWeatherDataAnomalies]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @v_procname NVARCHAR(100) = 'CheckWeatherDataAnomalies',
        @LogMessage NVARCHAR(MAX),
        @ErrorMessage NVARCHAR(MAX),
        @LocationId INT,
        @Yesterday DATE = CAST(GETDATE() - 1 AS DATE)

    BEGIN TRY
        -- Log start
        SET @LogMessage = 'Starting procedure ' + @v_procname;
        EXEC dbo.LogMessage @LogMessage;

        -- 1. Check for missing 24 hourly inserts per location yesterday
        IF EXISTS (
            SELECT LocationId
            FROM weather_data
            WHERE CAST(Date_Time AS DATE) = @Yesterday
            GROUP BY LocationId
            HAVING COUNT(*) <> 24
        )
        BEGIN
            INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Message)
            SELECT 
                LocationId,
                GETDATE(),
                'DataCheck',
                COUNT(*),
                'Less than 24 hourly records for yesterday'
            FROM weather_data
            WHERE CAST(Date_Time AS DATE) = @Yesterday
            GROUP BY LocationId
            HAVING COUNT(*) <> 24;
        END

        -- 2. Check for NULL values in key fields
        IF EXISTS (
            SELECT 1
            FROM weather_data
            WHERE temperature_2m IS NULL
               OR relative_humidity_2m IS NULL
               OR wind_speed_10m IS NULL
               OR wind_direction_10m IS NULL
               OR Date_Time IS NULL
               OR LocationId IS NULL
        )
        BEGIN
            INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Message)
            SELECT 
                ISNULL(LocationId, -1),
                ISNULL(Date_Time, GETDATE()),
                'NullCheck',
                NULL,
                'One or more NULL values found in weather_data'
            FROM weather_data
            WHERE temperature_2m IS NULL
               OR relative_humidity_2m IS NULL
               OR wind_speed_10m IS NULL
               OR wind_direction_10m IS NULL
               OR Date_Time IS NULL
               OR LocationId IS NULL;
        END

        -- 3. Loop through each location
        DECLARE LocationCursor CURSOR FOR
        SELECT DISTINCT LocationId FROM weather_data;

        OPEN LocationCursor;
        FETCH NEXT FROM LocationCursor INTO @LocationId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- CTE: last 1440 records for this location
            ;WITH Last1440 AS (
                SELECT TOP 1440 *
                FROM weather_data
                WHERE LocationId = @LocationId
                ORDER BY ID DESC
            ),
            Stats AS (
                SELECT
                    STDEV(temperature_2m) AS StdTemp,
                    STDEV(relative_humidity_2m) AS StdHumidity,
                    STDEV(wind_speed_10m) AS StdWindSpeed,
                    STDEV(wind_direction_10m) AS StdWindDir,
                    AVG(temperature_2m) AS AvgTemp,
                    AVG(relative_humidity_2m) AS AvgHumidity,
                    AVG(wind_speed_10m) AS AvgWindSpeed,
                    AVG(wind_direction_10m) AS AvgWindDir
                FROM Last1440
            )
            SELECT * INTO #TempStats FROM Stats;

            DECLARE @LatestId INT = (
                SELECT MAX(ID) FROM weather_data WHERE LocationId = @LocationId
            );

            DECLARE
                @Temp FLOAT,
                @Humidity FLOAT,
                @WindSpeed FLOAT,
                @WindDir FLOAT,
                @DT DATETIME;

            SELECT
                @Temp = temperature_2m,
                @Humidity = relative_humidity_2m,
                @WindSpeed = wind_speed_10m,
                @WindDir = wind_direction_10m,
                @DT = Date_Time
            FROM weather_data
            WHERE ID = @LatestId;

            -- Check deviations
            IF ABS(@Temp - (SELECT AvgTemp FROM #TempStats)) > (SELECT StdTemp FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'temperature_2m', @Temp, (SELECT StdTemp FROM #TempStats), 'Temperature outside standard deviation');

            IF ABS(@Humidity - (SELECT AvgHumidity FROM #TempStats)) > (SELECT StdHumidity FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'relative_humidity_2m', @Humidity, (SELECT StdHumidity FROM #TempStats), 'Humidity outside standard deviation');

            IF ABS(@WindSpeed - (SELECT AvgWindSpeed FROM #TempStats)) > (SELECT StdWindSpeed FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'wind_speed_10m', @WindSpeed, (SELECT StdWindSpeed FROM #TempStats), 'Wind speed outside standard deviation');

            IF ABS(@WindDir - (SELECT AvgWindDir FROM #TempStats)) > (SELECT StdWindDir FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'wind_direction_10m', @WindDir, (SELECT StdWindDir FROM #TempStats), 'Wind direction outside standard deviation');

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
END