# 🛠️ MultiRtspViewer - Technical Developer Guide

**Version:** 1.0.0
**Tech Stack:** .NET 8, WPF, LibVLC, OnnxRuntime (DirectML)

---

## 🏗️ Architecture Overview

This project follows the **MVVM (Model-View-ViewModel)** architectural pattern to ensure separation of concerns and testability.

### High-Level Components

1.  **UI Layer (Views)**: WPF XAML files responsible only for display. They bind to ViewModels.
2.  **Presentation Logic (ViewModels)**: Handles state, commands, and UI logic.
3.  **Service Layer (Services)**: Reusable business logic (AI, Database, Logging).
4.  **Data Layer (Models)**: Data structures and SQLite entities.

```mermaid
graph TD
    UI[Views (XAML)] <--> VM[ViewModels]
    VM --> S1[Camera Service]
    VM --> S2[AI Service]
    VM --> S3[Alert Service]
    S1 --> DB[(SQLite DB)]
    S2 --> ONNX[OnnxRuntime AI]
    S3 --> LOG[Log Files]
```

---

## 🧠 The AI Pipeline (Smart Vision)

The application integrates local AI to detect objects in real-time without sending data to the cloud.

### core Components
*   **`IAIProvider`**: Interface defining how AI services behave.
*   **`YoloV8Service`**: Concrete implementation using YOLOv8 (nano) model.
    *   **GPU Acceleration**: Uses `DirectML` to tap into the User's GPU if available.
    *   **Fallback**: Automatically falls back to CPU if no GPU is found.
*   **`YoloParser`**: Decodes raw tensor outputs from the model into readable bounding boxes (NMS algorithm).

### How it works (The Loop)
1.  **Enable**: User toggles "Enable AI" in the UI.
2.  **Snapshot**: `CameraViewModel` takes a lightweight snapshot of the video stream every ~200ms.
3.  **Inference**: The image is sent to the `YoloV8Service`.
4.  **Draw**: Detected objects are returned, and Red Bounding Boxes are drawn on the `Canvas` layer over the video.

---

## 🎥 Video Core (LibVLCSharp)

We use **LibVLC** (the core of VLC Player) for robust RTSP streaming.

*   **Hardware Decoding**: Enabled by default (`:avcodec-hw=any`) for low CPU usage.
*   **Low Latency**: Configured buffer times (`NetworkCaching`) to minimize lag.
*   **Eco Mode**: When the app is minimized or the grid is hidden, streams can pause to save resources.

---

## 📂 Project Structure

```text
src/MultiRtspViewer/
├── Models/                 # Data objects
│   ├── AI/                 # AI Result models
│   ├── Database/           # SQLite Entities (EF Core style)
│   └── CameraModel.cs      # Observable UI models
├── Services/               # Business Logic
│   ├── AI/                 # YOLO & OnnxRuntime logic
│   ├── Database/           # SQLite Connection & Migrations
│   └── CameraService.cs    # CRUD operations for Cameras
├── ViewModels/             # MVVM Logic
│   ├── MainViewModel.cs    # App Entry & Orchestrator
│   └── CameraViewModel.cs  # Individual Camera Logic (Video + AI)
└── Views/                  # UI (XAML)
    ├── Controls/           # Reusable UI components
    └── CameraView.xaml     # The Video Player + AI Overlay
```

---

## 🚀 Key Workflows

### 1. Adding a Camera
*   User enters RTSP URL.
*   `CameraService` saves it to SQLite `cameras.db`.
*   `MainViewModel` reloads the grid.

### 2. Auto-Reconnection
*   If a stream fails (network loss), `CameraViewModel` enters a **Retry Loop**.
*   Exponential Backoff: Retries in 1s, 2s, 5s... to avoid spamming the network.

---

## 🛠️ Building & Extending

**Prerequisites:**
*   .NET 8 SDK
*   Visual Studio 2022 or VS Code

**Build Command:**
```powershell
dotnet build
```

**Running Tests:**
Currently, manual testing is recommended due to hardware dependencies (GPU/Webcam).
```powershell
dotnet run
```
