<#
.SYNOPSIS
    This script copies a SQL Server database.

.DESCRIPTION
    1. Displays an introduction message first.
    2. Prompts for the source and target database names, as well as optional parameter to drop the target database if it exists.
    3. Connects to a specified SQL Server instance using the provided credentials.
    4. Detaches the source DB, copies its data files, then reattaches both source and target DBs.

.PARAMETER sourceDbName
    The name of the source database to copy.

.PARAMETER targetDbName
    The name of the new copied database.

.PARAMETER dropTargetDb
    Optional switch to drop the target database if it already exists.

.NOTES
    - Update $instanceName, $username, and $password placeholders in the script.
    - Requires the SqlServer PowerShell module.
    - Ensure you have the required permissions on the target SQL Server instance.

.EXAMPLE
    .\Copy-Database-2022.ps1 -sourceDbName db1 -targetDbName db2 -dropTargetDb
#>

Write-Host "--------------------------------------------------------"
Write-Host " Welcome to the Copy-Database-2022 script!"
Write-Host " This script copies a SQL Server database to a new name."
Write-Host " You can change the SQL Server instance name, username,"
Write-Host " and password in the script."
Write-Host " Usage: .\Copy-Database-2022.ps1 -sourceDbName <SourceDatabase>"
Write-Host "        -targetDbName <TargetDatabase> [-dropTargetDb]"
Write-Host "--------------------------------------------------------"
Write-Host ""

# -- Remove [CmdletBinding()] and use simple parameter logic --
# Prompt for required parameters. If you intend to pass them on the command line,
# you'll need additional checks to handle that scenario.

if (-not $sourceDbName) {
    $sourceDbName = Read-Host "Enter the source database name"
}

if (-not $targetDbName) {
    $targetDbName = Read-Host "Enter the target database name"
}

# For the optional dropTargetDb parameter, just handle with a prompt or standard variable
if (-not $dropTargetDb) {
    $userResponse = Read-Host "Drop the target DB if it exists? (Y/N)"
    $dropTargetDb = $userResponse -eq 'Y'
}

Write-Host "Source database: $sourceDbName"
Write-Host "Target database: $targetDbName"
if ($dropTargetDb) {
    Write-Host "The target database will be dropped if it already exists."
}
Write-Host "--------------------------------------------------------"

# Set your SQL Server instance name and credentials
$instanceName = "ex-jwang"
$username     = "sa"
$password     = "blue"

Write-Host "Source database: $sourceDbName"
Write-Host "Target database: $targetDbName"
if ($dropTargetDb) {
    Write-Host "The script will drop the target database if it already exists."
}
Write-Host "--------------------------------------------------------"

# Import the SqlServer module (if you haven't already)
Import-Module SqlServer -ErrorAction SilentlyContinue

Write-Host "Attempting to connect to SQL Server instance '$instanceName'..."

# Build the connection string
$connectionString = "Server=$instanceName;Database=master;User ID=$username;Password=$password;Encrypt=True;TrustServerCertificate=True"

try {
    # 1) Check if target DB exists
    $checkDbQuery = "
        SELECT CASE WHEN db_id('$targetDbName') IS NOT NULL THEN 1 ELSE 0 END AS ExistsFlag;
    "
    $targetExists = Invoke-Sqlcmd -ConnectionString $connectionString -Query $checkDbQuery |
        Select-Object -ExpandProperty ExistsFlag

    if ($targetExists -eq 1 -and $dropTargetDb) {
        Write-Host "Target database '$targetDbName' already exists."
        Write-Host "Dropping existing target database '$targetDbName'..."
        Invoke-Sqlcmd -ConnectionString $connectionString -Query "
            ALTER DATABASE [$targetDbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [$targetDbName];
        "
        Write-Host "Dropped target database '$targetDbName'."
    }

    # 2) Detach the source database
    Write-Host "Detaching source database '$sourceDbName'..."
    Invoke-Sqlcmd -ConnectionString $connectionString -Query "
        ALTER DATABASE [$sourceDbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
        EXEC sp_detach_db [$sourceDbName];
    "
    Write-Host "Detached source database '$sourceDbName'."

    # 3) Copy .mdf/.ldf files - adjust these paths to match your environment
    $dataPath  = "C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA"
    $sourceMdf = Join-Path $dataPath "$sourceDbName.mdf"
    $sourceLdf = Join-Path $dataPath ($sourceDbName + "_log.ldf")
    $targetMdf = Join-Path $dataPath "$targetDbName.mdf"
    $targetLdf = Join-Path $dataPath ($targetDbName + "_log.ldf")

    Write-Host "Copying database files to create '$targetDbName'..."
    Copy-Item -Path $sourceMdf -Destination $targetMdf -Force
    Copy-Item -Path $sourceLdf -Destination $targetLdf -Force
    Write-Host "Copied database files for '$targetDbName'."

    # 4) Reattach the source database
    Write-Host "Attaching source database '$sourceDbName'..."
    Invoke-Sqlcmd -ConnectionString $connectionString -Query "
        CREATE DATABASE [$sourceDbName]
        ON (FILENAME = N'$sourceMdf'),
           (FILENAME = N'$sourceLdf')
        FOR ATTACH;
    "
    Write-Host "Attached source database '$sourceDbName'."

    # 5) Attach the newly copied database
    Write-Host "Attaching target database '$targetDbName'..."
    Invoke-Sqlcmd -ConnectionString $connectionString -Query "
        CREATE DATABASE [$targetDbName]
        ON (FILENAME = N'$targetMdf'),
           (FILENAME = N'$targetLdf')
        FOR ATTACH;
    "
    Write-Host "Attached target database '$targetDbName'."

    Write-Host "--------------------------------------------------------"
    Write-Host "Database '$targetDbName' has been successfully created by copying '$sourceDbName'."
}
catch {
    Write-Host "An error occurred:"
    Write-Host $_.Exception.Message
}
finally {
    # Attempt to set the source DB to multi-user
    try {
        Invoke-Sqlcmd -ConnectionString $connectionString -Query "
            ALTER DATABASE [$sourceDbName] SET MULTI_USER;
        " -ErrorAction SilentlyContinue
    } catch {}

    # Attempt to set the target DB to multi-user
    try {
        Invoke-Sqlcmd -ConnectionString $connectionString -Query "
            ALTER DATABASE [$targetDbName] SET MULTI_USER;
        " -ErrorAction SilentlyContinue
    } catch {}
}