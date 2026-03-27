using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PRSC_Player_Auction_System
{
    /// <summary>
    /// All DB operations. Change connectionString to match your SQL Server instance.
    /// Table schema expected:
    ///   Players(Id INT PK IDENTITY, Name NVARCHAR, Position NVARCHAR, SkillLevel NVARCHAR,
    ///           BasePrice DECIMAL, SoldPrice DECIMAL, AssignedTeam NVARCHAR,
    ///           IsSold BIT, VideoPath NVARCHAR NULL)
    ///   Settings(SettingName NVARCHAR PK, SettingValue NVARCHAR)
    /// </summary>
    public static class DatabaseHelper
    {
        // ── Change server name here if needed ──────────────────────────
        private static readonly string connectionString =
            @"Server=DESKTOP-BF5OMUT\SQLEXPRESS;Database=PRSC_Auction_DB;Trusted_Connection=True;";

        // ═══════════════════════════════════════════════════════════════
        //  GET ALL PLAYERS
        // ═══════════════════════════════════════════════════════════════
        public static List<Player> GetAllPlayers()
        {
            var players = new List<Player>();

            using (var conn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("SELECT * FROM Players ORDER BY Id", conn);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var p = new Player
                        {
                            Id           = (int)reader["Id"],
                            Name         = reader["Name"].ToString(),
                            Position     = SafeString(reader, "Position"),
                            SkillLevel   = SafeString(reader, "SkillLevel", "Medium"),
                            BasePrice    = (decimal)reader["BasePrice"],
                            SoldPrice    = SafeDecimal(reader, "SoldPrice"),
                            AssignedTeam = SafeString(reader, "AssignedTeam", "—"),
                            VideoPath    = SafeString(reader, "VideoPath"),
                            IsSold       = (bool)reader["IsSold"]   // sets Status too
                        };
                        players.Add(p);
                    }
                }
            }

            return players;
        }

        // ═══════════════════════════════════════════════════════════════
        //  ADD PLAYER  → returns new Id
        // ═══════════════════════════════════════════════════════════════
        public static int AddPlayer(Player player)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                const string sql = @"
                    INSERT INTO Players
                        (Name, Position, SkillLevel, BasePrice, SoldPrice, AssignedTeam, IsSold, VideoPath)
                    VALUES
                        (@Name, @Position, @SkillLevel, @BasePrice, @SoldPrice, @AssignedTeam, @IsSold, @VideoPath);
                    SELECT SCOPE_IDENTITY();";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name",         player.Name);
                cmd.Parameters.AddWithValue("@Position",     player.Position);
                cmd.Parameters.AddWithValue("@SkillLevel",   player.SkillLevel);
                cmd.Parameters.AddWithValue("@BasePrice",    player.BasePrice);
                cmd.Parameters.AddWithValue("@SoldPrice",    player.SoldPrice);
                cmd.Parameters.AddWithValue("@AssignedTeam", player.AssignedTeam);
                cmd.Parameters.AddWithValue("@IsSold",       player.IsSold);
                cmd.Parameters.AddWithValue("@VideoPath",    (object)player.VideoPath ?? DBNull.Value);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  UPDATE PLAYER (full)
        // ═══════════════════════════════════════════════════════════════
        public static void UpdatePlayer(Player player)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                const string sql = @"
                    UPDATE Players SET
                        Name         = @Name,
                        Position     = @Position,
                        SkillLevel   = @SkillLevel,
                        BasePrice    = @BasePrice,
                        SoldPrice    = @SoldPrice,
                        AssignedTeam = @AssignedTeam,
                        IsSold       = @IsSold,
                        VideoPath    = @VideoPath
                    WHERE Id = @Id";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Name",         player.Name);
                cmd.Parameters.AddWithValue("@Position",     player.Position);
                cmd.Parameters.AddWithValue("@SkillLevel",   player.SkillLevel);
                cmd.Parameters.AddWithValue("@BasePrice",    player.BasePrice);
                cmd.Parameters.AddWithValue("@SoldPrice",    player.SoldPrice);
                cmd.Parameters.AddWithValue("@AssignedTeam", player.AssignedTeam);
                cmd.Parameters.AddWithValue("@IsSold",       player.IsSold);
                cmd.Parameters.AddWithValue("@VideoPath",    (object)player.VideoPath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Id",           player.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  DELETE PLAYER
        // ═══════════════════════════════════════════════════════════════
        public static void DeletePlayer(int playerId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM Players WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", playerId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  LOTTERY: mark one player sold and assign team
        // ═══════════════════════════════════════════════════════════════
        public static void AssignPlayerToTeam(int playerId, string teamName, decimal soldPrice)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                const string sql = @"
                    UPDATE Players
                    SET IsSold = 1, AssignedTeam = @Team, SoldPrice = @SoldPrice
                    WHERE Id = @Id";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Team",      teamName);
                cmd.Parameters.AddWithValue("@SoldPrice", soldPrice);
                cmd.Parameters.AddWithValue("@Id",        playerId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  RESET ALL (mark all available, clear sold price & team)
        // ═══════════════════════════════════════════════════════════════
        public static void ResetAllPlayers()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                const string sql = @"
                    UPDATE Players
                    SET IsSold = 0, SoldPrice = 0, AssignedTeam = '—'";

                var cmd = new SqlCommand(sql, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  TEAM FUND  (Settings table)
        // ═══════════════════════════════════════════════════════════════
        public static decimal GetTeamFund(string teamName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(
                    "SELECT SettingValue FROM Settings WHERE SettingName = @Key", conn);
                cmd.Parameters.AddWithValue("@Key", teamName + "Fund");

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? decimal.Parse(result.ToString()) : 100000m;
            }
        }

        public static void UpdateTeamFund(string teamName, decimal fund)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                const string sql = @"
                    IF EXISTS (SELECT 1 FROM Settings WHERE SettingName = @Key)
                        UPDATE Settings SET SettingValue = @Val WHERE SettingName = @Key
                    ELSE
                        INSERT INTO Settings (SettingName, SettingValue) VALUES (@Key, @Val)";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Key", teamName + "Fund");
                cmd.Parameters.AddWithValue("@Val", fund.ToString());

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  SAFE READER HELPERS
        // ═══════════════════════════════════════════════════════════════
        private static string SafeString(SqlDataReader r, string col, string def = "")
        {
            try { return r[col] == DBNull.Value ? def : r[col].ToString(); }
            catch { return def; }
        }

        private static decimal SafeDecimal(SqlDataReader r, string col, decimal def = 0)
        {
            try { return r[col] == DBNull.Value ? def : (decimal)r[col]; }
            catch { return def; }
        }
    }
}
