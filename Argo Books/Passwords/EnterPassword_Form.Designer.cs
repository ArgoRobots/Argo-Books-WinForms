namespace Argo_Books.Passwords
{
    partial class EnterPassword_Form
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
            Enter_Button = new Guna.UI2.WinForms.Guna2Button();
            Password_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            EnterPassword_Label = new Label();
            Message_LinkLabel = new LinkLabel();
            PasswordEye_Button = new Guna.UI2.WinForms.Guna2CircleButton();
            SuspendLayout();
            // 
            // Enter_Button
            // 
            Enter_Button.Anchor = AnchorStyles.Top;
            Enter_Button.BackColor = Color.Transparent;
            Enter_Button.BorderColor = Color.LightGray;
            Enter_Button.BorderRadius = 2;
            Enter_Button.BorderThickness = 1;
            Enter_Button.CustomizableEdges = customizableEdges1;
            Enter_Button.Enabled = false;
            Enter_Button.FillColor = Color.White;
            Enter_Button.Font = new Font("Segoe UI", 10F);
            Enter_Button.ForeColor = Color.Black;
            Enter_Button.Location = new Point(232, 208);
            Enter_Button.Name = "Enter_Button";
            Enter_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Enter_Button.Size = new Size(215, 50);
            Enter_Button.TabIndex = 3;
            Enter_Button.Tag = "";
            Enter_Button.Text = "Enter";
            Enter_Button.Click += Enter_Button_Click;
            // 
            // Password_TextBox
            // 
            Password_TextBox.Anchor = AnchorStyles.Top;
            Password_TextBox.CustomizableEdges = customizableEdges3;
            Password_TextBox.DefaultText = "";
            Password_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            Password_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            Password_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            Password_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            Password_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            Password_TextBox.Font = new Font("Segoe UI", 9F);
            Password_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Password_TextBox.Location = new Point(164, 117);
            Password_TextBox.Margin = new Padding(4, 5, 4, 5);
            Password_TextBox.MaxLength = 32;
            Password_TextBox.Name = "Password_TextBox";
            Password_TextBox.PasswordChar = '•';
            Password_TextBox.PlaceholderText = "Password";
            Password_TextBox.SelectedText = "";
            Password_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Password_TextBox.ShortcutsEnabled = false;
            Password_TextBox.Size = new Size(350, 50);
            Password_TextBox.TabIndex = 1;
            Password_TextBox.TextChanged += Password_TextBox_TextChanged;
            Password_TextBox.KeyDown += Password_TextBox_KeyDown;
            // 
            // EnterPassword_Label
            // 
            EnterPassword_Label.Anchor = AnchorStyles.Top;
            EnterPassword_Label.AutoSize = true;
            EnterPassword_Label.Font = new Font("Segoe UI", 16F);
            EnterPassword_Label.Location = new Point(221, 23);
            EnterPassword_Label.Name = "EnterPassword_Label";
            EnterPassword_Label.Size = new Size(237, 45);
            EnterPassword_Label.TabIndex = 0;
            EnterPassword_Label.Text = "Enter password";
            // 
            // Message_LinkLabel
            // 
            Message_LinkLabel.Anchor = AnchorStyles.Top;
            Message_LinkLabel.AutoSize = true;
            Message_LinkLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Message_LinkLabel.LinkArea = new LinkArea(0, 0);
            Message_LinkLabel.Location = new Point(287, 302);
            Message_LinkLabel.Name = "Message_LinkLabel";
            Message_LinkLabel.Size = new Size(105, 31);
            Message_LinkLabel.TabIndex = 0;
            Message_LinkLabel.Text = "Message";
            Message_LinkLabel.LinkClicked += Message_LinkLabel_LinkClicked;
            // 
            // PasswordEye_Button
            // 
            PasswordEye_Button.Anchor = AnchorStyles.Top;
            PasswordEye_Button.DisabledState.BorderColor = Color.DarkGray;
            PasswordEye_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            PasswordEye_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            PasswordEye_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            PasswordEye_Button.FillColor = Color.Empty;
            PasswordEye_Button.Font = new Font("Segoe UI", 9F);
            PasswordEye_Button.ForeColor = Color.Black;
            PasswordEye_Button.Image = Properties.Resources.HideBlack;
            PasswordEye_Button.ImageSize = new Size(30, 30);
            PasswordEye_Button.Location = new Point(472, 125);
            PasswordEye_Button.Name = "PasswordEye_Button";
            PasswordEye_Button.Padding = new Padding(3);
            PasswordEye_Button.ShadowDecoration.CustomizableEdges = customizableEdges5;
            PasswordEye_Button.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            PasswordEye_Button.Size = new Size(34, 34);
            PasswordEye_Button.TabIndex = 2;
            PasswordEye_Button.Click += PasswordEye_Button_Click;
            // 
            // EnterPassword_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(678, 354);
            Controls.Add(PasswordEye_Button);
            Controls.Add(Message_LinkLabel);
            Controls.Add(Enter_Button);
            Controls.Add(Password_TextBox);
            Controls.Add(EnterPassword_Label);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MinimumSize = new Size(700, 410);
            Name = "EnterPassword_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Shown += EnterPassword_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button Enter_Button;
        private Guna.UI2.WinForms.Guna2TextBox Password_TextBox;
        private Label EnterPassword_Label;
        private LinkLabel Message_LinkLabel;
        private Guna.UI2.WinForms.Guna2CircleButton PasswordEye_Button;
    }
}