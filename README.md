# eJay Decompressor

A Windows Forms application for converting eJay music files (.pxd) to WAV files. Features a modern, user-friendly interface with drag-and-drop support and batch conversion capabilities.

## Features

- Drag and drop interface for easy file selection
- Batch conversion of multiple PXD files
- Options for output directory selection
- File conflict handling (Overwrite/Rename)
- Option to delete PXD files after conversion
- Real-time conversion progress and logging
- Modern, clean user interface

## Project Structure

- `eJayDecompressorApp/` - Main application source code
- `lib/` - Required DLLs (`pxd32d5_d4.dll`, `eJ_Tool.dll`)
- `build/` - Output directory after deployment
- `deploy.ps1` - Deployment script
- `install_dotnet.ps1` - .NET installation script

## Requirements

- Windows operating system
- .NET 6.0 x86 (32-bit) SDK and runtime
- Required DLLs: `pxd32d5_d4.dll` and `eJ_Tool.dll` (obtain from your eJay installation)

## Building

1. Install .NET 6.0 SDK if not already installed:
   ```
   ./install_dotnet.ps1
   ```

2. Place required DLLs in the `lib/` directory:
   - `pxd32d5_d4.dll`
   - `eJ_Tool.dll`

3. Build the application:
   ```
   dotnet build eJayDecompressorApp/eJayDecompressorApp.csproj -c Release
   ```

4. Deploy the application:
   ```
   ./deploy.ps1
   ```

## Usage

1. Run `eJayDecompressorApp.exe` from the `build/` directory
2. Drag and drop one or more `.pxd` files onto the window
3. Configure options:
   - Choose output directory (Same as PXD or User Directory)
   - Select file conflict handling (Overwrite or Rename)
   - Optionally enable PXD file deletion after conversion
4. Click "Convert Files" to start the conversion
5. Monitor progress in the log window

## Notes

- This repository assumes you obtained `pxd32d5_d4.dll` and `eJ_Tool.dll` legally by purchasing the original eJay software. These files are not included in the repository.
- For best results, always use the deployment script to gather all dependencies in the `build/` directory.
- The application requires 32-bit .NET 6.0 runtime due to the 32-bit nature of the eJay DLLs.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
