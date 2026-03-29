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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvPlayers = new System.Windows.Forms.DataGridView();
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.btnAddPlayer = new System.Windows.Forms.Button();
            this.btnEditPlayer = new System.Windows.Forms.Button();
            this.btnDeletePlayer = new System.Windows.Forms.Button();
            this.btnLottery = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.pnlTeamA = new System.Windows.Forms.Panel();
            this.lblTeamALabel = new System.Windows.Forms.Label();
            this.txtTeamAName = new System.Windows.Forms.TextBox();
            this.lblTeamAFundLbl = new System.Windows.Forms.Label();
            this.txtTeamAFund = new System.Windows.Forms.TextBox();
            this.btnTeamAInfo = new System.Windows.Forms.Button();   // ← NEW
            this.pnlTeamB = new System.Windows.Forms.Panel();
            this.lblTeamBLabel = new System.Windows.Forms.Label();
            this.txtTeamBName = new System.Windows.Forms.TextBox();
            this.lblTeamBFundLbl = new System.Windows.Forms.Label();
            this.txtTeamBFund = new System.Windows.Forms.TextBox();
            this.btnTeamBInfo = new System.Windows.Forms.Button();   // ← NEW
            this.pnlStats = new System.Windows.Forms.Panel();
            this.lblSoldPlayersCount = new System.Windows.Forms.Label();
            this.lblStatusBar = new System.Windows.Forms.Label();
            this.pnlDivider = new System.Windows.Forms.Panel();
            this.pnlTopDivider = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).BeginInit();
            this.pnlToolbar.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.pnlTeamA.SuspendLayout();
            this.pnlTeamB.SuspendLayout();
            this.pnlStats.SuspendLayout();
            this.SuspendLayout();

            // ── dgvPlayers ─────────────────────────────────────────────────
            this.dgvPlayers.AllowUserToAddRows = false;
            this.dgvPlayers.AllowUserToDeleteRows = false;

            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(20, 20, 20);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            this.dgvPlayers.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;

            this.dgvPlayers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPlayers.BackgroundColor = System.Drawing.Color.FromArgb(12, 12, 12);
            this.dgvPlayers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPlayers.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;

            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(22, 22, 22);
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPlayers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPlayers.ColumnHeadersHeight = 40;
            this.dgvPlayers.ColumnHeadersHeightSizeMode =
                System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(15, 15, 15);
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(220, 220, 220);
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(0, 100, 0);
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPlayers.DefaultCellStyle = dataGridViewCellStyle3;

            this.dgvPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPlayers.EnableHeadersVisualStyles = false;
            this.dgvPlayers.GridColor = System.Drawing.Color.FromArgb(38, 38, 38);
            this.dgvPlayers.MultiSelect = false;
            this.dgvPlayers.Name = "dgvPlayers";
            this.dgvPlayers.ReadOnly = true;
            this.dgvPlayers.RowHeadersVisible = false;
            this.dgvPlayers.RowHeadersWidth = 62;
            this.dgvPlayers.RowTemplate.Height = 36;
            this.dgvPlayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlayers.TabIndex = 0;
            this.dgvPlayers.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPlayers_CellContentClick_1);
            this.dgvPlayers.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPlayers_CellFormatting);

            // ── pnlToolbar ─────────────────────────────────────────────────
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(18, 18, 18);
            this.pnlToolbar.Controls.Add(this.btnAddPlayer);
            this.pnlToolbar.Controls.Add(this.btnEditPlayer);
            this.pnlToolbar.Controls.Add(this.btnDeletePlayer);
            this.pnlToolbar.Controls.Add(this.btnLottery);
            this.pnlToolbar.Controls.Add(this.btnReset);
            this.pnlToolbar.Controls.Add(this.btnExportPdf);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(938, 65);
            this.pnlToolbar.TabIndex = 2;

            // ── Toolbar buttons ────────────────────────────────────────────
            MakeButton(this.btnAddPlayer, "➕  Add Player", 12, System.Drawing.Color.FromArgb(0, 120, 0));
            MakeButton(this.btnEditPlayer, "✏️  Edit Player", 154, System.Drawing.Color.FromArgb(0, 80, 160));
            MakeButton(this.btnDeletePlayer, "🗑  Delete", 296, System.Drawing.Color.FromArgb(160, 30, 30));
            MakeButton(this.btnLottery, "🎲  Lottery", 438, System.Drawing.Color.FromArgb(120, 0, 160));
            MakeButton(this.btnReset, "🔄  Reset", 580, System.Drawing.Color.FromArgb(100, 80, 0));

            this.btnAddPlayer.TabIndex = 0;
            this.btnEditPlayer.TabIndex = 1;
            this.btnDeletePlayer.TabIndex = 2;
            this.btnLottery.TabIndex = 3;
            this.btnReset.TabIndex = 4;

            this.btnAddPlayer.Click += new System.EventHandler(this.btnAddPlayer_Click);
            this.btnEditPlayer.Click += new System.EventHandler(this.btnEditPlayer_Click);
            this.btnDeletePlayer.Click += new System.EventHandler(this.btnDeletePlayer_Click);
            this.btnLottery.Click += new System.EventHandler(this.btnLottery_Click);
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);

            // ── btnExportPdf ───────────────────────────────────────────────
            this.btnExportPdf.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExportPdf.BackColor = System.Drawing.Color.FromArgb(160, 80, 0);
            this.btnExportPdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExportPdf.FlatAppearance.BorderSize = 0;
            this.btnExportPdf.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(200, 110, 10);
            this.btnExportPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportPdf.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExportPdf.ForeColor = System.Drawing.Color.White;
            this.btnExportPdf.Location = new System.Drawing.Point(938 - 158 - 12, 13);
            this.btnExportPdf.Size = new System.Drawing.Size(158, 38);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.TabIndex = 5;
            this.btnExportPdf.Text = "📄  Export PDF";
            this.btnExportPdf.UseVisualStyleBackColor = false;
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);

            // ── pnlFooter ──────────────────────────────────────────────────
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(8, 8, 8);
            this.pnlFooter.Controls.Add(this.pnlTeamA);
            this.pnlFooter.Controls.Add(this.pnlTeamB);
            this.pnlFooter.Controls.Add(this.pnlStats);
            this.pnlFooter.Controls.Add(this.lblStatusBar);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(938, 100);
            this.pnlFooter.TabIndex = 5;

            // ══════════════════════════════════════════════════════════════
            //  TEAM A PANEL  (width expanded to 322 to fit info icon)
            // ══════════════════════════════════════════════════════════════
            this.pnlTeamA.BackColor = System.Drawing.Color.FromArgb(0, 55, 0);
            this.pnlTeamA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTeamA.Controls.Add(this.lblTeamALabel);
            this.pnlTeamA.Controls.Add(this.txtTeamAName);
            this.pnlTeamA.Controls.Add(this.lblTeamAFundLbl);
            this.pnlTeamA.Controls.Add(this.txtTeamAFund);
            this.pnlTeamA.Controls.Add(this.btnTeamAInfo);            // ← NEW
            this.pnlTeamA.Location = new System.Drawing.Point(14, 14);
            this.pnlTeamA.Name = "pnlTeamA";
            this.pnlTeamA.Size = new System.Drawing.Size(322, 72); // wider for icon
            this.pnlTeamA.TabIndex = 0;

            this.lblTeamALabel.AutoSize = true;
            this.lblTeamALabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTeamALabel.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.lblTeamALabel.Location = new System.Drawing.Point(10, 6);
            this.lblTeamALabel.Name = "lblTeamALabel";
            this.lblTeamALabel.Text = "🏆  TEAM A";
            this.lblTeamALabel.TabIndex = 0;

            this.txtTeamAName.BackColor = System.Drawing.Color.FromArgb(0, 35, 0);
            this.txtTeamAName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamAName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtTeamAName.ForeColor = System.Drawing.Color.White;
            this.txtTeamAName.Location = new System.Drawing.Point(10, 28);
            this.txtTeamAName.Name = "txtTeamAName";
            this.txtTeamAName.Size = new System.Drawing.Size(130, 34);
            this.txtTeamAName.TabIndex = 1;
            this.txtTeamAName.Text = "Team Alpha";

            this.lblTeamAFundLbl.AutoSize = true;
            this.lblTeamAFundLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTeamAFundLbl.ForeColor = System.Drawing.Color.Gold;
            this.lblTeamAFundLbl.Location = new System.Drawing.Point(147, 30);
            this.lblTeamAFundLbl.Name = "lblTeamAFundLbl";
            this.lblTeamAFundLbl.Text = "৳";
            this.lblTeamAFundLbl.TabIndex = 2;

            this.txtTeamAFund.BackColor = System.Drawing.Color.FromArgb(0, 35, 0);
            this.txtTeamAFund.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamAFund.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtTeamAFund.ForeColor = System.Drawing.Color.Gold;
            this.txtTeamAFund.Location = new System.Drawing.Point(162, 28);
            this.txtTeamAFund.Name = "txtTeamAFund";
            this.txtTeamAFund.Size = new System.Drawing.Size(118, 34);
            this.txtTeamAFund.TabIndex = 3;
            this.txtTeamAFund.Text = "100,000";
            this.txtTeamAFund.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ── btnTeamAInfo  (📊 icon, right side of Team A panel) ────────
            this.btnTeamAInfo.BackColor = System.Drawing.Color.FromArgb(0, 80, 0);
            this.btnTeamAInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeamAInfo.FlatAppearance.BorderSize = 0;
            this.btnTeamAInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 130, 0);
            this.btnTeamAInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeamAInfo.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.btnTeamAInfo.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.btnTeamAInfo.Location = new System.Drawing.Point(286, 10);
            this.btnTeamAInfo.Size = new System.Drawing.Size(28, 50);
            this.btnTeamAInfo.Name = "btnTeamAInfo";
            this.btnTeamAInfo.TabIndex = 4;
            this.btnTeamAInfo.Text = "📊";
            this.btnTeamAInfo.UseVisualStyleBackColor = false;
            this.btnTeamAInfo.Click += new System.EventHandler(this.btnTeamAInfo_Click);

            // ══════════════════════════════════════════════════════════════
            //  TEAM B PANEL  (shifted right, same width 322)
            // ══════════════════════════════════════════════════════════════
            this.pnlTeamB.BackColor = System.Drawing.Color.FromArgb(0, 0, 55);
            this.pnlTeamB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTeamB.Controls.Add(this.lblTeamBLabel);
            this.pnlTeamB.Controls.Add(this.txtTeamBName);
            this.pnlTeamB.Controls.Add(this.lblTeamBFundLbl);
            this.pnlTeamB.Controls.Add(this.txtTeamBFund);
            this.pnlTeamB.Controls.Add(this.btnTeamBInfo);            // ← NEW
            this.pnlTeamB.Location = new System.Drawing.Point(348, 14); // adjusted x
            this.pnlTeamB.Name = "pnlTeamB";
            this.pnlTeamB.Size = new System.Drawing.Size(322, 72);
            this.pnlTeamB.TabIndex = 1;

            this.lblTeamBLabel.AutoSize = true;
            this.lblTeamBLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTeamBLabel.ForeColor = System.Drawing.Color.FromArgb(100, 149, 237);
            this.lblTeamBLabel.Location = new System.Drawing.Point(10, 6);
            this.lblTeamBLabel.Name = "lblTeamBLabel";
            this.lblTeamBLabel.Text = "🏆  TEAM B";
            this.lblTeamBLabel.TabIndex = 0;

            this.txtTeamBName.BackColor = System.Drawing.Color.FromArgb(0, 0, 35);
            this.txtTeamBName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamBName.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtTeamBName.ForeColor = System.Drawing.Color.White;
            this.txtTeamBName.Location = new System.Drawing.Point(10, 28);
            this.txtTeamBName.Name = "txtTeamBName";
            this.txtTeamBName.Size = new System.Drawing.Size(130, 34);
            this.txtTeamBName.TabIndex = 1;
            this.txtTeamBName.Text = "Team Beta";

            this.lblTeamBFundLbl.AutoSize = true;
            this.lblTeamBFundLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTeamBFundLbl.ForeColor = System.Drawing.Color.Gold;
            this.lblTeamBFundLbl.Location = new System.Drawing.Point(147, 30);
            this.lblTeamBFundLbl.Name = "lblTeamBFundLbl";
            this.lblTeamBFundLbl.Text = "৳";
            this.lblTeamBFundLbl.TabIndex = 2;

            this.txtTeamBFund.BackColor = System.Drawing.Color.FromArgb(0, 0, 35);
            this.txtTeamBFund.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTeamBFund.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtTeamBFund.ForeColor = System.Drawing.Color.Gold;
            this.txtTeamBFund.Location = new System.Drawing.Point(162, 28);
            this.txtTeamBFund.Name = "txtTeamBFund";
            this.txtTeamBFund.Size = new System.Drawing.Size(118, 34);
            this.txtTeamBFund.TabIndex = 3;
            this.txtTeamBFund.Text = "100,000";
            this.txtTeamBFund.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ── btnTeamBInfo ───────────────────────────────────────────────
            this.btnTeamBInfo.BackColor = System.Drawing.Color.FromArgb(0, 0, 80);
            this.btnTeamBInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTeamBInfo.FlatAppearance.BorderSize = 0;
            this.btnTeamBInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(0, 0, 130);
            this.btnTeamBInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTeamBInfo.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.btnTeamBInfo.ForeColor = System.Drawing.Color.FromArgb(100, 149, 237);
            this.btnTeamBInfo.Location = new System.Drawing.Point(286, 10);
            this.btnTeamBInfo.Size = new System.Drawing.Size(28, 50);
            this.btnTeamBInfo.Name = "btnTeamBInfo";
            this.btnTeamBInfo.TabIndex = 4;
            this.btnTeamBInfo.Text = "📊";
            this.btnTeamBInfo.UseVisualStyleBackColor = false;
            this.btnTeamBInfo.Click += new System.EventHandler(this.btnTeamBInfo_Click);

            // ── pnlStats ───────────────────────────────────────────────────
            this.pnlStats.BackColor = System.Drawing.Color.FromArgb(25, 25, 10);
            this.pnlStats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStats.Controls.Add(this.lblSoldPlayersCount);
            this.pnlStats.Location = new System.Drawing.Point(682, 14);  // adjusted x
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Size = new System.Drawing.Size(230, 72);
            this.pnlStats.TabIndex = 2;

            this.lblSoldPlayersCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSoldPlayersCount.Font = new System.Drawing.Font("Impact", 20F);
            this.lblSoldPlayersCount.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.lblSoldPlayersCount.Location = new System.Drawing.Point(0, 0);
            this.lblSoldPlayersCount.Name = "lblSoldPlayersCount";
            this.lblSoldPlayersCount.Size = new System.Drawing.Size(228, 70);
            this.lblSoldPlayersCount.TabIndex = 0;
            this.lblSoldPlayersCount.Text = "Sold: 0 / 0";
            this.lblSoldPlayersCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── lblStatusBar ───────────────────────────────────────────────
            this.lblStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatusBar.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(110, 110, 110);
            this.lblStatusBar.Location = new System.Drawing.Point(0, 80);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Size = new System.Drawing.Size(938, 20);
            this.lblStatusBar.TabIndex = 4;
            this.lblStatusBar.Text = "  Ready";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ── pnlDivider ─────────────────────────────────────────────────
            this.pnlDivider.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            this.pnlDivider.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDivider.Name = "pnlDivider";
            this.pnlDivider.Size = new System.Drawing.Size(938, 2);
            this.pnlDivider.TabIndex = 1;

            // ── pnlTopDivider ──────────────────────────────────────────────
            this.pnlTopDivider.BackColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.pnlTopDivider.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopDivider.Name = "pnlTopDivider";
            this.pnlTopDivider.Size = new System.Drawing.Size(938, 2);
            this.pnlTopDivider.TabIndex = 3;

            // ── lblTitle ───────────────────────────────────────────────────
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(5, 5, 5);
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Impact", 26F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(938, 68);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "⚽  PRSC PLAYER AUCTION SYSTEM  ⚽";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ── MainForm ───────────────────────────────────────────────────
            this.BackColor = System.Drawing.Color.FromArgb(10, 10, 10);
            this.ClientSize = new System.Drawing.Size(938, 564);
            this.Controls.Add(this.dgvPlayers);
            this.Controls.Add(this.pnlDivider);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlTopDivider);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlFooter);
            this.MinimumSize = new System.Drawing.Size(960, 620);
            this.Name = "MainForm";
            this.Text = "PRSC Player Auction System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            ((System.ComponentModel.ISupportInitialize)(this.dgvPlayers)).EndInit();
            this.pnlToolbar.ResumeLayout(false);
            this.pnlFooter.ResumeLayout(false);
            this.pnlTeamA.ResumeLayout(false);
            this.pnlTeamA.PerformLayout();
            this.pnlTeamB.ResumeLayout(false);
            this.pnlTeamB.PerformLayout();
            this.pnlStats.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── Button factory ─────────────────────────────────────────────────
        private void MakeButton(System.Windows.Forms.Button btn, string text, int x,
                                System.Drawing.Color color)
        {
            btn.Text = text;
            btn.Size = new System.Drawing.Size(132, 38);
            btn.Location = new System.Drawing.Point(x, 13);
            btn.BackColor = color;
            btn.ForeColor = System.Drawing.Color.White;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
            btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(
                Math.Min(color.R + 30, 255),
                Math.Min(color.G + 30, 255),
                Math.Min(color.B + 30, 255));
        }

        // ── Controls ───────────────────────────────────────────────────────
        private System.Windows.Forms.DataGridView dgvPlayers;
        private System.Windows.Forms.Panel pnlToolbar, pnlFooter, pnlDivider,
                                           pnlTopDivider, pnlTeamA, pnlTeamB, pnlStats;
        private System.Windows.Forms.Button btnAddPlayer, btnEditPlayer,
                                            btnDeletePlayer, btnLottery, btnReset,
                                            btnExportPdf,
                                            btnTeamAInfo, btnTeamBInfo;          // ← NEW
        private System.Windows.Forms.Label lblTitle, lblTeamALabel, lblTeamAFundLbl,
                                            lblTeamBLabel, lblTeamBFundLbl,
                                            lblSoldPlayersCount, lblStatusBar;
        private System.Windows.Forms.TextBox txtTeamAName, txtTeamAFund,
                                             txtTeamBName, txtTeamBFund;
    }
}