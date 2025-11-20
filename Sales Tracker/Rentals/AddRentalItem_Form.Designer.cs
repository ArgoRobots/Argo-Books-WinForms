using Sales_Tracker.Classes;

namespace Sales_Tracker.Rentals
{
    partial class AddRentalItem_Form
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            RentalItemID_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            RentalItemID_Label = new Label();
            ProductName_Label = new Label();
            ProductName_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            TotalQuantity_Label = new Label();
            TotalQuantity_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            RentalRate_Label = new Label();
            RentalRate_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            RateType_Label = new Label();
            RateType_ComboBox = new Guna.UI2.WinForms.Guna2ComboBox();
            AddRentalItem_Button = new Guna.UI2.WinForms.Guna2Button();
            WarningProduct_LinkLabel = new LinkLabel();
            WarningProduct_PictureBox = new PictureBox();
            SecurityDeposit_Label = new Label();
            SecurityDeposit_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Notes_Label = new Label();
            Notes_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            AddRentalItem_Label = new Label();
            ((System.ComponentModel.ISupportInitialize)WarningProduct_PictureBox).BeginInit();
            SuspendLayout();
            // 
            // RentalItemID_TextBox
            // 
            RentalItemID_TextBox.Anchor = AnchorStyles.Top;
            RentalItemID_TextBox.CustomizableEdges = customizableEdges1;
            RentalItemID_TextBox.DefaultText = "";
            RentalItemID_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            RentalItemID_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            RentalItemID_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            RentalItemID_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            RentalItemID_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            RentalItemID_TextBox.Font = new Font("Segoe UI", 9F);
            RentalItemID_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            RentalItemID_TextBox.Location = new Point(104, 153);
            RentalItemID_TextBox.Margin = new Padding(4, 5, 4, 5);
            RentalItemID_TextBox.MaxLength = 50;
            RentalItemID_TextBox.Name = "RentalItemID_TextBox";
            RentalItemID_TextBox.PlaceholderText = "";
            RentalItemID_TextBox.SelectedText = "";
            RentalItemID_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            RentalItemID_TextBox.ShortcutsEnabled = false;
            RentalItemID_TextBox.Size = new Size(300, 50);
            RentalItemID_TextBox.TabIndex = 1;
            RentalItemID_TextBox.Tag = "";
            RentalItemID_TextBox.TextChanged += ValidateInputs;
            // 
            // RentalItemID_Label
            // 
            RentalItemID_Label.Anchor = AnchorStyles.Top;
            RentalItemID_Label.AutoSize = true;
            RentalItemID_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RentalItemID_Label.Location = new Point(104, 118);
            RentalItemID_Label.Name = "RentalItemID_Label";
            RentalItemID_Label.Size = new Size(88, 31);
            RentalItemID_Label.TabIndex = 0;
            RentalItemID_Label.Text = "Item ID";
            // 
            // ProductName_Label
            // 
            ProductName_Label.Anchor = AnchorStyles.Top;
            ProductName_Label.AutoSize = true;
            ProductName_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ProductName_Label.Location = new Point(412, 118);
            ProductName_Label.Name = "ProductName_Label";
            ProductName_Label.Size = new Size(157, 31);
            ProductName_Label.TabIndex = 0;
            ProductName_Label.Text = "Product name";
            // 
            // ProductName_TextBox
            // 
            ProductName_TextBox.Anchor = AnchorStyles.Top;
            ProductName_TextBox.CustomizableEdges = customizableEdges3;
            ProductName_TextBox.DefaultText = "";
            ProductName_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            ProductName_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            ProductName_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            ProductName_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            ProductName_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            ProductName_TextBox.Font = new Font("Segoe UI", 9F);
            ProductName_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            ProductName_TextBox.Location = new Point(412, 153);
            ProductName_TextBox.Margin = new Padding(4, 5, 4, 5);
            ProductName_TextBox.MaxLength = 50;
            ProductName_TextBox.Name = "ProductName_TextBox";
            ProductName_TextBox.PlaceholderText = "";
            ProductName_TextBox.SelectedText = "";
            ProductName_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            ProductName_TextBox.ShortcutsEnabled = false;
            ProductName_TextBox.Size = new Size(300, 50);
            ProductName_TextBox.TabIndex = 2;
            ProductName_TextBox.Tag = "";
            // 
            // TotalQuantity_Label
            // 
            TotalQuantity_Label.Anchor = AnchorStyles.Top;
            TotalQuantity_Label.AutoSize = true;
            TotalQuantity_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TotalQuantity_Label.Location = new Point(720, 118);
            TotalQuantity_Label.Name = "TotalQuantity_Label";
            TotalQuantity_Label.Size = new Size(154, 31);
            TotalQuantity_Label.TabIndex = 0;
            TotalQuantity_Label.Text = "Total quantity";
            // 
            // TotalQuantity_TextBox
            // 
            TotalQuantity_TextBox.Anchor = AnchorStyles.Top;
            TotalQuantity_TextBox.CustomizableEdges = customizableEdges5;
            TotalQuantity_TextBox.DefaultText = "";
            TotalQuantity_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            TotalQuantity_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            TotalQuantity_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            TotalQuantity_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            TotalQuantity_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            TotalQuantity_TextBox.Font = new Font("Segoe UI", 9F);
            TotalQuantity_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            TotalQuantity_TextBox.Location = new Point(720, 153);
            TotalQuantity_TextBox.Margin = new Padding(4, 5, 4, 5);
            TotalQuantity_TextBox.MaxLength = 8;
            TotalQuantity_TextBox.Name = "TotalQuantity_TextBox";
            TotalQuantity_TextBox.PlaceholderText = "";
            TotalQuantity_TextBox.SelectedText = "";
            TotalQuantity_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges6;
            TotalQuantity_TextBox.ShortcutsEnabled = false;
            TotalQuantity_TextBox.Size = new Size(150, 50);
            TotalQuantity_TextBox.TabIndex = 3;
            TotalQuantity_TextBox.TextChanged += ValidateInputs;
            // 
            // RentalRate_Label
            // 
            RentalRate_Label.Anchor = AnchorStyles.Top;
            RentalRate_Label.AutoSize = true;
            RentalRate_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RentalRate_Label.Location = new Point(243, 256);
            RentalRate_Label.Name = "RentalRate_Label";
            RentalRate_Label.Size = new Size(124, 31);
            RentalRate_Label.TabIndex = 0;
            RentalRate_Label.Text = "Rental rate";
            // 
            // RentalRate_TextBox
            // 
            RentalRate_TextBox.Anchor = AnchorStyles.Top;
            RentalRate_TextBox.CustomizableEdges = customizableEdges7;
            RentalRate_TextBox.DefaultText = "";
            RentalRate_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            RentalRate_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            RentalRate_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            RentalRate_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            RentalRate_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            RentalRate_TextBox.Font = new Font("Segoe UI", 9F);
            RentalRate_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            RentalRate_TextBox.Location = new Point(243, 292);
            RentalRate_TextBox.Margin = new Padding(4, 5, 4, 5);
            RentalRate_TextBox.MaxLength = 8;
            RentalRate_TextBox.Name = "RentalRate_TextBox";
            RentalRate_TextBox.PlaceholderText = "";
            RentalRate_TextBox.SelectedText = "";
            RentalRate_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges8;
            RentalRate_TextBox.ShortcutsEnabled = false;
            RentalRate_TextBox.Size = new Size(150, 50);
            RentalRate_TextBox.TabIndex = 4;
            RentalRate_TextBox.TextChanged += ValidateInputs;
            // 
            // RateType_Label
            // 
            RateType_Label.Anchor = AnchorStyles.Top;
            RateType_Label.AutoSize = true;
            RateType_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            RateType_Label.Location = new Point(401, 256);
            RateType_Label.Name = "RateType_Label";
            RateType_Label.Size = new Size(111, 31);
            RateType_Label.TabIndex = 0;
            RateType_Label.Text = "Rate type";
            // 
            // RateType_ComboBox
            // 
            RateType_ComboBox.Anchor = AnchorStyles.Top;
            RateType_ComboBox.BackColor = Color.Transparent;
            RateType_ComboBox.CustomizableEdges = customizableEdges9;
            RateType_ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            RateType_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            RateType_ComboBox.FocusedColor = Color.FromArgb(94, 148, 255);
            RateType_ComboBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            RateType_ComboBox.Font = new Font("Segoe UI", 10F);
            RateType_ComboBox.ForeColor = Color.FromArgb(68, 88, 112);
            RateType_ComboBox.ItemHeight = 44;
            RateType_ComboBox.Items.AddRange(new object[] { "Day", "Week", "Month" });
            RateType_ComboBox.Location = new Point(401, 292);
            RateType_ComboBox.Name = "RateType_ComboBox";
            RateType_ComboBox.ShadowDecoration.CustomizableEdges = customizableEdges10;
            RateType_ComboBox.Size = new Size(150, 50);
            RateType_ComboBox.TabIndex = 5;
            // 
            // AddRentalItem_Button
            // 
            AddRentalItem_Button.Anchor = AnchorStyles.Top;
            AddRentalItem_Button.BackColor = Color.Transparent;
            AddRentalItem_Button.BorderColor = Color.LightGray;
            AddRentalItem_Button.BorderRadius = 2;
            AddRentalItem_Button.BorderThickness = 1;
            AddRentalItem_Button.CustomizableEdges = customizableEdges11;
            AddRentalItem_Button.Enabled = false;
            AddRentalItem_Button.FillColor = Color.White;
            AddRentalItem_Button.Font = new Font("Segoe UI", 10F);
            AddRentalItem_Button.ForeColor = Color.Black;
            AddRentalItem_Button.Location = new Point(382, 543);
            AddRentalItem_Button.Name = "AddRentalItem_Button";
            AddRentalItem_Button.ShadowDecoration.CustomizableEdges = customizableEdges12;
            AddRentalItem_Button.Size = new Size(214, 48);
            AddRentalItem_Button.TabIndex = 11;
            AddRentalItem_Button.Tag = "";
            AddRentalItem_Button.Text = "Add rental item";
            AddRentalItem_Button.Click += AddRentalItem_Button_Click;
            // 
            // WarningProduct_LinkLabel
            // 
            WarningProduct_LinkLabel.Anchor = AnchorStyles.Top;
            WarningProduct_LinkLabel.AutoSize = true;
            WarningProduct_LinkLabel.Font = new Font("Segoe UI", 10F);
            WarningProduct_LinkLabel.LinkArea = new LinkArea(19, 15);
            WarningProduct_LinkLabel.Location = new Point(456, 212);
            WarningProduct_LinkLabel.Name = "WarningProduct_LinkLabel";
            WarningProduct_LinkLabel.Size = new Size(322, 33);
            WarningProduct_LinkLabel.TabIndex = 4;
            WarningProduct_LinkLabel.TabStop = true;
            WarningProduct_LinkLabel.Text = "No products exist. Create one here";
            WarningProduct_LinkLabel.UseCompatibleTextRendering = true;
            WarningProduct_LinkLabel.LinkClicked += WarningProduct_LinkLabel_LinkClicked;
            // 
            // WarningProduct_PictureBox
            // 
            WarningProduct_PictureBox.Anchor = AnchorStyles.Top;
            WarningProduct_PictureBox.Image = Properties.Resources.ExclamationMark;
            WarningProduct_PictureBox.Location = new Point(422, 212);
            WarningProduct_PictureBox.Name = "WarningProduct_PictureBox";
            WarningProduct_PictureBox.Size = new Size(29, 28);
            WarningProduct_PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            WarningProduct_PictureBox.TabIndex = 41;
            WarningProduct_PictureBox.TabStop = false;
            // 
            // SecurityDeposit_Label
            // 
            SecurityDeposit_Label.Anchor = AnchorStyles.Top;
            SecurityDeposit_Label.AutoSize = true;
            SecurityDeposit_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SecurityDeposit_Label.Location = new Point(558, 256);
            SecurityDeposit_Label.Name = "SecurityDeposit_Label";
            SecurityDeposit_Label.Size = new Size(178, 31);
            SecurityDeposit_Label.TabIndex = 0;
            SecurityDeposit_Label.Text = "Security deposit";
            // 
            // SecurityDeposit_TextBox
            // 
            SecurityDeposit_TextBox.Anchor = AnchorStyles.Top;
            SecurityDeposit_TextBox.CustomizableEdges = customizableEdges13;
            SecurityDeposit_TextBox.DefaultText = "";
            SecurityDeposit_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            SecurityDeposit_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            SecurityDeposit_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            SecurityDeposit_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            SecurityDeposit_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            SecurityDeposit_TextBox.Font = new Font("Segoe UI", 9F);
            SecurityDeposit_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            SecurityDeposit_TextBox.Location = new Point(558, 292);
            SecurityDeposit_TextBox.Margin = new Padding(4, 5, 4, 5);
            SecurityDeposit_TextBox.MaxLength = 8;
            SecurityDeposit_TextBox.Name = "SecurityDeposit_TextBox";
            SecurityDeposit_TextBox.PlaceholderText = "";
            SecurityDeposit_TextBox.SelectedText = "";
            SecurityDeposit_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges14;
            SecurityDeposit_TextBox.ShortcutsEnabled = false;
            SecurityDeposit_TextBox.Size = new Size(150, 50);
            SecurityDeposit_TextBox.TabIndex = 9;
            SecurityDeposit_TextBox.TextChanged += ValidateInputs;
            // 
            // Notes_Label
            // 
            Notes_Label.Anchor = AnchorStyles.Top;
            Notes_Label.AutoSize = true;
            Notes_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Notes_Label.Location = new Point(400, 365);
            Notes_Label.Name = "Notes_Label";
            Notes_Label.Size = new Size(179, 31);
            Notes_Label.TabIndex = 0;
            Notes_Label.Text = "Notes (optional)";
            // 
            // Notes_TextBox
            // 
            Notes_TextBox.Anchor = AnchorStyles.Top;
            Notes_TextBox.AutoSize = true;
            Notes_TextBox.CustomizableEdges = customizableEdges15;
            Notes_TextBox.DefaultText = "";
            Notes_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            Notes_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            Notes_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            Notes_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            Notes_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            Notes_TextBox.Font = new Font("Segoe UI", 9F);
            Notes_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Notes_TextBox.Location = new Point(227, 403);
            Notes_TextBox.Margin = new Padding(6, 8, 6, 8);
            Notes_TextBox.MaxLength = 1000;
            Notes_TextBox.Multiline = true;
            Notes_TextBox.Name = "Notes_TextBox";
            Notes_TextBox.PlaceholderText = "";
            Notes_TextBox.SelectedText = "";
            Notes_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges16;
            Notes_TextBox.ShortcutsEnabled = false;
            Notes_TextBox.Size = new Size(525, 105);
            Notes_TextBox.TabIndex = 10;
            // 
            // AddRentalItem_Label
            // 
            AddRentalItem_Label.Anchor = AnchorStyles.Top;
            AddRentalItem_Label.AutoSize = true;
            AddRentalItem_Label.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AddRentalItem_Label.Location = new Point(368, 33);
            AddRentalItem_Label.Name = "AddRentalItem_Label";
            AddRentalItem_Label.Size = new Size(242, 45);
            AddRentalItem_Label.TabIndex = 42;
            AddRentalItem_Label.Text = "Add rental item";
            // 
            // AddRentalItem_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(978, 634);
            Controls.Add(AddRentalItem_Label);
            Controls.Add(Notes_Label);
            Controls.Add(Notes_TextBox);
            Controls.Add(SecurityDeposit_Label);
            Controls.Add(SecurityDeposit_TextBox);
            Controls.Add(WarningProduct_LinkLabel);
            Controls.Add(WarningProduct_PictureBox);
            Controls.Add(AddRentalItem_Button);
            Controls.Add(RateType_Label);
            Controls.Add(RateType_ComboBox);
            Controls.Add(RentalRate_Label);
            Controls.Add(RentalRate_TextBox);
            Controls.Add(TotalQuantity_Label);
            Controls.Add(TotalQuantity_TextBox);
            Controls.Add(ProductName_Label);
            Controls.Add(ProductName_TextBox);
            Controls.Add(RentalItemID_Label);
            Controls.Add(RentalItemID_TextBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            MinimumSize = new Size(1000, 690);
            Name = "AddRentalItem_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Activated += AddRentalItem_Form_Activated;
            FormClosed += AddRentalItem_Form_FormClosed;
            Shown += AddRentalItem_Form_Shown;
            Resize += AddRentalItem_Form_Resize;
            ((System.ComponentModel.ISupportInitialize)WarningProduct_PictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2TextBox RentalItemID_TextBox;
        private Label RentalItemID_Label;
        private Label ProductName_Label;
        public Guna.UI2.WinForms.Guna2TextBox ProductName_TextBox;
        private Label TotalQuantity_Label;
        private Guna.UI2.WinForms.Guna2TextBox TotalQuantity_TextBox;
        private Label RentalRate_Label;
        private Guna.UI2.WinForms.Guna2TextBox RentalRate_TextBox;
        private Label RateType_Label;
        private Guna.UI2.WinForms.Guna2ComboBox RateType_ComboBox;
        private Guna.UI2.WinForms.Guna2Button AddRentalItem_Button;
        private LinkLabel WarningProduct_LinkLabel;
        private PictureBox WarningProduct_PictureBox;
        private Label SecurityDeposit_Label;
        private Guna.UI2.WinForms.Guna2TextBox SecurityDeposit_TextBox;
        private Label Notes_Label;
        private Guna.UI2.WinForms.Guna2TextBox Notes_TextBox;
        private Label AddRentalItem_Label;
    }
}