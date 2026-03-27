using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PRSC_Player_Auction_System
{
    public partial class MainForm : Form
    {
        private List<Player> players = new List<Player>();

        // ═══════════════════════════════════════════════════════════════
        //  PUBLIC FUND PROPERTIES  ← used by FullScreenAuctionForm
        // ═══════════════════════════════════════════════════════════════
        public decimal TeamAFund
        {
            get
            {
                decimal.TryParse(txtTeamAFund.Text.Replace(",", "").Trim(), out decimal v);
                return v;
            }
            set
            {
                txtTeamAFund.Text = value.ToString("N0");
                try { DatabaseHelper.UpdateTeamFund(txtTeamAName.Text, value); } catch { }
            }
        }

        public decimal TeamBFund
        {
            get
            {
                decimal.TryParse(txtTeamBFund.Text.Replace(",", "").Trim(), out decimal v);
                return v;
            }
            set
            {
                txtTeamBFund.Text = value.ToString("N0");
                try { DatabaseHelper.UpdateTeamFund(txtTeamBName.Text, value); } catch { }
            }
        }

        public string TeamAName => txtTeamAName.Text;
        public string TeamBName => txtTeamBName.Text;

        // ═══════════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public MainForm()
        {
            InitializeComponent();
            LoadFromDatabase();
        }

        // ═══════════════════════════════════════════════════════════════
        //  LOAD FROM DATABASE
        // ═══════════════════════════════════════════════════════════════
        private void LoadFromDatabase()
        {
            try
            {
                players = DatabaseHelper.GetAllPlayers();
                txtTeamAFund.Text = DatabaseHelper.GetTeamFund(txtTeamAName.Text).ToString("N0");
                txtTeamBFund.Text = DatabaseHelper.GetTeamFund(txtTeamBName.Text).ToString("N0");
            }
            catch (Exception ex)
            {
                players = new List<Player>();
                lblStatusBar.Text = $"  ⚠ DB unavailable — offline mode. ({ex.Message})";
            }

            BindGrid();
            UpdateStats();
        }

        // ═══════════════════════════════════════════════════════════════
        //  GRID
        // ═══════════════════════════════════════════════════════════════
        private void BindGrid()
        {
            dgvPlayers.AutoGenerateColumns = false;
            var bs = new BindingSource { DataSource = players };
            dgvPlayers.DataSource = bs;
        }

        public void RefreshGrid()   // public so FullScreenAuctionForm can call it
        {
            if (dgvPlayers.DataSource is BindingSource bs)
                bs.ResetBindings(false);
            else
                BindGrid();

            UpdateStats();
        }

        // ═══════════════════════════════════════════════════════════════
        //  STATS
        // ═══════════════════════════════════════════════════════════════
        private void UpdateStats()
        {
            int total = players.Count;
            int sold = players.Count(p => p.IsSold);
            lblSoldPlayersCount.Text = $"Sold: {sold} / {total}";
            lblStatusBar.Text = $"  Players: {total}  |  Sold: {sold}  |  Available: {total - sold}";
        }

        // ═══════════════════════════════════════════════════════════════
        //  CELL FORMATTING
        // ═══════════════════════════════════════════════════════════════
        private void dgvPlayers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= players.Count) return;

            var player = players[e.RowIndex];
            var row = dgvPlayers.Rows[e.RowIndex];

            if (player.IsSold)
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(120, 120, 120);
                row.DefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(50, 50, 50);
            }
            else
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(220, 220, 220);
                row.DefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 100, 0);
            }

            if (dgvPlayers.Columns[e.ColumnIndex].Name == "col_Status")
            {
                e.CellStyle.ForeColor = player.IsSold
                    ? Color.FromArgb(255, 80, 80)
                    : Color.FromArgb(50, 205, 50);
                e.CellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            }

            if (dgvPlayers.Columns[e.ColumnIndex].Name == "col_AssignedTeam" && player.IsSold)
                e.CellStyle.ForeColor = Color.FromArgb(100, 180, 255);
        }

        // ═══════════════════════════════════════════════════════════════
        //  ADD PLAYER
        // ═══════════════════════════════════════════════════════════════
        private void btnAddPlayer_Click(object sender, EventArgs e)
        {
            using (var dlg = new AddPlayerForm())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var p = dlg.Result;
                    try { p.Id = DatabaseHelper.AddPlayer(p); }
                    catch { p.Id = players.Count > 0 ? players.Max(x => x.Id) + 1 : 1; }

                    players.Add(p);
                    RefreshGrid();

                    if (dgvPlayers.Rows.Count > 0)
                        dgvPlayers.Rows[dgvPlayers.Rows.Count - 1].Selected = true;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  EDIT PLAYER
        // ═══════════════════════════════════════════════════════════════
        private void btnEditPlayer_Click(object sender, EventArgs e)
        {
            var player = GetSelectedPlayer();
            if (player == null) { ShowInfo("Please select a player to edit."); return; }

            using (var dlg = new AddPlayerForm(player))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var ed = dlg.Result;
                    player.Name = ed.Name;
                    player.Position = ed.Position;
                    player.SkillLevel = ed.SkillLevel;
                    player.BasePrice = ed.BasePrice;
                    player.SoldPrice = ed.SoldPrice;
                    player.AssignedTeam = ed.AssignedTeam;
                    player.Status = ed.Status;
                    player.VideoPath = ed.VideoPath;

                    try { DatabaseHelper.UpdatePlayer(player); } catch { }
                    RefreshGrid();
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  DELETE PLAYER
        // ═══════════════════════════════════════════════════════════════
        private void btnDeletePlayer_Click(object sender, EventArgs e)
        {
            var player = GetSelectedPlayer();
            if (player == null) { ShowInfo("Please select a player to delete."); return; }

            if (MessageBox.Show($"Delete \"{player.Name}\"?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { DatabaseHelper.DeletePlayer(player.Id); } catch { }
                players.Remove(player);
                RefreshGrid();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  LOTTERY  ← opens FullScreenAuctionForm for selected player
        // ═══════════════════════════════════════════════════════════════
        private void btnLottery_Click(object sender, EventArgs e)
        {
            var available = players.Where(p => !p.IsSold).ToList();
            if (available.Count == 0)
            {
                ShowInfo("No available players for auction.");
                return;
            }

            // Pick random available player
            var picked = available[new Random().Next(available.Count)];

            // Open full-screen auction form
            using (var auction = new FullScreenAuctionForm(picked, this))
            {
                auction.ShowDialog();
            }

            // Refresh after auction closes (fund & sold status may have changed)
            RefreshGrid();
        }

        // ═══════════════════════════════════════════════════════════════
        //  RESET
        // ═══════════════════════════════════════════════════════════════
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset ALL auction data? This cannot be undone.",
                    "Confirm Reset", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { DatabaseHelper.ResetAllPlayers(); } catch { }
                LoadFromDatabase();
                lblStatusBar.Text = "  Auction reset. Ready to begin.";
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════
        private Player GetSelectedPlayer()
        {
            if (dgvPlayers.SelectedRows.Count == 0) return null;
            int idx = dgvPlayers.SelectedRows[0].Index;
            return (idx >= 0 && idx < players.Count) ? players[idx] : null;
        }

        private void ShowInfo(string msg) =>
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}