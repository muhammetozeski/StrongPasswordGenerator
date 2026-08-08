namespace StrongPasswordGenerator;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(640, 680);
        this.Name = "MainForm";
        this.Text = "Cryptographic Password Generator";
        this.ResumeLayout(false);
    }

    #endregion
}
