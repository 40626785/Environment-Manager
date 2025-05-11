CREATE PROCEDURE pkg_archive_air_quality
AS
BEGIN
    -- Declare variables
    DECLARE @v_procname VARCHAR(30) = 'Air quality archive script';
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
        FROM Archive_Air_Quality;

        -- Count of current table
        SELECT @v_count_current_date_table = COUNT(*)
        FROM Air_Quality;

        -- Count of rows to be archived
        SELECT @v_count_rows_to_archive = COUNT(*)
        FROM Air_Quality
        WHERE Date < DATEADD(MONTH, -12, GETDATE());

        SET @LogMessage = 'Counts complete. Starting inserts into archive table. There are ' + CAST(@v_count_rows_to_archive AS NVARCHAR) + ' rows to archive';
        EXEC dbo.LogMessage @LogMessage;

        -- Insert data older than one year into Archive_Air_Quality
        -- Insert data older than one year into Archive_Air_Quality
-- Insert data older than one year into Archive_Air_Quality
INSERT INTO Archive_Air_Quality (
    [Date], 
    [Time], 
    [Nitrogen_dioxide], 
    [Sulphur_dioxide], 
    [PM2_5_particulate_matter], 
    [PM10_particulate_matter],
    [LocationId]
)
SELECT 
    [Date], 
    [Time], 
    [Nitrogen_dioxide], 
    [Sulphur_dioxide], 
    [PM2_5_particulate_matter], 
    [PM10_particulate_matter],
    [LocationId]
FROM Air_Quality
WHERE [Date] < DATEADD(MONTH, -12, GETDATE());



        -- Count archive table after inserts
        SELECT @v_count_archive_table_after_insert = COUNT(*)
        FROM Archive_Air_Quality;

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

        -- Delete the archived data from Air_Quality
        DELETE FROM Air_Quality
        WHERE Date < DATEADD(MONTH, -12, GETDATE());

        -- Count of current table after deletion
        SELECT @v_count_current_date_table_after_del = COUNT(*)
        FROM Air_Quality;

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
