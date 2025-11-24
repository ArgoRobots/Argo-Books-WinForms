namespace Argo_Books
{
    partial class Log_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Log_Form));
            Log_RichTextBox = new RichTextBox();
            Clear_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // Log_RichTextBox
            // 
            Log_RichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Log_RichTextBox.BackColor = Color.FromArgb(250, 250, 250);
            Log_RichTextBox.BorderStyle = BorderStyle.None;
            Log_RichTextBox.Font = new Font("Segoe UI", 12F);
            Log_RichTextBox.Location = new Point(18, 18);
            Log_RichTextBox.Name = "Log_RichTextBox";
            Log_RichTextBox.ReadOnly = true;
            Log_RichTextBox.Size = new Size(1342, 690);
            Log_RichTextBox.TabIndex = 0;
            Log_RichTextBox.Text = "";
            Log_RichTextBox.LinkClicked += RichTextBox_LinkClicked;
            Log_RichTextBox.TextChanged += RichTextBox_TextChanged;
            // 
            // Clear_Button
            // 
            Clear_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Clear_Button.BorderThickness = 1;
            Clear_Button.CustomizableEdges = customizableEdges1;
            Clear_Button.DisabledState.BorderColor = Color.DarkGray;
            Clear_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Clear_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Clear_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Clear_Button.FillColor = Color.White;
            Clear_Button.Font = new Font("Segoe UI", 10F);
            Clear_Button.ForeColor = Color.Black;
            Clear_Button.Location = new Point(1186, 737);
            Clear_Button.Name = "Clear_Button";
            Clear_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Clear_Button.Size = new Size(180, 45);
            Clear_Button.TabIndex = 3;
            Clear_Button.Text = "Clear";
            Clear_Button.Click += ClearButton_Click;
            // 
            // Log_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(1378, 794);
            Controls.Add(Clear_Button);
            Controls.Add(Log_RichTextBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 600);
            Name = "Log_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Log";
            Shown += Log_form_Shown;
            ResumeLayout(false);
        }

        #endregion
        public System.Windows.Forms.RichTextBox Log_RichTextBox;
        public Guna.UI2.WinForms.Guna2Button Clear_Button;
    }
}