# Multi-RTSP Viewer (Cyberpunk Security Edition) 🚀

![License](https://img.shields.io/badge/license-MIT-cyan)
![Platform](https://img.shields.io/badge/platform-Windows-00a2ed)
![Framework](https://img.shields.io/badge/.NET-9.0-512bd4)

An advanced, high-performance RTSP stream viewer designed for security professionals. Built with a **Cyberpunk Security** aesthetic, featuring smooth animations, a multi-client hierarchy, and ultra-fast batch ingestion.

---

## ✨ Key Features

- 🌌 **Cyberpunk Aesthetic**: Sleek dark mode with neon cyan accents and glassmorphism effects.
- ⚡ **Multi-Stream Grid**: Dynamic grid layout (1x1 to 4x4 and Auto) powered by **LibVLC**.
- 🏢 **Multi-Client Hierarchy**: Organize your surveillance network by clients and sub-locations.
- 📋 **Batch RTSP Import**: High-speed "Paste-Bin" ingestion—add 50+ cameras in seconds using simple text parsing.
- 🔄 **Auto-Reconnect**: Robust backoff-based reconnection logic for unstable network streams.
- 📦 **Portable Release**: Single-file executable deployment—no complex installation required.
- 🛠️ **Management Dashboard**: Integrated client manager to rename, delete, and organize your setup.

---

## 🛠️ Technology Stack

- **Core**: .NET 9.0 (WPF)
- **Video Engine**: [LibVLCSharp](https://github.com/videolan/libvlcsharp) (v3.9.5+)
- **Architecture**: MVVM (CommunityToolkit.Mvvm)
- **Database**: SQLite (EF Core 9)
- **Styling**: Vanilla WPF XAML + Storyboard Animations

---

## 🚀 Getting Started

### Prerequisites
- Windows 10 or 11
- [VLC Media Player](https://www.videolan.org/vlc/) installed (optional, app includes bundled LibVLC for portable builds)

### Installation
1. Download the latest release from the `release/` folder.
2. Run `MultiRtspViewer.exe`.

### Development
To build from source:
```powershell
# Clone the repository
git clone https://github.com/your-repo/multi-rtsp.git

# Navigate to the project
cd multi-rtsp/src/MultiRtspViewer

# Build & Run
dotnet run
```

---

## 📦 Building a Release

Use the provided PowerShell scripts for automated builds:

- **Debug Build**: `./build-debug.ps1`
- **Release (Single-File)**: `./build-release.ps1` -> Output in `/release`

---

## 📋 Batch Import Syntax
In the Batch Import dialog, use the following syntax:
```text
Lobby | rtsp://192.168.1.50/stream1
Warehouse | rtsp://admin:pass@192.168.1.60/live
rtsp://wowza.demo.com/stream (Auto-names as "Camera N")
```

---

## 📜 License
This project is licensed under the MIT License - see the LICENSE file for details.

---

## 🤖 Built with Antigravity
*Engineered by Antigravity—the Advanced Agentic Coding Assistant.*
