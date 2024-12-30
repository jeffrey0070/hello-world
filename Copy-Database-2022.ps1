<#
.SYNOPSIS
    This script copies a SQL Server database.

.DESCRIPTION
    1. Displays an introduction message first.
    2. Prompts for the source and target database names.
    3. Checks if the target database exists and prompts to drop it if it does.
    4. Connects to a specified SQL Server instance using the provided credentials.
    5. Detaches the source DB, copies its data files, then reattaches both source and target DBs.

.PARAMETER sourceDbName
    The name of the source database to copy.

.PARAMETER targetDbName
    The name of the new copied database.

.NOTES
    - Update $instanceName, $username, and $password placeholders in the script.
    - Requires the SqlServer PowerShell module.
    - Ensure you have the required permissions on the target SQL Server instance.

.EXAMPLE
    .\Copy-Database-2022.ps1 -sourceDbName db1 -targetDbName db2
#>

Write-Host "--------------------------------------------------------"
Write-Host " Welcome to the Copy-Database-2022 script!"
Write-Host " This script copies a SQL Server database to a new name."
Write-Host " You can change the SQL Server instance name, username,"
Write-Host " and password in the script."
Write-Host " Usage: .\Copy-Database-2022.ps1 -sourceDbName <SourceDatabase>"
Write-Host "        -targetDbName <TargetDatabase>"
Write-Host "--------------------------------------------------------"
Write-Host ""

# Prompt for required parameters
if (-not $sourceDbName) {
    $sourceDbName = Read-Host "Enter the source database name"
}

if (-not $targetDbName) {
    $targetDbName = Read-Host "Enter the target database name"
}

Write-Host "Source database: $sourceDbName"
Write-Host "Target database: $targetDbName"
Write-Host "--------------------------------------------------------"

# Set your SQL Server instance name and credentials
$instanceName = "ex-jwang"
$username     = "sa"
$password     = "blue"

Write-Host "Attempting to connect to SQL Server instance '$instanceName'..."

# Import the SqlServer module (if you haven't already)
Import-Module SqlServer -ErrorAction SilentlyContinue

# Build the connection string
$connectionString = "Server=$instanceName;Database=master;User ID=$username;Password=$password;Encrypt=True;TrustServerCertificate=True"

try {
    # 1) Check if target DB exists
    $checkDbQuery = "
        SELECT CASE WHEN db_id('$targetDbName') IS NOT NULL THEN 1 ELSE 0 END AS ExistsFlag;
    "
    $targetExists = Invoke-Sqlcmd -ConnectionString $connectionString -Query $checkDbQuery |
        Select-Object -ExpandProperty ExistsFlag

    if ($targetExists -eq 1) {
        # Target database exists, prompt the user
        $userResponse = Read-Host "Target database '$targetDbName' already exists. Drop the target DB if it exists? (Y/N)"
        if ($userResponse -eq 'Y') {
            Write-Host "Dropping existing target database '$targetDbName'..."
            Invoke-Sqlcmd -ConnectionString $connectionString -Query "
                ALTER DATABASE [$targetDbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [$targetDbName];
            "
            Write-Host "Dropped target database '$targetDbName'."
        } else {
            Write-Host "Operation cancelled by user."
            exit
        }
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

    # 4) Attach the newly copied database
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
    # Ensure the source database is re-attached
    try {
        Invoke-Sqlcmd -ConnectionString $connectionString -Query "
            CREATE DATABASE [$sourceDbName]
            ON (FILENAME = N'$sourceMdf'),
               (FILENAME = N'$sourceLdf')
            FOR ATTACH;
        " -ErrorAction SilentlyContinue
        Write-Host "Ensured source database '$sourceDbName' is re-attached."
    } catch {
        Write-Host "Failed to re-attach source database '$sourceDbName'."
    }

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