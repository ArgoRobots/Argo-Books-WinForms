namespace Argo_Books.Rentals
{
    partial class CurrentRentals_Form
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Title_Label = new Label();
            FilterOverdue_CheckBox = new Guna.UI2.WinForms.Guna2CustomCheckBox();
            FilterOverdue_Label = new Label();
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
            Title_Label.Size = new Size(237, 45);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Current Rentals";
            // 
            // FilterOverdue_CheckBox
            // 
            FilterOverdue_CheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            FilterOverdue_CheckBox.Animated = true;
            FilterOverdue_CheckBox.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            FilterOverdue_CheckBox.CheckedState.BorderRadius = 2;
            FilterOverdue_CheckBox.CheckedState.BorderThickness = 0;
            FilterOverdue_CheckBox.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            FilterOverdue_CheckBox.CustomizableEdges = customizableEdges1;
            FilterOverdue_CheckBox.Location = new Point(898, 49);
            FilterOverdue_CheckBox.Name = "FilterOverdue_CheckBox";
            FilterOverdue_CheckBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            FilterOverdue_CheckBox.Size = new Size(20, 20);
            FilterOverdue_CheckBox.TabIndex = 1;
            FilterOverdue_CheckBox.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            FilterOverdue_CheckBox.UncheckedState.BorderRadius = 2;
            FilterOverdue_CheckBox.UncheckedState.BorderThickness = 0;
            FilterOverdue_CheckBox.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            FilterOverdue_CheckBox.CheckedChanged += FilterOverdue_CheckBox_CheckedChanged;
            // 
            // FilterOverdue_Label
            // 
            FilterOverdue_Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            FilterOverdue_Label.AutoSize = true;
            FilterOverdue_Label.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FilterOverdue_Label.Location = new Point(916, 40);
            FilterOverdue_Label.Name = "FilterOverdue_Label";
            FilterOverdue_Label.Padding = new Padding(5);
            FilterOverdue_Label.Size = new Size(196, 38);
            FilterOverdue_Label.TabIndex = 2;
            FilterOverdue_Label.Text = "Show Overdue Only";
            FilterOverdue_Label.Click += FilterOverdue_Label_Click;
            // 
            // Search_TextBox
            // 
            Search_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Search_TextBox.CustomizableEdges = customizableEdges3;
            Search_TextBox.DefaultText = "";
            Search_TextBox.Font = new Font("Segoe UI", 9F);
            Search_TextBox.IconRight = Properties.Resources.CloseGray;
            Search_TextBox.IconRightOffset = new Point(5, 0);
            Search_TextBox.IconRightSize = new Size(22, 22);
            Search_TextBox.Location = new Point(1133, 33);
            Search_TextBox.Margin = new Padding(6, 8, 6, 8);
            Search_TextBox.MaxLength = 100;
            Search_TextBox.Name = "Search_TextBox";
            Search_TextBox.PlaceholderText = "Search current rentals";
            Search_TextBox.SelectedText = "";
            Search_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
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
            Total_Label.Location = new Point(1369, 705);
            Total_Label.Margin = new Padding(4, 0, 4, 0);
            Total_Label.Name = "Total_Label";
            Total_Label.Size = new Size(64, 30);
            Total_Label.TabIndex = 4;
            Total_Label.Text = "Total:";
            // 
            // CurrentRentals_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1448, 744);
            Controls.Add(FilterOverdue_CheckBox);
            Controls.Add(Total_Label);
            Controls.Add(ShowingResultsFor_Label);
            Controls.Add(Search_TextBox);
            Controls.Add(FilterOverdue_Label);
            Controls.Add(Title_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1470, 600);
            Name = "CurrentRentals_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += CurrentRentals_Form_FormClosed;
            Shown += CurrentRentals_Form_Shown;
            Resize += CurrentRentals_Form_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private Guna.UI2.WinForms.Guna2CustomCheckBox FilterOverdue_CheckBox;
        private Label FilterOverdue_Label;
        private Guna.UI2.WinForms.Guna2TextBox Search_TextBox;
        private Label ShowingResultsFor_Label;
        private Label Total_Label;
    }
}
