# Download .NET 6.0 SDK installer
$url = "https://dot.net/v1/dotnet-install.ps1"
$installerPath = "$env:TEMP\dotnet-install.ps1"

Write-Host "Downloading .NET 6.0 SDK installer..."
Invoke-WebRequest -Uri $url -OutFile $installerPath

# Run the installer
Write-Host "Installing .NET 6.0 SDK..."
& $installerPath -Version 6.0.419

# Add to PATH if not already present
$dotnetPath = "$env:ProgramFiles\dotnet"
if (-not $env:Path.Contains($dotnetPath)) {
    $env:Path = "$dotnetPath;$env:Path"
    [Environment]::SetEnvironmentVariable("Path", $env:Path, [System.EnvironmentVariableTarget]::User)
}

Write-Host "Installation complete. Please restart your terminal to use .NET 6.0 SDK." 