# PLAN-multi-client-hierarchy

> **Goal:** Transform the application from a single global list to a structured, multi-client management system using SQLite for robustness and a "Client-First" hierarchy for usability.

---

## 🎨 UI/UX Design Specifications
**Theme:** Cyberpunk Security (Dark Mode)
**Palette:** Slate-900 (Bg), Slate-800 (Sidebar), Cyan-400 (Accent), Red-500 (Danger)

### 1. Sidebar Navigation (Left Panel)
- **Width:** Fixed 260px, collapsible.
- **Style:**
  - Background: `#1e293b` (Slate-800) with subtle noise texture.
  - Item Height: 48px (Touch-friendly base).
  - Selected State: Left Cyan border (3px) + Gradient background (`linear-gradient(90deg, #22d3ee10, transparent)`).
- **Typography:**
  - Headers: `Segoe UI` Uppercase, tracked wide.
  - Items: `Inter`/`Segoe UI` Regular.

### 2. Client Dashboard (Main Area)
- **Breadcrumbs:** Top bar showing `Home > Clients > [Client Name]`.
- **Stats Row:** Small cards showing "Total Cameras", "Online", "Offline".
- **Empty State:**
  - Centered "Select Client" message.
  - Animated "Radar Scan" effect or similar cyberpunk visual.

---

## 🏗️ Phase 1: Database Foundation (SQLite)
**Objective:** Replace `cameras.json` with a robust relational database.

- [x] **Install Dependencies**
  - Add NuGet package: `Microsoft.EntityFrameworkCore.Sqlite`
  - Add NuGet package: `Microsoft.EntityFrameworkCore.Tools`
- [x] **Define Entities`** (`Models/Database`)
  - `Client` (Id, Name, Description, CreatedAt)
  - `Camera` (Id, ClientId, Name, RtspUrl, Position, Status) - *Foreign Key to Client*
- [x] **Setup Database Context**
  - Create `AppDbContext` class inheriting from `DbContext`
  - Configure `OnConfiguring` with SQLite connection string
- [x] **Create Migration Service**
  - Implement `DatabaseService.Initialize()` ensuring tables exist
  - **Migration Task:** Read existing `cameras.json`, create a default "Legacy Configuration" client, and move existing cameras there.

## 🧠 Phase 2: Service Layer Refactoring
**Objective:** Abstract logic to handle Client/Camera relationships.

- [x] **Create `ClientService`**
  - Methods: `GetAll()`, `Create(name)`, `Delete(id)`, `Update(client)`
- [x] **Update `ConfigService` -> `CameraService`**
  - Refactor to work with DB instead of JSON
  - Update methods: `GetByClient(clientId)`, `Add(camera, clientId)`, `Delete(id)`

## 🎨 Phase 3: UI - Structure & Navigation
**Objective:** Implement the Sidebar and Client selection workflow.

- [x] **Main Layout Redesign** (`MainWindow.xaml`)
  - Introduce **Grid Layout** with 2 Columns: `Auto` (Sidebar) and `*` (Main Content)
- [x] **Sidebar Component** (`Views/SidebarView.xaml`)
  - List of Clients (ListBox)
  - `+ Add Client` button at top
  - Selected item binds to `MainViewModel.SelectedClient`
- [x] **Sidebar ViewModel** (`ViewModels/SidebarViewModel.cs`)
  - ObservableCollection of `ClientModel`
  - Commands for Adding/Removing Clients

## 🔌 Phase 4: Integration - Client Dashboard
**Objective:** Connect the Sidebar selection to the Main View.

- [x] **Update `MainViewModel`**
  - Add property `CurrentClient` (ClientModel)
  - When `CurrentClient` changes -> Trigger `LoadCameras(clientId)`
  - Bind "Sidebar" visibility (toggleable)
- [x] **Context-Aware "Add Camera"**
  - Modify `AddCameraCommand` to require `CurrentClient != null`
  - Automatically assign the new camera to the selected client ID
  - **Blocker:** If no client selected, show prompt to create/select one.

## 🧪 Phase 5: Verification & Polish
- [x] **Testing**
  - Verify migration of old data
  - detailed CRUD tests for Clients
  - detailed CRUD tests for Cameras within Clients
- [x] **UX Polish**
  - Empty state for "No Client Selected"
  - Breadcrumb or Header showing "Current Client: Store #1"

---

## 📋 Verification Checklist

1. **Migration Check:** Does the app start without crashing and show old cameras under a default client?
2. **Client Isolation:** When switching Client A -> Client B, do cameras update instantly without cross-contamination?
3. **Persistence:** If I restart the app, does it remember the last selected client?
4. **Safety:** Can I delete a client (and does it warn about deleting all its cameras)?
