using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PRSC_Player_Auction_System
{
    public partial class AddPlayerForm : Form
    {
        public Player Result { get; private set; }
        private readonly Player _existing;
        private readonly List<string> _teamNames;

        // ═══════════════════════════════════════════════════════════════
        //  CONSTRUCTOR
        // ═══════════════════════════════════════════════════════════════
        public AddPlayerForm(Player existing = null, List<string> teamNames = null)
        {
            _existing = existing;
            _teamNames = teamNames;
            InitializeComponent();

            if (existing != null)
                this.Text = "Edit Player";
        }

        // ═══════════════════════════════════════════════════════════════
        //  FORM LOAD
        // ═══════════════════════════════════════════════════════════════
        private void AddPlayerForm_Load(object sender, EventArgs e)
        {
            // ── Populate team dropdown dynamically from MainForm ──
            cmbTeam.Items.Clear();
            cmbTeam.Items.Add("");  // blank / unassigned option
            if (_teamNames != null)
                foreach (var t in _teamNames)
                    if (!string.IsNullOrWhiteSpace(t))
                        cmbTeam.Items.Add(t);

            if (_existing == null) return;

            // Pre-fill fields when editing
            txtName.Text = _existing.Name;
            txtPosition.Text = _existing.Position;
            cmbSkill.Text = _existing.SkillLevel;
            txtBasePrice.Text = _existing.BasePrice > 0
                                      ? _existing.BasePrice.ToString("N0") : "";
            txtSoldPrice.Text = _existing.SoldPrice > 0
                                      ? _existing.SoldPrice.ToString("N0") : "";
            cmbStatus.Text = _existing.Status;
            txtVideoPath.Text = _existing.VideoPath ?? "";

            // Set team AFTER items are populated so it matches correctly
            cmbTeam.Text = _existing.AssignedTeam ?? "";
        }

        // ═══════════════════════════════════════════════════════════════
        //  SAVE
        // ═══════════════════════════════════════════════════════════════
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Player name is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            Result = new Player
            {
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
        }

        // ═══════════════════════════════════════════════════════════════
        //  BROWSE VIDEO
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