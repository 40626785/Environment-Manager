CREATE PROCEDURE [dbo].[CheckWaterQualityAnomalies]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @v_procname NVARCHAR(100) = 'CheckWaterQualityAnomalies',
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
            FROM Archive_Water_Quality
            WHERE [Date] = @Yesterday
            GROUP BY LocationId
            HAVING COUNT(*) <> 24
        )
        BEGIN
            INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Message)
            SELECT 
                LocationId,
                GETDATE(),
                'WaterQualityDataCheck',
                COUNT(*),
                'Less than 24 hourly water quality records for yesterday'
            FROM Archive_Water_Quality
            WHERE [Date] = @Yesterday
            GROUP BY LocationId
            HAVING COUNT(*) <> 24;
        END

        -- 2. Check for NULL values in key fields
        IF EXISTS (
            SELECT 1
            FROM Archive_Water_Quality
            WHERE Nitrate_mg_l_1 IS NULL
               OR Nitrite_less_thank_mg_l_1 IS NULL
               OR Phosphate_mg_l_1 IS NULL
               OR EC_cfu_100ml IS NULL
               OR [Date] IS NULL
               OR [Time] IS NULL
               OR LocationId IS NULL
        )
        BEGIN
            INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Message)
            SELECT 
                ISNULL(LocationId, -1),
                GETDATE(),
                'WaterQualityNullCheck',
                NULL,
                'One or more NULL values found in Archive_Water_Quality'
            FROM Archive_Water_Quality
            WHERE Nitrate_mg_l_1 IS NULL
               OR Nitrite_less_thank_mg_l_1 IS NULL
               OR Phosphate_mg_l_1 IS NULL
               OR EC_cfu_100ml IS NULL
               OR [Date] IS NULL
               OR [Time] IS NULL
               OR LocationId IS NULL;
        END

        -- 3. Loop through each location
        DECLARE LocationCursor CURSOR FOR
        SELECT DISTINCT LocationId FROM Archive_Water_Quality;

        OPEN LocationCursor;
        FETCH NEXT FROM LocationCursor INTO @LocationId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            ;WITH Last1440 AS (
                SELECT TOP 1440 *
                FROM Archive_Water_Quality
                WHERE LocationId = @LocationId
                ORDER BY ID DESC
            ),
            Stats AS (
                SELECT
                    STDEV(Nitrate_mg_l_1) AS StdNitrate,
                    STDEV(Nitrite_less_thank_mg_l_1) AS StdNitrite,
                    STDEV(Phosphate_mg_l_1) AS StdPhosphate,
                    STDEV(EC_cfu_100ml) AS StdEC,
                    AVG(Nitrate_mg_l_1) AS AvgNitrate,
                    AVG(Nitrite_less_thank_mg_l_1) AS AvgNitrite,
                    AVG(Phosphate_mg_l_1) AS AvgPhosphate,
                    AVG(EC_cfu_100ml) AS AvgEC
                FROM Last1440
            )
            SELECT * INTO #TempStats FROM Stats;

            DECLARE @LatestId INT = (
                SELECT MAX(ID) FROM Archive_Water_Quality WHERE LocationId = @LocationId
            );

            DECLARE
                @Nitrate FLOAT,
                @Nitrite FLOAT,
                @Phosphate FLOAT,
                @EC FLOAT,
                @DT DATETIME;

            SELECT
                @Nitrate = Nitrate_mg_l_1,
                @Nitrite = Nitrite_less_thank_mg_l_1,
                @Phosphate = Phosphate_mg_l_1,
                @EC = EC_cfu_100ml,
                @DT = CAST([Date] AS DATETIME) + CAST([Time] AS DATETIME)
            FROM Archive_Water_Quality
            WHERE ID = @LatestId;

            -- Check deviations
            IF ABS(@Nitrate - (SELECT AvgNitrate FROM #TempStats)) > (SELECT StdNitrate FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'Nitrate_mg_l_1', @Nitrate, (SELECT StdNitrate FROM #TempStats), 'Nitrate outside standard deviation');

            IF ABS(@Nitrite - (SELECT AvgNitrite FROM #TempStats)) > (SELECT StdNitrite FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'Nitrite_less_thank_mg_l_1', @Nitrite, (SELECT StdNitrite FROM #TempStats), 'Nitrite outside standard deviation');

            IF ABS(@Phosphate - (SELECT AvgPhosphate FROM #TempStats)) > (SELECT StdPhosphate FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'Phosphate_mg_l_1', @Phosphate, (SELECT StdPhosphate FROM #TempStats), 'Phosphate outside standard deviation');

            IF ABS(@EC - (SELECT AvgEC FROM #TempStats)) > (SELECT StdEC FROM #TempStats)
                INSERT INTO AlertTable (LocationId, Date_Time, Parameter, Value, Deviation, Message)
                VALUES (@LocationId, @DT, 'EC_cfu_100ml', @EC, (SELECT StdEC FROM #TempStats), 'E. coli count outside standard deviation');

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
