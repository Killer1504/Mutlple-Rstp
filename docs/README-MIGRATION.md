# Migration to SQLite

## Overview
The application has moved from a flat JSON configuration (`cameras.json`) to a relational SQLite database (`multirtsp.db`). This enables:
- Multiple Clients (grouping cameras)
- Better data integrity
- Faster saving/loading

## Migration Process
1. On first startup, the app checks for `MultiRtspViewer/multirtsp.db` in your Local AppData.
2. If the DB is empty, it checks for the old `cameras.json`.
3. It creates a "Default Client" and moves all existing cameras into it.
4. Your old config is preserved but ignored by the new version.

## Database Location
`%LOCALAPPDATA%\MultiRtspViewer\multirtsp.db`

## How to use
- Use the **Left Sidebar** to manage Clients (sites/stores).
- Select a client to view its cameras.
- Cameras added are assigned to the **currently selected** client.
