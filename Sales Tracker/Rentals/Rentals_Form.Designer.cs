namespace Sales_Tracker
{
    partial class Rentals_Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Title_Label = new Label();
            AddRentalItem_Button = new Guna.UI2.WinForms.Guna2Button();
            Search_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            ShowingResultsFor_Label = new Label();
            Total_Label = new Label();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 15.75F);
            Title_Label.Location = new Point(29, 33);
            Title_Label.Margin = new Padding(4, 0, 4, 0);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(250, 45);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Rental Inventory";
            // 
            // AddRentalItem_Button
            // 
            AddRentalItem_Button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AddRentalItem_Button.BorderRadius = 4;
            AddRentalItem_Button.CustomizableEdges = customizableEdges5;
            AddRentalItem_Button.Font = new Font("Segoe UI", 10F);
            AddRentalItem_Button.ForeColor = Color.White;
            AddRentalItem_Button.Location = new Point(933, 33);
            AddRentalItem_Button.Margin = new Padding(4, 5, 4, 5);
            AddRentalItem_Button.Name = "AddRentalItem_Button";
            AddRentalItem_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            AddRentalItem_Button.Size = new Size(220, 50);
            AddRentalItem_Button.TabIndex = 1;
            AddRentalItem_Button.Text = "Add Rental Item";
            AddRentalItem_Button.Click += AddRentalItem_Button_Click;
            // 
            // Search_TextBox
            // 
            Search_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Search_TextBox.CustomizableEdges = customizableEdges7;
            Search_TextBox.DefaultText = "";
            Search_TextBox.Font = new Font("Segoe UI", 9F);
            Search_TextBox.IconRight = Properties.Resources.CloseGray;
            Search_TextBox.IconRightOffset = new Point(5, 0);
            Search_TextBox.IconRightSize = new Size(22, 22);
            Search_TextBox.Location = new Point(1163, 33);
            Search_TextBox.Margin = new Padding(6, 8, 6, 8);
            Search_TextBox.MaxLength = 100;
            Search_TextBox.Name = "Search_TextBox";
            Search_TextBox.PlaceholderText = "Search rental items";
            Search_TextBox.SelectedText = "";
            Search_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Search_TextBox.Size = new Size(300, 50);
            Search_TextBox.TabIndex = 2;
            Search_TextBox.IconRightClick += Search_TextBox_IconRightClick;
            Search_TextBox.TextChanged += Search_TextBox_TextChanged;
            // 
            // ShowingResultsFor_Label
            // 
            ShowingResultsFor_Label.Anchor = AnchorStyles.Top;
            ShowingResultsFor_Label.AutoSize = true;
            ShowingResultsFor_Label.Font = new Font("Segoe UI", 11.25F);
            ShowingResultsFor_Label.Location = new Point(633, 108);
            ShowingResultsFor_Label.Margin = new Padding(4, 0, 4, 0);
            ShowingResultsFor_Label.Name = "ShowingResultsFor_Label";
            ShowingResultsFor_Label.Size = new Size(209, 31);
            ShowingResultsFor_Label.TabIndex = 3;
            ShowingResultsFor_Label.Text = "Showing results for";
            ShowingResultsFor_Label.TextAlign = ContentAlignment.MiddleCenter;
            ShowingResultsFor_Label.Visible = false;
            // 
            // Total_Label
            // 
            Total_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Total_Label.AutoSize = true;
            Total_Label.Font = new Font("Segoe UI", 11F);
            Total_Label.Location = new Point(1399, 705);
            Total_Label.Margin = new Padding(4, 0, 4, 0);
            Total_Label.Name = "Total_Label";
            Total_Label.Size = new Size(64, 30);
            Total_Label.TabIndex = 4;
            Total_Label.Text = "Total:";
            // 
            // Rentals_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1478, 744);
            Controls.Add(Total_Label);
            Controls.Add(ShowingResultsFor_Label);
            Controls.Add(Search_TextBox);
            Controls.Add(AddRentalItem_Button);
            Controls.Add(Title_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1500, 600);
            Name = "Rentals_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += ManageRentals_Form_FormClosed;
            Shown += ManageRentals_Form_Shown;
            Resize += ManageRentals_Form_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private Guna.UI2.WinForms.Guna2Button AddRentalItem_Button;
        private Guna.UI2.WinForms.Guna2TextBox Search_TextBox;
        private Label ShowingResultsFor_Label;
        private Label Total_Label;
    }
}