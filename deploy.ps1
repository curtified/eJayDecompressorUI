# Create build directory if it doesn't exist
if (-not (Test-Path -Path "build")) {
    New-Item -ItemType Directory -Path "build"
}

# Copy all build artifacts from the new merged project
Copy-Item -Path "eJayDecompressorApp\bin\Release\net6.0-windows\*" -Destination "build" -Recurse

# Copy required DLLs from lib directory
Copy-Item -Path "lib\pxd32d5_d4.dll" -Destination "build"
Copy-Item -Path "lib\eJ_Tool.dll" -Destination "build"

Write-Host "Deployment completed. Files are in the 'build' directory."
Write-Host "Please make sure to copy the required DLLs (pxd32d5_d4.dll and eJ_Tool.dll) to the build directory." 