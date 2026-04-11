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

        // ── TIMER SYSTEM ────────────────────────────────────────────────
        private Timer _auctionTimer;
        private int _remainingSeconds = 15;
        private const int AUCTION_TIME_LIMIT = 15;
        private bool _timerStarted = false;
        private string _lastBidderTeam = "";
        private string _activeBidTeam = "";
        private bool _popupBiddingActive = false;

        // ── UI refs needed after build ───────────────────────────────────
        private Panel _pnlBidding;
        private Panel _pnlTimer;
        private Label _lblPlayerName;
        private Label _lblPosition;
        private Label _lblCurrentPrice;
        private Label _lblTeamAFund;
        private Label _lblTeamBFund;
        private Label _lblTeamAName;
        private Label _lblTeamBName;
        private Panel _teamACard;
        private Panel _teamBCard;
        private Label _lblTimerCount;
        private Label _lblTimerStatus;
        private Panel _timerProgressBar;
        private Button _btnPlus1000, _btnMinus1000;
        private Button _btnPlus500, _btnMinus500;
        private Button _btnPlus200, _btnMinus200;
        private Button _btnPlus300, _btnMinus300;
        private Button _btnSellToA, _btnSellToB;
        private Button _btnClose, _btnSkipVideo;
        private Button _btnTimerStop, _btnTimerReset;
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
        static readonly Color ORANGE = Color.FromArgb(255, 140, 0);
        static readonly Color ORANGE_BG = Color.FromArgb(40, 20, 0);
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
            _player.LastBidder = "";

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
            InitializeTimer();
            _activeBidTeam = _mainForm.TeamAName;
            UpdatePriceDisplay();
            UpdateFundDisplay();
            UpdateActiveBidTeamUI();
            StartVideo();
        }

        // ═══════════════════════════════════════════════════════════════
        //  TIMER INITIALIZATION
        // ═══════════════════════════════════════════════════════════════
        private void InitializeTimer()
        {
            _auctionTimer = new Timer { Interval = 1000 }; // 1 second
            _auctionTimer.Tick += AuctionTimer_Tick;
        }

        private void AuctionTimer_Tick(object sender, EventArgs e)
        {
            _remainingSeconds--;
            UpdateTimerDisplay();

            if (_remainingSeconds <= 0)
            {
                _auctionTimer.Stop();
                AutoSellToLastBidder();
            }
        }

        private void StartAuctionTimer()
        {
            if (!_timerStarted)
            {
                _timerStarted = true;
                _remainingSeconds = AUCTION_TIME_LIMIT;
                UpdateTimerDisplay();
                _auctionTimer.Start();
                _lblTimerStatus.ForeColor = ORANGE;
                UpdateActiveBidTeamUI();
            }
        }

        private void ResetAuctionTimer()
        {
            _remainingSeconds = AUCTION_TIME_LIMIT;
            UpdateTimerDisplay();

            if (_timerStarted)
            {
                _auctionTimer.Stop();
                _auctionTimer.Start();
            }
        }

        private void StopAuctionTimer()
        {
            _auctionTimer.Stop();
            _lblTimerStatus.ForeColor = DIM;
            UpdateActiveBidTeamUI();
        }

        private void ManualResetTimer()
        {
            _auctionTimer.Stop();
            _timerStarted = false;
            _remainingSeconds = AUCTION_TIME_LIMIT;
            UpdateTimerDisplay();
            _lblTimerStatus.ForeColor = DIM;
            UpdateActiveBidTeamUI();
        }

        private void UpdateTimerDisplay()
        {
            _lblTimerCount.Text = _remainingSeconds.ToString();

            // Update progress bar width
            int maxWidth = _pnlTimer.Width - 32; // Account for padding
            int progressWidth = (int)((double)_remainingSeconds / AUCTION_TIME_LIMIT * maxWidth);
            _timerProgressBar.Width = Math.Max(0, progressWidth);

            // Change color based on time remaining
            if (_remainingSeconds <= 5)
            {
                _lblTimerCount.ForeColor = RED;
                _timerProgressBar.BackColor = RED;
            }
            else if (_remainingSeconds <= 10)
            {
                _lblTimerCount.ForeColor = ORANGE;
                _timerProgressBar.BackColor = ORANGE;
            }
            else
            {
                _lblTimerCount.ForeColor = GREEN;
                _timerProgressBar.BackColor = GREEN;
            }
        }

        private void AutoSellToLastBidder()
        {
            _popupBiddingActive = false;

            if (string.IsNullOrEmpty(_lastBidderTeam))
            {
                MessageBox.Show(
                    "⚠  No bids placed yet!\nTimer expired without any bids.",
                    "No Bids", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SafeClose();
                return;
            }

            SellToLastBidder(true);
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
            _teamACard = BuildTeamCard(
                "TEAM A", _mainForm.TeamAName, _mainForm.TeamAFund,
                GREEN, GREEN_BG, out _lblTeamAName, out _lblTeamAFund);
            content.Controls.Add(_teamACard, 0, 0);

            // ─── TEAM B CARD ─────────────────────────────────────────
            _teamBCard = BuildTeamCard(
                "TEAM B", _mainForm.TeamBName, _mainForm.TeamBFund,
                BLUE, BLUE_BG, out _lblTeamBName, out _lblTeamBFund);
            content.Controls.Add(_teamBCard, 2, 0);

            // ─── CENTRE PANEL ────────────────────────────────────────
            var centre = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(10, 0, 10, 0),
                Margin = Padding.Empty
            };
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 22));  // player card
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 28));  // price card
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 14));  // TIMER CARD
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 21));  // bid buttons
            centre.RowStyles.Add(new RowStyle(SizeType.Percent, 15));  // sell row
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

            // ══════════════════════════════════════════════════════════
            //  TIMER CARD
            // ══════════════════════════════════════════════════════════
            _pnlTimer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ORANGE_BG,
                Margin = new Padding(0, 0, 0, 8)
            };
            DrawBorder(_pnlTimer, ORANGE, 2);

            // Timer layout
            var timerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(16, 8, 16, 8),
                Margin = Padding.Empty
            };
            timerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            timerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            timerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            // Left: Timer count
            var timerCountPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            _lblTimerCount = new Label
            {
                Text = "15",
                ForeColor = GREEN,
                Font = new Font("Impact", 42F),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            var lblSeconds = new Label
            {
                Text = "SECONDS",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Bottom,
                Height = 14
            };

            timerCountPanel.Controls.Add(_lblTimerCount);
            timerCountPanel.Controls.Add(lblSeconds);
            timerLayout.Controls.Add(timerCountPanel, 0, 0);

            // Center: Progress bar area
            var progressPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            _lblTimerStatus = new Label
            {
                Text = "⏱  WAITING FOR FIRST BID",
                ForeColor = DIM,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 20
            };

            var progressContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(15, 15, 20),
                Padding = new Padding(0, 8, 0, 8)
            };
            DrawBorder(progressContainer, BORDER, 1);

            _timerProgressBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = progressContainer.Width - 32,
                BackColor = GREEN,
                Margin = Padding.Empty
            };

            progressContainer.Controls.Add(_timerProgressBar);
            progressPanel.Controls.Add(_lblTimerStatus);
            progressPanel.Controls.Add(progressContainer);
            timerLayout.Controls.Add(progressPanel, 1, 0);

            // Right: Control buttons
            var controlPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = Padding.Empty
            };
            controlPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            controlPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            _btnTimerStop = new Button
            {
                Text = "⏸  PAUSE",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(60, 40, 10),
                ForeColor = ORANGE,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(2, 2, 2, 2)
            };
            _btnTimerStop.FlatAppearance.BorderSize = 1;
            _btnTimerStop.FlatAppearance.BorderColor = ORANGE;

            _btnTimerReset = new Button
            {
                Text = "↻  RESET",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(50, 20, 20),
                ForeColor = RED,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(2, 2, 2, 2)
            };
            _btnTimerReset.FlatAppearance.BorderSize = 1;
            _btnTimerReset.FlatAppearance.BorderColor = RED;

            controlPanel.Controls.Add(_btnTimerStop, 0, 0);
            controlPanel.Controls.Add(_btnTimerReset, 0, 1);
            timerLayout.Controls.Add(controlPanel, 2, 0);

            _pnlTimer.Controls.Add(timerLayout);
            centre.Controls.Add(_pnlTimer, 0, 2);

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
            centre.Controls.Add(bidPanel, 0, 3);

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
            _txtCustomBid.KeyDown += TxtCustomBid_KeyDown;

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
            centre.Controls.Add(sellRow, 0, 4);

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
            _btnTimerStop.Click += (s, e) => StopAuctionTimer();
            _btnTimerReset.Click += (s, e) => ManualResetTimer();
            WireBidderSelect(_teamACard, _mainForm.TeamAName);
            WireBidderSelect(_teamBCard, _mainForm.TeamBName);
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
        //  SELL EVENTS
        // ═══════════════════════════════════════════════════════════════
        private void BtnSellToA_Click(object sender, EventArgs e) =>
            SellTo(_mainForm.TeamAName, _mainForm.TeamAFund, f => _mainForm.TeamAFund = f, false);

        private void BtnSellToB_Click(object sender, EventArgs e) =>
            SellTo(_mainForm.TeamBName, _mainForm.TeamBFund, f => _mainForm.TeamBFund = f, false);

        private void SellTo(string teamName, decimal fund, Action<decimal> setFund, bool isAutoSell)
        {
            if (fund < _player.CurrentPrice)
            {
                MessageBox.Show(
                    $"⚠  {teamName} does not have enough funds!\n\n" +
                    $"Required : BDT {_player.CurrentPrice:N0}\n" +
                    $"Available: BDT {fund:N0}",
                    "Insufficient Funds", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (isAutoSell)
                    SafeClose();

                return;
            }

            _player.AssignedTeam = teamName;
            _player.IsSold = true;
            _player.SoldPrice = _player.CurrentPrice;
            setFund(fund - _player.CurrentPrice);

            try { DatabaseHelper.AssignPlayerToTeam(_player.Id, teamName, _player.SoldPrice); }
            catch { }

            string saleType = isAutoSell ? "⏱ AUTO-SOLD" : "✅ SOLD";
            MessageBox.Show(
                $"{saleType}\n\n{_player.Name} → {teamName}\nPrice: BDT {_player.SoldPrice:N0}",
                isAutoSell ? "Timer Expired - Auto Sold!" : "Player Sold!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            SafeClose();
        }

        // ═══════════════════════════════════════════════════════════════
        //  CUSTOM BID
        // ═══════════════════════════════════════════════════════════════
        private void BtnCustomBid_Click(object sender, EventArgs e)
        {
            TryPlaceBidFromInput();
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════
        private void ChangePrice(decimal delta)
        {
            if (_popupBiddingActive) return;

            if (delta > 0)
            {
                TryPlaceFirstBid(_player.CurrentPrice + delta);
                return;
            }

            decimal n = _player.CurrentPrice + delta;
            if (n < _player.BasePrice) return;
            _player.CurrentPrice = n;
            UpdatePriceDisplay();
        }

        private void TxtCustomBid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;
            e.Handled = true;
            TryPlaceBidFromInput();
        }

        private void TryPlaceBidFromInput()
        {
            string t = _txtCustomBid.Text.Trim();
            if (string.IsNullOrWhiteSpace(t) || t == "Custom amount")
            {
                MessageBox.Show("Please enter a custom bid amount.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(t, out decimal amt))
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCustomBid.Text = "Custom amount";
                _txtCustomBid.ForeColor = DIM;
                return;
            }

            TryPlaceFirstBid(amt);
        }

        private void TryPlaceFirstBid(decimal amount)
        {
            if (_popupBiddingActive) return;

            if (!ValidateBidForTeam(_activeBidTeam, amount, true))
                return;

            ApplyBid(_activeBidTeam, amount);
            PreparePopupMode(_activeBidTeam);
            RunBidPopupLoop(_lastBidderTeam);
        }

        private void SetActiveBidTeam(string teamName)
        {
            if (_popupBiddingActive) return;
            _activeBidTeam = teamName;
            UpdateActiveBidTeamUI();
        }

        private void PreparePopupMode(string nextBidTeam)
        {
            _popupBiddingActive = true;
            _activeBidTeam = nextBidTeam;
            SetMainBiddingControlsEnabled(false);
            _remainingSeconds = AUCTION_TIME_LIMIT;
            UpdateTimerDisplay();
            UpdateActiveBidTeamUI();
        }

        private void UpdateActiveBidTeamUI()
        {
            if (_lblTeamAName == null || _lblTeamBName == null || _lblTimerStatus == null) return;

            bool isTeamAActive = _activeBidTeam == _mainForm.TeamAName;
            _lblTeamAName.ForeColor = isTeamAActive ? GREEN : WHITE;
            _lblTeamBName.ForeColor = isTeamAActive ? WHITE : BLUE;

            string timerState;
            if (_popupBiddingActive)
                timerState = "POPUP TURN ACTIVE";
            else if (_timerStarted && _auctionTimer != null && _auctionTimer.Enabled)
                timerState = "TIMER ACTIVE";
            else if (_timerStarted)
                timerState = "TIMER PAUSED";
            else
                timerState = "WAITING FOR FIRST BID";
            _lblTimerStatus.Text = $"⏱  {timerState}  |  NEXT BID: {_activeBidTeam}";
        }

        private void ApplyBid(string biddingTeam, decimal amount)
        {
            _player.CurrentPrice = amount;
            _lastBidderTeam = biddingTeam;
            _activeBidTeam = GetOppositeTeam(biddingTeam);
            _timerStarted = true;
            _remainingSeconds = AUCTION_TIME_LIMIT;

            _txtCustomBid.Text = "Custom amount";
            _txtCustomBid.ForeColor = DIM;
            UpdatePriceDisplay();
            UpdateTimerDisplay();
            UpdateActiveBidTeamUI();
        }

        private void RunBidPopupLoop(string previousBidTeam)
        {
            string nextTeam = GetOppositeTeam(previousBidTeam);

            while (!_isClosing && _popupBiddingActive)
            {
                using (var popup = new BidTurnForm(
                    nextTeam,
                    _player.CurrentPrice,
                    _lastBidderTeam,
                    AUCTION_TIME_LIMIT,
                    GetTeamAccent(nextTeam),
                    GetMinimumNextBid(),
                    GetFundForTeam(nextTeam)))
                {
                    var dialogResult = popup.ShowDialog(this);
                    if (dialogResult != DialogResult.OK)
                    {
                        _popupBiddingActive = false;
                        SetMainBiddingControlsEnabled(true);
                        UpdateActiveBidTeamUI();
                        return;
                    }

                    if (popup.Action == BidTurnAction.PlaceBid)
                    {
                        if (!ValidateBidForTeam(nextTeam, popup.BidAmount, true))
                            continue;

                        ApplyBid(nextTeam, popup.BidAmount);
                        nextTeam = GetOppositeTeam(nextTeam);
                        continue;
                    }

                    if (popup.Action == BidTurnAction.SellToLastBidder)
                    {
                        SellToLastBidder(false);
                        return;
                    }

                    if (popup.Action == BidTurnAction.Timeout)
                    {
                        AutoSellToLastBidder();
                        return;
                    }

                    _popupBiddingActive = false;
                    SetMainBiddingControlsEnabled(true);
                    UpdateActiveBidTeamUI();
                    return;
                }
            }
        }

        private bool ValidateBidForTeam(string teamName, decimal amount, bool showMessage)
        {
            bool hasPreviousBid = !string.IsNullOrEmpty(_lastBidderTeam);
            bool invalidAmount = hasPreviousBid
                ? amount <= _player.CurrentPrice
                : amount < _player.CurrentPrice;

            if (invalidAmount)
            {
                if (showMessage)
                {
                    string text = hasPreviousBid
                        ? $"Bid must be higher than current bid: BDT {_player.CurrentPrice:N0}"
                        : $"First bid must be at least current/base price: BDT {_player.CurrentPrice:N0}";
                    MessageBox.Show(
                        text,
                        "Bid Too Low",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                return false;
            }

            decimal fund = teamName == _mainForm.TeamAName ? _mainForm.TeamAFund : _mainForm.TeamBFund;
            if (fund < amount)
            {
                if (showMessage)
                {
                    MessageBox.Show(
                        $"⚠  {teamName} does not have enough funds!\n\n" +
                        $"Required : BDT {amount:N0}\n" +
                        $"Available: BDT {fund:N0}",
                        "Insufficient Funds",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                return false;
            }

            return true;
        }

        private void SetMainBiddingControlsEnabled(bool enabled)
        {
            _btnPlus1000.Enabled = enabled;
            _btnPlus500.Enabled = enabled;
            _btnPlus300.Enabled = enabled;
            _btnPlus200.Enabled = enabled;
            _btnMinus1000.Enabled = enabled;
            _btnMinus500.Enabled = enabled;
            _btnMinus300.Enabled = enabled;
            _btnMinus200.Enabled = enabled;
            _btnSellToA.Enabled = enabled;
            _btnSellToB.Enabled = enabled;
            _btnCustomBid.Enabled = enabled;
            _txtCustomBid.Enabled = enabled;
            _btnTimerStop.Enabled = enabled && !_popupBiddingActive;
            _btnTimerReset.Enabled = enabled && !_popupBiddingActive;
        }

        private decimal GetMinimumNextBid()
        {
            return _player.CurrentPrice + 1;
        }

        private decimal GetFundForTeam(string teamName)
        {
            return teamName == _mainForm.TeamAName ? _mainForm.TeamAFund : _mainForm.TeamBFund;
        }

        private Color GetTeamAccent(string teamName)
        {
            return teamName == _mainForm.TeamAName ? GREEN : BLUE;
        }

        private string GetOppositeTeam(string teamName)
        {
            return teamName == _mainForm.TeamAName ? _mainForm.TeamBName : _mainForm.TeamAName;
        }

        private void SellToLastBidder(bool isAutoSell)
        {
            if (_lastBidderTeam == _mainForm.TeamAName)
            {
                SellTo(_mainForm.TeamAName, _mainForm.TeamAFund, f => _mainForm.TeamAFund = f, isAutoSell);
                return;
            }

            if (_lastBidderTeam == _mainForm.TeamBName)
            {
                SellTo(_mainForm.TeamBName, _mainForm.TeamBFund, f => _mainForm.TeamBFund = f, isAutoSell);
            }
        }

        private void WireBidderSelect(Control control, string teamName)
        {
            control.Cursor = Cursors.Hand;
            control.Click += (s, e) => SetActiveBidTeam(teamName);

            foreach (Control child in control.Controls)
                WireBidderSelect(child, teamName);
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
            _popupBiddingActive = false;

            // Stop and cleanup timer
            if (_auctionTimer != null)
            {
                _auctionTimer.Stop();
                _auctionTimer.Dispose();
                _auctionTimer = null;
            }

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
        {
            base.OnFormClosed(e);
            _isClosing = true;

            if (_auctionTimer != null)
            {
                _auctionTimer.Stop();
                _auctionTimer.Dispose();
            }

            CleanupVlc();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isClosing = true;

                if (_auctionTimer != null)
                {
                    _auctionTimer.Stop();
                    _auctionTimer.Dispose();
                    _auctionTimer = null;
                }

                CleanupVlc();
            }
            base.Dispose(disposing);
        }

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

    internal enum BidTurnAction
    {
        None,
        PlaceBid,
        SellToLastBidder,
        Timeout
    }

    internal sealed class BidTurnForm : Form
    {
        private readonly string _teamName;
        private readonly decimal _currentPrice;
        private readonly string _lastBidderTeam;
        private readonly int _timeLimitSeconds;
        private readonly Color _accentColor;
        private readonly decimal _minimumBid;
        private readonly decimal _availableFund;
        private readonly Timer _timer;

        private int _remainingSeconds;
        private Label _lblTimer;
        private TextBox _txtBid;
        private Button _btnSell;
        private Button[] _shortcutButtons;
        private decimal[] _shortcutIncrements;
        private int _selectedShortcutIndex;
        private bool _shortcutSelectionPrimed = true;

        public BidTurnAction Action { get; private set; }
        public decimal BidAmount { get; private set; }

        public BidTurnForm(
            string teamName,
            decimal currentPrice,
            string lastBidderTeam,
            int timeLimitSeconds,
            Color accentColor,
            decimal minimumBid,
            decimal availableFund)
        {
            _teamName = teamName;
            _currentPrice = currentPrice;
            _lastBidderTeam = lastBidderTeam;
            _timeLimitSeconds = timeLimitSeconds;
            _accentColor = accentColor;
            _minimumBid = minimumBid;
            _availableFund = availableFund;
            _remainingSeconds = timeLimitSeconds;

            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            ShowInTaskbar = false;
            TopMost = true;
            KeyPreview = true;
            BackColor = Color.FromArgb(18, 18, 24);
            ClientSize = new Size(500, 340);
            Text = $"{teamName} Bidding";

            BuildUi();
            KeyDown += BidTurnForm_KeyDown;

            _timer = new Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                Padding = new Padding(18),
                BackColor = Color.Transparent
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var header = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(26, 26, 36) };
            var headerLine = new Panel { Dock = DockStyle.Top, Height = 3, BackColor = _accentColor };
            header.Controls.Add(headerLine);

            var lblTitle = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"{_teamName} TURN",
                ForeColor = _accentColor,
                Font = new Font("Impact", 26F),
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(lblTitle);
            root.Controls.Add(header, 0, 0);

            var info = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                BackColor = Color.Transparent
            };
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));

            var lblCurrent = new Label
            {
                Dock = DockStyle.Fill,
                Text = $"Current Bid: BDT {_currentPrice:N0}\r\nMinimum Next: BDT {_minimumBid:N0}\r\nAvailable Fund: BDT {_availableFund:N0}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblTimer = new Label
            {
                Dock = DockStyle.Fill,
                Text = _remainingSeconds.ToString(),
                ForeColor = _accentColor,
                Font = new Font("Impact", 36F),
                TextAlign = ContentAlignment.MiddleCenter
            };
            info.Controls.Add(lblCurrent, 0, 0);
            info.Controls.Add(_lblTimer, 1, 0);
            root.Controls.Add(info, 0, 1);

            var bidPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };
            bidPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            bidPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _txtBid = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Center,
                Text = _minimumBid.ToString("N0")
            };
            _txtBid.KeyDown += TxtBid_KeyDown;
            _txtBid.TextChanged += (s, e) =>
            {
                if (_txtBid.Focused)
                {
                    _shortcutSelectionPrimed = false;
                    UpdateShortcutSelectionUi();
                }
            };

            var shortcuts = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 4
            };
            _shortcutIncrements = new decimal[] { 200, 300, 500, 1000 };
            _shortcutButtons = new Button[4];
            shortcuts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            shortcuts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            shortcuts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            shortcuts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            _shortcutButtons[0] = MakeShortcut("+200", 200);
            _shortcutButtons[1] = MakeShortcut("+300", 300);
            _shortcutButtons[2] = MakeShortcut("+500", 500);
            _shortcutButtons[3] = MakeShortcut("+1000", 1000);
            shortcuts.Controls.Add(_shortcutButtons[0], 0, 0);
            shortcuts.Controls.Add(_shortcutButtons[1], 1, 0);
            shortcuts.Controls.Add(_shortcutButtons[2], 2, 0);
            shortcuts.Controls.Add(_shortcutButtons[3], 3, 0);

            bidPanel.Controls.Add(_txtBid, 0, 0);
            bidPanel.Controls.Add(shortcuts, 0, 1);
            root.Controls.Add(bidPanel, 0, 2);

            var actionRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2
            };
            actionRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            actionRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));

            var btnPlaceBid = new Button
            {
                Dock = DockStyle.Fill,
                Text = "PLACE BID",
                BackColor = _accentColor,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnPlaceBid.FlatAppearance.BorderSize = 0;
            btnPlaceBid.Click += (s, e) => SubmitBid();

            _btnSell = new Button
            {
                Dock = DockStyle.Fill,
                Text = string.IsNullOrEmpty(_lastBidderTeam) ? "WAITING FOR FIRST BID" : $"SELL TO {_lastBidderTeam}",
                BackColor = Color.FromArgb(75, 50, 12),
                ForeColor = Color.Gold,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Enabled = !string.IsNullOrEmpty(_lastBidderTeam)
            };
            _btnSell.FlatAppearance.BorderSize = 0;
            _btnSell.Click += (s, e) =>
            {
                Action = BidTurnAction.SellToLastBidder;
                DialogResult = DialogResult.OK;
                Close();
            };

            actionRow.Controls.Add(btnPlaceBid, 0, 0);
            actionRow.Controls.Add(_btnSell, 1, 0);
            root.Controls.Add(actionRow, 0, 3);

            var lblHint = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Press Enter to submit this team's bid. If this team is late, timer expiry auto-sells to the previous bidder.",
                ForeColor = Color.FromArgb(200, 200, 210),
                Font = new Font("Segoe UI", 9.5F),
                TextAlign = ContentAlignment.TopLeft
            };
            root.Controls.Add(lblHint, 0, 4);

            Shown += (s, e) =>
            {
                _txtBid.Focus();
                _txtBid.SelectAll();
                UpdateShortcutSelectionUi();
            };
        }

        private Button MakeShortcut(string text, decimal increment)
        {
            var button = new Button
            {
                Dock = DockStyle.Fill,
                Text = text,
                BackColor = Color.FromArgb(34, 34, 46),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3)
            };
            button.FlatAppearance.BorderColor = _accentColor;
            button.FlatAppearance.BorderSize = 1;
            button.Click += (s, e) =>
            {
                for (int i = 0; i < _shortcutButtons.Length; i++)
                {
                    if (_shortcutButtons[i] != button) continue;
                    _selectedShortcutIndex = i;
                    break;
                }

                ApplySelectedShortcut();
            };
            return button;
        }

        private void BidTurnForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                MoveShortcutSelection(-1);
                return;
            }

            if (e.KeyCode == Keys.Right)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                MoveShortcutSelection(1);
                return;
            }

            if (e.KeyCode != Keys.Enter) return;

            if (_shortcutSelectionPrimed)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                ApplySelectedShortcut();
            }
        }

        private void MoveShortcutSelection(int direction)
        {
            _selectedShortcutIndex = (_selectedShortcutIndex + direction + _shortcutButtons.Length) % _shortcutButtons.Length;
            _shortcutSelectionPrimed = true;
            UpdateShortcutSelectionUi();
        }

        private void ApplySelectedShortcut()
        {
            _txtBid.Text = (_currentPrice + _shortcutIncrements[_selectedShortcutIndex]).ToString("N0");
            _txtBid.Focus();
            _txtBid.SelectAll();
            _shortcutSelectionPrimed = false;
            UpdateShortcutSelectionUi();
        }

        private void UpdateShortcutSelectionUi()
        {
            if (_shortcutButtons == null) return;

            for (int i = 0; i < _shortcutButtons.Length; i++)
            {
                bool isSelected = i == _selectedShortcutIndex;
                _shortcutButtons[i].BackColor = isSelected ? _accentColor : Color.FromArgb(34, 34, 46);
                _shortcutButtons[i].ForeColor = isSelected ? Color.Black : Color.White;
            }
        }

        private void TxtBid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.SuppressKeyPress = true;
            e.Handled = true;
            SubmitBid();
        }

        private void SubmitBid()
        {
            var raw = _txtBid.Text.Replace(",", "").Trim();
            if (!decimal.TryParse(raw, out var amount))
            {
                MessageBox.Show("Please enter a valid bid amount.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtBid.Focus();
                _txtBid.SelectAll();
                return;
            }

            if (amount < _minimumBid)
            {
                MessageBox.Show($"Bid must be at least BDT {_minimumBid:N0}.", "Bid Too Low",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtBid.Focus();
                _txtBid.SelectAll();
                return;
            }

            if (amount > _availableFund)
            {
                MessageBox.Show($"This team only has BDT {_availableFund:N0} available.", "Insufficient Funds",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtBid.Focus();
                _txtBid.SelectAll();
                return;
            }

            BidAmount = amount;
            Action = BidTurnAction.PlaceBid;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _remainingSeconds--;
            _lblTimer.Text = _remainingSeconds.ToString();
            _lblTimer.ForeColor = _remainingSeconds <= 5 ? Color.FromArgb(220, 55, 55) : _accentColor;

            if (_remainingSeconds > 0) return;

            Action = BidTurnAction.Timeout;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
