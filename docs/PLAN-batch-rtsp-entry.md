# PLAN: Batch RTSP Entry (Paste-Bin)

Implementation plan for high-speed bulk camera ingestion.

## Context
Target: Users needing to add many cameras simultaneously.
Strategy: Option A (Multi-line text parsing).

## Phase 1: ViewModel & Parsing Logic
- [x] Create `MultiRtspViewer.ViewModels.BatchImportViewModel`
- [x] Implement `ParseLines` method:
    - Support format: `rtsp://...` (Auto-name: "Cam [N]")
    - Support format: `Name|rtsp://...`
- [x] Validation logic: Use `Uri.IsWellFormedUriString` and verify `rtsp` scheme.
- [x] Statistics: Count valid vs invalid entries for UI feedback.

## Phase 2: UI Components
- [x] Create `MultiRtspViewer.Views.BatchImportDialog.xaml`
    - Large `TextBox` (AcceptsReturn="True", VerticalScrollBarVisibility="Auto")
    - Custom Style: Cyberpunk dark background, cyan caret.
    - Summary Text: "X Cameras ready to import" (Dynamic update via PropertyChanged).
    - Buttons: "Import" and "Cancel".

## Phase 3: Database Integration
- [x] Update `CameraService.cs`:
    - Add `AddCamerasBulk(int clientId, IEnumerable<(string name, string url)> cameraList)`
    - Implement within a single DB transaction for performance.
- [x] Wire up `BatchImportViewModel` to use this service.

## Phase 4: Main UI Wiring
- [x] Update `MainViewModel.cs`:
    - Add `BatchAddCameraCommand`.
    - Logic: Open `BatchImportDialog`, then call `LoadCameras()` to refresh grid.
- [x] Update `MainWindow.xaml`:
    - Add "Bulk Import" button next to the single "Add Camera" button.
    - Icon: Use `Symbol: List` or "++".

## Verification Checklist
- [x] **Test Case 1: Simple URLs** - Paste 5 raw URLs. Expect "Camera 1" to "Camera 5".
- [x] **Test Case 2: Named Entry** - Paste `Lobby | rtsp://...`. Expect name "Lobby".
- [x] **Test Case 3: Mixed/Invalid** - Paste 2 valid, 1 invalid. Expect 2 imported, warning shown for the failure.
- [x] **Test Case 4: Client Scoping** - Verify cameras go to the *selected* client only.
