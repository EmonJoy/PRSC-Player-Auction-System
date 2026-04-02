namespace PRSC_Player_Auction_System
{
    partial class SplashForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();

                _animTimer?.Stop(); _animTimer?.Dispose();
                _exitTimer?.Stop(); _exitTimer?.Dispose();

                _fontImpact72?.Dispose();
                _fontSeg14?.Dispose();
                _fontSeg11Italic?.Dispose();
                _fontSeg11Regular?.Dispose();
                _fontSeg11Bold?.Dispose();
                _fontSeg9?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "SplashForm";
            this.Text = "PRSC Player Auction System";
            this.Load += new System.EventHandler(this.SplashForm_Load);

            this.ResumeLayout(false);
        }

        #endregion
    }
}