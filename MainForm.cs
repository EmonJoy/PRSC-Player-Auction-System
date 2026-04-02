using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;

using PdfFont = iTextSharp.text.Font;
using PdfDoc = iTextSharp.text.Document;
using PdfRect = iTextSharp.text.Rectangle;
using WinFont = System.Drawing.Font;
using WinRect = System.Drawing.Rectangle;

namespace PRSC_Player_Auction_System
{

    public class TeamReportForm : Form
    {
        private readonly string _teamName;
        private readonly decimal _initialFund;
        private readonly decimal _remainingFund;
        private readonly List<Player> _bought;
        private readonly bool _isTeamA;

        private readonly Color _accent;
        private readonly Color _panelBg;
        private readonly Color _darkBg = Color.FromArgb(10, 10, 10);
        private readonly Color _rowBg = Color.FromArgb(18, 18, 18);
        private readonly Color _altBg = Color.FromArgb(25, 25, 25);

        public TeamReportForm(string teamName, decimal initialFund,
                              decimal remainingFund, List<Player> bought,
                              bool isTeamA)
        {
            _teamName = teamName;
            _initialFund = initialFund;
            _remainingFund = remainingFund;
            _bought = bought ?? new List<Player>();
            _isTeamA = isTeamA;
            _accent = isTeamA ? Color.FromArgb(50, 205, 50)
                              : Color.FromArgb(100, 149, 237);
            _panelBg = isTeamA ? Color.FromArgb(0, 40, 0)
                               : Color.FromArgb(0, 0, 40);

            InitializeComponent();  // ← ADD THIS LINE
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = $"Team Report — {_teamName}";
            this.Size = new System.Drawing.Size(680, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = _darkBg;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            decimal spent = _initialFund - _remainingFund;

            var lblTitle = new Label
            {
                Text = $"🏆  {_teamName}",
                Font = new WinFont("Impact", 22F),
                ForeColor = _accent,
                BackColor = Color.FromArgb(5, 5, 5),
                Dock = DockStyle.Top,
                Height = 56,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var div1 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 2,
                BackColor = _accent
            };

            var pnlSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = _panelBg,
                Padding = new Padding(10, 8, 10, 8)
            };

            Label MkStat(string lbl, string val, Color valColor, int x)
            {
                var p = new Panel
                {
                    Location = new System.Drawing.Point(x, 4),
                    Size = new System.Drawing.Size(190, 52),
                    BackColor = Color.Transparent
                };
                p.Controls.Add(new Label
                {
                    Text = lbl,
                    Font = new WinFont("Segoe UI", 7.5F),
                    ForeColor = Color.FromArgb(140, 140, 140),
                    Location = new System.Drawing.Point(0, 0),
                    AutoSize = true
                });
                p.Controls.Add(new Label
                {
                    Text = val,
                    Font = new WinFont("Segoe UI", 13F, FontStyle.Bold),
                    ForeColor = valColor,
                    Location = new System.Drawing.Point(0, 18),
                    AutoSize = true
                });
                pnlSummary.Controls.Add(p);
                return null;
            }

            MkStat("PLAYERS BOUGHT", _bought.Count.ToString(),
                   Color.FromArgb(220, 220, 220), 10);
            MkStat("TOTAL SPENT", $"৳ {spent:N0}",
                   Color.FromArgb(255, 80, 80), 210);
            MkStat("REMAINING FUND", $"৳ {_remainingFund:N0}",
                   Color.Gold, 410);

            var div2 = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(40, 40, 40)
            };

            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = _darkBg,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(35, 35, 35),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeight = 36,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.RowTemplate.Height = 34;

            var hdrStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = _accent,
                Font = new WinFont("Segoe UI", 9.5F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(0)
            };
            dgv.ColumnHeadersDefaultCellStyle = hdrStyle;

            var cellStyle = new DataGridViewCellStyle
            {
                BackColor = _rowBg,
                ForeColor = Color.FromArgb(220, 220, 220),
                Font = new WinFont("Segoe UI", 9.5F),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                SelectionBackColor = Color.FromArgb(0, 70, 0),
                SelectionForeColor = Color.White,
                Padding = new Padding(0)
            };
            dgv.DefaultCellStyle = cellStyle;

