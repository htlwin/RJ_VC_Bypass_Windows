# RJ VC Bypass - Windows Edition

A Windows application for network portal bypass and voucher code testing.

## ⚠️ Disclaimer

This software is provided for **educational purposes only**. Use only on networks you own or have explicit permission to test. Unauthorized access to computer networks may violate laws in your jurisdiction.

## Requirements

- Windows 10/11
- .NET 6.0 Runtime (or SDK for building)

## Building from Source

### Option 1: Using the build script

```batch
build.bat
```

### Option 2: Manual build

```batch
# Restore packages
dotnet restore

# Build
dotnet build --configuration Release

# Publish as single executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Running the Application

After building, run:

```batch
dotnet run
```

Or use the compiled executable in `bin\Release\net6.0-windows\`

## Features

### Main Dashboard
- Connect/Bypass button with real-time status
- Terminal log showing bypass operations
- License status and remaining days
- Quick access to speedtest and support

### Brute Force Attack
- Test voucher codes in a specified range
- Multi-threaded testing (20 parallel threads)
- Real-time progress and speed display
- Found keys are saved and displayed

### License System
- Device-bound licenses
- Server-side verification
- Expiration tracking
- QR code support (via clipboard)

## Configuration

Application data is stored in:
```
%APPDATA%\RJ_VC_Bypass\config.json
```

## Project Structure

```
RJ_VC_Bypass_Windows/
├── Program.cs              # Entry point
├── AppConfig.cs            # API configuration
├── SecurityHelper.cs       # Encryption/decryption
├── HttpHelper.cs           # HTTP client wrapper
├── BypassEngine.cs         # Core bypass logic
├── BruteForceEngine.cs     # Brute force engine
├── ConfigManager.cs        # Settings management
├── StartForm.cs            # Login/start screen
├── MainForm.cs             # Main dashboard
├── DashboardForm.cs        # License dashboard
├── BruteForceForm.cs       # Brute force UI
├── RJ_VC_Bypass.csproj     # Project file
└── app.manifest            # Application manifest
```

## Technical Details

### Bypass Engine
- Monitors network connectivity via Google connectivity check
- Detects captive portal redirects
- Extracts session IDs from portal URLs
- Sends authentication tokens to maintain access
- Multi-threaded keep-alive pings

### HTTP Client
- SSL certificate bypass for testing
- Cookie management
- Automatic redirect following
- Custom user agent rotation

## Troubleshooting

### Build Errors
- Ensure .NET 6.0 SDK is installed
- Run `dotnet restore` to download dependencies

### Runtime Errors
- Check Windows Firewall settings
- Ensure network connectivity
- Verify license key is valid

## License

This project is for educational purposes only.

## Contact

For support, contact: https://t.me/injectionvoucher
