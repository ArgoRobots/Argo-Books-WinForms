namespace Argo_Books
{
    partial class EULA_Form
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Title_Label = new Label();
            EULA_RichTextBox = new RichTextBox();
            Accept_CheckBox = new Guna.UI2.WinForms.Guna2CustomCheckBox();
            Accept_Label = new Label();
            Accept_Button = new Guna.UI2.WinForms.Guna2Button();
            Decline_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            Title_Label.Location = new Point(372, 9);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(389, 38);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "End User License Agreement";
            Title_Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // EULA_RichTextBox
            // 
            EULA_RichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            EULA_RichTextBox.Font = new Font("Segoe UI", 9.75F);
            EULA_RichTextBox.Location = new Point(20, 70);
            EULA_RichTextBox.Name = "EULA_RichTextBox";
            EULA_RichTextBox.ReadOnly = true;
            EULA_RichTextBox.Size = new Size(1088, 660);
            EULA_RichTextBox.TabIndex = 1;
            EULA_RichTextBox.Text = "";
            // 
            // Accept_CheckBox
            // 
            Accept_CheckBox.Anchor = AnchorStyles.Top;
            Accept_CheckBox.Animated = true;
            Accept_CheckBox.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Accept_CheckBox.CheckedState.BorderRadius = 2;
            Accept_CheckBox.CheckedState.BorderThickness = 0;
            Accept_CheckBox.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Accept_CheckBox.CustomizableEdges = customizableEdges1;
            Accept_CheckBox.Location = new Point(22, 764);
            Accept_CheckBox.Name = "Accept_CheckBox";
            Accept_CheckBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Accept_CheckBox.Size = new Size(20, 20);
            Accept_CheckBox.TabIndex = 260;
            Accept_CheckBox.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Accept_CheckBox.UncheckedState.BorderRadius = 2;
            Accept_CheckBox.UncheckedState.BorderThickness = 0;
            Accept_CheckBox.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            Accept_CheckBox.CheckedChanged += Accept_CheckBox_CheckedChanged;
            // 
            // Accept_Label
            // 
            Accept_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Accept_Label.AutoSize = true;
            Accept_Label.Font = new Font("Segoe UI", 10F);
            Accept_Label.Location = new Point(40, 755);
            Accept_Label.Name = "Accept_Label";
            Accept_Label.Padding = new Padding(5);
            Accept_Label.Size = new Size(522, 38);
            Accept_Label.TabIndex = 259;
            Accept_Label.Tag = "";
            Accept_Label.Text = "I have read and accept the terms of the license agreement";
            Accept_Label.Click += AcceptLabel_Click;
            // 
            // Accept_Button
            // 
            Accept_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Accept_Button.BorderColor = Color.LightGray;
            Accept_Button.BorderRadius = 2;
            Accept_Button.BorderThickness = 1;
            Accept_Button.CustomizableEdges = customizableEdges3;
            Accept_Button.Enabled = false;
            Accept_Button.FillColor = Color.FromArgb(250, 250, 250);
            Accept_Button.Font = new Font("Segoe UI", 9F);
            Accept_Button.ForeColor = Color.Black;
            Accept_Button.Location = new Point(742, 755);
            Accept_Button.Name = "Accept_Button";
            Accept_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Accept_Button.Size = new Size(180, 45);
            Accept_Button.TabIndex = 261;
            Accept_Button.Text = "Accept";
            Accept_Button.Click += Accept_Button_Click;
            // 
            // Decline_Button
            // 
            Decline_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Decline_Button.BorderColor = Color.LightGray;
            Decline_Button.BorderRadius = 2;
            Decline_Button.BorderThickness = 1;
            Decline_Button.CustomizableEdges = customizableEdges5;
            Decline_Button.FillColor = Color.FromArgb(250, 250, 250);
            Decline_Button.Font = new Font("Segoe UI", 9F);
            Decline_Button.ForeColor = Color.Black;
            Decline_Button.Location = new Point(928, 755);
            Decline_Button.Name = "Decline_Button";
            Decline_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Decline_Button.Size = new Size(180, 45);
            Decline_Button.TabIndex = 262;
            Decline_Button.Text = "Decline";
            Decline_Button.Click += Decline_Button_Click;
            // 
            // EULA_Form
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1128, 814);
            Controls.Add(Decline_Button);
            Controls.Add(Accept_Button);
            Controls.Add(Accept_CheckBox);
            Controls.Add(Accept_Label);
            Controls.Add(EULA_RichTextBox);
            Controls.Add(Title_Label);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EULA_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            FormClosing += EULA_Form_FormClosing;
            Shown += EULA_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private RichTextBox EULA_RichTextBox;
        public Guna.UI2.WinForms.Guna2CustomCheckBox Accept_CheckBox;
        private Label Accept_Label;
        private Guna.UI2.WinForms.Guna2Button Accept_Button;
        private Guna.UI2.WinForms.Guna2Button Decline_Button;
    }
}