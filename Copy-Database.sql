DECLARE @DropTargetTable BIT = 0;  -- Default value is 0

DECLARE @SourceDatabase NVARCHAR(128) = N'Jeff_IUB_Dev1';
DECLARE @TargetDatabase NVARCHAR(128) = N'db2';
DECLARE @BackupFile NVARCHAR(260);

-- Get the default backup directory
DECLARE @BackupLocation NVARCHAR(260);
EXEC master.dbo.xp_instance_regread
    N'HKEY_LOCAL_MACHINE',
    N'Software\Microsoft\MSSQLServer\MSSQLServer',
    N'BackupDirectory',
    @BackupLocation OUTPUT;

IF @BackupLocation IS NULL
BEGIN
    -- Set a default path if unable to retrieve the backup directory
    SET @BackupLocation = N'C:\Temp';  -- Ensure this directory exists and is writable by SQL Server
END

SET @BackupFile = @BackupLocation + N'\' + @SourceDatabase + N'_copy.bak';

-- Check if the target database exists
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = @TargetDatabase)
BEGIN
    IF @DropTargetTable = 1
    BEGIN
        PRINT 'Database [' + @TargetDatabase + '] already exists. It will be dropped and recreated.';
        -- Script to drop the existing database
        DECLARE @DropDBScript NVARCHAR(MAX) = N'
        ALTER DATABASE [' + @TargetDatabase + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        DROP DATABASE [' + @TargetDatabase + N'];';
        
        PRINT 'Executing script to drop existing database:';
        PRINT @DropDBScript;
        EXEC(@DropDBScript);
    END
    ELSE
    BEGIN
        PRINT 'Warning: Database [' + @TargetDatabase + '] already exists. The script will not overwrite it.';
        PRINT 'Hint: Set @DropTargetTable = 1 to force overwrite the existing database.';
        PRINT 'Exiting script.';
        RETURN;
    END
END
ELSE
BEGIN
    PRINT 'Database [' + @TargetDatabase + '] does not exist. Proceeding to create it.';
END

-- Backup the source database
DECLARE @BackupScript NVARCHAR(MAX) = N'BACKUP DATABASE [' + @SourceDatabase + N'] TO DISK = N''' + @BackupFile + N''' WITH INIT;';
PRINT 'Executing backup script:';
PRINT @BackupScript;
EXEC(@BackupScript);

-- Retrieve logical file names from the backup
DECLARE @FileListCmd NVARCHAR(MAX) = N'RESTORE FILELISTONLY FROM DISK = N''' + @BackupFile + N'''';
PRINT 'Retrieving logical file names from backup:';
PRINT @FileListCmd;

DECLARE @LogicalDataFileName NVARCHAR(128), @LogicalLogFileName NVARCHAR(128);

-- Adjust the @FileList table to match your SQL Server version
DECLARE @FileList TABLE (
    LogicalName NVARCHAR(128),
    PhysicalName NVARCHAR(260),
    [Type] CHAR(1),
    FileGroupName NVARCHAR(128) NULL,
    Size BIGINT,
    MaxSize BIGINT,
    FileId INT,
    CreateLSN NUMERIC(25,0) NULL,
    DropLSN NUMERIC(25,0) NULL,
    UniqueId UNIQUEIDENTIFIER NULL,
    ReadOnlyLSN NUMERIC(25,0) NULL,
    ReadWriteLSN NUMERIC(25,0) NULL,
    BackupSizeInBytes BIGINT NULL,
    SourceBlockSize INT NULL,
    FileGroupId INT NULL,
    LogGroupGUID UNIQUEIDENTIFIER NULL,
    DifferentialBaseLSN NUMERIC(25,0) NULL,
    DifferentialBaseGUID UNIQUEIDENTIFIER NULL,
    IsReadOnly BIT NULL,
    IsPresent BIT NULL,
    TDEThumbprint VARBINARY(32) NULL,
    SnapshotUrl NVARCHAR(360) NULL  -- Include if applicable
);

INSERT INTO @FileList
EXEC(@FileListCmd);

-- Get logical file names
SELECT @LogicalDataFileName = LogicalName FROM @FileList WHERE [Type] = 'D';
SELECT @LogicalLogFileName = LogicalName FROM @FileList WHERE [Type] = 'L';

-- *** Modified Section Starts Here ***
-- Get default data and log file paths by querying the master database's file paths
DECLARE @DataPath NVARCHAR(260), @LogPath NVARCHAR(260);

-- Retrieve the physical file paths of the master database data file
SELECT 
    @DataPath = LEFT(physical_name, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)))
FROM sys.master_files
WHERE database_id = DB_ID('master') AND file_id = 1; -- file_id = 1 for data file

-- Retrieve the physical file paths of the master database log file
SELECT 
    @LogPath = LEFT(physical_name, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)))
FROM sys.master_files
WHERE database_id = DB_ID('master') AND file_id = 2; -- file_id = 2 for log file

-- Ensure paths end with a backslash
IF @DataPath IS NOT NULL AND RIGHT(@DataPath, 1) <> '\' SET @DataPath = @DataPath + '\';
IF @LogPath IS NOT NULL AND RIGHT(@LogPath, 1) <> '\' SET @LogPath = @LogPath + '\';

-- If, for some reason, retrieval failed, fall back to backup location
IF @DataPath IS NULL OR @LogPath IS NULL
BEGIN
    PRINT 'Failed to retrieve default data or log paths from the master database. Using backup directory as fallback.';
    SET @DataPath = @BackupLocation + N'\';
    SET @LogPath = @BackupLocation + N'\';
END
-- *** Modified Section Ends Here ***

-- Restore the backup to create the target database
DECLARE @RestoreScript NVARCHAR(MAX) = N'
BEGIN TRY
    RESTORE DATABASE [' + @TargetDatabase + N']
    FROM DISK = N''' + @BackupFile + N'''
    WITH REPLACE,
    MOVE N''' + @LogicalDataFileName + N''' TO N''' + @DataPath + @TargetDatabase + N'.mdf'',
    MOVE N''' + @LogicalLogFileName + N''' TO N''' + @LogPath + @TargetDatabase + N'_log.ldf'';
    PRINT ''Database [' + @TargetDatabase + N'] restored successfully.'';
END TRY
BEGIN CATCH
    PRINT ''Error during restoration: '' + ERROR_MESSAGE();
    RETURN;
END CATCH;
';
PRINT 'Executing restore script:';
PRINT @RestoreScript;
EXEC(@RestoreScript);
