# Cryptographic Password Generator

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![C# 14](https://img.shields.io/badge/C%23-14.0-239120?style=for-the-badge&logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Windows%20WinForms-0078D6?style=for-the-badge&logo=windows)
![License](https://img.shields.io/badge/License-MIT-blue?style=for-the-badge)

A high-performance, modern Windows desktop application for generating cryptographically secure passwords. Built with **C# 14**, **.NET 10**, and **Windows Forms**, featuring a custom dark-mode UI, real-time bit entropy calculation, and hardware-backed cryptographic randomness.

---

## 🌟 Core Features

- **Hardware-Grade Entropy:** Utilizes `System.Security.Cryptography.RandomNumberGenerator` for true cryptographic randomness.
- **Deep Customization:** Fine-grained control over character sets including ambiguous character exclusion (`O`, `0`, `l`, `1`).
- **Dynamic Entropy Math:** Real-time calculation of password pool size and total Shannon entropy (Bits).
- **Crack Time Estimation:** Dynamically calculates how long a 1 PetaHash/s supercomputer would take to crack the password.
- **Custom Password Analysis (Two-Way Sync):** Type your own password and watch length, complexity toggles, and crack time adapt instantly.
- **Responsive UI:** Fully responsive design that cleanly scales with window resizing.
- **Zero-Dependency Portable Native Binary:** Deploys as a single standalone executable (AOT ready).
- **Instant Live Regeneration**: Any adjustment to options, length sliders, or bit inputs dynamically generates a new password immediately.
- **Guaranteed Character Set Inclusion**: Ensures at least one character from every active set is present in the output before shuffling.
- **Fisher-Yates Cryptographic Shuffle**: Prevents positional bias in generated passwords.
- **Fine-Grained Character Toggles**:
  - Uppercase letters (`A-Z`)
  - Lowercase letters (`a-z`)
  - Numeric digits (`0-9`)
  - Standard special symbols (`!@#$%^&*()_+-=[]{}|;:,.<>?`)
  - Extended symbols (`~`/\'"`<>{}`)
  - Ambiguous character filter (`O`, `0`, `l`, `1`, `I`, `|`, `B`, `8`)
  - Space character inclusion (` `)
- **Visual Strength Rating Meter**: Live color-coded visual indicator displaying entropy bits and security tier (*Weak*, *Moderate*, *Strong*, *Ultra Secure*).
- **Clipboard Integration**: One-click password copy with transient visual feedback.

---

## Security Architecture

### Cryptographic Randomness
Unlike standard pseudo-random number generators (`System.Random`), this application utilizes .NET's `System.Security.Cryptography.RandomNumberGenerator`. This provides cryptographically secure entropy generated directly from system kernel entropy sources.

### Mathematical Entropy Model
Password entropy is calculated using Shannon Entropy:
$$E = L \times \log_2(N)$$

Where:
- $E$ = Shannon Entropy in bits
- $L$ = Password length in characters
- $N$ = Total number of unique characters in the active character pool (after exclusions)

### Generation Pipeline
1. **Pool Compilation**: Active character sets are compiled and filtered for ambiguous characters if enabled.
2. **Guaranteed Inclusion**: One random character is sampled from each active character set using `RandomNumberGenerator.GetInt32`.
3. **Unbiased Filling**: Remaining characters are randomly sampled from the combined pool.
4. **Secure Shuffle**: The character array is shuffled using an in-place Fisher-Yates algorithm backed by cryptographic RNG.

---

## Building from Source

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows 10 / 11

### Build Steps
```bash
# Clone repository
git clone https://github.com/muhammetozeski/StrongPasswordGenerator.git
cd StrongPasswordGenerator

# Build solution
dotnet build -c Release
```

### Publishing Executables

**1. Portable Single Executable (Self-Contained):**
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o publish/Portable
```

**2. Framework-Dependent Single Executable:**
```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o publish/FrameworkDependent
```

---

## Releases & Downloads

Pre-built single-file binaries are available on the [GitHub Releases](https://github.com/muhammetozeski/StrongPasswordGenerator/releases) page:
- **`StrongPasswordGenerator.exe`**: Fully portable self-contained executable (no .NET runtime installation required).
- **`StrongPasswordGenerator-FrameworkDependent-RequiresNET10.exe`**: Lightweight executable requiring .NET 10 Desktop Runtime.

---

## License
Distributed under the MIT License. See `LICENSE` for more information.
