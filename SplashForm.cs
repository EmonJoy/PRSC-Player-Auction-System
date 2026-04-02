using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PRSC_Player_Auction_System
{
    public partial class SplashForm : Form
    {
        // ── Timers ──────────────────────────────────────────────────────
        private Timer _animTimer;
        private Timer _exitTimer;

        // ── Animation state ─────────────────────────────────────────────
        private float _tick = 0f;
        private float _glowAlpha = 0f;
        private bool _glowUp = true;
        private float _dividerW = 0f;
        private float _slideOffset = 0f;
        private bool _exiting = false;

        // ── Floating balls ──────────────────────────────────────────────
        private struct Ball
        {
            public PointF Pos, Vel;
            public float Radius, Phase, Speed;
        }
        private readonly List<Ball> _balls = new List<Ball>();

        // ── Stars ───────────────────────────────────────────────────────
        private struct Star
        {
            public PointF P;
            public float R, Phase;
        }
        private readonly List<Star> _stars = new List<Star>();

        // ── Cached fonts (created once, disposed on form close) ─────────
        // BUG FIX: Previously fonts were disposed inside DrawCentredText
        // via "using (font)" which caused crashes when the same font
        // object was passed in on the next paint call.
        private Font _fontImpact72;
        private Font _fontSeg14;
        private Font _fontSeg11Italic;
        private Font _fontSeg11Regular;
        private Font _fontSeg11Bold;
        private Font _fontSeg9;

        // ── Colors ──────────────────────────────────────────────────────
        private readonly Color _bg1 = Color.FromArgb(5, 5, 16);
        private readonly Color _bg2 = Color.FromArgb(10, 10, 32);
        private readonly Color _green = Color.FromArgb(50, 205, 50);
        private readonly Color _blue = Color.FromArgb(100, 149, 237);
        private readonly Color _gold = Color.FromArgb(255, 215, 0);
        private readonly Color _darkGreen = Color.FromArgb(10, 40, 10);

        // ── Swipe / drag tracking ───────────────────────────────────────
        private int _dragStartY = -1;
        private bool _dragging = false;

        public SplashForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(5, 5, 16);
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Cursor = Cursors.Hand;
            this.KeyPreview = true;

            // Create all fonts once here so they are never disposed mid-use
            _fontImpact72 = new Font("Impact", 72, FontStyle.Bold);
            _fontSeg14 = new Font("Segoe UI", 14, FontStyle.Regular);
            _fontSeg11Italic = new Font("Segoe UI", 11, FontStyle.Italic);
            _fontSeg11Regular = new Font("Segoe UI", 11, FontStyle.Regular);
            _fontSeg11Bold = new Font("Segoe UI", 11, FontStyle.Bold);
            _fontSeg9 = new Font("Segoe UI", 9, FontStyle.Regular);

            var rng = new Random();

            // Generate stars
            for (int i = 0; i < 120; i++)
            {
                _stars.Add(new Star
                {
                    P = new PointF(rng.Next(0, 1920), rng.Next(0, 800)),
                    R = (float)(rng.NextDouble() * 1.8 + 0.4),
                    Phase = (float)(rng.NextDouble() * Math.PI * 2)
                });
            }

            // Generate floating balls
            for (int i = 0; i < 6; i++)
            {
                // BUG FIX: velocity was so small balls barely moved;
                // increased range slightly for visible animation.
                _balls.Add(new Ball
                {
                    Pos = new PointF(rng.Next(80, 1840), rng.Next(60, 600)),
                    Vel = new PointF(
                                (float)(rng.NextDouble() * 1.0 - 0.5),
                                (float)(rng.NextDouble() * 1.0 - 0.5)),
                    Radius = rng.Next(14, 28),
                    Phase = (float)(rng.NextDouble() * Math.PI * 2),
                    Speed = (float)(rng.NextDouble() * 0.03 + 0.01)
                });
            }

            // Animation timer ~60fps
            _animTimer = new Timer { Interval = 16 };
            _animTimer.Tick += OnAnimTick;
            _animTimer.Start();

            // Auto-exit after 8s
            _exitTimer = new Timer { Interval = 8000 };
            _exitTimer.Tick += ExitTimer_Tick;
            _exitTimer.Start();

            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.KeyDown += SplashForm_KeyDown;
            this.Paint += OnPaint;
        }

        private void SplashForm_Load(object sender, EventArgs e) { }

        private void ExitTimer_Tick(object sender, EventArgs e)
        {
            _exitTimer.Stop();
            StartExit();
        }

        private void SplashForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_exiting) StartExit();
        }

        // ═══════════════════════════════════════════════════════════════
        //  ANIMATION TICK
        // ═══════════════════════════════════════════════════════════════
        private void OnAnimTick(object sender, EventArgs e)
        {
            _tick += 0.025f;

            // Glow pulse
            if (_glowUp)
            {
                _glowAlpha += 0.03f;
                if (_glowAlpha >= 1f) { _glowAlpha = 1f; _glowUp = false; }
            }
            else
            {
                _glowAlpha -= 0.03f;
                if (_glowAlpha <= 0f) { _glowAlpha = 0f; _glowUp = true; }
            }

            // Divider expands to full width
            if (_dividerW < 1f)
                _dividerW = Math.Min(1f, _dividerW + 0.02f);

            // BUG FIX: Ball physics — was computing new position twice
            // (once before reversing, once after), causing jitter and
            // balls getting stuck on edges.
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            for (int i = 0; i < _balls.Count; i++)
            {
                var b = _balls[i];
                var nv = b.Vel;

                float nx = b.Pos.X + nv.X;
                float ny = b.Pos.Y + nv.Y;

                // Reverse velocity AND clamp position to avoid tunnel-through
                if (nx - b.Radius < 0)
                {
                    nv.X = Math.Abs(nv.X);
                    nx = b.Radius;
                }
                else if (nx + b.Radius > w)
                {
                    nv.X = -Math.Abs(nv.X);
                    nx = w - b.Radius;
                }

                if (ny - b.Radius < 0)
                {
                    nv.Y = Math.Abs(nv.Y);
                    ny = b.Radius;
                }
                else if (ny + b.Radius > h)
                {
                    nv.Y = -Math.Abs(nv.Y);
                    ny = h - b.Radius;
                }

                _balls[i] = new Ball
                {
                    Pos = new PointF(nx, ny),
                    Vel = nv,
                    Radius = b.Radius,
                    Phase = b.Phase + b.Speed,
                    Speed = b.Speed
                };
            }

            // BUG FIX: Slide exit — increased step for snappier feel,
            // and use Ease-in curve so motion accelerates naturally.
            if (_exiting)
            {
                _slideOffset += 0.055f;
                if (_slideOffset >= 1f)
                {
                    _animTimer.Stop();
                    OpenMainForm();
                    return;
                }
            }

            // Only invalidate the form if it's still valid
            if (!this.IsDisposed && this.IsHandleCreated)
                this.Invalidate();
        }

        // ═══════════════════════════════════════════════════════════════
        //  PAINT
        // ═══════════════════════════════════════════════════════════════
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            // BUG FIX: Ease-in curve for slide so it accelerates, not linear
            float ease = _slideOffset * _slideOffset;
            int slideY = (int)(ease * h * 1.15f);

            // ── Background ───────────────────────────────────────────────
            using (var bgBrush = new LinearGradientBrush(
                new Rectangle(0, -slideY, w, h + slideY),
                _bg1, _bg2, LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(bgBrush, 0, -slideY, w, h + slideY);
            }

            // ── Stars ────────────────────────────────────────────────────
            foreach (var st in _stars)
            {
                float a = (float)(0.2 + 0.6 * (Math.Sin(st.Phase + _tick * 0.8) * 0.5 + 0.5));
                using (var sb = new SolidBrush(Color.FromArgb((int)(a * 220), 220, 220, 220)))
                    g.FillEllipse(sb, st.P.X, st.P.Y - slideY, st.R * 2, st.R * 2);
            }

            // ── Grass bar ────────────────────────────────────────────────
            using (var gb = new LinearGradientBrush(
                new Rectangle(0, h - 100 - slideY, w, 100),
                Color.Transparent, _darkGreen, LinearGradientMode.Vertical))
            {
                g.FillRectangle(gb, 0, h - 100 - slideY, w, 100);
            }

            // ── Floating footballs ───────────────────────────────────────
            foreach (var b in _balls)
                DrawFootball(g, b.Pos.X, b.Pos.Y - slideY, b.Radius,
                    (float)(b.Phase * 180.0 / Math.PI), 0.35f);

            // ── Corner decorations ───────────────────────────────────────
            int m = 20, cs = 60;
            using (var p = new Pen(Color.FromArgb(50, _green), 1.5f))
            {
                g.DrawLines(p, new[] {
                    new Point(m,         m + cs),
                    new Point(m,         m),
                    new Point(m + cs,    m)
                });
                g.DrawLines(p, new[] {
                    new Point(w - m - cs, m),
                    new Point(w - m,      m),
                    new Point(w - m,      m + cs)
                });
                g.DrawLines(p, new[] {
                    new Point(m,      h - m - cs - slideY),
                    new Point(m,      h - m      - slideY),
                    new Point(m + cs, h - m      - slideY)
                });
                g.DrawLines(p, new[] {
                    new Point(w - m - cs, h - m - slideY),
                    new Point(w - m,      h - m - slideY),
                    new Point(w - m,      h - m - cs - slideY)
                });
            }

            int cx = w / 2;
            int cy = h / 2 - slideY;

            // ── Outer ring pulse ─────────────────────────────────────────
            int ringR = 90;
            float glowA = 60 + _glowAlpha * 140;
            using (var rp = new Pen(Color.FromArgb((int)glowA, _green), 2.5f))
                g.DrawEllipse(rp, cx - ringR, cy - 230 - ringR, ringR * 2, ringR * 2);

            // ── Center football ──────────────────────────────────────────
            DrawFootball(g, cx, cy - 230, 52, _tick * 30f, 1.0f);

            // ── PRSC Title ───────────────────────────────────────────────
            {
                float gA = 0.5f + _glowAlpha * 0.5f;
                using (var brush = new SolidBrush(Color.FromArgb((int)(gA * 255), _green)))
                {
                    var sz = g.MeasureString("PRSC", _fontImpact72);
                    g.DrawString("PRSC", _fontImpact72, brush, cx - sz.Width / 2, cy - 140);
                }
            }

            // ── Subtitle ─────────────────────────────────────────────────
            DrawCentredText(g, "PLAYER AUCTION SYSTEM", _fontSeg14,
                Color.FromArgb(160, 200, 160), cx, cy - 52);

            // ── Tagline ──────────────────────────────────────────────────
            DrawCentredText(g, "Premier Sports Club", _fontSeg11Italic,
                Color.FromArgb(120, _blue), cx, cy - 24);

            // ── Divider line ─────────────────────────────────────────────
            int divMaxW = 260;
            int divW = (int)(divMaxW * _dividerW);
            using (var dp = new Pen(Color.FromArgb(180, _green), 1.5f))
                g.DrawLine(dp, cx - divW / 2, cy + 8, cx + divW / 2, cy + 8);

            // ── Developer credit ─────────────────────────────────────────
            {
                string pre = "Developed by  ";
                string dev = "Emon Joy";

                var s1 = g.MeasureString(pre, _fontSeg11Regular);
                var s2 = g.MeasureString(dev, _fontSeg11Bold);

                float tx = cx - (s1.Width + s2.Width) / 2;
                float ty = cy + 26;

                using (var b1 = new SolidBrush(Color.FromArgb(140, 200, 140)))
                    g.DrawString(pre, _fontSeg11Regular, b1, tx, ty);

                float gldA = 0.6f + _glowAlpha * 0.4f;
                using (var b2 = new SolidBrush(Color.FromArgb((int)(gldA * 255), _gold)))
                    g.DrawString(dev, _fontSeg11Bold, b2, tx + s1.Width, ty);
            }

            // ── Version / year ───────────────────────────────────────────
            DrawCentredText(g,
                "v1.0  |  " + DateTime.Now.Year, _fontSeg9,
                Color.FromArgb(60, 180, 160), cx, cy + 58);

            // ── Swipe hint ───────────────────────────────────────────────
            if (!_exiting)
            {
                float hintA = (float)(0.35 + 0.3 * Math.Sin(_tick * 2));
                float hintY = (float)(cy + 88 + Math.Sin(_tick * 2) * 5);

                DrawCentredText(g,
                    "▲   SWIPE UP OR CLICK TO ENTER", _fontSeg9,
                    Color.FromArgb((int)(hintA * 255), 200, 200, 200), cx, hintY);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  DRAW FOOTBALL
        // ═══════════════════════════════════════════════════════════════
        private void DrawFootball(Graphics g, float cx, float cy, float r,
                                  float rotDeg, float opacity)
        {
            int a = (int)(opacity * 255);

            // Shadow
            using (var sb = new SolidBrush(Color.FromArgb((int)(opacity * 40), 0, 0, 0)))
                g.FillEllipse(sb, cx - r + 3, cy - r + 3, r * 2, r * 2);

            // White body
            using (var wb = new SolidBrush(Color.FromArgb(a, 240, 240, 240)))
                g.FillEllipse(wb, cx - r, cy - r, r * 2, r * 2);

            GraphicsState state = g.Save();
            g.TranslateTransform(cx, cy);
            g.RotateTransform(rotDeg);

            float pr = r * 0.38f;
            PointF[] patchCentres =
            {
                new PointF(0,         0),
                new PointF(0,        -pr * 1.7f),
                new PointF( pr*1.5f,  pr * 0.85f),
                new PointF(-pr*1.5f,  pr * 0.85f),
                new PointF( pr*0.9f, -pr * 1.4f),
                new PointF(-pr*0.9f, -pr * 1.4f),
            };

            using (var pb = new SolidBrush(Color.FromArgb(a, 20, 20, 20)))
            {
                foreach (var pc in patchCentres)
                {
                    var pts = new PointF[6];
                    float hr = pr * 0.72f;
                    for (int i = 0; i < 6; i++)
                    {
                        double ang = i * Math.PI / 3 + Math.PI / 6;
                        pts[i] = new PointF(
                            pc.X + (float)(hr * Math.Cos(ang)),
                            pc.Y + (float)(hr * Math.Sin(ang)));
                    }
                    using (var gp = new GraphicsPath())
                    {
                        gp.AddEllipse(-r, -r, r * 2, r * 2);
                        g.SetClip(gp);
                        g.FillPolygon(pb, pts);
                        g.ResetClip();
                    }
                }
            }

            // Highlight
            using (var hl = new SolidBrush(Color.FromArgb((int)(opacity * 80), 255, 255, 255)))
                g.FillEllipse(hl, -r * 0.55f, -r * 0.6f, r * 0.5f, r * 0.35f);

            // Outline
            using (var bp = new Pen(Color.FromArgb((int)(opacity * 120), 80, 80, 80), 1f))
                g.DrawEllipse(bp, -r, -r, r * 2, r * 2);

            g.Restore(state);
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPER: centered text
        //  BUG FIX: Removed "using (font)" — fonts are owned by the form,
        //  not this helper. Disposing here was the primary crash cause.
        // ═══════════════════════════════════════════════════════════════
        private void DrawCentredText(Graphics g, string text, Font font,
                                     Color color, float cx, float cy)
        {
            using (var b = new SolidBrush(color))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString(text, font, b, cx, cy, sf);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  SWIPE / MOUSE
        // ═══════════════════════════════════════════════════════════════
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _dragStartY = e.Y;
            _dragging = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging || _exiting) return;
            if (_dragStartY - e.Y > 60)
                StartExit();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_dragging && !_exiting)
            {
                // Treat small drag (< 20px) as a tap/click
                if (Math.Abs(e.Y - _dragStartY) < 20)
                    StartExit();
            }
            _dragging = false;
        }

        // ═══════════════════════════════════════════════════════════════
        //  EXIT
        // ═══════════════════════════════════════════════════════════════
        private void StartExit()
        {
            if (_exiting) return;
            _exiting = true;
            _exitTimer?.Stop();
        }

        private void OpenMainForm()
        {
            if (this.IsDisposed) return;

            this.BeginInvoke(new Action(() =>
            {
                try
                {
                    MainForm main = new MainForm();
                    main.Show();
                    // Hide first — closing the splash directly exits the app
                    // if Program.cs used Application.Run(new SplashForm()).
                    // We close it only after MainForm itself closes.
                    this.Hide();
                    main.FormClosed += (s, e) => this.Close();
                }
                catch { }
            }));
        }

    }
}