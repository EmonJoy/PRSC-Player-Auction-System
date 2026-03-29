using System;
using System.Drawing;
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

        // ── UI ──────────────────────────────────────────────────────────
        private Panel _pnlBidding;
        private Label _lblPlayerName;
        private Label _lblPosition;
        private Label _lblCurrentPrice;
        private Label _lblTeamAFund;
        private Label _lblTeamBFund;
        private Button _btnPlus1000;
        private Button _btnMinus1000;
        private Button _btnPlus500;
        private Button _btnMinus500;
        private Button _btnPlus200;
        private Button _btnMinus200;
        private Button _btnPlus300;
        private Button _btnMinus300;
        private Button _btnSellToA;
        private Button _btnSellToB;
        private Button _btnClose;
        private Button _btnSkipVideo;
        private TextBox _txtCustomBid;
        private Button _btnCustomBid;

        // fields
        private bool _vlcCleaned = false;
        private bool _isClosing = false;

        // ═══════════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public FullScreenAuctionForm(Player player, MainForm mainForm)
        {
            _player = player;
            _mainForm = mainForm;

            // Reset current price to base price for this auction round
            _player.CurrentPrice = _player.BasePrice;

            // ── Form setup ──────────────────────────────────────────────
            this.BackColor = Color.Black;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) SafeClose(); };

            // ── VLC init ────────────────────────────────────────────────
            try
            {
                Core.Initialize();
                _libVLC = new LibVLC();
                _mediaPlayer = new MediaPlayer(_libVLC);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("VLC Init Error: " + ex.Message);
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
                _videoView = new VideoView
                {
                    MediaPlayer = _mediaPlayer,
                    Dock = DockStyle.Fill
                };
                this.Controls.Add(_videoView);
            }

            // ── Bidding panel (hidden until video ends / skipped) ───────
            _pnlBidding = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Visible = false
            };
            this.Controls.Add(_pnlBidding);

            // ── Main layout: 5 rows ─────────────────────────────────────
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            _pnlBidding.Controls.Add(mainLayout);

            // ── Row 0: Player info ──────────────────────────────────────
            var pnlInfo = new Panel { Dock = DockStyle.Fill };

            _lblPlayerName = new Label
            {
                Dock = DockStyle.Top,
                Height = 60,
                ForeColor = Color.Gold,
                Font = new Font("Impact", 44F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = _player.Name
            };

            _lblPosition = new Label
            {
                Dock = DockStyle.Top,
                Height = 28,
                ForeColor = Color.FromArgb(180, 180, 180),
                Font = new Font("Segoe UI", 14F),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = $"{_player.Position}  •  {_player.SkillLevel}"
            };

            pnlInfo.Controls.Add(_lblPosition);
            pnlInfo.Controls.Add(_lblPlayerName);
            mainLayout.Controls.Add(pnlInfo, 0, 0);

            // ── Row 1: Current price ────────────────────────────────────
            _lblCurrentPrice = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.Lime,
                Font = new Font("Impact", 70F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainLayout.Controls.Add(_lblCurrentPrice, 0, 1);

            // ── Row 2: Bid buttons (8 buttons) ──────────────────────────
            var btnLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 1,
                Padding = new Padding(10)
            };
            for (int i = 0; i < 8; i++)
                btnLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 8));

            _btnPlus1000 = MakeBtn("+1,000", Color.FromArgb(60, 120, 60), 18);
            _btnMinus1000 = MakeBtn("−1,000", Color.FromArgb(120, 60, 60), 18);
            _btnPlus500 = MakeBtn("+500", Color.FromArgb(50, 100, 50), 16);
            _btnMinus500 = MakeBtn("−500", Color.FromArgb(100, 50, 50), 16);
            _btnPlus300 = MakeBtn("+300", Color.FromArgb(40, 90, 40), 14);
            _btnMinus300 = MakeBtn("−300", Color.FromArgb(90, 40, 40), 14);
            _btnPlus200 = MakeBtn("+200", Color.FromArgb(35, 80, 35), 14);
            _btnMinus200 = MakeBtn("−200", Color.FromArgb(80, 35, 35), 14);

            btnLayout.Controls.Add(_btnPlus1000, 0, 0);
            btnLayout.Controls.Add(_btnMinus1000, 1, 0);
            btnLayout.Controls.Add(_btnPlus500, 2, 0);
            btnLayout.Controls.Add(_btnMinus500, 3, 0);
            btnLayout.Controls.Add(_btnPlus300, 4, 0);
            btnLayout.Controls.Add(_btnMinus300, 5, 0);
            btnLayout.Controls.Add(_btnPlus200, 6, 0);
            btnLayout.Controls.Add(_btnMinus200, 7, 0);

            mainLayout.Controls.Add(btnLayout, 0, 2);

            // ── Row 3: Sell buttons + Custom bid ─────────────────────────
            var sellCustomLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                Padding = new Padding(10)
            };
            sellCustomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            sellCustomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            sellCustomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            sellCustomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            sellCustomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

            _btnSellToA = MakeBtn($"SELL → {_mainForm.TeamAName}", Color.FromArgb(0, 140, 0), 16);
            _btnSellToB = MakeBtn($"SELL → {_mainForm.TeamBName}", Color.FromArgb(0, 20, 180), 16);

            _txtCustomBid = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F),
                TextAlign = HorizontalAlignment.Center,
                Text = "Amount"
            };
            _txtCustomBid.GotFocus += (s, e) =>
            {
                if (_txtCustomBid.Text == "Amount")
                {
                    _txtCustomBid.Text = string.Empty;
                    _txtCustomBid.ForeColor = Color.White;
                }
            };
            _txtCustomBid.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_txtCustomBid.Text))
                {
                    _txtCustomBid.Text = "Amount";
                    _txtCustomBid.ForeColor = Color.FromArgb(100, 100, 100);
                }
            };
            _txtCustomBid.ForeColor = Color.FromArgb(100, 100, 100);

            _btnCustomBid = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Custom Bid",
                BackColor = Color.FromArgb(100, 100, 40),
                ForeColor = Color.White,
                Font = new Font("Impact", 12F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnCustomBid.FlatAppearance.BorderSize = 0;

            sellCustomLayout.Controls.Add(_btnSellToA, 0, 0);
            sellCustomLayout.Controls.Add(_btnSellToB, 1, 0);
            sellCustomLayout.Controls.Add(new Label(), 2, 0);
            sellCustomLayout.Controls.Add(_txtCustomBid, 3, 0);
            sellCustomLayout.Controls.Add(_btnCustomBid, 4, 0);

            mainLayout.Controls.Add(sellCustomLayout, 0, 3);

            // ── Row 4: Fund display ─────────────────────────────────────
            var fundLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(20, 5, 20, 5)
            };
            fundLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            fundLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            _lblTeamAFund = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(120, 255, 120),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblTeamBFund = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(120, 200, 255),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            fundLayout.Controls.Add(_lblTeamAFund, 0, 0);
            fundLayout.Controls.Add(_lblTeamBFund, 1, 0);
            mainLayout.Controls.Add(fundLayout, 0, 4);

            // ── Overlay buttons (Close + Skip) ──────────────────────────
            _btnClose = new Button
            {
                Text = "✕",
                BackColor = Color.FromArgb(180, 20, 20),
                ForeColor = Color.White,
                Size = new Size(50, 35),
                Location = new Point(15, 15),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _btnClose.FlatAppearance.BorderSize = 0;

            _btnSkipVideo = new Button
            {
                Text = "⏭  Skip Video",
                BackColor = Color.FromArgb(80, 80, 80),
                ForeColor = Color.White,
                Size = new Size(150, 35),
                Location = new Point(75, 15),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Cursor = Cursors.Hand
            };
            _btnSkipVideo.FlatAppearance.BorderSize = 0;

            this.Controls.Add(_btnClose);
            this.Controls.Add(_btnSkipVideo);
            _btnClose.BringToFront();
            _btnSkipVideo.BringToFront();

            // ── Wire events ─────────────────────────────────────────────
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
        //  VIDEO
        // ═══════════════════════════════════════════════════════════════
        private void StartVideo()
        {
            try
            {
                if (_mediaPlayer != null && !string.IsNullOrEmpty(_player.VideoPath) && File.Exists(_player.VideoPath))
                {
                    var media = new Media(_libVLC, _player.VideoPath, FromType.FromPath);
                    _mediaPlayer.Play(media);

                    // Use timer to check if video has ended instead of relying on events
                    _videoCheckTimer = new Timer();
                    _videoCheckTimer.Interval = 500;  // Check every 500ms
                    _videoCheckTimer.Tick += VideoCheckTimer_Tick;
                    _videoCheckTimer.Start();
                }
                else
                {
                    ShowBiddingPanel();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("StartVideo Error: " + ex.Message);
                ShowBiddingPanel();
            }
        }

        private void VideoCheckTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_isClosing || _biddingShown)
                {
                    _videoCheckTimer?.Stop();
                    return;
                }

                // Check if video is still playing
                if (_mediaPlayer != null && !_mediaPlayer.IsPlaying)
                {
                    _videoCheckTimer?.Stop();
                    ShowBiddingPanel();
                }
            }
            catch
            {
                // Silently ignore errors
            }
        }

        private void ShowBiddingPanel()
        {
            if (_biddingShown) return;
            _biddingShown = true;

            try
            {
                // Stop timer
                if (_videoCheckTimer != null)
                {
                    _videoCheckTimer.Stop();
                    _videoCheckTimer.Dispose();
                    _videoCheckTimer = null;
                }

                // Stop video playback
                if (_mediaPlayer != null)
                {
                    try { _mediaPlayer.Stop(); } catch { }
                }

                // Hide video view
                if (_videoView != null)
                {
                    _videoView.Visible = false;
                }

                // Hide skip button
                _btnSkipVideo.Visible = false;

                // Show bidding panel
                _pnlBidding.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ShowBiddingPanel Error: " + ex.Message);
            }
        }

        private void CleanupVlc()
        {
            if (_vlcCleaned) return;
            _vlcCleaned = true;

            try
            {
                // Stop and dispose timer
                if (_videoCheckTimer != null)
                {
                    try
                    {
                        _videoCheckTimer.Stop();
                        _videoCheckTimer.Dispose();
                    }
                    catch { }
                    _videoCheckTimer = null;
                }

                // Stop media player
                if (_mediaPlayer != null)
                {
                    try
                    {
                        _mediaPlayer.Stop();
                    }
                    catch { }
                }

                // Dispose video view
                if (_videoView != null)
                {
                    try
                    {
                        _videoView.MediaPlayer = null;
                        _videoView.Dispose();
                    }
                    catch { }
                    _videoView = null;
                }

                // Dispose media player
                if (_mediaPlayer != null)
                {
                    try
                    {
                        _mediaPlayer.Dispose();
                    }
                    catch { }
                    _mediaPlayer = null;
                }

                // Dispose LibVLC
                if (_libVLC != null)
                {
                    try
                    {
                        _libVLC.Dispose();
                    }
                    catch { }
                    _libVLC = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CleanupVlc Error: " + ex.Message);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  SELL EVENTS
        // ═══════════════════════════════════════════════════════════════
        private void BtnSellToA_Click(object sender, EventArgs e)
        {
            SellTo(_mainForm.TeamAName,
                   _mainForm.TeamAFund,
                   f => _mainForm.TeamAFund = f);
        }

        private void BtnSellToB_Click(object sender, EventArgs e)
        {
            SellTo(_mainForm.TeamBName,
                   _mainForm.TeamBFund,
                   f => _mainForm.TeamBFund = f);
        }

        private void SellTo(string teamName, decimal fund, Action<decimal> setFund)
        {
            if (fund < _player.CurrentPrice)
            {
                MessageBox.Show(
                    $"⚠ {teamName} does not have enough funds!\n\n" +
                    $"Required : ৳ {_player.CurrentPrice:N0}\n" +
                    $"Available: ৳ {fund:N0}",
                    "Insufficient Funds",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _player.AssignedTeam = teamName;
            _player.IsSold = true;
            _player.SoldPrice = _player.CurrentPrice;

            setFund(fund - _player.CurrentPrice);

            try
            {
                DatabaseHelper.AssignPlayerToTeam(_player.Id, teamName, _player.SoldPrice);
            }
            catch { }

            MessageBox.Show(
                $"✅  {_player.Name} sold to {teamName} for ৳ {_player.SoldPrice:N0}",
                "Sold!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            SafeClose();
        }

        // ═══════════════════════════════════════════════════════════════
        //  CUSTOM BID
        // ═══════════════════════════════════════════════════════════════
        private void BtnCustomBid_Click(object sender, EventArgs e)
        {
            string bidText = _txtCustomBid.Text.Trim();

            if (string.IsNullOrWhiteSpace(bidText) || bidText == "Amount")
            {
                MessageBox.Show("Please enter a custom bid amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(bidText, out decimal customAmount))
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCustomBid.Text = "Amount";
                _txtCustomBid.ForeColor = Color.FromArgb(100, 100, 100);
                return;
            }           

            if (customAmount < _player.BasePrice)
            {
                MessageBox.Show($"Bid must be at least ৳ {_player.BasePrice:N0}", "Bid Too Low", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _player.CurrentPrice = customAmount;
            UpdatePriceDisplay();
            _txtCustomBid.Text = "Amount";
            _txtCustomBid.ForeColor = Color.FromArgb(100, 100, 100);
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════════════════════════════
        private void ChangePrice(decimal delta)
        {
            decimal next = _player.CurrentPrice + delta;
            if (next < _player.BasePrice) return;
            _player.CurrentPrice = next;
            UpdatePriceDisplay();
        }

        private void UpdatePriceDisplay()
        {
            _lblCurrentPrice.Text = $"৳ {_player.CurrentPrice:N0}";
        }

        private void UpdateFundDisplay()
        {
            _lblTeamAFund.Text = $"🟢 {_mainForm.TeamAName}  ৳ {_mainForm.TeamAFund:N0}";
            _lblTeamBFund.Text = $"৳ {_mainForm.TeamBFund:N0}  {_mainForm.TeamBName} 🔵";
        }

        private void SafeClose()
        {
            _isClosing = true;
            CleanupVlc();
            try
            {
                this.Close();
            }
            catch { }
        }

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

        // ── Dispose VLC on form close ───────────────────────────────────
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _isClosing = true;
            CleanupVlc();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isClosing = true;
                CleanupVlc();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FullScreenAuctionForm
            // 
            this.ClientSize = new System.Drawing.Size(921, 621);
            this.Name = "FullScreenAuctionForm";
            this.Load += new System.EventHandler(this.FullScreenAuctionForm_Load);
            this.ResumeLayout(false);

        }

        private void FullScreenAuctionForm_Load(object sender, EventArgs e)
        {

        }
    }
}