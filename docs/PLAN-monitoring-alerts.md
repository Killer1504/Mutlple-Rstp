# Plan: Monitoring & Alerts System

## 1. Objective
Transform `MultiRtspViewer` from a passive viewer into an active monitoring system. It will log system events to local files and send external alerts (Discord/Telegram) when cameras are offline for a specified duration (default 15-20 mins).

## 2. Architecture & Design

### 2.1. Logging Strategy
- **Local Logs:** Use simple file-based logging (`logs/system_YYYYMMDD.log`).
- **Rotation:** Daily log rotation to prevent massive files.
- **Content:** Startup/shutdown events, camera connection/disconnection events, and alarm triggers.

### 2.2. Alerting Logic
- **Watchdog:** A background service polling every 1 minute.
- **Trigger Condition:** `AlertEnabled == true` AND `Camera.Status == Offline` AND `(Now - LastHeartbeat) > ThresholdMinutes`.
- **Spam Prevention:** Only send ONE alert per outage incident. Reset state on reconnection.

### 2.3. User Interface (Settings)
- New Tab: **"Notifications"**
- **Master Toggle:** Enable/Disable Alerts.
- **Threshold:** Input (minutes).
- **Service Provider:** Dropdown [ Discord | Telegram | Generic Webhook ].
- **Dynamic Inputs:**
  - **Discord:** `Webhook URL`
  - **Telegram:** `Bot Token`, `Chat ID`
- **Action:** `Test Alert` button to verify configuration immediately.

## 3. Implementation Steps

### Phase 1: Foundations (Logging & Config)
- [ ] **Task 1.1: Logging Infrastructure**
  - Create `LogService` to write timestamped logs to `AppData`.
  - Log app startup and camera state changes.
- [ ] **Task 1.2: Settings Model Update**
  - Add `NotificationSettings` class to `AppSettings`.
  - Properties: `Enabled`, `ThresholdMinutes`, `ProviderType`, `WebhookUrl`, `BotToken`, `ChatId`.

### Phase 2: Alert Service (Backend)
- [ ] **Task 2.1: Alert Service Core**
  - Create `AlertService` class (Singleton).
  - Implement `CheckMonitoring()` loop (Timer based).
  - Implement logic to track "Alert Sent" state per camera to avoid spam.
- [ ] **Task 2.2: Provider Adapters**
  - Implement `SendDiscordAlert(message)` (HTTP POST).
  - Implement `SendTelegramAlert(message)` (HTTP POST).
  - Implement `SendGenericAlert(message)` (JSON POST).

### Phase 3: Settings UI
- [ ] **Task 3.1: Settings View Update**
  - Add "Notifications" tab to `SettingsView.xaml`.
  - Implement dynamic visibility (show/hide Telegram fields based on dropdown).
- [ ] **Task 3.2: Test Button Logic**
  - Implement `TestAlertCommand` in simple `SettingsViewModel` extension or `MainViewModel`.

### Phase 4: Integration
- [ ] **Task 4.1: Wire it up**
  - initialize `AlertService` in `App.xaml.cs` or `MainViewModel`.
  - Ensure it respects the `EcoMode` (alerts should probably still work even if "Paused"? -> *Decision: If Paused by EcoMode, do NOT alert? Or Alert? Assuming Paused = Intentional, so NO Alert.*).

## 4. Verification Checkpoints (Phase X)

- [ ] **Log Test:** Run app, connect/disconnect camera. Check `logs` folder for entries.
- [ ] **Discord Test:** Configure valid Webhook. Click "Test". Check Discord channel.
- [ ] **Telegram Test:** Configure Bot/ChatID. Click "Test". Check Telegram app.
- [ ] **Threshold Test:** Set threshold to 1 min. Disconnect camera. Wait 1 min. Verify alert arrives.
- [ ] **Recovery Test:** Reconnect camera. Verify "Camera Recovered" log entry (and optional "Up" alert if desired).
