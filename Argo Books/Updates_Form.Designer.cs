
namespace Argo_Books.Settings.Menus
{
    partial class Updates_Form
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Status_Label = new Label();
            Update_Button = new Guna.UI2.WinForms.Guna2Button();
            WhatsNew_LinkLabel = new LinkLabel();
            NotNow_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // Status_Label
            // 
            Status_Label.Anchor = AnchorStyles.Top;
            Status_Label.AutoSize = true;
            Status_Label.Font = new Font("Segoe UI", 15F);
            Status_Label.Location = new Point(148, 105);
            Status_Label.Name = "Status_Label";
            Status_Label.Size = new Size(433, 41);
            Status_Label.TabIndex = 0;
            Status_Label.Text = "Argo Books is up to date";
            // 
            // Update_Button
            // 
            Update_Button.Anchor = AnchorStyles.Top;
            Update_Button.BorderRadius = 3;
            Update_Button.CustomizableEdges = customizableEdges1;
            Update_Button.DisabledState.BorderColor = Color.DarkGray;
            Update_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Update_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Update_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Update_Button.FillColor = Color.FromArgb(0, 103, 192);
            Update_Button.Font = new Font("Segoe UI", 9.75F);
            Update_Button.ForeColor = Color.White;
            Update_Button.Location = new Point(224, 244);
            Update_Button.Name = "Update_Button";
            Update_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Update_Button.Size = new Size(280, 45);
            Update_Button.TabIndex = 1;
            Update_Button.Text = "Update";
            Update_Button.Click += Update_Button_Click;
            // 
            // WhatsNew_LinkLabel
            // 
            WhatsNew_LinkLabel.Anchor = AnchorStyles.Top;
            WhatsNew_LinkLabel.AutoSize = true;
            WhatsNew_LinkLabel.Font = new Font("Segoe UI", 10.5F);
            WhatsNew_LinkLabel.LinkArea = new LinkArea(10, 10);
            WhatsNew_LinkLabel.Location = new Point(257, 162);
            WhatsNew_LinkLabel.Name = "WhatsNew_LinkLabel";
            WhatsNew_LinkLabel.Size = new Size(215, 34);
            WhatsNew_LinkLabel.TabIndex = 2;
            WhatsNew_LinkLabel.TabStop = true;
            WhatsNew_LinkLabel.Text = "Check out what's new";
            WhatsNew_LinkLabel.UseCompatibleTextRendering = true;
            WhatsNew_LinkLabel.LinkClicked += WhatsNew_LinkLabel_LinkClicked;
            // 
            // NotNow_Button
            // 
            NotNow_Button.Anchor = AnchorStyles.Top;
            NotNow_Button.BorderRadius = 3;
            NotNow_Button.BorderThickness = 1;
            NotNow_Button.CustomizableEdges = customizableEdges3;
            NotNow_Button.DisabledState.BorderColor = Color.DarkGray;
            NotNow_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            NotNow_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            NotNow_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            NotNow_Button.FillColor = Color.White;
            NotNow_Button.Font = new Font("Segoe UI", 9.75F);
            NotNow_Button.ForeColor = Color.Black;
            NotNow_Button.Location = new Point(224, 295);
            NotNow_Button.Name = "NotNow_Button";
            NotNow_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            NotNow_Button.Size = new Size(280, 45);
            NotNow_Button.TabIndex = 3;
            NotNow_Button.Text = "Not now";
            NotNow_Button.Click += NotNow_Button_Click;
            // 
            // Updates_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(728, 444);
            Controls.Add(NotNow_Button);
            Controls.Add(WhatsNew_LinkLabel);
            Controls.Add(Update_Button);
            Controls.Add(Status_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(750, 500);
            Name = "Updates_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += Updates_Form_FormClosing;
            Shown += Updates_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label Status_Label;
        private Guna.UI2.WinForms.Guna2Button Update_Button;
        private LinkLabel WhatsNew_LinkLabel;
        private Guna.UI2.WinForms.Guna2Button NotNow_Button;
    }
}