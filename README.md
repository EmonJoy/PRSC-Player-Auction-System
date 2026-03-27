# PRSC Player Auction System (SQL Server Edition)

A complete, real-time player auction system built with C# WinForms and SQL Server (SSMS).

## Features
- **SQL Server Integration**: All player data and team funds are persisted in a database.
- **Main Dashboard**: Manage player list, view sold counts, and edit team funds.
- **Add Player**: Add players with name, base price, and video/image path.
- **Lottery System**: Randomly selects an unsold player to start an auction.
- **Full-Screen Auction**: High-impact auction interface with real-time price updates and team fund tracking.
- **Media Support**: Displays image previews or video paths for manual playback.

## Setup Instructions

### 1. Database Setup (SSMS)
1. Open **SQL Server Management Studio (SSMS)**.
2. Connect to your local SQL Server instance.
3. Open the `SQL_Schema.sql` file included in this project.
4. Execute the script to create the `PRSC_Auction_DB` database and required tables.

### 2. Connection String Configuration
1. Open the project in **Visual Studio**.
2. Open `DatabaseHelper.cs`.
3. Update the `connectionString` variable if your SQL Server instance name is different from the default (`Server=.;`).

### 3. How to Run
1. Ensure the target framework is set to **.NET Framework 4.7.2**.
2. Build the solution (Ctrl+Shift+B).
3. Run the application (F5).

## Troubleshooting Resource Errors
If you previously saw errors like "Resource file cannot be found", this version fixes them by:
- Removing the dependency on external `.resx` files for simple UI elements.
- Ensuring all forms are correctly defined in the `.csproj` file.
- Using code-based initialization for UI components.

## Notes
- **Video Playback**: The system saves the video path in the database. During the auction, it displays the path. You can open the video file on your laptop for high-quality playback during the event.
- **Funds**: Funds are updated in real-time in the database whenever a player is sold or funds are manually edited.
