using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace PRSC_Player_Auction_System
{
    public class FullScreenAuctionForm : Form
    {
        // ── State ───────────────────────────────────────────────────────
        private readonly Player _player;
        private readonly MainForm _mainForm;
        private bool _biddingShown = false;

        // ── VLC ─────────────────────────────────────────────────────────
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private VideoView _videoView;
        private Timer _videoCheckTimer;

        // ── UI refs needed after build ───────────────────────────────────
        private Panel _pnlBidding;
        private Label _lblPlayerName;
        private Label _lblPosition;
        private Label _lblCurrentPrice;
        private Label _lblTeamAFund;
        private Label _lblTeamBFund;
        private Label _lblTeamAName;
        private Label _lblTeamBName;
        private Button _btnPlus1000, _btnMinus1000;
        private Button _btnPlus500, _btnMinus500;
        private Button _btnPlus200, _btnMinus200;
        private Button _btnPlus300, _btnMinus300;
        private Button _btnSellToA, _btnSellToB;
        private Button _btnClose, _btnSkipVideo;
        private TextBox _txtCustomBid;
        private Button _btnCustomBid;

        private bool _vlcCleaned = false;
        private bool _isClosing = false;

        // ── Design tokens ────────────────────────────────────────────────
        static readonly Color BG = Color.FromArgb(8, 8, 14);
        static readonly Color CARD = Color.FromArgb(16, 16, 24);
        static readonly Color CARD2 = Color.FromArgb(22, 22, 34);
        static readonly Color BORDER = Color.FromArgb(45, 45, 65);
        static readonly Color GREEN = Color.FromArgb(50, 210, 50);
        static readonly Color GREEN_BG = Color.FromArgb(10, 40, 10);
        static readonly Color GREEN_BTN = Color.FromArgb(20, 90, 20);
        static readonly Color GOLD = Color.FromArgb(255, 200, 0);
        static readonly Color BLUE = Color.FromArgb(80, 160, 255);
        static readonly Color BLUE_BG = Color.FromArgb(5, 15, 50);
        static readonly Color BLUE_BTN = Color.FromArgb(10, 30, 120);
        static readonly Color RED = Color.FromArgb(220, 55, 55);
        static readonly Color RED_BTN = Color.FromArgb(100, 20, 20);
        static readonly Color DIM = Color.FromArgb(110, 110, 130);
        static readonly Color WHITE = Color.FromArgb(235, 235, 240);

        // ═══════════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public FullScreenAuctionForm(Player player, MainForm mainForm)
        {
            _player = player;
            _mainForm = mainForm;
            _player.CurrentPrice = _player.BasePrice;

            this.BackColor = BG;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) SafeClose(); };

            try
            {
                Core.Initialize();
                _libVLC = new LibVLC();
                _mediaPlayer = new MediaPlayer(_libVLC);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("VLC Init: " + ex.Message);
            }

            BuildUI();
            UpdatePriceDisplay();
            UpdateFundDisplay();
            StartVideo();
        }

        // ═══════════════════════════════════════════════════════════════
        //  BUILD UI
        // ═══════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            // ── Video layer ─────────────────────────────────────────────
            if (_mediaPlayer != null)
            {
                _videoView = new VideoView { MediaPlayer = _mediaPlayer, Dock = DockStyle.Fill };
                this.Controls.Add(_videoView);
            }

            // ── Master bidding panel ────────────────────────────────────
            _pnlBidding = new Panel { Dock = DockStyle.Fill, BackColor = BG, Visible = false };
            this.Controls.Add(_pnlBidding);

            // ══════════════════════════════════════════════════════════
            //  ROOT TABLE:  topBar(56) | content(fill) | bottomBar(64)
            // ══════════════════════════════════════════════════════════
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            _pnlBidding.Controls.Add(root);

            // ══════════════════════════════════════════════════════════
            //  TOP BAR
            // ══════════════════════════════════════════════════════════
            var topBar = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(11, 11, 18) };
            var topLine = new Panel { Dock = DockStyle.Bottom, Height = 2, BackColor = GREEN };
            topBar.Controls.Add(topLine);

            var lblBrand = new Label
            {
                Text = "⚽  PRSC PLAYER AUCTION SYSTEM",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 10F),
                AutoSize = true,
                Location = new Point(18, 18)
            };
            topBar.Controls.Add(lblBrand);

            // LIVE pill
            var lblLive = new Label
            {
                Text = "● LIVE",
                ForeColor = Color.White,
                BackColor = RED,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Size = new Size(60, 22),
                TextAlign = ContentAlignment.MiddleCenter
            };
            topBar.Resize += (s, e) => lblLive.Location = new Point(topBar.Width - 80, 17);
            topBar.Controls.Add(lblLive);
            root.Controls.Add(topBar, 0, 0);

            // ══════════════════════════════════════════════════════════
            //  CONTENT ROW:  TeamA(22%) | Centre(56%) | TeamB(22%)
            // ══════════════════════════════════════════════════════════
            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(14, 12, 14, 8),
                Margin = Padding.Empty
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21));
            root.Controls.Add(content, 0, 1);

            // ─── TEAM A CARD ─────────────────────────────────────────
            content.Controls.Add(BuildTeamCard(
                "TEAM A", _mainForm.TeamAName, _mainForm.TeamAFund,
                GREEN, GREEN_BG, out _lblTeamAName, out _lblTeamAFund), 0, 0);

            // ─── TEAM B CARD ─────────────────────────────────────────
            content.Controls.Add(BuildTeamCard(
                "TEAM B", _mainForm.TeamBName, _mainForm.TeamBFund,
                BLUE, BLUE_BG, out _lblTeamBName, out _lblTeamBFund), 2, 0);

            // ─── CENTRE PANEL ────────────────────────────────────────
            var centre = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 0, 10, 0),
                Margin = Padding.Empty
            };
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 26));  // player card
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 32));  // price card
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 25));  // bid buttons
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 17));  // sell row
            content.Controls.Add(centre, 1, 0);

            // ── PLAYER INFO CARD ─────────────────────────────────────
            var playerCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CARD,
                Margin = new Padding(0, 0, 0, 8)
            };
            DrawBorder(playerCard, BORDER, 1);

            // Top accent strip
            var playerStrip = new Panel { Dock = DockStyle.Top, Height = 3, BackColor = GOLD };
            playerCard.Controls.Add(playerStrip);

            // Skill badge
            var lblSkill = new Label
            {
                Text = _player.SkillLevel ?? "PRO",
                ForeColor = GOLD,
                BackColor = Color.FromArgb(45, 38, 0),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Size = new Size(68, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            playerCard.Resize += (s, e) =>
                lblSkill.Location = new Point((playerCard.Width - lblSkill.Width) / 2, 10);
            playerCard.Controls.Add(lblSkill);

            _lblPosition = new Label
            {
                Text = _player.Position ?? "",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 12F),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 26
            };

            _lblPlayerName = new Label
            {
                Text = _player.Name,
                ForeColor = GOLD,
                Font = new Font("Impact", 36F),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            playerCard.Controls.Add(_lblPlayerName);
            playerCard.Controls.Add(_lblPosition);
            centre.Controls.Add(playerCard, 0, 0);

            // ── PRICE CARD ───────────────────────────────────────────
            var priceCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = GREEN_BG,
                Margin = new Padding(0, 0, 0, 8)
            };
            DrawBorder(priceCard, GREEN, 2);

            var lblBidCaption = new Label
            {
                Text = "CURRENT BID",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 24
            };

            _lblCurrentPrice = new Label
            {
                ForeColor = GREEN,
                Font = new Font("Impact", 54F),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var lblBase = new Label
            {
                Text = $"Base price: BDT {_player.BasePrice:N0}",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 8F),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 20
            };

            priceCard.Controls.Add(_lblCurrentPrice);
            priceCard.Controls.Add(lblBidCaption);
            priceCard.Controls.Add(lblBase);
            centre.Controls.Add(priceCard, 0, 1);

            // ── BID BUTTONS (2 rows × 4 cols) ────────────────────────
            var bidPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 6)
            };

            var bidGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 4,
                BackColor = Color.Transparent,
                Padding = Padding.Empty
            };
            for (int i = 0; i < 4; i++)
                bidGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            bidGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            bidGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            _btnPlus1000 = BidBtn("+1,000", Color.FromArgb(18, 80, 18), GREEN, 15);
            _btnPlus500 = BidBtn("+500", Color.FromArgb(15, 68, 15), GREEN, 13);
            _btnPlus300 = BidBtn("+300", Color.FromArgb(12, 56, 12), GREEN, 12);
            _btnPlus200 = BidBtn("+200", Color.FromArgb(10, 46, 10), GREEN, 12);
            _btnMinus1000 = BidBtn("−1,000", Color.FromArgb(90, 15, 15), RED, 15);
            _btnMinus500 = BidBtn("−500", Color.FromArgb(75, 12, 12), RED, 13);
            _btnMinus300 = BidBtn("−300", Color.FromArgb(62, 10, 10), RED, 12);
            _btnMinus200 = BidBtn("−200", Color.FromArgb(52, 8, 8), RED, 12);

            bidGrid.Controls.Add(_btnPlus1000, 0, 0);
            bidGrid.Controls.Add(_btnPlus500, 1, 0);
            bidGrid.Controls.Add(_btnPlus300, 2, 0);
            bidGrid.Controls.Add(_btnPlus200, 3, 0);
            bidGrid.Controls.Add(_btnMinus1000, 0, 1);
            bidGrid.Controls.Add(_btnMinus500, 1, 1);
            bidGrid.Controls.Add(_btnMinus300, 2, 1);
            bidGrid.Controls.Add(_btnMinus200, 3, 1);

            bidPanel.Controls.Add(bidGrid);
            centre.Controls.Add(bidPanel, 0, 2);

            // ── SELL ROW ─────────────────────────────────────────────
            var sellRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 4,
                BackColor = Color.Transparent,
                Padding = Padding.Empty
            };
            sellRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
            sellRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
            sellRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));
            sellRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11));

            _btnSellToA = SellBtn(
                $"✔  SELL TO  {_mainForm.TeamAName}",
                Color.FromArgb(0, 110, 0), GREEN);

            _btnSellToB = SellBtn(
                $"✔  SELL TO  {_mainForm.TeamBName}",
                Color.FromArgb(0, 35, 130), BLUE);

            _txtCustomBid = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = CARD2,
                ForeColor = DIM,
                Font = new Font("Segoe UI", 10F),
                TextAlign = HorizontalAlignment.Center,
                Text = "Custom amount",
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(4, 2, 4, 2)
            };
            _txtCustomBid.GotFocus += (s, e) =>
            {
                if (_txtCustomBid.Text == "Custom amount")
                { _txtCustomBid.Text = ""; _txtCustomBid.ForeColor = WHITE; }
            };
            _txtCustomBid.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_txtCustomBid.Text))
                { _txtCustomBid.Text = "Custom amount"; _txtCustomBid.ForeColor = DIM; }
            };

            _btnCustomBid = new Button
            {
                Dock = DockStyle.Fill,
                Text = "SET",
                BackColor = Color.FromArgb(70, 50, 160),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 2)
            };
            _btnCustomBid.FlatAppearance.BorderSize = 0;

            sellRow.Controls.Add(_btnSellToA, 0, 0);
            sellRow.Controls.Add(_btnSellToB, 1, 0);
            sellRow.Controls.Add(_txtCustomBid, 2, 0);
            sellRow.Controls.Add(_btnCustomBid, 3, 0);
            centre.Controls.Add(sellRow, 0, 3);

            // ══════════════════════════════════════════════════════════
            //  BOTTOM FUND BAR
            // ══════════════════════════════════════════════════════════
            var bottomBar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(11, 11, 18)
            };
            var topBorderLine = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = BORDER };
            bottomBar.Controls.Add(topBorderLine);

            var fundRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(28, 0, 28, 0)
            };
            fundRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            fundRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            fundRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            bottomBar.Controls.Add(fundRow);

            _lblTeamAFund = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = GREEN,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            var lblMid = new Label
            {
                Dock = DockStyle.Fill,
                Text = "PRSC  •  AUCTION",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 8F),
                TextAlign = ContentAlignment.MiddleCenter
            };
            _lblTeamBFund = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = BLUE,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            fundRow.Controls.Add(_lblTeamAFund, 0, 0);
            fundRow.Controls.Add(lblMid, 1, 0);
            fundRow.Controls.Add(_lblTeamBFund, 2, 0);
            root.Controls.Add(bottomBar, 0, 2);

            // ══════════════════════════════════════════════════════════
            //  OVERLAY: Close + Skip buttons (always on top)
            // ══════════════════════════════════════════════════════════
            _btnClose = new Button
            {
                Text = "✕",
                BackColor = Color.FromArgb(150, 20, 20),
                ForeColor = Color.White,
                Size = new Size(42, 30),
                Location = new Point(14, 13),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnClose.FlatAppearance.BorderSize = 0;

            _btnSkipVideo = new Button
            {
                Text = "⏭  Skip Video",
                BackColor = Color.FromArgb(45, 45, 55),
                ForeColor = WHITE,
                Size = new Size(130, 30),
                Location = new Point(62, 13),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Cursor = Cursors.Hand
            };
            _btnSkipVideo.FlatAppearance.BorderSize = 0;

            this.Controls.Add(_btnClose);
            this.Controls.Add(_btnSkipVideo);
            _btnClose.BringToFront();
            _btnSkipVideo.BringToFront();

            // ── Wire events ──────────────────────────────────────────────
            _btnPlus1000.Click += (s, e) => ChangePrice(+1000);
            _btnMinus1000.Click += (s, e) => ChangePrice(-1000);
            _btnPlus500.Click += (s, e) => ChangePrice(+500);
            _btnMinus500.Click += (s, e) => ChangePrice(-500);
            _btnPlus300.Click += (s, e) => ChangePrice(+300);
            _btnMinus300.Click += (s, e) => ChangePrice(-300);
            _btnPlus200.Click += (s, e) => ChangePrice(+200);
            _btnMinus200.Click += (s, e) => ChangePrice(-200);
            _btnSellToA.Click += BtnSellToA_Click;
            _btnSellToB.Click += BtnSellToB_Click;
            _btnCustomBid.Click += BtnCustomBid_Click;
            _btnClose.Click += (s, e) => SafeClose();
            _btnSkipVideo.Click += (s, e) => ShowBiddingPanel();
        }

        // ═══════════════════════════════════════════════════════════════
        //  TEAM CARD BUILDER
        // ═══════════════════════════════════════════════════════════════
        private Panel BuildTeamCard(string tag, string name, decimal fund,
                                    Color accent, Color bg,
                                    out Label nameOut, out Label fundOut)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = bg,
                Padding = new Padding(14, 12, 14, 12),
                Margin = Padding.Empty
            };
            DrawBorder(card, accent, 2);

            // Top accent strip
            var strip = new Panel { Dock = DockStyle.Top, Height = 3, BackColor = accent };
            card.Controls.Add(strip);

            // Tag label  e.g. "TEAM A"
            var lblTag = new Label
            {
                Text = tag,
                ForeColor = accent,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblTag);

            // Team name
            var lblName = new Label
            {
                Text = name,
                ForeColor = Color.White,
                Font = new Font("Impact", 17F),
                Dock = DockStyle.Top,
                Height = 34,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblName);
            nameOut = lblName;

            // Divider
            var div = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = BORDER };
            card.Controls.Add(div);

            // "REMAINING FUNDS" tag
            var lblFundTag = new Label
            {
                Text = "REMAINING FUNDS",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 18,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblFundTag);

            // Fund value
            var lblFund = new Label
            {
                Text = $"BDT {fund:N0}",
                ForeColor = GOLD,
                Font = new Font("Impact", 15F),
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblFund);
            fundOut = lblFund;

            // Spacer
            var spacer = new Panel { Dock = DockStyle.Top, Height = 8, BackColor = Color.Transparent };
            card.Controls.Add(spacer);

            // "PLAYERS ACQUIRED" tag
            var lblAcqTag = new Label
            {
                Text = "PLAYERS ACQUIRED",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 18,
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblAcqTag);

            return card;
        }

        // ═══════════════════════════════════════════════════════════════
        //  BUTTON FACTORIES
        // ═══════════════════════════════════════════════════════════════
        private Button BidBtn(string text, Color bg, Color fg, int fs)
        {
            var b = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = bg,
                ForeColor = fg,
                Font = new Font("Segoe UI", fs, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(3, 3, 3, 3)
            };
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = Color.FromArgb(
                Math.Min(bg.R + 28, 255),
                Math.Min(bg.G + 28, 255),
                Math.Min(bg.B + 28, 255));
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(bg.R + 18, 255),
                Math.Min(bg.G + 18, 255),
                Math.Min(bg.B + 18, 255));
            return b;
        }

        private Button SellBtn(string text, Color bg, Color accent)
        {
            var b = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = bg,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(3, 2, 3, 2)
            };
            b.FlatAppearance.BorderSize = 2;
            b.FlatAppearance.BorderColor = accent;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(bg.R + 22, 255),
                Math.Min(bg.G + 22, 255),
                Math.Min(bg.B + 22, 255));
            return b;
        }

        // Draws a 1px border via Paint event (no Rectangle ambiguity)
        private static void DrawBorder(Panel p, Color color, int w = 1)
        {
            p.Paint += (s, e) =>
            {
                using (var pen = new Pen(color, w))
                {
                    e.Graphics.DrawRectangle(pen, w / 2, w / 2,
                        p.Width - w, p.Height - w);
                }
            };
        }

        // ═══════════════════════════════════════════════════════════════
        //  VIDEO  (logic unchanged)
        // ═══════════════════════════════════════════════════════════════
        private void StartVideo()
        {
            try
            {
                if (_mediaPlayer != null &&
                    !string.IsNullOrEmpty(_player.VideoPath) &&
                    File.Exists(_player.VideoPath))
                {
                    var media = new Media(_libVLC, _player.VideoPath, FromType.FromPath);
                    _mediaPlayer.Play(media);
                    _videoCheckTimer = new Timer { Interval = 500 };
                    _videoCheckTimer.Tick += VideoCheckTimer_Tick;
                    _videoCheckTimer.Start();
                }
                else ShowBiddingPanel();
            }
            catch { ShowBiddingPanel(); }
        }

        private void VideoCheckTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_isClosing || _biddingShown) { _videoCheckTimer?.Stop(); return; }
                if (_mediaPlayer != null && !_mediaPlayer.IsPlaying)
                { _videoCheckTimer?.Stop(); ShowBiddingPanel(); }
            }
            catch { }
        }

        private void ShowBiddingPanel()
        {
            if (_biddingShown) return;
            _biddingShown = true;
            try
            {
                _videoCheckTimer?.Stop(); _videoCheckTimer?.Dispose(); _videoCheckTimer = null;
                try { _mediaPlayer?.Stop(); } catch { }
                if (_videoView != null) _videoView.Visible = false;
                _btnSkipVideo.Visible = false;
                _pnlBidding.Visible = true;
            }
            catch { }
        }

        private void CleanupVlc()
        {
            if (_vlcCleaned) return;
            _vlcCleaned = true;
            try { _videoCheckTimer?.Stop(); _videoCheckTimer?.Dispose(); } catch { }
            _videoCheckTimer = null;
            try { _mediaPlayer?.Stop(); } catch { }
            try { if (_videoView != null) { _videoView.MediaPlayer = null; _videoView.Dispose(); } } catch { }
            _videoView = null;
            try { _mediaPlayer?.Dispose(); } catch { }
            _mediaPlayer = null;
            try { _libVLC?.Dispose(); } catch { }
            _libVLC = null;
        }

        // ═══════════════════════════════════════════════════════════════
        //  SELL EVENTS  (logic unchanged)
        // ═══════════════════════════════════════════════════════════════
        private void BtnSellToA_Click(object sender, EventArgs e) =>
            SellTo(_mainForm.TeamAName, _mainForm.TeamAFund, f => _mainForm.TeamAFund = f);

        private void BtnSellToB_Click(object sender, EventArgs e) =>
            SellTo(_mainForm.TeamBName, _mainForm.TeamBFund, f => _mainForm.TeamBFund = f);

        private void SellTo(string teamName, decimal fund, Action<decimal> setFund)
        {
            if (fund < _player.CurrentPrice)
            {
                MessageBox.Show(
                    $"⚠  {teamName} does not have enough funds!\n\n" +
                    $"Required : BDT {_player.CurrentPrice:N0}\n" +
                    $"Available: BDT {fund:N0}",
                    "Insufficient Funds", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _player.AssignedTeam = teamName;
            _player.IsSold = true;
            _player.SoldPrice = _player.CurrentPrice;
            setFund(fund - _player.CurrentPrice);

            try { DatabaseHelper.AssignPlayerToTeam(_player.Id, teamName, _player.SoldPrice); }
            catch { }

            MessageBox.Show(
                $"✅  {_player.Name} sold to {teamName}\nfor BDT {_player.SoldPrice:N0}",
                "Player Sold!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SafeClose();
        }

        // ═══════════════════════════════════════════════════════════════
        //  CUSTOM BID  (logic unchanged)
        // ═══════════════════════════════════════════════════════════════
        private void BtnCustomBid_Click(object sender, EventArgs e)
        {
            string t = _txtCustomBid.Text.Trim();
            if (string.IsNullOrWhiteSpace(t) || t == "Custom amount")
            { MessageBox.Show("Please enter a custom bid amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!decimal.TryParse(t, out decimal amt))
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCustomBid.Text = "Custom amount"; _txtCustomBid.ForeColor = DIM; return;
            }
            if (amt < _player.BasePrice)
            { MessageBox.Show($"Bid must be at least BDT {_player.BasePrice:N0}", "Bid Too Low", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            _player.CurrentPrice = amt;
            _txtCustomBid.Text = "Custom amount";
            _txtCustomBid.ForeColor = DIM;
            UpdatePriceDisplay();
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════
        private void ChangePrice(decimal delta)
        {
            decimal n = _player.CurrentPrice + delta;
            if (n < _player.BasePrice) return;
            _player.CurrentPrice = n;
            UpdatePriceDisplay();
        }

        private void UpdatePriceDisplay()
        {
            _lblCurrentPrice.Text = $"BDT  {_player.CurrentPrice:N0}";
        }

        private void UpdateFundDisplay()
        {
            _lblTeamAFund.Text = $"▶  {_mainForm.TeamAName}   BDT {_mainForm.TeamAFund:N0}";
            _lblTeamBFund.Text = $"BDT {_mainForm.TeamBFund:N0}   {_mainForm.TeamBName}  ◀";
        }

        private void SafeClose()
        {
            _isClosing = true;
            CleanupVlc();
            try { this.Close(); } catch { }
        }

        // kept for compatibility — not used in new UI but avoids any stray call errors
        private Button MakeBtn(string text, Color color, int fontSize)
        {
            var b = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Impact", fontSize),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(2)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        { base.OnFormClosed(e); _isClosing = true; CleanupVlc(); }

        protected override void Dispose(bool disposing)
        { if (disposing) { _isClosing = true; CleanupVlc(); } base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(921, 621);
            this.Name = "FullScreenAuctionForm";
            this.Load += new System.EventHandler(this.FullScreenAuctionForm_Load);
            this.ResumeLayout(false);
        }

        private void FullScreenAuctionForm_Load(object sender, EventArgs e) { }
    }
}