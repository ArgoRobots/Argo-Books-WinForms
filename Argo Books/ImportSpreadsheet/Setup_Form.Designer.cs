namespace Argo_Books.ImportSpreadsheet
{
    partial class Setup_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setup_Form));
            FormBack_Panel = new Guna.UI2.WinForms.Guna2Panel();
            SuspendLayout();
            // 
            // FormBack_Panel
            // 
            FormBack_Panel.BackColor = SystemColors.ButtonFace;
            FormBack_Panel.BorderColor = Color.Black;
            FormBack_Panel.CustomizableEdges = customizableEdges1;
            FormBack_Panel.Dock = DockStyle.Fill;
            FormBack_Panel.Location = new Point(0, 0);
            FormBack_Panel.Margin = new Padding(0);
            FormBack_Panel.Name = "FormBack_Panel";
            FormBack_Panel.ShadowDecoration.CustomizableEdges = customizableEdges2;
            FormBack_Panel.Size = new Size(1128, 794);
            FormBack_Panel.TabIndex = 177;
            // 
            // Setup_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1128, 794);
            Controls.Add(FormBack_Panel);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 5, 6, 5);
            MinimumSize = new Size(1100, 650);
            Name = "Setup_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Import spreadsheet";
            ResumeLayout(false);
        }

        #endregion

        public Guna.UI2.WinForms.Guna2Panel FormBack_Panel;
    }
}