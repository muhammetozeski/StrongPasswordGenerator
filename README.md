<div align="center">
  <img src="icon.ico" width="128" alt="Strong Password Generator Logo">
  <h1>Strong Password Generator</h1>
  <p>A highly secure, offline, and lightweight application designed to generate cryptographically strong passwords tailored to your precise needs.</p>
  
  <p>
    <a href="https://github.com/muhammetozeski/StrongPasswordGenerator/releases"><img src="https://img.shields.io/github/v/release/muhammetozeski/StrongPasswordGenerator?style=for-the-badge&color=success" alt="Release"></a>
    <a href="https://dotnet.microsoft.com/download/dotnet/10.0"><img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10"></a>
    <a href="https://github.com/muhammetozeski/StrongPasswordGenerator/blob/master/LICENSE"><img src="https://img.shields.io/github/license/muhammetozeski/StrongPasswordGenerator?style=for-the-badge&color=blue" alt="License"></a>
  </p>
</div>

## 🛡️ Security Architecture

Strong Password Generator is built with security as its primary directive:
- **Offline Generation**: Does not require an internet connection, ensuring your passwords are never transmitted over the network.
- **Cryptographic Randomness**: Utilizes `System.Security.Cryptography.RandomNumberGenerator` for true cryptographic randomness, avoiding the predictability of standard pseudo-random number generators.
- **No Telemetry**: Absolutely zero telemetry, tracking, or data collection.
- **Memory Safety**: Built on .NET 10, benefiting from advanced memory safety features to prevent buffer overflows and related exploits.

## ✨ Features

- **Customizable Complexity**: Choose the exact length of your password.
- **Character Sets**: Toggle uppercase letters, lowercase letters, numbers, and special symbols based on your requirements.
- **Instant Copy**: One-click copying to clipboard for seamless workflow.
- **Visual Feedback**: Real-time strength estimation and generation feedback.
- **Modern UI**: Clean, intuitive Windows Forms interface optimized for speed.

## 🚀 Installation & Usage

You can download the application directly from the [Releases](https://github.com/muhammetozeski/StrongPasswordGenerator/releases) page. We offer two versions to best suit your environment:

1. **Portable (Self-Contained)**:
   - Does not require .NET 10 to be installed on your system.
   - Run immediately out of the box.
   - File: `StrongPasswordGenerator.exe`

2. **Framework-Dependent**:
   - Requires [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0) to be installed.
   - Significantly smaller file size.
   - File: `StrongPasswordGenerator-FrameworkDependent-RequiresNET10.exe`

### Steps
1. Download the executable of your choice.
2. Place it anywhere on your machine.
3. Run the application to start generating secure passwords instantly.

## 🛠️ Build from Source

To compile the project yourself, you will need the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```powershell
# Clone the repository
git clone https://github.com/muhammetozeski/StrongPasswordGenerator.git
cd StrongPasswordGenerator

# Publish the Portable Version
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/Portable

# Publish the Framework-Dependent Version
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./publish/FrameworkDependent
```

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
