using Sales_Tracker.Classes;

namespace Sales_Tracker
{
    partial class Categories_Form
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
            Category_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            AddCategory_Button = new Guna.UI2.WinForms.Guna2Button();
            AddCategory_Label = new Label();
            CategoryName_Label = new Label();
            WarningCategoryName_PictureBox = new PictureBox();
            WarningCategoryName_Label = new Label();
            Search_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            ShowingResultsFor_Label = new Label();
            Purchase_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            ForPurchase_Label = new Label();
            ForSale_Label = new Label();
            Sale_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            Total_Label = new Label();
            Rent_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            ForRent_Label = new Label();
            ((System.ComponentModel.ISupportInitialize)WarningCategoryName_PictureBox).BeginInit();
            SuspendLayout();
            // 
            // Category_TextBox
            // 
            Category_TextBox.Anchor = AnchorStyles.Top;
            Category_TextBox.CustomizableEdges = customizableEdges1;
            Category_TextBox.DefaultText = "";
            Category_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            Category_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            Category_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            Category_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            Category_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            Category_TextBox.Font = new Font("Segoe UI", 9F);
            Category_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Category_TextBox.Location = new Point(175, 150);
            Category_TextBox.Margin = new Padding(4, 5, 4, 5);
            Category_TextBox.MaxLength = 32;
            Category_TextBox.Name = "Category_TextBox";
            Category_TextBox.PlaceholderText = "";
            Category_TextBox.SelectedText = "";
            Category_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Category_TextBox.ShortcutsEnabled = false;
            Category_TextBox.Size = new Size(300, 50);
            Category_TextBox.TabIndex = 1;
            Category_TextBox.TextChanged += Category_TextBox_TextChanged;
            Category_TextBox.KeyDown += Category_TextBox_KeyDown;
            // 
            // AddCategory_Button
            // 
            AddCategory_Button.Anchor = AnchorStyles.Top;
            AddCategory_Button.BackColor = Color.Transparent;
            AddCategory_Button.BorderColor = Color.LightGray;
            AddCategory_Button.BorderRadius = 2;
            AddCategory_Button.BorderThickness = 1;
            AddCategory_Button.CustomizableEdges = customizableEdges3;
            AddCategory_Button.Enabled = false;
            AddCategory_Button.FillColor = Color.White;
            AddCategory_Button.Font = new Font("Segoe UI", 10F);
            AddCategory_Button.ForeColor = Color.Black;
            AddCategory_Button.Location = new Point(482, 150);
            AddCategory_Button.Name = "AddCategory_Button";
            AddCategory_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            AddCategory_Button.Size = new Size(215, 50);
            AddCategory_Button.TabIndex = 2;
            AddCategory_Button.Tag = "";
            AddCategory_Button.Text = "Add category";
            AddCategory_Button.Click += AddCategory_Button_Click;
            // 
            // AddCategory_Label
            // 
            AddCategory_Label.Anchor = AnchorStyles.Top;
            AddCategory_Label.AutoSize = true;
            AddCategory_Label.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AddCategory_Label.Location = new Point(484, 30);
            AddCategory_Label.Name = "AddCategory_Label";
            AddCategory_Label.Size = new Size(211, 45);
            AddCategory_Label.TabIndex = 0;
            AddCategory_Label.Text = "Add category";
            // 
            // CategoryName_Label
            // 
            CategoryName_Label.Anchor = AnchorStyles.Top;
            CategoryName_Label.AutoSize = true;
            CategoryName_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CategoryName_Label.Location = new Point(175, 111);
            CategoryName_Label.Name = "CategoryName_Label";
            CategoryName_Label.Size = new Size(169, 31);
            CategoryName_Label.TabIndex = 0;
            CategoryName_Label.Text = "Category name";
            // 
            // WarningCategoryName_PictureBox
            // 
            WarningCategoryName_PictureBox.Anchor = AnchorStyles.Top;
            WarningCategoryName_PictureBox.Image = Properties.Resources.ExclamationMark;
            WarningCategoryName_PictureBox.Location = new Point(187, 208);
            WarningCategoryName_PictureBox.Name = "WarningCategoryName_PictureBox";
            WarningCategoryName_PictureBox.Size = new Size(28, 28);
            WarningCategoryName_PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            WarningCategoryName_PictureBox.TabIndex = 39;
            WarningCategoryName_PictureBox.TabStop = false;
            WarningCategoryName_PictureBox.Visible = false;
            // 
            // WarningCategoryName_Label
            // 
            WarningCategoryName_Label.Anchor = AnchorStyles.Top;
            WarningCategoryName_Label.AutoSize = true;
            WarningCategoryName_Label.Font = new Font("Segoe UI", 10F);
            WarningCategoryName_Label.Location = new Point(221, 208);
            WarningCategoryName_Label.Name = "WarningCategoryName_Label";
            WarningCategoryName_Label.Size = new Size(213, 28);
            WarningCategoryName_Label.TabIndex = 38;
            WarningCategoryName_Label.Text = "Category already exists";
            WarningCategoryName_Label.Visible = false;
            // 
            // Search_TextBox
            // 
            Search_TextBox.Anchor = AnchorStyles.Top;
            Search_TextBox.CustomizableEdges = customizableEdges5;
            Search_TextBox.DefaultText = "";
            Search_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            Search_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            Search_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            Search_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            Search_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            Search_TextBox.Font = new Font("Segoe UI", 9F);
            Search_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Search_TextBox.IconRight = Properties.Resources.CloseGray;
            Search_TextBox.IconRightOffset = new Point(5, 0);
            Search_TextBox.IconRightSize = new Size(22, 22);
            Search_TextBox.Location = new Point(704, 150);
            Search_TextBox.Margin = new Padding(4, 5, 4, 5);
            Search_TextBox.MaxLength = 32;
            Search_TextBox.Name = "Search_TextBox";
            Search_TextBox.PlaceholderText = "Search for categories";
            Search_TextBox.SelectedText = "";
            Search_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Search_TextBox.ShortcutsEnabled = false;
            Search_TextBox.Size = new Size(300, 50);
            Search_TextBox.TabIndex = 49;
            Search_TextBox.IconRightClick += Search_TextBox_IconRightClick;
            Search_TextBox.TextChanged += Search_TextBox_TextChanged;
            // 
            // ShowingResultsFor_Label
            // 
            ShowingResultsFor_Label.Anchor = AnchorStyles.Top;
            ShowingResultsFor_Label.AutoSize = true;
            ShowingResultsFor_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShowingResultsFor_Label.Location = new Point(485, 209);
            ShowingResultsFor_Label.Name = "ShowingResultsFor_Label";
            ShowingResultsFor_Label.Size = new Size(209, 31);
            ShowingResultsFor_Label.TabIndex = 50;
            ShowingResultsFor_Label.Text = "Showing results for";
            // 
            // Purchase_RadioButton
            // 
            Purchase_RadioButton.Animated = true;
            Purchase_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Purchase_RadioButton.CheckedState.BorderThickness = 0;
            Purchase_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Purchase_RadioButton.CheckedState.InnerColor = Color.White;
            Purchase_RadioButton.Location = new Point(20, 42);
            Purchase_RadioButton.Name = "Purchase_RadioButton";
            Purchase_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges7;
            Purchase_RadioButton.Size = new Size(25, 25);
            Purchase_RadioButton.TabIndex = 51;
            Purchase_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Purchase_RadioButton.UncheckedState.BorderThickness = 2;
            Purchase_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Purchase_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            Purchase_RadioButton.CheckedChanged += Purchase_RadioButton_CheckedChanged;
            // 
            // ForPurchase_Label
            // 
            ForPurchase_Label.AutoSize = true;
            ForPurchase_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForPurchase_Label.Location = new Point(43, 34);
            ForPurchase_Label.Name = "ForPurchase_Label";
            ForPurchase_Label.Padding = new Padding(5);
            ForPurchase_Label.Size = new Size(155, 41);
            ForPurchase_Label.TabIndex = 52;
            ForPurchase_Label.Text = "For purchase";
            // 
            // ForSale_Label
            // 
            ForSale_Label.AutoSize = true;
            ForSale_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForSale_Label.Location = new Point(227, 34);
            ForSale_Label.Name = "ForSale_Label";
            ForSale_Label.Padding = new Padding(5);
            ForSale_Label.Size = new Size(102, 41);
            ForSale_Label.TabIndex = 54;
            ForSale_Label.Text = "For sale";
            // 
            // Sale_RadioButton
            // 
            Sale_RadioButton.Animated = true;
            Sale_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Sale_RadioButton.CheckedState.BorderThickness = 0;
            Sale_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Sale_RadioButton.CheckedState.InnerColor = Color.White;
            Sale_RadioButton.Location = new Point(204, 42);
            Sale_RadioButton.Name = "Sale_RadioButton";
            Sale_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Sale_RadioButton.Size = new Size(25, 25);
            Sale_RadioButton.TabIndex = 53;
            Sale_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Sale_RadioButton.UncheckedState.BorderThickness = 2;
            Sale_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Sale_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            Sale_RadioButton.CheckedChanged += Sale_RadioButton_CheckedChanged;
            // 
            // Total_Label
            // 
            Total_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Total_Label.AutoSize = true;
            Total_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Total_Label.Location = new Point(936, 684);
            Total_Label.Name = "Total_Label";
            Total_Label.Size = new Size(68, 31);
            Total_Label.TabIndex = 55;
            Total_Label.Text = "Total:";
            // 
            // Rent_RadioButton
            // 
            Rent_RadioButton.Animated = true;
            Rent_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Rent_RadioButton.CheckedState.BorderThickness = 0;
            Rent_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Rent_RadioButton.CheckedState.InnerColor = Color.White;
            Rent_RadioButton.Location = new Point(335, 42);
            Rent_RadioButton.Name = "Rent_RadioButton";
            Rent_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges9;
            Rent_RadioButton.Size = new Size(25, 25);
            Rent_RadioButton.TabIndex = 62;
            Rent_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Rent_RadioButton.UncheckedState.BorderThickness = 2;
            Rent_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Rent_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            Rent_RadioButton.CheckedChanged += Rent_RadioButton_CheckedChanged;
            // 
            // ForRent_Label
            // 
            ForRent_Label.AutoSize = true;
            ForRent_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForRent_Label.Location = new Point(358, 34);
            ForRent_Label.Name = "ForRent_Label";
            ForRent_Label.Padding = new Padding(5);
            ForRent_Label.Size = new Size(103, 41);
            ForRent_Label.TabIndex = 61;
            ForRent_Label.Text = "For rent";
            // 
            // Categories_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1178, 724);
            Controls.Add(Rent_RadioButton);
            Controls.Add(ForRent_Label);
            Controls.Add(Total_Label);
            Controls.Add(Sale_RadioButton);
            Controls.Add(Purchase_RadioButton);
            Controls.Add(ForSale_Label);
            Controls.Add(ForPurchase_Label);
            Controls.Add(ShowingResultsFor_Label);
            Controls.Add(Search_TextBox);
            Controls.Add(WarningCategoryName_PictureBox);
            Controls.Add(WarningCategoryName_Label);
            Controls.Add(CategoryName_Label);
            Controls.Add(Category_TextBox);
            Controls.Add(AddCategory_Button);
            Controls.Add(AddCategory_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            MinimumSize = new Size(1200, 780);
            Name = "Categories_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += Categories_Form_FormClosed;
            Shown += Categories_Form_Shown;
            Resize += Categories_Form_Resize;
            ((System.ComponentModel.ISupportInitialize)WarningCategoryName_PictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Guna.UI2.WinForms.Guna2TextBox Category_TextBox;
        private Guna.UI2.WinForms.Guna2Button AddCategory_Button;
        private Label AddCategory_Label;
        private Label CategoryName_Label;
        private PictureBox WarningCategoryName_PictureBox;
        private Label WarningCategoryName_Label;
        private Guna.UI2.WinForms.Guna2TextBox Search_TextBox;
        private Label ShowingResultsFor_Label;
        private Label ForPurchase_Label;
        private Label ForSale_Label;
        private Guna.UI2.WinForms.Guna2CustomRadioButton Sale_RadioButton;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Purchase_RadioButton;
        public Label Total_Label;
        private Label ForRent_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Rent_RadioButton;
    }
}