# PLAN-rtsp-grid-viewer

> **Goal**: Build a Windows Desktop App (WPF) for unlimited dynamic RTSP video monitoring.
> **Tech Stack**: C# .NET 8, WPF, LibVLCSharp, CommunityToolkit.Mvvm.

## 📦 Phase 1: Core Foundation (MVP)
*Goal: Get one stream playing in a modern WPF window.*

- [ ] **Project Initialization**
  - Create WPF Project (.NET 8)
  - Install NuGet: `LibVLCSharp.WPF`, `CommunityToolkit.Mvvm`, `Newtonsoft.Json`.
  - Configure `App.xaml` for LibVLC initialization.
- [ ] **MVVM Scaffolding**
  - `MainViewModel`: Holds the list of cameras.
  - `CameraViewModel`: Wraps the VLC Player logic.
- [ ] **Basic UI**
  - Single `VideoView` implementation.
  - Test with 1 hardcoded RTSP stream.

## 💾 Phase 2: Data & Configuration
*Goal: UI for adding/editing cameras and saving state.*

- [ ] **Persistence Layer**
  - Create `ConfigService`.
  - Define `CameraModel` (Id, Name, Url, Status).
  - Implement `Load/Save` to `cameras.json` in AppData.
- [ ] **Management UI**
  - Add "Add Camera" Dialog (Modal).
  - Add Context Menu (Right-click -> Edit/Delete).

## 🔲 Phase 3: Unleashing the Grid
*Goal: Handle "Unlimited" dynamic layout.*

- [ ] **Dynamic Layout Engine**
  - Replace static Grid with `ItemsControl` using `UniformGrid` as ItemsPanel.
  - Bind `ItemsSource` to `ObservableCollection<CameraViewModel>`.
  - **New Feature**: Add "Grid Layout Selector" (ComboBox/Buttons) for users to force 3x3, 4x4, or "Auto" modes.
  - Implement binding for `UniformGrid.Rows` and `UniformGrid.Columns`.
- [ ] **Performance Tuning (Critical)**
  - Enable Hardware Acceleration in LibVLC (`:avcodec-hw=d3d11va`).
  - Implement "mute audio" by default (saves CPU).
  - Implement "lazy loading" (don't start stream until attached to view).

## 🚀 Phase 4: Robustness & Polish
*Goal: Production readiness.*

- [ ] **Auto-Reconnect System (Priority)**
  - Detect `EncounteredError` or `EndReached`.
  - Implement reliable "Watchdog" timer to restart frozen streams.
  - Exponential backoff retry logic (e.g., 2s, 5s, 10s).
- [ ] **UX Polish**
  - Overlay Name/Status on video.
  - Loading spinners (Skeleton loader style).
  - "No Signal" placeholder.


## 🎨 Design System (Pro Max)
> **Style**: Cyberpunk Security / Dark Glass
> **Font**: Fira Sans (Headers), Roboto (UI), JetBrains Mono (Overlays)

- [ ] **Global Theme**
  - Background: `Slate-900` (#0f172a)
  - Surface: `Slate-800` (Glassmorphism 80% opacity)
  - Accent: `Cyan-400` (#22d3ee) (Active Streams), `Rose-500` (Errors)
- [ ] **Stream Container**
  - 1px Cyan Border on Active/Selected.
  - "No Signal" Placeholder: CRT Scanline effect (ShaderEffect).
  - Overlay: Semi-transparent bottom bar with Bitrate/FPS stats.
- [ ] **Controls**
  - Floating Action Button (FAB) for "Add Camera".
  - Layout Selector: Segmented Control (3x3 | 4x4 | Auto).

## 🧑‍💻 Agent Assignments

| Domain | Agent | Responsibility |
|OSS| `backend-specialist` | Persistence (JSON), LibVLC orchestration, Thread management. |
|UI/UX| `frontend-specialist`| WPF XAML, Styles, UniformGrid layout, Dialogs. |