            var altStyle = new DataGridViewCellStyle(cellStyle)
            { BackColor = _altBg };
            dgv.AlternatingRowsDefaultCellStyle = altStyle;

            void AddCol(string hdr, string prop, int fw)
            {
                var c = new DataGridViewTextBoxColumn
                {
                    HeaderText = hdr,
                    DataPropertyName = prop,
                    FillWeight = fw,
                    MinimumWidth = 60
                };
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns.Add(c);
            }
            AddCol("#", "Id", 6);
            AddCol("Player Name", "Name", 30);
            AddCol("Position", "Position", 18);
            AddCol("Skill", "SkillLevel", 14);
            AddCol("Sold Price ৳", "SoldPrice", 18);

            dgv.AutoGenerateColumns = false;
            dgv.DataSource = new BindingSource { DataSource = _bought };

            dgv.CellFormatting += (s, ev) =>
            {
                if (ev.ColumnIndex == 4 && ev.RowIndex >= 0)
                {
                    ev.CellStyle.ForeColor = Color.Gold;
                    ev.CellStyle.Font = new WinFont("Segoe UI", 9.5F, FontStyle.Bold);
                    ev.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            };

            var pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                BackColor = Color.FromArgb(14, 14, 14)
            };

            Button MkBtn(string txt, Color bg, int x)
            {
                var b = new Button
                {
                    Text = txt,
                    Location = new System.Drawing.Point(x, 9),
                    Size = new System.Drawing.Size(160, 34),
                    BackColor = bg,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new WinFont("Segoe UI", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                b.FlatAppearance.BorderSize = 0;
                pnlBottom.Controls.Add(b);
                return b;
            }

            var btnExport = MkBtn("📄  Export PDF", Color.FromArgb(160, 80, 0), 10);
            var btnClose = MkBtn("✖  Close", Color.FromArgb(80, 20, 20),
                                 this.ClientSize.Width - 178);

            btnExport.Click += (s, ev) => ExportTeamPdf();
            btnClose.Click += (s, ev) => this.Close();

            this.Controls.Add(dgv);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(div2);
            this.Controls.Add(pnlSummary);
            this.Controls.Add(div1);
            this.Controls.Add(lblTitle);
        }

        private void ExportTeamPdf()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = $"Export {_teamName} Report";
                sfd.Filter = "PDF Files (*.pdf)|*.pdf";
                sfd.FileName = $"PRSC_{_teamName.Replace(" ", "_")}_Report_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    BuildTeamPdf(sfd.FileName);
                    if (MessageBox.Show("PDF exported!\nOpen it now?", "Done",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Export failed:\n{ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BuildTeamPdf(string path)
        {
            string fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string[] cands = { "calibri.ttf", "arial.ttf", "verdana.ttf", "tahoma.ttf" };
            string fontPath = cands.Select(f => Path.Combine(fontsDir, f))
                                   .FirstOrDefault(File.Exists);

            iTextSharp.text.Font F(float sz, int style, BaseColor c)
            {
                try
                {
                    if (fontPath != null)
                    {
                        var bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        return new iTextSharp.text.Font(bf, sz, style, c);
                    }
                }
                catch { }
                return FontFactory.GetFont(FontFactory.HELVETICA, sz, style, c);
            }

            var cAccent = _isTeamA ? new BaseColor(50, 205, 50) : new BaseColor(100, 149, 237);
            var cGold = new BaseColor(255, 215, 0);
            var cRed = new BaseColor(255, 80, 80);
            var cLight = new BaseColor(220, 220, 220);
            var cDim = new BaseColor(140, 140, 140);
            var cHdr = new BaseColor(20, 20, 20);
            var cRowDark = new BaseColor(15, 15, 15);
            var cRowAlt = new BaseColor(25, 25, 25);
            var cPanelBg = _isTeamA ? new BaseColor(0, 40, 0) : new BaseColor(0, 0, 40);

            var doc = new PdfDoc(PageSize.A4, 30f, 30f, 30f, 25f);
            PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            doc.Open();

            decimal spent = _initialFund - _remainingFund;

            doc.Add(new Paragraph($"🏆  {_teamName}", F(24f, iTextSharp.text.Font.BOLD, cAccent))
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 2f });
            doc.Add(new Paragraph("PRSC Player Auction — Team Report", F(9f, iTextSharp.text.Font.NORMAL, cDim))
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 12f });

            var tSum = new PdfPTable(3) { WidthPercentage = 100f, SpacingAfter = 14f };
            tSum.SetWidths(new float[] { 33f, 34f, 33f });

            PdfPCell SumCell(string label, string value, iTextSharp.text.Font vFont)
            {
                var inner = new PdfPTable(1) { WidthPercentage = 100f };
                inner.AddCell(new PdfPCell(new Phrase(label, F(7.5f, iTextSharp.text.Font.NORMAL, cDim)))
                {
                    BackgroundColor = cPanelBg,
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 8f,
                    PaddingBottom = 2f
                });
                inner.AddCell(new PdfPCell(new Phrase(value, vFont))
                {
                    BackgroundColor = cPanelBg,
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 8f
                });
                return new PdfPCell(inner)
                { Border = PdfPCell.BOX, BorderColor = cAccent, BorderWidth = 1.2f, Padding = 0f };
            }

            tSum.AddCell(SumCell("PLAYERS BOUGHT", _bought.Count.ToString(),
                F(16f, iTextSharp.text.Font.BOLD, cLight)));
            tSum.AddCell(SumCell("TOTAL SPENT", $"BDT {spent:N0}",
                F(13f, iTextSharp.text.Font.BOLD, cRed)));
            tSum.AddCell(SumCell("REMAINING FUND", $"BDT {_remainingFund:N0}",
                F(13f, iTextSharp.text.Font.BOLD, cGold)));
            doc.Add(tSum);

            if (_bought.Count == 0)
            {
                doc.Add(new Paragraph("No players purchased yet.",
                    F(11f, iTextSharp.text.Font.ITALIC, cDim))
                { Alignment = Element.ALIGN_CENTER });
            }
            else
            {
                float[] cw = { 6f, 28f, 18f, 14f, 18f, 16f };
                var tbl = new PdfPTable(cw.Length)
                { WidthPercentage = 100f, HeaderRows = 1, SpacingBefore = 4f };
                tbl.SetWidths(cw);

                PdfPCell HdrCell(string txt) =>
                    new PdfPCell(new Phrase(txt, F(9f, iTextSharp.text.Font.BOLD, cAccent)))
                    {
                        BackgroundColor = cHdr,
                        BorderWidthBottom = 1.5f,
                        BorderWidthTop = 0f,
                        BorderWidthLeft = 0f,
                        BorderWidthRight = 0f,
                        BorderColor = cAccent,
                        Padding = 7f,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };

                PdfPCell DCell(string txt, iTextSharp.text.Font font, BaseColor bg) =>
                    new PdfPCell(new Phrase(txt ?? "", font))
                    {
                        BackgroundColor = bg,
                        BorderWidthBottom = 0.3f,
                        BorderWidthTop = 0f,
                        BorderWidthLeft = 0f,
                        BorderWidthRight = 0f,
                        BorderColor = new BaseColor(30, 30, 30),
                        Padding = 6f,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };

                tbl.AddCell(HdrCell("#"));
                tbl.AddCell(HdrCell("Player Name"));
                tbl.AddCell(HdrCell("Position"));
                tbl.AddCell(HdrCell("Skill"));
                tbl.AddCell(HdrCell("Sold Price (BDT)"));
                tbl.AddCell(HdrCell("Base Price (BDT)"));

                bool alt = false; int n = 0;
                foreach (var p in _bought)
                {
                    n++;
                    var bg = alt ? cRowAlt : cRowDark; alt = !alt;
                    var fNorm = F(8.5f, iTextSharp.text.Font.NORMAL, cLight);
                    var fPrice = F(8.5f, iTextSharp.text.Font.BOLD, cGold);
                    tbl.AddCell(DCell(n.ToString(), fNorm, bg));
                    tbl.AddCell(DCell(p.Name, fNorm, bg));
                    tbl.AddCell(DCell(p.Position, fNorm, bg));
                    tbl.AddCell(DCell(p.SkillLevel, fNorm, bg));
                    tbl.AddCell(DCell(p.SoldPrice.ToString("N0"), fPrice, bg));
                    tbl.AddCell(DCell(p.BasePrice.ToString("N0"), fNorm, bg));
                }
                doc.Add(tbl);

                doc.Add(new Paragraph(
                    $"\nTotal spent on {_bought.Count} player(s):   BDT {spent:N0}   |   Remaining fund:   BDT {_remainingFund:N0}",
                    F(8.5f, iTextSharp.text.Font.BOLD, cGold))
                { Alignment = Element.ALIGN_RIGHT, SpacingBefore = 6f });
            }

            doc.Add(new Paragraph(
                $"\nPRSC Auction System   |   {_teamName}   |   Printed: {DateTime.Now:dd MMM yyyy HH:mm}\nDevelop by Emon Joy",
                F(7f, iTextSharp.text.Font.NORMAL, cDim))
            { Alignment = Element.ALIGN_CENTER, SpacingBefore = 10f });

            doc.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TeamReportForm
            // 
            this.ClientSize = new System.Drawing.Size(833, 680);
            this.Name = "TeamReportForm";
            this.ResumeLayout(false);

        }

        private void TeamReportForm_Load(object sender, EventArgs e) { }
    }

    // ════════════════════════════════════════════════════════════════════
    //  MAIN FORM
    // ════════════════════════════════════════════════════════════════════
    public partial class MainForm : Form
    {
        private List<Player> players = new List<Player>();

        // ── Fund properties ───────────────────────────────────────────────
        // FIX: Use fixed keys "TeamA" / "TeamB" instead of dynamic team names
        public decimal TeamAFund
        {
            get
            {
                if (txtTeamAFund == null) return 0;
                decimal.TryParse(txtTeamAFund.Text.Replace(",", "").Trim(), out decimal v);
                return v;
            }
            set
            {
                if (txtTeamAFund == null) return;
                txtTeamAFund.Text = value.ToString("N0");
                try { DatabaseHelper.UpdateTeamFund("TeamA", value); } catch { }
            }
        }

        public decimal TeamBFund
        {
            get
            {
                if (txtTeamBFund == null) return 0;
                decimal.TryParse(txtTeamBFund.Text.Replace(",", "").Trim(), out decimal v);
                return v;
            }
            set
            {
                if (txtTeamBFund == null) return;
                txtTeamBFund.Text = value.ToString("N0");
                try { DatabaseHelper.UpdateTeamFund("TeamB", value); } catch { }
            }
        }

        public string TeamAName => txtTeamAName?.Text ?? "Team Alpha";
        public string TeamBName => txtTeamBName?.Text ?? "Team Beta";

        // ── Constructor ───────────────────────────────────────────────────
        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetupGrid();
            LoadFromDatabase();
        }

        // ── Grid setup ────────────────────────────────────────────────────
        private void SetupGrid()
        {
            if (dgvPlayers == null) return;
            dgvPlayers.AutoGenerateColumns = false;
            if (dgvPlayers.Columns.Count > 0) return;

            AddColumn("#", "Id", 5, DataGridViewContentAlignment.MiddleCenter);
            AddColumn("Player Name", "Name", 22, DataGridViewContentAlignment.MiddleCenter);
            AddColumn("Position", "Position", 13, DataGridViewContentAlignment.MiddleCenter);
            AddColumn("Skill", "SkillLevel", 10, DataGridViewContentAlignment.MiddleCenter);
            AddColumn("Base Price (BDT)", "BasePrice", 14, DataGridViewContentAlignment.MiddleCenter, format: "N0");
            AddColumn("Sold Price (BDT)", "SoldPrice", 14, DataGridViewContentAlignment.MiddleCenter, format: "N0");
            AddColumn("Assigned Team", "AssignedTeam", 14, DataGridViewContentAlignment.MiddleCenter);
            AddColumn("Status", "Status", 8, DataGridViewContentAlignment.MiddleCenter, name: "colStatus");
        }

        private void AddColumn(string header, string propName, int fillWeight,
                               DataGridViewContentAlignment align,
                               string format = null, string name = null)
        {
            var col = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = propName,
                Name = name ?? ("col_" + propName),
                FillWeight = fillWeight > 0 ? fillWeight : 1,
                Visible = fillWeight > 0,
                MinimumWidth = fillWeight > 0 ? 60 : 2
            };
            col.HeaderCell.Style.Alignment = align;
            col.DefaultCellStyle.Alignment = align;
            if (format != null) col.DefaultCellStyle.Format = format;
            dgvPlayers.Columns.Add(col);
        }

