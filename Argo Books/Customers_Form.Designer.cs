namespace Argo_Books
{
    partial class Customers_Form
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
            Total_Label = new Label();
            AddCustomer_Button = new Guna.UI2.WinForms.Guna2Button();
            ShowingResultsFor_Label = new Label();
            Search_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Title_Label = new Label();
            SuspendLayout();
            // 
            // Total_Label
            // 
            Total_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Total_Label.AutoSize = true;
            Total_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Total_Label.Location = new Point(1220, 824);
            Total_Label.Name = "Total_Label";
            Total_Label.Size = new Size(68, 31);
            Total_Label.TabIndex = 56;
            Total_Label.Text = "Total:";
            // 
            // AddCustomer_Button
            // 
            AddCustomer_Button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AddCustomer_Button.BorderRadius = 4;
            AddCustomer_Button.CustomizableEdges = customizableEdges1;
            AddCustomer_Button.Font = new Font("Segoe UI", 10F);
            AddCustomer_Button.ForeColor = Color.White;
            AddCustomer_Button.Location = new Point(758, 33);
            AddCustomer_Button.Margin = new Padding(4, 5, 4, 5);
            AddCustomer_Button.Name = "AddCustomer_Button";
            AddCustomer_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            AddCustomer_Button.Size = new Size(220, 50);
            AddCustomer_Button.TabIndex = 82;
            AddCustomer_Button.Text = "Add Customer";
            AddCustomer_Button.Click += AddCustomer_Button_Click;
            // 
            // ShowingResultsFor_Label
            // 
            ShowingResultsFor_Label.Anchor = AnchorStyles.Top;
            ShowingResultsFor_Label.AutoSize = true;
            ShowingResultsFor_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShowingResultsFor_Label.Location = new Point(547, 108);
            ShowingResultsFor_Label.Name = "ShowingResultsFor_Label";
            ShowingResultsFor_Label.Size = new Size(209, 31);
            ShowingResultsFor_Label.TabIndex = 83;
            ShowingResultsFor_Label.Text = "Showing results for";
            // 
            // Search_TextBox
            // 
            Search_TextBox.Anchor = AnchorStyles.Top;
            Search_TextBox.CustomizableEdges = customizableEdges3;
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
            Search_TextBox.Location = new Point(988, 33);
            Search_TextBox.Margin = new Padding(6, 8, 6, 8);
            Search_TextBox.MaxLength = 32;
            Search_TextBox.Name = "Search_TextBox";
            Search_TextBox.PlaceholderText = "Search for customers";
            Search_TextBox.SelectedText = "";
            Search_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Search_TextBox.ShortcutsEnabled = false;
            Search_TextBox.Size = new Size(300, 50);
            Search_TextBox.TabIndex = 84;
            Search_TextBox.IconRightClick += Search_TextBox_IconRightClick;
            Search_TextBox.TextChanged += Search_TextBox_TextChanged;
            // 
            // Title_Label
            // 
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 15.75F);
            Title_Label.Location = new Point(29, 33);
            Title_Label.Margin = new Padding(4, 0, 4, 0);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(296, 45);
            Title_Label.TabIndex = 85;
            Title_Label.Text = "Manage Customers";
            // 
            // Customers_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1303, 864);
            Controls.Add(Title_Label);
            Controls.Add(ShowingResultsFor_Label);
            Controls.Add(Search_TextBox);
            Controls.Add(AddCustomer_Button);
            Controls.Add(Total_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            MinimumSize = new Size(1325, 920);
            Name = "Customers_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += Customers_Form_FormClosed;
            Shown += Customers_Form_Shown;
            Resize += Customers_Form_Resize;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public Label Total_Label;
        private Guna.UI2.WinForms.Guna2Button AddCustomer_Button;
        private Label ShowingResultsFor_Label;
        private Guna.UI2.WinForms.Guna2TextBox Search_TextBox;
        private Label Title_Label;
    }
}