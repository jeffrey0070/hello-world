# Define parameters
Param(
    [Parameter(Mandatory = $true)]
    [string]$sourceDbName,   # Name of the source database

    [Parameter(Mandatory = $true)]
    [string]$targetDbName,   # Name for the new copied database

    [switch]$dropTargetDb     # Switch to drop target database if it exists
)

# Display introductory message
Write-Host "This script copies a SQL Server database."
Write-Host "You can change the SQL Server instance name, username, and password in the script."
Write-Host "Usage: .\Copy-Database-2022.ps1 -sourceDbName <SourceDatabaseName> -targetDbName <TargetDatabaseName> [-dropTargetDb]"
Write-Host "Parameters:"
Write-Host "`t-sourceDbName: Name of the source database to be copied."
Write-Host "`t-targetDbName: Name for the new copied database."
Write-Host "`t-dropTargetDb (Optional): Drop the target database if it exists."

# Define SQL Server instance name, username, and password
$serverName = "ex-jwang"    # Replace with your SQL Server instance name
$username = "sa"            # Replace with your SQL Server username
$password = "blue"          # Replace with your SQL Server password

# Try to import the SqlServer module
if (-not (Get-Module -Name SqlServer)) {
    try {
        Import-Module SqlServer -ErrorAction Stop
    } catch {
        Write-Host "The 'SqlServer' module is not installed. Installing it now..."
        Install-Module -Name SqlServer -Scope CurrentUser -Force -AllowClobber
        Import-Module SqlServer
    }
}

# Verify that the SMO assembly is loaded
if (-not ("Microsoft.SqlServer.Management.Smo.Server" -as [type])) {
    Write-Host "Loading SMO assemblies..."
    $assemblyPaths = @(
        "${env:ProgramFiles(x86)}\Microsoft SQL Server\190\SDK\Assemblies\Microsoft.SqlServer.Smo.dll",
        "${env:ProgramFiles}\Microsoft SQL Server\190\SDK\Assemblies\Microsoft.SqlServer.Smo.dll"
    )
    $assemblyLoaded = $false
    foreach ($assemblyPath in $assemblyPaths) {
        if (Test-Path $assemblyPath) {
            try {
                Add-Type -Path $assemblyPath -ErrorAction Stop
                $assemblyLoaded = $true
                Write-Host "Loaded SMO assembly from path: $assemblyPath"
                break
            } catch {
                Write-Host "Failed to load SMO assembly from path: $assemblyPath" -ForegroundColor Yellow
            }
        }
    }
    if (-not $assemblyLoaded) {
        Write-Host "SMO assemblies not found. Please ensure SMO is installed for SQL Server 2022." -ForegroundColor Red
        exit
    }
}

# Test SQL Server connection
try {
    $server = New-Object Microsoft.SqlServer.Management.Smo.Server $serverName
    $server.ConnectionContext.LoginSecure = $false
    $server.ConnectionContext.Login = $username
    $server.ConnectionContext.Password = $password
    $server.ConnectionContext.Connect()
    Write-Host "Successfully connected to SQL Server instance '$serverName'."
} catch {
    Write-Host "Failed to connect to SQL Server instance '$serverName'. Error: $_" -ForegroundColor Red
    exit
}

# Check if source database exists
$sourceDb = $server.Databases[$sourceDbName]
if ($sourceDb -eq $null) {
    Write-Host "Source database '$sourceDbName' does not exist." -ForegroundColor Red
    exit
}

