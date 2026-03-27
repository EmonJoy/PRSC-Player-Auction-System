using System;
using System.Drawing;
using System.Windows.Forms;

namespace PRSC_Player_Auction_System
{
    /// <summary>
    /// Add / Edit player dialog.
    /// Class is partial — layout lives in AddPlayerForm.Designer.cs
    /// </summary>
    public partial class AddPlayerForm : Form
    {
        // ── Public result (set after user clicks Save) ──────────────────
        public Player Result { get; private set; }

        // ── Optional: player being edited ──────────────────────────────
        private readonly Player _existing;

        // ═══════════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public AddPlayerForm(Player existing = null)
        {
            _existing = existing;
            InitializeComponent();   // wired to Designer.cs

            if (existing != null)
                this.Text = "Edit Player";
        }

        // ═══════════════════════════════════════════════════════════════
        //  FORM LOAD  ← required by Designer's Load event hook
        // ═══════════════════════════════════════════════════════════════
        private void AddPlayerForm_Load(object sender, EventArgs e)
        {
            if (_existing == null) return;

            // Pre-fill fields when editing an existing player
            txtName.Text = _existing.Name;
            txtPosition.Text = _existing.Position;
            cmbSkill.Text = _existing.SkillLevel;
            txtBasePrice.Text = _existing.BasePrice > 0
                                    ? _existing.BasePrice.ToString("N0") : "";
            txtSoldPrice.Text = _existing.SoldPrice > 0
                                    ? _existing.SoldPrice.ToString("N0") : "";
            cmbTeam.Text = _existing.AssignedTeam;
            cmbStatus.Text = _existing.Status;
            txtVideoPath.Text = _existing.VideoPath ?? "";
        }

        // ═══════════════════════════════════════════════════════════════
        //  SAVE  ← required by Designer's btnSave.Click event hook
        // ═══════════════════════════════════════════════════════════════
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Player name is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;   // keep form open
                return;
            }

            Result = new Player
            {
                // Carry forward Id if editing (MainForm will set it for new players)
                Id = _existing?.Id ?? 0,
                Name = txtName.Text.Trim(),
                Position = txtPosition.Text.Trim(),
                SkillLevel = cmbSkill.Text,
                BasePrice = ParseMoney(txtBasePrice.Text),
                SoldPrice = ParseMoney(txtSoldPrice.Text),
                AssignedTeam = cmbTeam.Text,
                Status = cmbStatus.Text,
                VideoPath = txtVideoPath.Text.Trim()
            };
            // DialogResult.OK is already set on btnSave in Designer — form closes
        }

        // ═══════════════════════════════════════════════════════════════
        //  BROWSE VIDEO  ← required by Designer's btnBrowse.Click event hook
        // ═══════════════════════════════════════════════════════════════
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Video Files|*.mp4;*.avi;*.mkv;*.mov|All Files|*.*";
                ofd.Title = "Select Player Highlight Video";
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtVideoPath.Text = ofd.FileName;
            }
        }

        // ═══════════════════════════════════════════════════════════════
        //  HELPER
        // ═══════════════════════════════════════════════════════════════
        private static decimal ParseMoney(string s)
        {
            decimal.TryParse(s.Replace(",", "").Trim(), out decimal v);
            return v;
        }
    }
}