        // ── Load / bind ───────────────────────────────────────────────────
        // FIX: Split into two try/catch blocks so a fund error doesn't
        //      also wipe out the player list, and use fixed keys "TeamA"/"TeamB"
        private void LoadFromDatabase()
        {
            try
            {
                players = DatabaseHelper.GetAllPlayers() ?? new List<Player>();
            }
            catch (Exception ex)
            {
                players = new List<Player>();
                if (lblStatusBar != null)
                    lblStatusBar.Text = $"  ⚠ DB unavailable — offline mode. ({ex.Message})";
            }

            try
            {
                if (txtTeamAFund != null)
                    txtTeamAFund.Text = DatabaseHelper.GetTeamFund("TeamA").ToString("N0");
                if (txtTeamBFund != null)
                    txtTeamBFund.Text = DatabaseHelper.GetTeamFund("TeamB").ToString("N0");
            }
            catch
            {
                // Fund records missing — fall back to default values silently
                if (txtTeamAFund != null) txtTeamAFund.Text = "100,000";
                if (txtTeamBFund != null) txtTeamBFund.Text = "100,000";
            }

            BindGrid();
            UpdateStats();
        }

        private void BindGrid()
        {
            if (dgvPlayers == null) return;
            dgvPlayers.AutoGenerateColumns = false;
            dgvPlayers.DataSource = new BindingSource { DataSource = players };
        }

