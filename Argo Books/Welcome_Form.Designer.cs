namespace Argo_Books
{
    partial class Welcome_Form
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
            Documentation_LinkLabel = new LinkLabel();
            YouTube_LinkLabel = new LinkLabel();
            Continue_Button = new Guna.UI2.WinForms.Guna2Button();
            WebBrowser = new WebBrowser();
            HowToGetStarted_Label = new Label();
            DontShowAgain_CheckBox = new Guna.UI2.WinForms.Guna2CustomCheckBox();
            DontShowAgain_Label = new Label();
            SuspendLayout();
            // 
            // Documentation_LinkLabel
            // 
            Documentation_LinkLabel.Anchor = AnchorStyles.Bottom;
            Documentation_LinkLabel.AutoSize = true;
            Documentation_LinkLabel.Font = new Font("Segoe UI", 10F);
            Documentation_LinkLabel.LinkArea = new LinkArea(32, 7);
            Documentation_LinkLabel.Location = new Point(373, 695);
            Documentation_LinkLabel.Margin = new Padding(4, 0, 4, 0);
            Documentation_LinkLabel.Name = "Documentation_LinkLabel";
            Documentation_LinkLabel.Size = new Size(383, 33);
            Documentation_LinkLabel.TabIndex = 564;
            Documentation_LinkLabel.TabStop = true;
            Documentation_LinkLabel.Text = "or see the documentation on our website";
            Documentation_LinkLabel.UseCompatibleTextRendering = true;
            Documentation_LinkLabel.LinkClicked += Documentation_LinkLabel_LinkClicked;
            // 
            // YouTube_LinkLabel
            // 
            YouTube_LinkLabel.Anchor = AnchorStyles.Bottom;
            YouTube_LinkLabel.AutoSize = true;
            YouTube_LinkLabel.Font = new Font("Segoe UI", 10F);
            YouTube_LinkLabel.LinkArea = new LinkArea(25, 15);
            YouTube_LinkLabel.Location = new Point(360, 657);
            YouTube_LinkLabel.Margin = new Padding(4, 0, 4, 0);
            YouTube_LinkLabel.Name = "YouTube_LinkLabel";
            YouTube_LinkLabel.Size = new Size(408, 33);
            YouTube_LinkLabel.TabIndex = 563;
            YouTube_LinkLabel.TabStop = true;
            YouTube_LinkLabel.Text = "Watch more videos on our YouTube channel";
            YouTube_LinkLabel.UseCompatibleTextRendering = true;
            YouTube_LinkLabel.LinkClicked += YouTube_LinkLabel_LinkClicked;
            // 
            // Continue_Button
            // 
            Continue_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Continue_Button.BorderColor = Color.FromArgb(217, 221, 226);
            Continue_Button.BorderRadius = 3;
            Continue_Button.BorderThickness = 1;
            Continue_Button.CustomizableEdges = customizableEdges1;
            Continue_Button.DisabledState.BorderColor = Color.DarkGray;
            Continue_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Continue_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Continue_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Continue_Button.FillColor = Color.White;
            Continue_Button.Font = new Font("Segoe UI", 10F);
            Continue_Button.ForeColor = Color.Black;
            Continue_Button.Location = new Point(933, 755);
            Continue_Button.Margin = new Padding(6, 5, 6, 5);
            Continue_Button.Name = "Continue_Button";
            Continue_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Continue_Button.Size = new Size(180, 45);
            Continue_Button.TabIndex = 562;
            Continue_Button.Text = "Continue";
            Continue_Button.Click += Continue_Button_Click;
            // 
            // WebBrowser
            // 
            WebBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            WebBrowser.Location = new Point(64, 147);
            WebBrowser.Margin = new Padding(6, 5, 6, 5);
            WebBrowser.MinimumSize = new Size(33, 38);
            WebBrowser.Name = "WebBrowser";
            WebBrowser.ScrollBarsEnabled = false;
            WebBrowser.Size = new Size(1000, 493);
            WebBrowser.TabIndex = 561;
            // 
            // HowToGetStarted_Label
            // 
            HowToGetStarted_Label.Anchor = AnchorStyles.Top;
            HowToGetStarted_Label.AutoSize = true;
            HowToGetStarted_Label.Font = new Font("Segoe UI", 16F);
            HowToGetStarted_Label.Location = new Point(334, 22);
            HowToGetStarted_Label.Margin = new Padding(6, 0, 6, 0);
            HowToGetStarted_Label.Name = "HowToGetStarted_Label";
            HowToGetStarted_Label.Size = new Size(461, 90);
            HowToGetStarted_Label.TabIndex = 560;
            HowToGetStarted_Label.Text = "Welcome to Argo Books\r\nHere's how to get started";
            HowToGetStarted_Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DontShowAgain_CheckBox
            // 
            DontShowAgain_CheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            DontShowAgain_CheckBox.Animated = true;
            DontShowAgain_CheckBox.Checked = true;
            DontShowAgain_CheckBox.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            DontShowAgain_CheckBox.CheckedState.BorderRadius = 2;
            DontShowAgain_CheckBox.CheckedState.BorderThickness = 0;
            DontShowAgain_CheckBox.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            DontShowAgain_CheckBox.CustomizableEdges = customizableEdges3;
            DontShowAgain_CheckBox.Location = new Point(884, 767);
            DontShowAgain_CheckBox.Name = "DontShowAgain_CheckBox";
            DontShowAgain_CheckBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            DontShowAgain_CheckBox.Size = new Size(20, 20);
            DontShowAgain_CheckBox.TabIndex = 566;
            DontShowAgain_CheckBox.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            DontShowAgain_CheckBox.UncheckedState.BorderRadius = 2;
            DontShowAgain_CheckBox.UncheckedState.BorderThickness = 0;
            DontShowAgain_CheckBox.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            // 
            // DontShowAgain_Label
            // 
            DontShowAgain_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            DontShowAgain_Label.AutoSize = true;
            DontShowAgain_Label.Font = new Font("Segoe UI", 10F);
            DontShowAgain_Label.Location = new Point(676, 758);
            DontShowAgain_Label.Name = "DontShowAgain_Label";
            DontShowAgain_Label.Padding = new Padding(5);
            DontShowAgain_Label.Size = new Size(210, 38);
            DontShowAgain_Label.TabIndex = 565;
            DontShowAgain_Label.Tag = "";
            DontShowAgain_Label.Text = "Don't show this again";
            // 
            // Welcome_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1128, 814);
            Controls.Add(DontShowAgain_CheckBox);
            Controls.Add(DontShowAgain_Label);
            Controls.Add(Documentation_LinkLabel);
            Controls.Add(YouTube_LinkLabel);
            Controls.Add(Continue_Button);
            Controls.Add(WebBrowser);
            Controls.Add(HowToGetStarted_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "Welcome_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Shown += Welcome_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LinkLabel Documentation_LinkLabel;
        private LinkLabel YouTube_LinkLabel;
        public Guna.UI2.WinForms.Guna2Button Continue_Button;
        private WebBrowser WebBrowser;
        private Label HowToGetStarted_Label;
        public Guna.UI2.WinForms.Guna2CustomCheckBox DontShowAgain_CheckBox;
        private Label DontShowAgain_Label;
    }
}