<div align="center">
  <img src="icon.ico" width="128" alt="Strong Password Generator Logo">
  <h1>Strong Password Generator</h1>
  <p>An offline Windows password generator. Passwords come from the operating system's cryptographic random number generator, and you set either the character length or the entropy target in bits.</p>
  
  <p>
    <a href="https://github.com/muhammetozeski/StrongPasswordGenerator/releases"><img src="https://img.shields.io/github/v/release/muhammetozeski/StrongPasswordGenerator?style=for-the-badge&color=success" alt="Release"></a>
    <a href="https://dotnet.microsoft.com/download/dotnet/10.0"><img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10"></a>
    <a href="https://github.com/muhammetozeski/StrongPasswordGenerator/blob/master/LICENSE"><img src="https://img.shields.io/github/license/muhammetozeski/StrongPasswordGenerator?color=blue&style=for-the-badge" alt="License"></a>
  </p>
</div>

## 🛡️ Security Architecture

- **Randomness source**: every character index and the final Fisher-Yates shuffle use `System.Security.Cryptography.RandomNumberGenerator`, not `System.Random`.
- **Guaranteed set coverage**: one character is drawn from each enabled character set before the rest of the password is filled from the combined pool, then the whole array is shuffled.
- **Offline**: the application makes no network calls. Nothing is uploaded, and there is no telemetry.
- **No persistence**: generated passwords are never written to disk; the application stores no settings or history.

## ✨ Features

- **256-bit default**: on startup the entropy target is 256 bits, and the character length required to reach it is derived from the active character pool.
- **Length and entropy stay in sync**: set the character length and the entropy is recalculated, or set an entropy target and the length is adjusted to match.
- **Character sets**: uppercase, lowercase, digits, special symbols, extended symbols, spaces, plus an option to exclude ambiguous characters (`O 0 l 1 I | B 8`).
- **Custom password analysis**: type your own password into the field and the length, character sets, entropy and crack time are recalculated for it.
- **Attacker threat model**: a logarithmic slider from 100 H/s up to 10^15 H/s, with named reference profiles (online attack, single GPU, botnet, state-sponsored) and an optional quantum toggle that halves the effective entropy.
- **Crack time estimate**: shown alongside a strength tier (Weak / Moderate / Strong / Ultra Secure) derived from that estimate.
- **Clipboard copy**: one click, with visual confirmation.

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