        public void RefreshGrid()
        {
            if (dgvPlayers == null) return;
            if (dgvPlayers.DataSource is BindingSource bs) bs.ResetBindings(false);
            else BindGrid();
            UpdateStats();
        }

        private void UpdateStats()
        {
            if (players == null) return;
            int total = players.Count;
            int sold = players.Count(p => p.IsSold);
            if (lblSoldPlayersCount != null)
                lblSoldPlayersCount.Text = $"Sold: {sold} / {total}";
            if (lblStatusBar != null)
                lblStatusBar.Text = $"  Players: {total}  |  Sold: {sold}  |  Available: {total - sold}";
        }

        // ── Cell formatting ───────────────────────────────────────────────
        private void dgvPlayers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (players == null || e.RowIndex < 0 || e.RowIndex >= players.Count) return;
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

            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (dgvPlayers.Columns[e.ColumnIndex].Name == "colStatus")
            {
                e.CellStyle.ForeColor = player.IsSold
                    ? Color.FromArgb(255, 80, 80)
                    : Color.FromArgb(50, 205, 50);
                e.CellStyle.Font = new WinFont("Segoe UI", 9.5F, FontStyle.Bold);
            }
            if (dgvPlayers.Columns[e.ColumnIndex].Name == "col_AssignedTeam" && player.IsSold)
                e.CellStyle.ForeColor = Color.FromArgb(100, 180, 255);
        }

