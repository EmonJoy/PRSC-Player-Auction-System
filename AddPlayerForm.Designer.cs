namespace PRSC_Player_Auction_System
{
    partial class AddPlayerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPosition = new System.Windows.Forms.Label();
            this.txtPosition = new System.Windows.Forms.TextBox();
            this.lblSkill = new System.Windows.Forms.Label();
            this.cmbSkill = new System.Windows.Forms.ComboBox();
            this.lblBasePrice = new System.Windows.Forms.Label();
            this.txtBasePrice = new System.Windows.Forms.TextBox();
            this.lblSoldPrice = new System.Windows.Forms.Label();
            this.txtSoldPrice = new System.Windows.Forms.TextBox();
            this.lblTeam = new System.Windows.Forms.Label();
            this.cmbTeam = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblVideoPath = new System.Windows.Forms.Label();
            this.txtVideoPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // ── Header label (title banner) ──────────────────────────────
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Height = 48;
            this.lblHeader.Text = "ADD / EDIT PLAYER";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHeader.Font = new System.Drawing.Font("Impact", 16F);
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(50, 205, 50);
            this.lblHeader.BackColor = System.Drawing.Color.FromArgb(8, 8, 8);

            // ── lblName ──────────────────────────────────────────────────
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 62);
            this.lblName.Text = "Player Name *";
            this.lblName.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── txtName ──────────────────────────────────────────────────
            this.txtName.Location = new System.Drawing.Point(170, 58);
            this.txtName.Size = new System.Drawing.Size(270, 26);
            this.txtName.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ── lblPosition ──────────────────────────────────────────────
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(20, 104);
            this.lblPosition.Text = "Position";
            this.lblPosition.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── txtPosition ──────────────────────────────────────────────
            this.txtPosition.Location = new System.Drawing.Point(170, 100);
            this.txtPosition.Size = new System.Drawing.Size(270, 26);
            this.txtPosition.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtPosition.ForeColor = System.Drawing.Color.White;
            this.txtPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ── lblSkill ─────────────────────────────────────────────────
            this.lblSkill.AutoSize = true;
            this.lblSkill.Location = new System.Drawing.Point(20, 146);
            this.lblSkill.Text = "Skill Level";
            this.lblSkill.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── cmbSkill ─────────────────────────────────────────────────
            this.cmbSkill.Location = new System.Drawing.Point(170, 142);
            this.cmbSkill.Size = new System.Drawing.Size(270, 26);
            this.cmbSkill.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.cmbSkill.ForeColor = System.Drawing.Color.White;
            this.cmbSkill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSkill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSkill.Items.AddRange(new object[] { "Low", "Medium", "High", "Elite" });
            this.cmbSkill.SelectedIndex = 1;

            // ── lblBasePrice ─────────────────────────────────────────────
            this.lblBasePrice.AutoSize = true;
            this.lblBasePrice.Location = new System.Drawing.Point(20, 188);
            this.lblBasePrice.Text = "Base Price (৳)";
            this.lblBasePrice.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── txtBasePrice ─────────────────────────────────────────────
            this.txtBasePrice.Location = new System.Drawing.Point(170, 184);
            this.txtBasePrice.Size = new System.Drawing.Size(270, 26);
            this.txtBasePrice.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtBasePrice.ForeColor = System.Drawing.Color.White;
            this.txtBasePrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBasePrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ── lblSoldPrice ─────────────────────────────────────────────
            this.lblSoldPrice.AutoSize = true;
            this.lblSoldPrice.Location = new System.Drawing.Point(20, 230);
            this.lblSoldPrice.Text = "Sold Price (৳)";
            this.lblSoldPrice.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── txtSoldPrice ─────────────────────────────────────────────
            this.txtSoldPrice.Location = new System.Drawing.Point(170, 226);
            this.txtSoldPrice.Size = new System.Drawing.Size(270, 26);
            this.txtSoldPrice.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtSoldPrice.ForeColor = System.Drawing.Color.White;
            this.txtSoldPrice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSoldPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ── lblTeam ──────────────────────────────────────────────────
            this.lblTeam.AutoSize = true;
            this.lblTeam.Location = new System.Drawing.Point(20, 272);
            this.lblTeam.Text = "Assigned Team";
            this.lblTeam.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── cmbTeam ──────────────────────────────────────────────────
            this.cmbTeam.Location = new System.Drawing.Point(170, 268);
            this.cmbTeam.Size = new System.Drawing.Size(270, 26);
            this.cmbTeam.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.cmbTeam.ForeColor = System.Drawing.Color.White;
            this.cmbTeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTeam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbTeam.Items.AddRange(new object[] { "—", "Team Alpha", "Team Beta" });
            this.cmbTeam.SelectedIndex = 0;

            // ── lblStatus ────────────────────────────────────────────────
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 314);
            this.lblStatus.Text = "Status";
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── cmbStatus ────────────────────────────────────────────────
            this.cmbStatus.Location = new System.Drawing.Point(170, 310);
            this.cmbStatus.Size = new System.Drawing.Size(270, 26);
            this.cmbStatus.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.cmbStatus.ForeColor = System.Drawing.Color.White;
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStatus.Items.AddRange(new object[] { "Available", "Sold" });
            this.cmbStatus.SelectedIndex = 0;

            // ── lblVideoPath ─────────────────────────────────────────────
            this.lblVideoPath.AutoSize = true;
            this.lblVideoPath.Location = new System.Drawing.Point(20, 356);
            this.lblVideoPath.Text = "Video Path";
            this.lblVideoPath.ForeColor = System.Drawing.Color.FromArgb(170, 170, 170);

            // ── txtVideoPath ─────────────────────────────────────────────
            this.txtVideoPath.Location = new System.Drawing.Point(170, 352);
            this.txtVideoPath.Size = new System.Drawing.Size(228, 26);
            this.txtVideoPath.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtVideoPath.ForeColor = System.Drawing.Color.White;
            this.txtVideoPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ── btnBrowse ────────────────────────────────────────────────
            this.btnBrowse.Location = new System.Drawing.Point(406, 350);
            this.btnBrowse.Size = new System.Drawing.Size(34, 28);
            this.btnBrowse.Text = "…";
            this.btnBrowse.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this.btnBrowse.ForeColor = System.Drawing.Color.White;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);

            // ── btnSave ──────────────────────────────────────────────────
            this.btnSave.Location = new System.Drawing.Point(170, 400);
            this.btnSave.Size = new System.Drawing.Size(125, 38);
            this.btnSave.Text = "✔  Save";
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // ── btnCancel ────────────────────────────────────────────────
            this.btnCancel.Location = new System.Drawing.Point(305, 400);
            this.btnCancel.Size = new System.Drawing.Size(125, 38);
            this.btnCancel.Text = "✖  Cancel";
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(120, 30, 30);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            // ── Form ─────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 455);
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 18);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add / Edit Player";
            this.Font = new System.Drawing.Font("Segoe UI", 10F);

            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.txtPosition);
            this.Controls.Add(this.lblSkill);
            this.Controls.Add(this.cmbSkill);
            this.Controls.Add(this.lblBasePrice);
            this.Controls.Add(this.txtBasePrice);
            this.Controls.Add(this.lblSoldPrice);
            this.Controls.Add(this.txtSoldPrice);
            this.Controls.Add(this.lblTeam);
            this.Controls.Add(this.cmbTeam);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.lblVideoPath);
            this.Controls.Add(this.txtVideoPath);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);

            this.Load += new System.EventHandler(this.AddPlayerForm_Load);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ── Control declarations ──────────────────────────────────────
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.TextBox txtPosition;
        private System.Windows.Forms.Label lblSkill;
        private System.Windows.Forms.ComboBox cmbSkill;
        private System.Windows.Forms.Label lblBasePrice;
        private System.Windows.Forms.TextBox txtBasePrice;
        private System.Windows.Forms.Label lblSoldPrice;
        private System.Windows.Forms.TextBox txtSoldPrice;
        private System.Windows.Forms.Label lblTeam;
        private System.Windows.Forms.ComboBox cmbTeam;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblVideoPath;
        private System.Windows.Forms.TextBox txtVideoPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}