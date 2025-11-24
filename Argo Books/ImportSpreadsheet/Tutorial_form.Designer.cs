namespace Argo_Books.ImportSpreadsheet
{
    partial class Tutorial_Form
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
            HowToGetStarted_Label = new Label();
            WebBrowser = new WebBrowser();
            Next_Button = new Guna.UI2.WinForms.Guna2Button();
            YouTube_LinkLabel = new LinkLabel();
            Documentation_LinkLabel = new LinkLabel();
            SuspendLayout();
            // 
            // HowToGetStarted_Label
            // 
            HowToGetStarted_Label.Anchor = AnchorStyles.Top;
            HowToGetStarted_Label.AutoSize = true;
            HowToGetStarted_Label.Font = new Font("Segoe UI", 16F);
            HowToGetStarted_Label.Location = new Point(406, 23);
            HowToGetStarted_Label.Margin = new Padding(6, 0, 6, 0);
            HowToGetStarted_Label.Name = "HowToGetStarted_Label";
            HowToGetStarted_Label.Size = new Size(288, 45);
            HowToGetStarted_Label.TabIndex = 542;
            HowToGetStarted_Label.Text = "How to get started";
            // 
            // WebBrowser
            // 
            WebBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            WebBrowser.Location = new Point(147, 105);
            WebBrowser.Margin = new Padding(6, 5, 6, 5);
            WebBrowser.MinimumSize = new Size(33, 38);
            WebBrowser.Name = "WebBrowser";
            WebBrowser.ScrollBarsEnabled = false;
            WebBrowser.Size = new Size(806, 395);
            WebBrowser.TabIndex = 545;
            // 
            // Next_Button
            // 
            Next_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Next_Button.BorderColor = Color.FromArgb(217, 221, 226);
            Next_Button.BorderRadius = 3;
            Next_Button.BorderThickness = 1;
            Next_Button.CustomizableEdges = customizableEdges1;
            Next_Button.DisabledState.BorderColor = Color.DarkGray;
            Next_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Next_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Next_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Next_Button.FillColor = Color.White;
            Next_Button.Font = new Font("Segoe UI", 10F);
            Next_Button.ForeColor = Color.Black;
            Next_Button.Location = new Point(905, 591);
            Next_Button.Margin = new Padding(6, 5, 6, 5);
            Next_Button.Name = "Next_Button";
            Next_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Next_Button.Size = new Size(180, 45);
            Next_Button.TabIndex = 557;
            Next_Button.Text = "Next";
            Next_Button.Click += Next_Button_Click;
            // 
            // YouTube_LinkLabel
            // 
            YouTube_LinkLabel.Anchor = AnchorStyles.Bottom;
            YouTube_LinkLabel.AutoSize = true;
            YouTube_LinkLabel.Font = new Font("Segoe UI", 10F);
            YouTube_LinkLabel.LinkArea = new LinkArea(33, 15);
            YouTube_LinkLabel.Location = new Point(320, 537);
            YouTube_LinkLabel.Margin = new Padding(4, 0, 4, 0);
            YouTube_LinkLabel.Name = "YouTube_LinkLabel";
            YouTube_LinkLabel.Size = new Size(461, 33);
            YouTube_LinkLabel.TabIndex = 558;
            YouTube_LinkLabel.TabStop = true;
            YouTube_LinkLabel.Text = "Watch the tutorial videos on our YouTube channel";
            YouTube_LinkLabel.UseCompatibleTextRendering = true;
            YouTube_LinkLabel.LinkClicked += YouTube_LinkLabel_LinkClicked;
            // 
            // Documentation_LinkLabel
            // 
            Documentation_LinkLabel.Anchor = AnchorStyles.Bottom;
            Documentation_LinkLabel.AutoSize = true;
            Documentation_LinkLabel.Font = new Font("Segoe UI", 10F);
            Documentation_LinkLabel.LinkArea = new LinkArea(32, 7);
            Documentation_LinkLabel.Location = new Point(359, 575);
            Documentation_LinkLabel.Margin = new Padding(4, 0, 4, 0);
            Documentation_LinkLabel.Name = "Documentation_LinkLabel";
            Documentation_LinkLabel.Size = new Size(383, 33);
            Documentation_LinkLabel.TabIndex = 559;
            Documentation_LinkLabel.TabStop = true;
            Documentation_LinkLabel.Text = "or see the documentation on our website";
            Documentation_LinkLabel.UseCompatibleTextRendering = true;
            Documentation_LinkLabel.LinkClicked += Documentation_LinkLabel_Click;
            // 
            // Tutorial_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 650);
            Controls.Add(Documentation_LinkLabel);
            Controls.Add(YouTube_LinkLabel);
            Controls.Add(Next_Button);
            Controls.Add(WebBrowser);
            Controls.Add(HowToGetStarted_Label);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(6, 5, 6, 5);
            Name = "Tutorial_Form";
            Shown += Tutorial_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label HowToGetStarted_Label;
        private System.Windows.Forms.WebBrowser WebBrowser;
        public Guna.UI2.WinForms.Guna2Button Next_Button;
        private System.Windows.Forms.LinkLabel YouTube_LinkLabel;
        private System.Windows.Forms.LinkLabel Documentation_LinkLabel;
    }
}