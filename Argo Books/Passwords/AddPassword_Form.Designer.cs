namespace Argo_Books.Passwords
{
    partial class AddPassword_Form
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            AddPassword_Label = new Label();
            Password_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            SetPassword_Button = new Guna.UI2.WinForms.Guna2Button();
            LengthRequirement_Label = new Label();
            UppercaseRequirement_Label = new Label();
            NumberRequirement_Label = new Label();
            SpecialCharacterRequirement_Label = new Label();
            PasswordWarning_Label = new Label();
            ConfirmPassword_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            PasswordsMatch_Label = new Label();
            PasswordEye_Button = new Guna.UI2.WinForms.Guna2CircleButton();
            ConfirmPasswordEye_Button = new Guna.UI2.WinForms.Guna2CircleButton();
            Length_Checkmark = new Guna.UI2.WinForms.Guna2CircleButton();
            Number_Checkmark = new Guna.UI2.WinForms.Guna2CircleButton();
            Uppercase_Checkmark = new Guna.UI2.WinForms.Guna2CircleButton();
            SpecialChar_Checkmark = new Guna.UI2.WinForms.Guna2CircleButton();
            SuspendLayout();
            // 
            // AddPassword_Label
            // 
            AddPassword_Label.Anchor = AnchorStyles.Top;
            AddPassword_Label.AutoSize = true;
            AddPassword_Label.Font = new Font("Segoe UI", 16F);
            AddPassword_Label.Location = new Point(250, 23);
            AddPassword_Label.Margin = new Padding(4, 0, 4, 0);
            AddPassword_Label.Name = "AddPassword_Label";
            AddPassword_Label.Size = new Size(223, 45);
            AddPassword_Label.TabIndex = 3;
            AddPassword_Label.Text = "Add password";
            // 
            // Password_TextBox
            // 
            Password_TextBox.Anchor = AnchorStyles.Top;
            Password_TextBox.CustomizableEdges = customizableEdges1;
            Password_TextBox.DefaultText = "";
            Password_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            Password_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            Password_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            Password_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            Password_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            Password_TextBox.Font = new Font("Segoe UI", 11F);
            Password_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Password_TextBox.Location = new Point(186, 120);
            Password_TextBox.Margin = new Padding(7, 10, 7, 10);
            Password_TextBox.MaxLength = 32;
            Password_TextBox.Name = "Password_TextBox";
            Password_TextBox.PasswordChar = '•';
            Password_TextBox.PlaceholderText = "Password";
            Password_TextBox.SelectedText = "";
            Password_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Password_TextBox.ShortcutsEnabled = false;
            Password_TextBox.Size = new Size(350, 50);
            Password_TextBox.TabIndex = 1;
            Password_TextBox.TextChanged += Password_TextBox_TextChanged;
            Password_TextBox.KeyDown += Password_TextBox_KeyDown;
            // 
            // SetPassword_Button
            // 
            SetPassword_Button.Anchor = AnchorStyles.Top;
            SetPassword_Button.BackColor = Color.Transparent;
            SetPassword_Button.BorderColor = Color.LightGray;
            SetPassword_Button.BorderRadius = 2;
            SetPassword_Button.BorderThickness = 1;
            SetPassword_Button.CustomizableEdges = customizableEdges3;
            SetPassword_Button.Enabled = false;
            SetPassword_Button.FillColor = Color.White;
            SetPassword_Button.Font = new Font("Segoe UI", 10F);
            SetPassword_Button.ForeColor = Color.Black;
            SetPassword_Button.Location = new Point(236, 613);
            SetPassword_Button.Margin = new Padding(6, 5, 6, 5);
            SetPassword_Button.Name = "SetPassword_Button";
            SetPassword_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            SetPassword_Button.Size = new Size(250, 50);
            SetPassword_Button.TabIndex = 5;
            SetPassword_Button.Tag = "";
            SetPassword_Button.Text = "Set password";
            SetPassword_Button.Click += SetPassword_Button_Click;
            // 
            // LengthRequirement_Label
            // 
            LengthRequirement_Label.Anchor = AnchorStyles.Top;
            LengthRequirement_Label.AutoSize = true;
            LengthRequirement_Label.Font = new Font("Segoe UI", 11.25F);
            LengthRequirement_Label.Location = new Point(241, 190);
            LengthRequirement_Label.Margin = new Padding(4, 0, 4, 0);
            LengthRequirement_Label.Name = "LengthRequirement_Label";
            LengthRequirement_Label.Size = new Size(220, 31);
            LengthRequirement_Label.TabIndex = 0;
            LengthRequirement_Label.Text = "At least 8 characters";
            // 
            // UppercaseRequirement_Label
            // 
            UppercaseRequirement_Label.Anchor = AnchorStyles.Top;
            UppercaseRequirement_Label.AutoSize = true;
            UppercaseRequirement_Label.Font = new Font("Segoe UI", 11.25F);
            UppercaseRequirement_Label.Location = new Point(241, 235);
            UppercaseRequirement_Label.Margin = new Padding(4, 0, 4, 0);
            UppercaseRequirement_Label.Name = "UppercaseRequirement_Label";
            UppercaseRequirement_Label.Size = new Size(275, 31);
            UppercaseRequirement_Label.TabIndex = 0;
            UppercaseRequirement_Label.Text = "Contains uppercase letter";
            // 
            // NumberRequirement_Label
            // 
            NumberRequirement_Label.Anchor = AnchorStyles.Top;
            NumberRequirement_Label.AutoSize = true;
            NumberRequirement_Label.Font = new Font("Segoe UI", 11.25F);
            NumberRequirement_Label.Location = new Point(241, 280);
            NumberRequirement_Label.Margin = new Padding(4, 0, 4, 0);
            NumberRequirement_Label.Name = "NumberRequirement_Label";
            NumberRequirement_Label.Size = new Size(189, 31);
            NumberRequirement_Label.TabIndex = 0;
            NumberRequirement_Label.Text = "Contains number";
            // 
            // SpecialCharacterRequirement_Label
            // 
            SpecialCharacterRequirement_Label.Anchor = AnchorStyles.Top;
            SpecialCharacterRequirement_Label.AutoSize = true;
            SpecialCharacterRequirement_Label.Font = new Font("Segoe UI", 11.25F);
            SpecialCharacterRequirement_Label.Location = new Point(241, 325);
            SpecialCharacterRequirement_Label.Margin = new Padding(4, 0, 4, 0);
            SpecialCharacterRequirement_Label.Name = "SpecialCharacterRequirement_Label";
            SpecialCharacterRequirement_Label.Size = new Size(281, 31);
            SpecialCharacterRequirement_Label.TabIndex = 0;
            SpecialCharacterRequirement_Label.Text = "Contains special character";
            // 
            // PasswordWarning_Label
            // 
            PasswordWarning_Label.Anchor = AnchorStyles.Top;
            PasswordWarning_Label.AutoSize = true;
            PasswordWarning_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PasswordWarning_Label.Location = new Point(84, 534);
            PasswordWarning_Label.Margin = new Padding(4, 0, 4, 0);
            PasswordWarning_Label.Name = "PasswordWarning_Label";
            PasswordWarning_Label.Size = new Size(554, 62);
            PasswordWarning_Label.TabIndex = 0;
            PasswordWarning_Label.Text = "If you forget your password, all your data will be lost!\r\nMake sure you record it somewhere safe.";
            PasswordWarning_Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ConfirmPassword_TextBox
            // 
            ConfirmPassword_TextBox.Anchor = AnchorStyles.Top;
            ConfirmPassword_TextBox.CustomizableEdges = customizableEdges5;
            ConfirmPassword_TextBox.DefaultText = "";
            ConfirmPassword_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            ConfirmPassword_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            ConfirmPassword_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            ConfirmPassword_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            ConfirmPassword_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            ConfirmPassword_TextBox.Font = new Font("Segoe UI", 11F);
            ConfirmPassword_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            ConfirmPassword_TextBox.Location = new Point(186, 404);
            ConfirmPassword_TextBox.Margin = new Padding(7, 10, 7, 10);
            ConfirmPassword_TextBox.MaxLength = 32;
            ConfirmPassword_TextBox.Name = "ConfirmPassword_TextBox";
            ConfirmPassword_TextBox.PasswordChar = '•';
            ConfirmPassword_TextBox.PlaceholderText = "Confirm password";
            ConfirmPassword_TextBox.SelectedText = "";
            ConfirmPassword_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges6;
            ConfirmPassword_TextBox.ShortcutsEnabled = false;
            ConfirmPassword_TextBox.Size = new Size(350, 50);
            ConfirmPassword_TextBox.TabIndex = 3;
            ConfirmPassword_TextBox.TextChanged += ConfirmPassword_TextBox_TextChanged;
            ConfirmPassword_TextBox.KeyDown += ConfirmPassword_TextBox_KeyDown;
            // 
            // PasswordsMatch_Label
            // 
            PasswordsMatch_Label.Anchor = AnchorStyles.Top;
            PasswordsMatch_Label.Font = new Font("Segoe UI", 12F);
            PasswordsMatch_Label.Location = new Point(225, 462);
            PasswordsMatch_Label.Margin = new Padding(4, 0, 4, 0);
            PasswordsMatch_Label.Name = "PasswordsMatch_Label";
            PasswordsMatch_Label.Size = new Size(272, 32);
            PasswordsMatch_Label.TabIndex = 0;
            PasswordsMatch_Label.Text = "Passwords do not match";
            PasswordsMatch_Label.TextAlign = ContentAlignment.MiddleCenter;
            PasswordsMatch_Label.Visible = false;
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
            PasswordEye_Button.Location = new Point(494, 128);
            PasswordEye_Button.Name = "PasswordEye_Button";
            PasswordEye_Button.Padding = new Padding(3);
            PasswordEye_Button.ShadowDecoration.CustomizableEdges = customizableEdges7;
            PasswordEye_Button.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            PasswordEye_Button.Size = new Size(34, 34);
            PasswordEye_Button.TabIndex = 2;
            PasswordEye_Button.Click += PasswordEye_Button_Click;
            // 
            // ConfirmPasswordEye_Button
            // 
            ConfirmPasswordEye_Button.Anchor = AnchorStyles.Top;
            ConfirmPasswordEye_Button.DisabledState.BorderColor = Color.DarkGray;
            ConfirmPasswordEye_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            ConfirmPasswordEye_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            ConfirmPasswordEye_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            ConfirmPasswordEye_Button.FillColor = Color.Empty;
            ConfirmPasswordEye_Button.Font = new Font("Segoe UI", 9F);
            ConfirmPasswordEye_Button.ForeColor = Color.Black;
            ConfirmPasswordEye_Button.Image = Properties.Resources.HideBlack;
            ConfirmPasswordEye_Button.ImageSize = new Size(30, 30);
            ConfirmPasswordEye_Button.Location = new Point(494, 412);
            ConfirmPasswordEye_Button.Name = "ConfirmPasswordEye_Button";
            ConfirmPasswordEye_Button.Padding = new Padding(3);
            ConfirmPasswordEye_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            ConfirmPasswordEye_Button.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            ConfirmPasswordEye_Button.Size = new Size(34, 34);
            ConfirmPasswordEye_Button.TabIndex = 4;
            ConfirmPasswordEye_Button.Click += ConfirmPasswordEye_Button_Click;
            // 
            // Length_Checkmark
            // 
            Length_Checkmark.DisabledState.BorderColor = Color.DarkGray;
            Length_Checkmark.DisabledState.CustomBorderColor = Color.DarkGray;
            Length_Checkmark.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Length_Checkmark.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Length_Checkmark.FillColor = Color.Empty;
            Length_Checkmark.Font = new Font("Segoe UI", 9F);
            Length_Checkmark.ForeColor = Color.Black;
            Length_Checkmark.Image = Properties.Resources.Checkmark;
            Length_Checkmark.ImageSize = new Size(30, 30);
            Length_Checkmark.Location = new Point(200, 190);
            Length_Checkmark.Name = "Length_Checkmark";
            Length_Checkmark.Padding = new Padding(3);
            Length_Checkmark.ShadowDecoration.CustomizableEdges = customizableEdges9;
            Length_Checkmark.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            Length_Checkmark.Size = new Size(34, 34);
            Length_Checkmark.TabIndex = 0;
            Length_Checkmark.Visible = false;
            // 
            // Number_Checkmark
            // 
            Number_Checkmark.DisabledState.BorderColor = Color.DarkGray;
            Number_Checkmark.DisabledState.CustomBorderColor = Color.DarkGray;
            Number_Checkmark.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Number_Checkmark.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Number_Checkmark.FillColor = Color.Empty;
            Number_Checkmark.Font = new Font("Segoe UI", 9F);
            Number_Checkmark.ForeColor = Color.Black;
            Number_Checkmark.Image = Properties.Resources.Checkmark;
            Number_Checkmark.ImageSize = new Size(30, 30);
            Number_Checkmark.Location = new Point(200, 280);
            Number_Checkmark.Name = "Number_Checkmark";
            Number_Checkmark.Padding = new Padding(3);
            Number_Checkmark.ShadowDecoration.CustomizableEdges = customizableEdges10;
            Number_Checkmark.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            Number_Checkmark.Size = new Size(34, 34);
            Number_Checkmark.TabIndex = 0;
            Number_Checkmark.Visible = false;
            // 
            // Uppercase_Checkmark
            // 
            Uppercase_Checkmark.DisabledState.BorderColor = Color.DarkGray;
            Uppercase_Checkmark.DisabledState.CustomBorderColor = Color.DarkGray;
            Uppercase_Checkmark.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Uppercase_Checkmark.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Uppercase_Checkmark.FillColor = Color.Empty;
            Uppercase_Checkmark.Font = new Font("Segoe UI", 9F);
            Uppercase_Checkmark.ForeColor = Color.Black;
            Uppercase_Checkmark.Image = Properties.Resources.Checkmark;
            Uppercase_Checkmark.ImageSize = new Size(30, 30);
            Uppercase_Checkmark.Location = new Point(200, 235);
            Uppercase_Checkmark.Name = "Uppercase_Checkmark";
            Uppercase_Checkmark.Padding = new Padding(3);
            Uppercase_Checkmark.ShadowDecoration.CustomizableEdges = customizableEdges11;
            Uppercase_Checkmark.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            Uppercase_Checkmark.Size = new Size(34, 34);
            Uppercase_Checkmark.TabIndex = 0;
            Uppercase_Checkmark.Visible = false;
            // 
            // SpecialChar_Checkmark
            // 
            SpecialChar_Checkmark.DisabledState.BorderColor = Color.DarkGray;
            SpecialChar_Checkmark.DisabledState.CustomBorderColor = Color.DarkGray;
            SpecialChar_Checkmark.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            SpecialChar_Checkmark.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            SpecialChar_Checkmark.FillColor = Color.Empty;
            SpecialChar_Checkmark.Font = new Font("Segoe UI", 9F);
            SpecialChar_Checkmark.ForeColor = Color.Black;
            SpecialChar_Checkmark.Image = Properties.Resources.Checkmark;
            SpecialChar_Checkmark.ImageSize = new Size(30, 30);
            SpecialChar_Checkmark.Location = new Point(200, 325);
            SpecialChar_Checkmark.Name = "SpecialChar_Checkmark";
            SpecialChar_Checkmark.Padding = new Padding(3);
            SpecialChar_Checkmark.ShadowDecoration.CustomizableEdges = customizableEdges12;
            SpecialChar_Checkmark.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            SpecialChar_Checkmark.Size = new Size(34, 34);
            SpecialChar_Checkmark.TabIndex = 0;
            SpecialChar_Checkmark.Visible = false;
            // 
            // AddPassword_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(723, 684);
            Controls.Add(SpecialChar_Checkmark);
            Controls.Add(Uppercase_Checkmark);
            Controls.Add(Number_Checkmark);
            Controls.Add(Length_Checkmark);
            Controls.Add(ConfirmPasswordEye_Button);
            Controls.Add(PasswordEye_Button);
            Controls.Add(PasswordsMatch_Label);
            Controls.Add(ConfirmPassword_TextBox);
            Controls.Add(PasswordWarning_Label);
            Controls.Add(SpecialCharacterRequirement_Label);
            Controls.Add(NumberRequirement_Label);
            Controls.Add(UppercaseRequirement_Label);
            Controls.Add(LengthRequirement_Label);
            Controls.Add(SetPassword_Button);
            Controls.Add(Password_TextBox);
            Controls.Add(AddPassword_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(745, 740);
            Name = "AddPassword_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Shown += AddPassword_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label AddPassword_Label;
        private Guna.UI2.WinForms.Guna2TextBox Password_TextBox;
        private Guna.UI2.WinForms.Guna2Button SetPassword_Button;
        private Label LengthRequirement_Label;
        private Label UppercaseRequirement_Label;
        private Label NumberRequirement_Label;
        private Label SpecialCharacterRequirement_Label;
        private Label PasswordWarning_Label;
        private Guna.UI2.WinForms.Guna2TextBox ConfirmPassword_TextBox;
        private Label PasswordsMatch_Label;
        private Guna.UI2.WinForms.Guna2CircleButton PasswordEye_Button;
        private Guna.UI2.WinForms.Guna2CircleButton ConfirmPasswordEye_Button;
        private Guna.UI2.WinForms.Guna2CircleButton Length_Checkmark;
        private Guna.UI2.WinForms.Guna2CircleButton Number_Checkmark;
        private Guna.UI2.WinForms.Guna2CircleButton Uppercase_Checkmark;
        private Guna.UI2.WinForms.Guna2CircleButton SpecialChar_Checkmark;
    }
}