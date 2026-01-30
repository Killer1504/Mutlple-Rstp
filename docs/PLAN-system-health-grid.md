# Plan: System Health Dashboard & MVP Features

> **Task**: Implement "Option B" (System Health) and MVP Grid features for the Multi-RTSP Manager.
> **Context**: User selected "System Health Dashboard" focus (reliability) + "Visual Command Center" MVP (grid/focus).
> **Goal**: Create a reliable, managed viewing experience with auto-reconnect, status indicators, and grid controls.

## 1. Overview

We are enhancing the `MultiRtspViewer` WPF application to support professional management capabilities. This moves the app from a simple "viewer" to a "security dashboard".

**Key Features:**
1.  **System Health (Reliability)**:
    - Visual Status Indicators (Online/Offline/Reconnecting).
    - Robust Auto-Reconnect logic (Exponential backoff).
    - Connection Event Logs (Toast/Snackbar notifications).
    - "Eco Mode" (Resource optimization when idle/minimized).
2.  **Manager MVP (Usability)**:
    - Grid Layout Persistence (Remember camera positions).
    - Focus Mode (Double-click to maximize/restore).

## 2. Project Type & Tech Stack

-   **Type**: Desktop Application (WPF)
-   **Language**: C# / .NET
-   **Framework**: MVVM (Confirmed existing structure)
-   **Video**: LibVLCSharp (Existing)

## 3. Success Criteria

-   [ ] **Reliability**: Disconnecting a camera cable results in a "Reconnecting" UI state within 5 seconds, and automatic restoration when reconnected.
-   [ ] **Usability**: Double-clicking a stream maximizes it; double-clicking again restores the exact grid capability.
-   [ ] **Persistence**: Restarting the app restores the exact same cameras in the exact same grid slots.
-   [ ] **Performance**: "Eco Mode" reduces CPU usage by >20% when app is minimized.

## 4. Architecture & File Structure

No new major directories needed. We will enhance existing MVVM classes.

```text
src/MultiRtspViewer/
├── ViewModels/
│   ├── CameraViewModel.cs       # UPDATE: Add ConnectionStatus, ReconnectLogic
│   ├── MainViewModel.cs         # UPDATE: Add GridPersistence, FocusMode logic
│   └── SettingsViewModel.cs     # UPDATE: Add EcoMode config
├── Views/
│   ├── MainWindow.xaml          # UPDATE: Add Grid triggers, Status Indicators
│   └── CameraGridItem.xaml      # UPDATE: Add Visual Overlays (Status dots)
├── Services/
│   ├── CameraService.cs         # UPDATE: Handle connection events
│   └── ConfigService.cs         # UPDATE: Save/Load Layouts
└── Models/
    └── AppSettings.cs           # UPDATE: Add Layout schema
```

## 5. Task Breakdown

### Phase 1: System Health (Reliability)

| Task ID | Name | Agent | Skills | Description |
| :--- | :--- | :--- | :--- | :--- |
| **H1** | **Add Connection State Model** | `backend-specialist` | `clean-code` | Update `CameraViewModel` with `ConnectionStatus` enum (Connected, Connecting, Offline, Error) and `LastHeartbeat` timestamp. |
| **H2** | **Implement Auto-Reconnect** | `backend-specialist` | `systematic-debugging` | Implement `RetryPolicy` in `CameraViewModel` or `CameraService`. Use exponential backoff (1s, 2s, 5s, 10s). Ensure `LibVLC` is disposed/recreated correctly on retry. |
| **H3** | **Health UI Indicators** | `frontend-specialist` | `frontend-design` | Update `CameraGridItem.xaml` to show a status dot (Green/Red/Yellow) and a simplified "Reconnecting..." overlay when offline. Bind to `ConnectionStatus`. |
| **H4** | **Eco Mode Logic** | `backend-specialist` | `performance-profiling` | Add logic to Pause/Stop streams when `MainWindow` state is Minimized or when a specific "Eco Mode" toggle is active. |

### Phase 2: MVP Layout Tools

| Task ID | Name | Agent | Skills | Description |
| :--- | :--- | :--- | :--- | :--- |
| **M1** | **Focus Mode (Zoom)** | `frontend-specialist` | `frontend-design` | Implement Double-Click on `CameraGridItem`. Logic: `MainViewModel` sets `SelectedCamera`, hides other items (or expands grid cell). Double-click again restores. |
| **M2** | **Grid Persistence Schema** | `backend-specialist` | `database-design` | Update `AppSettings.cs` to include `GridLayout` definition (List of CameraIDs in order). |
| **M3** | **Save/Load Layout** | `backend-specialist` | `clean-code` | On app close (or change), save current grid order. On startup, load cameras in that specific order. |
| **M4** | **Connection Notifications** | `frontend-specialist` | `frontend-design` | Add a simple `Snackbar`/`Toast` message queue in `MainViewModel` to show "Camera X Offline" alerts. |

## 6. Phase X: Verification

### Phase 1: System Health ✅
- [x] **Manual Test**: Unplug network cable for 10s. Verify visual "Offline". Plug back in. Verify "Green" status automatically.
- [x] **Manual Test**: Eco Mode - Minimize app, verify CPU drop and "Paused" status.

### Phase 2: MVP Layout Tools ✅
- [x] **Manual Test**: Change grid layout (2x2 → 3x3). Close App. Open App. Verify 3x3 is remembered.
- [x] **Manual Test**: Double-click camera 1. Verify Spotlight opens (Focus Mode).
- [x] **Manual Test**: Disconnect a camera. Verify toast notification appears at bottom-right.
- [x] **Manual Test**: Reconnect camera. Verify success notification appears.