        // ── Team report buttons ───────────────────────────────────────────
        private void btnTeamAInfo_Click(object sender, EventArgs e)
            => ShowTeamReport(isTeamA: true);

        private void btnTeamBInfo_Click(object sender, EventArgs e)
            => ShowTeamReport(isTeamA: false);

        private void ShowTeamReport(bool isTeamA)
        {
            string name = isTeamA ? TeamAName : TeamBName;
            decimal fund = isTeamA ? TeamAFund : TeamBFund;

            var bought = players
                .Where(p => p.IsSold &&
                       (p.AssignedTeam ?? "").Equals(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            decimal spent = bought.Sum(p => p.SoldPrice);
            decimal initialFund = fund + spent;

            using (var frm = new TeamReportForm(name, initialFund, fund, bought, isTeamA))
                frm.ShowDialog(this);
        }

        private List<string> GetTeamNames()
        {
            var names = new List<string>();
            if (!string.IsNullOrWhiteSpace(TeamAName)) names.Add(TeamAName);
            if (!string.IsNullOrWhiteSpace(TeamBName)) names.Add(TeamBName);
            return names;
        }

        // ── Toolbar handlers ──────────────────────────────────────────────
        private void btnAddPlayer_Click(object sender, EventArgs e)
        {
            using (var dlg = new AddPlayerForm(teamNames: GetTeamNames()))  // ← change
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

        private void btnEditPlayer_Click(object sender, EventArgs e)
        {
            var player = GetSelectedPlayer();
            if (player == null) { ShowInfo("Please select a player to edit."); return; }

            using (var dlg = new AddPlayerForm(player, GetTeamNames()))  // ← change
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

        private void btnLottery_Click(object sender, EventArgs e)
        {
            var available = players.Where(p => !p.IsSold).ToList();
            if (available.Count == 0) { ShowInfo("No available players for auction."); return; }
            var picked = available[new Random().Next(available.Count)];
            using (var auction = new FullScreenAuctionForm(picked, this))
                auction.ShowDialog();
            
            // CRITICAL FIX: Reload from database instead of just refreshing grid
    // This ensures all player states and team funds are synced from DB
    try { LoadFromDatabase(); }
    catch { RefreshGrid(); }  // Fallback to refresh if DB is unavailable                       
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reset ALL auction data? This cannot be undone.",
                    "Confirm Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { DatabaseHelper.ResetAllPlayers(); } catch { }
                LoadFromDatabase();
                if (lblStatusBar != null)
                    lblStatusBar.Text = "  Auction reset. Ready to begin.";
            }
        }

        // ── Export full PDF ───────────────────────────────────────────────
        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (players == null || players.Count == 0) { ShowInfo("No players to export."); return; }
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Player List as PDF";
                sfd.Filter = "PDF Files (*.pdf)|*.pdf";
                sfd.FileName = $"PRSC_PlayerList_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (sfd.ShowDialog() != DialogResult.OK) return;
                try
                {
                    ExportToPdf(sfd.FileName);
                    if (lblStatusBar != null)
                        lblStatusBar.Text = $"  PDF exported: {sfd.FileName}";
                    if (MessageBox.Show("PDF exported!\nOpen it now?", "Export Complete",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export PDF:\n{ex.Message}", "Export Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportToPdf(string filePath)
        {
            string teamAName = txtTeamAName?.Text ?? "Team A";
            string teamBName = txtTeamBName?.Text ?? "Team B";
            decimal teamAFund = TeamAFund;
            decimal teamBFund = TeamBFund;
            int total = players.Count;
            int sold = players.Count(p => p.IsSold);
            int avail = total - sold;
            var snapshot = players.ToList();

            string fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string[] cands = { "calibri.ttf", "arial.ttf", "verdana.ttf", "tahoma.ttf" };
            string fontPath = cands.Select(f => Path.Combine(fontsDir, f)).FirstOrDefault(File.Exists);

            iTextSharp.text.Font MkFont(float size, int style, BaseColor color)
            {
                try
                {
                    if (fontPath != null)
                    {
                        var bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        return new iTextSharp.text.Font(bf, size, style, color);
                    }
                }
                catch { }
                return FontFactory.GetFont(FontFactory.HELVETICA, size, style, color);
            }

            var cGreen = new BaseColor(50, 205, 50);
            var cGold = new BaseColor(255, 215, 0);
            var cBlue = new BaseColor(100, 180, 255);
            var cLight = new BaseColor(220, 220, 220);
            var cDim = new BaseColor(140, 140, 140);
            var cRed = new BaseColor(255, 80, 80);
            var cDarkRow = new BaseColor(15, 15, 15);
            var cAltRow = new BaseColor(25, 25, 25);
            var cTblHdr = new BaseColor(20, 20, 20);
            var cTeamABg = new BaseColor(0, 40, 0);
            var cTeamBBg = new BaseColor(0, 0, 40);
            var cStatsBg = new BaseColor(20, 20, 10);

            var fTitle = MkFont(22f, iTextSharp.text.Font.BOLD, cGreen);
            var fStamp = MkFont(7f, iTextSharp.text.Font.NORMAL, cDim);
            var fTeamAHdr = MkFont(14f, iTextSharp.text.Font.BOLD, cGreen);
            var fTeamBHdr = MkFont(14f, iTextSharp.text.Font.BOLD, cBlue);
            var fLabel = MkFont(7.5f, iTextSharp.text.Font.NORMAL, cDim);
            var fFund = MkFont(10f, iTextSharp.text.Font.BOLD, cGold);
            var fStatVal = MkFont(12f, iTextSharp.text.Font.BOLD, cGold);
            var fStatRed = MkFont(12f, iTextSharp.text.Font.BOLD, cRed);
            var fStatGrn = MkFont(12f, iTextSharp.text.Font.BOLD, cGreen);
            var fColHdr = MkFont(9f, iTextSharp.text.Font.BOLD, cGreen);
            var fCell = MkFont(8.5f, iTextSharp.text.Font.NORMAL, cLight);
            var fCellDim = MkFont(8.5f, iTextSharp.text.Font.NORMAL, cDim);
            var fCellBlue = MkFont(8.5f, iTextSharp.text.Font.NORMAL, cBlue);
            var fSold = MkFont(8.5f, iTextSharp.text.Font.BOLD, cRed);
            var fAvail = MkFont(8.5f, iTextSharp.text.Font.BOLD, cGreen);
            var fFooter = MkFont(7f, iTextSharp.text.Font.NORMAL, cDim);

            var doc = new PdfDoc(PageSize.A4.Rotate(), 25f, 25f, 25f, 20f);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph("PRSC PLAYER AUCTION SYSTEM", fTitle)
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 2f });
            doc.Add(new Paragraph(
                $"Generated: {DateTime.Now:dd MMMM yyyy   HH:mm} \nDevelop by Emon Joy", fStamp)
            { Alignment = Element.ALIGN_CENTER, SpacingAfter = 8f });

            var hdrTable = new PdfPTable(3) { WidthPercentage = 100f, SpacingAfter = 10f };
            hdrTable.SetWidths(new float[] { 38f, 38f, 24f });

            PdfPCell TR(string text, iTextSharp.text.Font font, BaseColor bg,
                        float pt = 5f, float pb = 5f) =>
                new PdfPCell(new Phrase(text, font))
                {
                    BackgroundColor = bg,
                    Border = PdfPCell.NO_BORDER,
                    PaddingLeft = 10f,
                    PaddingRight = 10f,
                    PaddingTop = pt,
                    PaddingBottom = pb,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };

            int cntA = snapshot.Count(p => p.IsSold &&
                (p.AssignedTeam ?? "").Equals(teamAName, StringComparison.OrdinalIgnoreCase));
            var tblA = new PdfPTable(1) { WidthPercentage = 100f };
            tblA.AddCell(TR("TEAM A", fLabel, cTeamABg, pt: 8f, pb: 2f));
            tblA.AddCell(TR(teamAName, fTeamAHdr, cTeamABg, pt: 0f, pb: 4f));
            tblA.AddCell(TR($"Fund: BDT {teamAFund:N0}", fFund, cTeamABg, pt: 2f, pb: 2f));
            tblA.AddCell(TR($"Players: {cntA}", fLabel, cTeamABg, pt: 2f, pb: 8f));
            hdrTable.AddCell(new PdfPCell(tblA)
            { Border = PdfPCell.BOX, BorderColor = cGreen, BorderWidth = 1.5f, Padding = 0f, PaddingRight = 5f });

            int cntB = snapshot.Count(p => p.IsSold &&
                (p.AssignedTeam ?? "").Equals(teamBName, StringComparison.OrdinalIgnoreCase));
            var tblB = new PdfPTable(1) { WidthPercentage = 100f };
            tblB.AddCell(TR("TEAM B", fLabel, cTeamBBg, pt: 8f, pb: 2f));
            tblB.AddCell(TR(teamBName, fTeamBHdr, cTeamBBg, pt: 0f, pb: 4f));
            tblB.AddCell(TR($"Fund: BDT {teamBFund:N0}", fFund, cTeamBBg, pt: 2f, pb: 2f));
            tblB.AddCell(TR($"Players: {cntB}", fLabel, cTeamBBg, pt: 2f, pb: 8f));
            hdrTable.AddCell(new PdfPCell(tblB)
            {
                Border = PdfPCell.BOX,
                BorderColor = cBlue,
                BorderWidth = 1.5f,
                Padding = 0f,
                PaddingLeft = 5f,
                PaddingRight = 5f
            });

            var tblStats = new PdfPTable(1) { WidthPercentage = 100f };
            PdfPCell SR(string lbl, string val, iTextSharp.text.Font vf)
            {
                var inner = new PdfPTable(1) { WidthPercentage = 100f };
                inner.AddCell(new PdfPCell(new Phrase(lbl, fLabel))
                {
                    BackgroundColor = cStatsBg,
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingTop = 5f,
                    PaddingBottom = 1f
                });
                inner.AddCell(new PdfPCell(new Phrase(val, vf))
                {
                    BackgroundColor = cStatsBg,
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 5f
                });
                return new PdfPCell(inner) { Border = PdfPCell.NO_BORDER, Padding = 0f };
            }
            tblStats.AddCell(SR("TOTAL", total.ToString(), fStatVal));
            tblStats.AddCell(SR("SOLD", sold.ToString(), fStatRed));
            tblStats.AddCell(SR("FREE", avail.ToString(), fStatGrn));
            hdrTable.AddCell(new PdfPCell(tblStats)
            {
                Border = PdfPCell.BOX,
                BorderColor = cGold,
                BorderWidth = 1.5f,
                Padding = 0f,
                PaddingLeft = 5f
            });

            doc.Add(hdrTable);

            float[] cw = { 4f, 20f, 12f, 9f, 13f, 13f, 14f, 10f, 5f };
            var tbl = new PdfPTable(cw.Length)
            { WidthPercentage = 100f, HeaderRows = 1, SpacingBefore = 4f };
            tbl.SetWidths(cw);

            foreach (var h in new[] { "#", "Player Name", "Position", "Skill",
                                      "Base Price", "Sold Price", "Assigned Team", "Status", "" })
                tbl.AddCell(new PdfPCell(new Phrase(h, fColHdr))
                {
                    BackgroundColor = cTblHdr,
                    BorderWidthBottom = 1.5f,
                    BorderWidthTop = 0f,
                    BorderWidthLeft = 0f,
                    BorderWidthRight = 0f,
                    BorderColor = cGreen,
                    Padding = 7f,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                });

            PdfPCell MC(string txt, iTextSharp.text.Font font, BaseColor bg) =>
                new PdfPCell(new Phrase(txt ?? "", font))
                {
                    BackgroundColor = bg,
                    BorderWidthBottom = 0.3f,
                    BorderWidthTop = 0f,
                    BorderWidthLeft = 0f,
                    BorderWidthRight = 0f,
                    BorderColor = new BaseColor(30, 30, 30),
                    Padding = 6f,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

            bool alt = false; int rn = 0;
            foreach (var p in snapshot)
            {
                rn++;
                var bg = alt ? cAltRow : cDarkRow; alt = !alt;
                bool isS = p.IsSold;
                var cf = isS ? fCellDim : fCell;
                tbl.AddCell(MC(rn.ToString(), cf, bg));
                tbl.AddCell(MC(p.Name, cf, bg));
                tbl.AddCell(MC(p.Position, cf, bg));
                tbl.AddCell(MC(p.SkillLevel, cf, bg));
                tbl.AddCell(MC(p.BasePrice.ToString("N0"), cf, bg));
                tbl.AddCell(MC(isS ? p.SoldPrice.ToString("N0") : "-", cf, bg));
                tbl.AddCell(MC(p.AssignedTeam ?? "", isS ? fCellBlue : cf, bg));
                tbl.AddCell(MC(p.Status ?? "", isS ? fSold : fAvail, bg));
                tbl.AddCell(MC(isS ? "SOLD" : "FREE",
                    isS ? MkFont(6f, iTextSharp.text.Font.BOLD, cRed)
                        : MkFont(6f, iTextSharp.text.Font.BOLD, cGreen), bg));
            }
            doc.Add(tbl);

            doc.Add(new Paragraph(
                $"\nPRSC Auction System   |   Total: {total}   |   Sold: {sold}   |   Available: {avail}   |   Printed: {DateTime.Now:dd MMM yyyy HH:mm}",
                fFooter)
            { Alignment = Element.ALIGN_CENTER, SpacingBefore = 6f });

            doc.Close();
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private Player GetSelectedPlayer()
        {
            if (dgvPlayers == null || dgvPlayers.SelectedRows.Count == 0) return null;
            int idx = dgvPlayers.SelectedRows[0].Index;
            return (idx >= 0 && idx < players.Count) ? players[idx] : null;
        }

        private void ShowInfo(string msg) =>
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void dgvPlayers_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dgvPlayers_CellContentClick_1(object sender, DataGridViewCellEventArgs e) { }
    }
}