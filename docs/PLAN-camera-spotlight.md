# PLAN: Camera Spotlight & Grid Optimization

Implementation plan for high-performance multi-stream monitoring with high-quality inspection capability.

## Phase 1: Automatic Grid Optimization
- [ ] Update `CameraViewModel.cs`:
    - Modify `PlayInternal()` to accept a `bool isFullQuality` parameter.
    - If `false` (Default for Grid):
        - Add `:avcodec-lowres=2`
        - Add `:no-audio`
    - If `true` (Spotlight):
        - Set `:avcodec-lowres=0`
        - Enable audio.

## Phase 2: Spotlight Dialog Component
- [ ] Create `MultiRtspViewer.Views.SpotlightDialog.xaml`
    - Design a large window (e.g., 1280x720 or 1600x900).
    - Include a `VideoView` occupying the full area.
    - Add a "CLOSE" button in the top-right corner.
- [ ] Create code-behind and logic to initialize a dedicated, full-quality `CameraViewModel`.

## Phase 3: Interaction & Commands
- [ ] Update `MainViewModel.cs`:
    - Add `OpenSpotlightCommand`.
    - Logic: Instantiate `SpotlightDialog`, pass the camera URL, and show window.
- [ ] Update `CameraGridItem.xaml`:
    - Add a `MouseButtonEventArgs` trigger or a `Button` wrapper for `MouseDoubleClick`.
    - Bind it to `OpenSpotlightCommand` on the parent DataContext.

## Phase 4: Verification
- [ ] **Performance Test**: Compare CPU usage with 4x4 Grid before vs. after optimization.
- [ ] **Visual Test**: Verify grid cameras appear slightly lower resolution (optimized) while spotlight is sharp.
- [ ] **Audio Test**: Verify grid is silent and spotlight has sound.
