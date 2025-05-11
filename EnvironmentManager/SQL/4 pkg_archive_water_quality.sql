CREATE PROCEDURE pkg_archive_water_quality
AS
BEGIN
    -- Declare variables
    DECLARE @v_procname VARCHAR(30) = 'Water quality archive script';
    DECLARE @v_count_archive_table INT = 0;
    DECLARE @v_count_current_date_table INT = 0;
    DECLARE @v_count_current_date_table_after_del INT = 0;
    DECLARE @v_count_rows_to_archive INT = 0;
    DECLARE @v_count_archive_table_after_insert INT = 0;
    DECLARE @LogMessage NVARCHAR(4000);
    DECLARE @ErrorMessage NVARCHAR(4000);

    -- Start a transaction
    BEGIN TRANSACTION;

    BEGIN TRY
        SET @LogMessage = 'Starting procedure ' + @v_procname;
        EXEC dbo.LogMessage @LogMessage;

        SET @LogMessage = 'Taking counts of tables';
        EXEC dbo.LogMessage @LogMessage;

        -- Count of archive table
        SELECT @v_count_archive_table = COUNT(*)
        FROM Archive_Water_Quality;

        -- Count of current table
        SELECT @v_count_current_date_table = COUNT(*)
        FROM Water_Quality;

        -- Count of rows to be archived
        SELECT @v_count_rows_to_archive = COUNT(*)
        FROM Water_Quality
        WHERE Date < DATEADD(MONTH, -12, GETDATE());

        SET @LogMessage = 'Counts complete. Starting inserts into archive table. There are ' + CAST(@v_count_rows_to_archive AS NVARCHAR) + ' rows to archive';
        EXEC dbo.LogMessage @LogMessage;

        -- Insert data older than one year into Archive_water_Quality
 INSERT INTO Archive_Water_Quality (
        [Date],
        [Time],
        [Nitrate_mg_l_1],
        [Nitrite_less_thank_mg_l_1],
        [Phosphate_mg_l_1],
        [EC_cfu_100ml],
        [LocationId]
    )
    SELECT
        [Date],
        [Time],
        [Nitrate_mg_l_1],
        [Nitrite_less_thank_mg_l_1],
        [Phosphate_mg_l_1],
        [EC_cfu_100ml],
        [LocationId]
    FROM Water_Quality
    WHERE [Date] < DATEADD(MONTH, -12, GETDATE());




        -- Count archive table after inserts
        SELECT @v_count_archive_table_after_insert = COUNT(*)
        FROM Archive_water_Quality;

        IF @v_count_archive_table_after_insert <> @v_count_archive_table + @v_count_rows_to_archive
        BEGIN
            -- Raise an error if the condition is met
            SET @ErrorMessage = 'Inserts into the archive table have not been successful.';
            EXEC dbo.LogError @ErrorMessage;

            THROW 50000, @ErrorMessage, 1;
        END
        ELSE
        BEGIN
            SET @LogMessage = 'Inserts into the archive have been completed';
            EXEC dbo.LogMessage @LogMessage;
        END;

        SET @LogMessage = 'Removing archived data from current table.';
        EXEC dbo.LogMessage @LogMessage;

        -- Delete the archived data from water_Quality
        DELETE FROM Water_Quality
        WHERE Date < DATEADD(MONTH, -12, GETDATE());

        -- Count of current table after deletion
        SELECT @v_count_current_date_table_after_del = COUNT(*)
        FROM Water_Quality;

        IF @v_count_current_date_table_after_del <> @v_count_current_date_table - @v_count_rows_to_archive
        BEGIN
            -- Raise an error if the condition is met
            SET @ErrorMessage = 'Deletion of archived rows has not been successful.';
            EXEC dbo.LogError @ErrorMessage;

            THROW 50000, @ErrorMessage, 1;
        END
        ELSE
        BEGIN
            SET @LogMessage = 'Deletion of archived rows has been completed';
            EXEC dbo.LogMessage @LogMessage;
        END;

        -- Commit the transaction if everything is successful
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Rollback the transaction if any error occurs
        ROLLBACK TRANSACTION;

        -- Log the error message
        SET @ErrorMessage = ERROR_MESSAGE();
        EXEC dbo.LogError @ErrorMessage;

        -- Re-throw the error
        THROW;
    END CATCH;
END;
