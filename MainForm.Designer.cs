using System;

namespace PRSC_Player_Auction_System
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvPlayers      = new System.Windows.Forms.DataGridView();
            this.pnlToolbar      = new System.Windows.Forms.Panel();
            this.btnAddPlayer    = new System.Windows.Forms.Button();
            this.btnEditPlayer   = new System.Windows.Forms.Button();
            this.btnDeletePlayer = new System.Windows.Forms.Button();
            this.btnLottery      = new System.Windows.Forms.Button();
            this.btnReset        = new System.Windows.Forms.Button();
            this.pnlFooter       = new System.Windows.Forms.Panel();
            this.pnlDivider      = new System.Windows.Forms.Panel();
            this.pnlTopDivider   = new System.Windows.Forms.Panel();
            this.lblTitle        = new System.Windows.Forms.Label();
            this.lblTeamALabel   = new System.Windows.Forms.Label();
            this.txtTeamAName    = new System.Windows.Forms.TextBox();
            this.txtTeamAFund    = new System.Windows.Forms.TextBox();
            this.lblTeamAFundLbl = new System.Windows.Forms.Label();
            this.lblTeamBLabel   = new System.Windows.Forms.Label();
            this.txtTeamBName    = new System.Windows.Forms.TextBox();
            this.txtTeamBFund    = new System.Windows.Forms.TextBox();
            this.lblTeamBFundLbl = new System.Windows.Forms.Label();
            this.lblSoldPlayersCount = new System.Windows.Forms.Label();
            this.lblStatusBar    = new System.Windows.Forms.Label();
            this.pnlTeamA        = new System.Windows.Forms.Panel();
            this.pnlTeamB        = new System.Windows.Forms.Panel();
            this.pnlStats        = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).BeginInit();
            this.pnlToolbar.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();

            // ─────────────────── FORM ───────────────────────────────────
            this.Text        = "PRSC Player Auction System";
            this.BackColor   = System.Drawing.Color.FromArgb(10, 10, 10);
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MinimumSize = new System.Drawing.Size(960, 620);

            // ─────────────────── TITLE ──────────────────────────────────
            this.lblTitle.Dock      = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Height    = 68;
            this.lblTitle.Text      = "⚽  PRSC PLAYER AUCTION SYSTEM  ⚽";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Font      = new System.Drawing.Font("Impact", 26F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(5, 5, 5);

            // ─────────────────── TOP DIVIDER ────────────────────────────
            this.pnlTopDivider.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTopDivider.Height    = 2;
            this.pnlTopDivider.BackColor = System.Drawing.Color.FromArgb(50, 205, 50);

            // ─────────────────── TOOLBAR ────────────────────────────────
            this.pnlToolbar.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height    = 65;
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(18, 18, 18);

            MakeButton(this.btnAddPlayer,    "➕  Add Player",   10,  System.Drawing.Color.FromArgb(34, 139, 34));
            MakeButton(this.btnEditPlayer,   "✏️  Edit Player",   155, System.Drawing.Color.FromArgb(60, 120, 200));
            MakeButton(this.btnDeletePlayer, "🗑️  Delete",        300, System.Drawing.Color.FromArgb(180, 40, 40));
            MakeButton(this.btnLottery,      "🎲  Lottery",       445, System.Drawing.Color.FromArgb(130, 0, 200));
            MakeButton(this.btnReset,        "🔄  Reset All",     590, System.Drawing.Color.FromArgb(80, 80, 80));

            this.pnlToolbar.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.btnAddPlayer, this.btnEditPlayer, this.btnDeletePlayer,
                this.btnLottery,   this.btnReset
            });

            this.btnAddPlayer.Click    += new System.EventHandler(this.btnAddPlayer_Click);
            this.btnEditPlayer.Click   += new System.EventHandler(this.btnEditPlayer_Click);
            this.btnDeletePlayer.Click += new System.EventHandler(this.btnDeletePlayer_Click);
            this.btnLottery.Click      += new System.EventHandler(this.btnLottery_Click);
            this.btnReset.Click        += new System.EventHandler(this.btnReset_Click);

            // ─────────────────── DIVIDER ────────────────────────────────
            this.pnlDivider.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlDivider.Height    = 2;
            this.pnlDivider.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);

            // ═════════════════ DATAGRIDVIEW ═══════════════════════════════
            this.dgvPlayers.Dock            = System.Windows.Forms.DockStyle.Fill;
            this.dgvPlayers.BackgroundColor = System.Drawing.Color.FromArgb(12, 12, 12);
            this.dgvPlayers.BorderStyle     = System.Windows.Forms.BorderStyle.None;
            this.dgvPlayers.GridColor       = System.Drawing.Color.FromArgb(38, 38, 38);

            // Column headers
            this.dgvPlayers.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            this.dgvPlayers.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.dgvPlayers.ColumnHeadersDefaultCellStyle.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.dgvPlayers.ColumnHeadersDefaultCellStyle.Padding   = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.dgvPlayers.ColumnHeadersHeight = 40;
            this.dgvPlayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvPlayers.EnableHeadersVisualStyles   = false;

            // Cells
            this.dgvPlayers.DefaultCellStyle.BackColor          = System.Drawing.Color.FromArgb(15, 15, 15);
            this.dgvPlayers.DefaultCellStyle.ForeColor          = System.Drawing.Color.FromArgb(220, 220, 220);
            this.dgvPlayers.DefaultCellStyle.Font               = new System.Drawing.Font("Segoe UI", 10F);
            this.dgvPlayers.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(0, 100, 0);
            this.dgvPlayers.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvPlayers.DefaultCellStyle.Padding            = new System.Windows.Forms.Padding(8, 0, 0, 0);

            this.dgvPlayers.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.dgvPlayers.AlternatingRowsDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);

            this.dgvPlayers.RowHeadersVisible    = false;
            this.dgvPlayers.AllowUserToAddRows    = false;
            this.dgvPlayers.AllowUserToDeleteRows = false;
            this.dgvPlayers.ReadOnly             = true;
            this.dgvPlayers.SelectionMode        = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlayers.AutoSizeColumnsMode  = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPlayers.RowTemplate.Height   = 36;
            this.dgvPlayers.CellBorderStyle      = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvPlayers.MultiSelect          = false;
            this.dgvPlayers.AutoGenerateColumns  = false;   // ← KEY FIX

            // ── Columns (DataPropertyName must match Player property names) ──
            AddColumn("#",             "Id",           5,   false);
            AddColumn("Player Name",   "Name",         22,  false);
            AddColumn("Position",      "Position",     13,  false);
            AddColumn("Skill",         "SkillLevel",   10,  false);
            AddColumn("Base Price (৳)","BasePrice",    14,  true);
            AddColumn("Sold Price (৳)","SoldPrice",    14,  true);
            AddColumn("Assigned Team", "AssignedTeam", 14,  false);
            AddColumn("Status",        "Status",       8,   false, "colStatus");
            AddColumn("Video",         "VideoPath",    0,   false);   // hidden (weight 0 → minimal)

            this.dgvPlayers.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPlayers_CellFormatting);

            // ─────────────────── FOOTER ─────────────────────────────────
            this.pnlFooter.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height    = 100;
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(8, 8, 8);

            // Team A
            this.pnlTeamA.Size        = new System.Drawing.Size(290, 72);
            this.pnlTeamA.Location    = new System.Drawing.Point(14, 14);
            this.pnlTeamA.BackColor   = System.Drawing.Color.FromArgb(0, 55, 0);
            this.pnlTeamA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblTeamALabel.Text      = "🏆  TEAM A";
            this.lblTeamALabel.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.lblTeamALabel.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTeamALabel.Location  = new System.Drawing.Point(10, 6);
            this.lblTeamALabel.AutoSize  = true;

            this.txtTeamAName.Location    = new System.Drawing.Point(10, 28);
            this.txtTeamAName.Size        = new System.Drawing.Size(130, 26);
            this.txtTeamAName.Text        = "Team Alpha";
            this.txtTeamAName.BackColor   = System.Drawing.Color.FromArgb(0, 35, 0);
            this.txtTeamAName.ForeColor   = System.Drawing.Color.White;
            this.txtTeamAName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamAName.Font        = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            this.lblTeamAFundLbl.Text      = "৳";
            this.lblTeamAFundLbl.ForeColor = System.Drawing.Color.Gold;
            this.lblTeamAFundLbl.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTeamAFundLbl.Location  = new System.Drawing.Point(147, 30);
            this.lblTeamAFundLbl.AutoSize  = true;

            this.txtTeamAFund.Location   = new System.Drawing.Point(162, 28);
            this.txtTeamAFund.Size       = new System.Drawing.Size(118, 26);
            this.txtTeamAFund.Text       = "100,000";
            this.txtTeamAFund.BackColor  = System.Drawing.Color.FromArgb(0, 35, 0);
            this.txtTeamAFund.ForeColor  = System.Drawing.Color.Gold;
            this.txtTeamAFund.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamAFund.Font       = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtTeamAFund.TextAlign  = System.Windows.Forms.HorizontalAlignment.Right;

            this.pnlTeamA.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lblTeamALabel, this.txtTeamAName,
                this.lblTeamAFundLbl, this.txtTeamAFund
            });

            // Team B
            this.pnlTeamB.Size        = new System.Drawing.Size(290, 72);
            this.pnlTeamB.Location    = new System.Drawing.Point(316, 14);
            this.pnlTeamB.BackColor   = System.Drawing.Color.FromArgb(0, 0, 55);
            this.pnlTeamB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblTeamBLabel.Text      = "🏆  TEAM B";
            this.lblTeamBLabel.ForeColor = System.Drawing.Color.FromArgb(100, 149, 237);
            this.lblTeamBLabel.Font      = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTeamBLabel.Location  = new System.Drawing.Point(10, 6);
            this.lblTeamBLabel.AutoSize  = true;

            this.txtTeamBName.Location    = new System.Drawing.Point(10, 28);
            this.txtTeamBName.Size        = new System.Drawing.Size(130, 26);
            this.txtTeamBName.Text        = "Team Beta";
            this.txtTeamBName.BackColor   = System.Drawing.Color.FromArgb(0, 0, 35);
            this.txtTeamBName.ForeColor   = System.Drawing.Color.White;
            this.txtTeamBName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamBName.Font        = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            this.lblTeamBFundLbl.Text      = "৳";
            this.lblTeamBFundLbl.ForeColor = System.Drawing.Color.Gold;
            this.lblTeamBFundLbl.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTeamBFundLbl.Location  = new System.Drawing.Point(147, 30);
            this.lblTeamBFundLbl.AutoSize  = true;

            this.txtTeamBFund.Location   = new System.Drawing.Point(162, 28);
            this.txtTeamBFund.Size       = new System.Drawing.Size(118, 26);
            this.txtTeamBFund.Text       = "100,000";
            this.txtTeamBFund.BackColor  = System.Drawing.Color.FromArgb(0, 0, 35);
            this.txtTeamBFund.ForeColor  = System.Drawing.Color.Gold;
            this.txtTeamBFund.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamBFund.Font       = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtTeamBFund.TextAlign  = System.Windows.Forms.HorizontalAlignment.Right;

            this.pnlTeamB.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.lblTeamBLabel, this.txtTeamBName,
                this.lblTeamBFundLbl, this.txtTeamBFund
            });

            // Stats
            this.pnlStats.Size        = new System.Drawing.Size(230, 72);
            this.pnlStats.Location    = new System.Drawing.Point(618, 14);
            this.pnlStats.BackColor   = System.Drawing.Color.FromArgb(25, 25, 10);
            this.pnlStats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.lblSoldPlayersCount.Text      = "Sold: 0 / 0";
            this.lblSoldPlayersCount.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.lblSoldPlayersCount.Font      = new System.Drawing.Font("Impact", 20F);
            this.lblSoldPlayersCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSoldPlayersCount.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlStats.Controls.Add(this.lblSoldPlayersCount);

            // Status bar
            this.lblStatusBar.Text      = "  Ready";
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(110, 110, 110);
            this.lblStatusBar.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStatusBar.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatusBar.Height    = 20;
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.pnlFooter.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                this.pnlTeamA, this.pnlTeamB, this.pnlStats, this.lblStatusBar
            });

            // ─────────────────── ADD TO FORM ────────────────────────────
            this.Controls.Add(this.dgvPlayers);
            this.Controls.Add(this.pnlDivider);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlTopDivider);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlFooter);

            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).EndInit();
            this.pnlToolbar.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── Button factory ──────────────────────────────────────────────
        private void MakeButton(System.Windows.Forms.Button btn, string text, int x,
                                System.Drawing.Color color)
        {
            btn.Text      = text;
            btn.Size      = new System.Drawing.Size(132, 38);
            btn.Location  = new System.Drawing.Point(x, 13);
            btn.BackColor = color;
            btn.ForeColor = System.Drawing.Color.White;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            btn.Cursor    = System.Windows.Forms.Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(
                Math.Min(color.R + 30, 255),
                Math.Min(color.G + 30, 255),
                Math.Min(color.B + 30, 255));
        }

        // ── Column factory ──────────────────────────────────────────────
        private void AddColumn(string header, string propName, int fillWeight,
                               bool rightAlign, string name = null)
        {
            var col = new System.Windows.Forms.DataGridViewTextBoxColumn
            {
                HeaderText       = header,
                DataPropertyName = propName,
                Name             = name ?? ("col_" + propName),
                FillWeight       = fillWeight > 0 ? fillWeight : 1,
                Visible          = fillWeight > 0
            };
            if (fillWeight > 0)
                col.MinimumWidth = 60;
            else
            {
                col.MinimumWidth = 2; // safe minimum; optionally set col.Width = 0 and col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            }
            if (rightAlign)
            {
                col.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                col.DefaultCellStyle.Format    = "N0";
            }
            this.dgvPlayers.Columns.Add(col);
        }

        // ─────────────────── CONTROLS ───────────────────────────────────
        private System.Windows.Forms.DataGridView dgvPlayers;
        private System.Windows.Forms.Panel pnlToolbar, pnlFooter, pnlDivider,
                                           pnlTopDivider, pnlTeamA, pnlTeamB, pnlStats;
        private System.Windows.Forms.Button btnAddPlayer, btnEditPlayer,
                                            btnDeletePlayer, btnLottery, btnReset;
        private System.Windows.Forms.Label  lblTitle, lblTeamALabel, lblTeamAFundLbl,
                                            lblTeamBLabel, lblTeamBFundLbl,
                                            lblSoldPlayersCount, lblStatusBar;
        private System.Windows.Forms.TextBox txtTeamAName, txtTeamAFund,
                                             txtTeamBName, txtTeamBFund;
    }
}