# Check if target database exists
$targetDb = $server.Databases[$targetDbName]
if ($targetDb -ne $null) {
    if ($dropTargetDb.IsPresent) {
        try {
            Write-Host "Dropping existing target database '$targetDbName'..."
            $server.KillAllProcesses($targetDbName)
            $targetDb.Drop()
            Write-Host "Dropped target database '$targetDbName'."
        } catch {
            Write-Host "Failed to drop target database '$targetDbName'. Error: $_" -ForegroundColor Red
            exit
        }
    } else {
        Write-Host "Target database '$targetDbName' already exists." -ForegroundColor Yellow
        # Prompt the user for input
        $userInput = Read-Host "Do you want to drop the existing database and proceed? (Y/N)"
        if ($userInput -eq "Y" -or $userInput -eq "y") {
            try {
                Write-Host "Dropping existing target database '$targetDbName'..."
                $server.KillAllProcesses($targetDbName)
                $targetDb.Drop()
                Write-Host "Dropped target database '$targetDbName'."
            } catch {
                Write-Host "Failed to drop target database '$targetDbName'. Error: $_" -ForegroundColor Red
                exit
            }
        } else {
            Write-Host "Operation cancelled. The target database already exists." -ForegroundColor Yellow
            exit
        }
    }
}

# Get file paths of the source database
$dataFile = $sourceDb.FileGroups[0].Files[0].FileName
$logFile = $sourceDb.LogFiles[0].FileName

# Define new file names for the target database
$targetDataFile = [IO.Path]::Combine([IO.Path]::GetDirectoryName($dataFile), "$targetDbName.mdf")
$targetLogFile = [IO.Path]::Combine([IO.Path]::GetDirectoryName($logFile), "$targetDbName.ldf")

# Detach the source database with error handling
try {
    Write-Host "Detaching source database '$sourceDbName'..."
    $server.KillAllProcesses($sourceDbName)
    $server.DetachDatabase($sourceDbName, $false)
    Write-Host "Detached source database '$sourceDbName'."
} catch {
    Write-Host "Failed to detach source database '$sourceDbName'. Error: $_" -ForegroundColor Red
    exit
}

# Copy the database files with error handling
try {
    Write-Host "Copying database files to create '$targetDbName'..."
    Copy-Item -Path $dataFile -Destination $targetDataFile -Force
    Copy-Item -Path $logFile -Destination $targetLogFile -Force
    Write-Host "Copied database files for '$targetDbName'."
} catch {
    Write-Host "Failed to copy database files. Error: $_" -ForegroundColor Red
    # Attempt to re-attach the source database before exiting
    try {
        Write-Host "Re-attaching source database '$sourceDbName'..."
        $sourceDbFiles = New-Object System.Collections.Specialized.StringCollection
        $sourceDbFiles.Add($dataFile)
        $sourceDbFiles.Add($logFile)
        $server.AttachDatabase($sourceDbName, $sourceDbFiles)
        Write-Host "Re-attached source database '$sourceDbName'."
    } catch {
        Write-Host "Failed to re-attach source database '$sourceDbName'. Manual intervention may be required. Error: $_" -ForegroundColor Red
    }
    exit
}

# Attach the source database back with error handling
try {
    Write-Host "Attaching source database '$sourceDbName'..."
    $sourceDbFiles = New-Object System.Collections.Specialized.StringCollection
    $sourceDbFiles.Add($dataFile)
    $sourceDbFiles.Add($logFile)
    $server.AttachDatabase($sourceDbName, $sourceDbFiles)
    Write-Host "Attached source database '$sourceDbName'."
} catch {
    Write-Host "Failed to attach source database '$sourceDbName'. Error: $_" -ForegroundColor Red
    exit
}

# Attach the target database with error handling
try {
    Write-Host "Attaching target database '$targetDbName'..."
    $targetDbFiles = New-Object System.Collections.Specialized.StringCollection
    $targetDbFiles.Add($targetDataFile)
    $targetDbFiles.Add($targetLogFile)
    $server.AttachDatabase($targetDbName, $targetDbFiles)
    Write-Host "Attached target database '$targetDbName'."
} catch {
    Write-Host "Failed to attach target database '$targetDbName'. Error: $_" -ForegroundColor Red
    exit
}

Write-Host "Database '$targetDbName' has been successfully created by copying '$sourceDbName'."