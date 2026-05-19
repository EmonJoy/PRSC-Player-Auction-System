using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace PRSC_Player_Auction_System
{
    public static class DatabaseHelper
    {
        private static readonly string connectionString =
            @"Server=DESKTOP-BF5OMUT\SQLEXPRESS;Database=PRSC_Auction_DB;Trusted_Connection=True;";

        public static List<Player> GetAllPlayers()
        {
            var players = new List<Player>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var columns = GetPlayerColumns(conn);

                using (var cmd = new SqlCommand("SELECT * FROM Players ORDER BY Id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var p = new Player
                        {
                            Id = (int)reader["Id"],
                            Name = SafeString(reader, "Name"),
                            Position = SafeString(reader, "Position"),
                            SkillLevel = SafeString(reader, "SkillLevel", "Medium"),
                            BasePrice = SafeDecimal(reader, "BasePrice"),
                            SoldPrice = SafeDecimal(reader, "SoldPrice"),
                            AssignedTeam = SafeString(reader, "AssignedTeam", "-"),
                            VideoPath = SafeString(reader, "VideoPath")
                        };

                        if (columns.Contains("Status"))
                            p.Status = SafeString(reader, "Status", p.Status);

                        if (columns.Contains("IsSold"))
                            p.IsSold = SafeBool(reader, "IsSold", p.IsSold);

                        players.Add(p);
                    }
                }
            }

            return players;
        }

        public static int AddPlayer(Player player)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var columns = GetPlayerColumns(conn);
                var values = BuildPlayerValues(player, columns);
                string columnList = string.Join(", ", values.Keys);
                string paramList = string.Join(", ", values.Keys.Select(k => "@" + k));

                string sql = $@"
                    INSERT INTO Players ({columnList})
                    VALUES ({paramList});
                    SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    foreach (var pair in values)
                        cmd.Parameters.AddWithValue("@" + pair.Key, pair.Value ?? DBNull.Value);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void UpdatePlayer(Player player)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var columns = GetPlayerColumns(conn);
                var values = BuildPlayerValues(player, columns);
                string setClause = string.Join(", ", values.Keys.Select(k => $"{k} = @{k}"));
                string sql = $"UPDATE Players SET {setClause} WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    foreach (var pair in values)
                        cmd.Parameters.AddWithValue("@" + pair.Key, pair.Value ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("@Id", player.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeletePlayer(int playerId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new SqlCommand("DELETE FROM Players WHERE Id = @Id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@Id", playerId);
                        cmd.ExecuteNonQuery();
                    }

                    using (var countCmd = new SqlCommand("SELECT COUNT(*) FROM Players", conn, tx))
                    {
                        int remaining = Convert.ToInt32(countCmd.ExecuteScalar());
                        if (remaining == 0)
                        {
                            using (var reseedCmd = new SqlCommand("DBCC CHECKIDENT ('Players', RESEED, 0)", conn, tx))
                                reseedCmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                }
            }
        }

        public static void AssignPlayerToTeam(int playerId, string teamName, decimal soldPrice)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        const string sql = @"
                            UPDATE Players
                            SET IsSold = 1, AssignedTeam = @Team, SoldPrice = @SoldPrice
                            WHERE Id = @Id";

                        using (var cmd = new SqlCommand(sql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Team", teamName);
                            cmd.Parameters.AddWithValue("@SoldPrice", soldPrice);
                            cmd.Parameters.AddWithValue("@Id", playerId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void ResetAllPlayers()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var columns = GetPlayerColumns(conn);

                var assignments = new List<string>();
                if (columns.Contains("IsSold")) assignments.Add("IsSold = 0");
                if (columns.Contains("SoldPrice")) assignments.Add("SoldPrice = 0");
                if (columns.Contains("AssignedTeam")) assignments.Add("AssignedTeam = @AssignedTeam");
                if (columns.Contains("Status")) assignments.Add("Status = @Status");

                if (assignments.Count == 0) return;

                string sql = "UPDATE Players SET " + string.Join(", ", assignments);
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (columns.Contains("AssignedTeam"))
                        cmd.Parameters.AddWithValue("@AssignedTeam", "-");
                    if (columns.Contains("Status"))
                        cmd.Parameters.AddWithValue("@Status", "Available");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static decimal GetTeamFund(string teamName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand(
                    "SELECT SettingValue FROM Settings WHERE SettingName = @Key", conn))
                {
                    cmd.Parameters.AddWithValue("@Key", teamName + "Fund");

                    var result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            $"No fund record found for team '{teamName}'. Please initialise the team fund before running the auction.");
                    }

                    return decimal.Parse(result.ToString(), CultureInfo.InvariantCulture);
                }
            }
        }

        public static void UpdateTeamFund(string teamName, decimal fund)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        const string sql = @"
                            IF EXISTS (SELECT 1 FROM Settings WHERE SettingName = @Key)
                                UPDATE Settings SET SettingValue = @Val WHERE SettingName = @Key
                            ELSE
                                INSERT INTO Settings (SettingName, SettingValue) VALUES (@Key, @Val)";

                        using (var cmd = new SqlCommand(sql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Key", teamName + "Fund");
                            cmd.Parameters.AddWithValue("@Val", fund.ToString(CultureInfo.InvariantCulture));
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static Dictionary<string, object> BuildPlayerValues(Player player, HashSet<string> columns)
        {
            var values = new Dictionary<string, object>();

            void AddIfExists(string column, object value)
            {
                if (columns.Contains(column))
                    values[column] = value ?? DBNull.Value;
            }

            AddIfExists("Name", player.Name ?? "");
            AddIfExists("Position", player.Position ?? "");
            AddIfExists("SkillLevel", player.SkillLevel ?? "Medium");
            AddIfExists("BasePrice", player.BasePrice);
            AddIfExists("SoldPrice", player.SoldPrice);
            AddIfExists("AssignedTeam", player.AssignedTeam ?? "-");
            AddIfExists("IsSold", player.IsSold);
            AddIfExists("VideoPath", string.IsNullOrWhiteSpace(player.VideoPath) ? DBNull.Value : (object)player.VideoPath);
            AddIfExists("Status", player.Status ?? "Available");

            // Legacy compatibility for copied databases that still contain this required column.
            AddIfExists("Value", player.BasePrice);

            return values;
        }

        private static HashSet<string> GetPlayerColumns(SqlConnection conn)
        {
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var cmd = new SqlCommand(
                "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Players'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    columns.Add(reader.GetString(0));
            }

            return columns;
        }

        private static string SafeString(SqlDataReader r, string col, string def = "")
        {
            try { return r[col] == DBNull.Value ? def : r[col].ToString(); }
            catch { return def; }
        }

        private static decimal SafeDecimal(SqlDataReader r, string col, decimal def = 0)
        {
            try { return r[col] == DBNull.Value ? def : Convert.ToDecimal(r[col]); }
            catch { return def; }
        }

        private static bool SafeBool(SqlDataReader r, string col, bool def = false)
        {
            try { return r[col] == DBNull.Value ? def : Convert.ToBoolean(r[col]); }
            catch { return def; }
        }
    }
}
