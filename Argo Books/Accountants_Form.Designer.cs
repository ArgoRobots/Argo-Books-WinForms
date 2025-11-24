using Argo_Books.Classes;

namespace Argo_Books
{
    partial class Accountants_Form
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
            WarningAccountantName_PictureBox = new PictureBox();
            WarningAccountantName_Label = new Label();
            AccountantName_Label = new Label();
            Accountant_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            AddAccountant_Button = new Guna.UI2.WinForms.Guna2Button();
            AddAccountant_Label = new Label();
            Search_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            ShowingResultsFor_Label = new Label();
            Total_Label = new Label();
            ((System.ComponentModel.ISupportInitialize)WarningAccountantName_PictureBox).BeginInit();
            SuspendLayout();
            // 
            // WarningAccountantName_PictureBox
            // 
            WarningAccountantName_PictureBox.Anchor = AnchorStyles.Top;
            WarningAccountantName_PictureBox.Image = Properties.Resources.ExclamationMark;
            WarningAccountantName_PictureBox.Location = new Point(185, 207);
            WarningAccountantName_PictureBox.Name = "WarningAccountantName_PictureBox";
            WarningAccountantName_PictureBox.Size = new Size(28, 28);
            WarningAccountantName_PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            WarningAccountantName_PictureBox.TabIndex = 47;
            WarningAccountantName_PictureBox.TabStop = false;
            WarningAccountantName_PictureBox.Visible = false;
            // 
            // WarningAccountantName_Label
            // 
            WarningAccountantName_Label.Anchor = AnchorStyles.Top;
            WarningAccountantName_Label.AutoSize = true;
            WarningAccountantName_Label.Font = new Font("Segoe UI", 10F);
            WarningAccountantName_Label.Location = new Point(225, 207);
            WarningAccountantName_Label.Name = "WarningAccountantName_Label";
            WarningAccountantName_Label.Size = new Size(233, 28);
            WarningAccountantName_Label.TabIndex = 46;
            WarningAccountantName_Label.Text = "Accountant already exists";
            WarningAccountantName_Label.Visible = false;
            // 
            // AccountantName_Label
            // 
            AccountantName_Label.Anchor = AnchorStyles.Top;
            AccountantName_Label.AutoSize = true;
            AccountantName_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AccountantName_Label.Location = new Point(170, 114);
            AccountantName_Label.Name = "AccountantName_Label";
            AccountantName_Label.Size = new Size(194, 31);
            AccountantName_Label.TabIndex = 40;
            AccountantName_Label.Text = "Accountant name";
            // 
            // Accountant_TextBox
            // 
            Accountant_TextBox.Anchor = AnchorStyles.Top;
            Accountant_TextBox.CustomizableEdges = customizableEdges1;
            Accountant_TextBox.DefaultText = "";
            Accountant_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            Accountant_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            Accountant_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            Accountant_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            Accountant_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            Accountant_TextBox.Font = new Font("Segoe UI", 9F);
            Accountant_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            Accountant_TextBox.Location = new Point(170, 150);
            Accountant_TextBox.Margin = new Padding(4, 5, 4, 5);
            Accountant_TextBox.MaxLength = 32;
            Accountant_TextBox.Name = "Accountant_TextBox";
            Accountant_TextBox.PlaceholderText = "";
            Accountant_TextBox.SelectedText = "";
            Accountant_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Accountant_TextBox.ShortcutsEnabled = false;
            Accountant_TextBox.Size = new Size(300, 50);
            Accountant_TextBox.TabIndex = 42;
            Accountant_TextBox.Tag = "1";
            Accountant_TextBox.TextChanged += Accountant_TextBox_TextChanged;
            Accountant_TextBox.KeyDown += Accountant_TextBox_KeyDown;
            // 
            // AddAccountant_Button
            // 
            AddAccountant_Button.Anchor = AnchorStyles.Top;
            AddAccountant_Button.BackColor = Color.Transparent;
            AddAccountant_Button.BorderColor = Color.LightGray;
            AddAccountant_Button.BorderRadius = 2;
            AddAccountant_Button.BorderThickness = 1;
            AddAccountant_Button.CustomizableEdges = customizableEdges3;
            AddAccountant_Button.Enabled = false;
            AddAccountant_Button.FillColor = Color.White;
            AddAccountant_Button.Font = new Font("Segoe UI", 10F);
            AddAccountant_Button.ForeColor = Color.Black;
            AddAccountant_Button.Location = new Point(482, 150);
            AddAccountant_Button.Name = "AddAccountant_Button";
            AddAccountant_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            AddAccountant_Button.Size = new Size(215, 50);
            AddAccountant_Button.TabIndex = 43;
            AddAccountant_Button.Tag = "";
            AddAccountant_Button.Text = "Add accountant";
            AddAccountant_Button.Click += AddAccountant_Button_Click;
            // 
            // AddAccountant_Label
            // 
            AddAccountant_Label.Anchor = AnchorStyles.Top;
            AddAccountant_Label.AutoSize = true;
            AddAccountant_Label.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AddAccountant_Label.Location = new Point(467, 30);
            AddAccountant_Label.Name = "AddAccountant_Label";
            AddAccountant_Label.Size = new Size(245, 45);
            AddAccountant_Label.TabIndex = 41;
            AddAccountant_Label.Text = "Add accountant";
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
            Search_TextBox.IconLeftSize = new Size(22, 22);
            Search_TextBox.IconRight = Properties.Resources.CloseGray;
            Search_TextBox.IconRightOffset = new Point(5, 0);
            Search_TextBox.Location = new Point(709, 150);
            Search_TextBox.Margin = new Padding(4, 5, 4, 5);
            Search_TextBox.MaxLength = 32;
            Search_TextBox.Name = "Search_TextBox";
            Search_TextBox.PlaceholderText = "Search for accountants";
            Search_TextBox.SelectedText = "";
            Search_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Search_TextBox.ShortcutsEnabled = false;
            Search_TextBox.Size = new Size(300, 50);
            Search_TextBox.TabIndex = 48;
            Search_TextBox.IconRightClick += Search_TextBox_IconRightClick;
            Search_TextBox.TextChanged += Search_TextBox_TextChanged;
            // 
            // ShowingResultsFor_Label
            // 
            ShowingResultsFor_Label.Anchor = AnchorStyles.Top;
            ShowingResultsFor_Label.AutoSize = true;
            ShowingResultsFor_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ShowingResultsFor_Label.Location = new Point(485, 207);
            ShowingResultsFor_Label.Name = "ShowingResultsFor_Label";
            ShowingResultsFor_Label.Size = new Size(209, 31);
            ShowingResultsFor_Label.TabIndex = 49;
            ShowingResultsFor_Label.Text = "Showing results for";
            // 
            // Total_Label
            // 
            Total_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Total_Label.AutoSize = true;
            Total_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Total_Label.Location = new Point(946, 684);
            Total_Label.Name = "Total_Label";
            Total_Label.Size = new Size(68, 31);
            Total_Label.TabIndex = 50;
            Total_Label.Text = "Total:";
            // 
            // Accountants_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1178, 724);
            Controls.Add(Total_Label);
            Controls.Add(ShowingResultsFor_Label);
            Controls.Add(Search_TextBox);
            Controls.Add(WarningAccountantName_PictureBox);
            Controls.Add(WarningAccountantName_Label);
            Controls.Add(AccountantName_Label);
            Controls.Add(Accountant_TextBox);
            Controls.Add(AddAccountant_Button);
            Controls.Add(AddAccountant_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            KeyPreview = true;
            MinimumSize = new Size(1200, 780);
            Name = "Accountants_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += Accountants_Form_FormClosed;
            Shown += Accountants_Form_Shown;
            Resize += Accountants_Form_Resize;
            ((System.ComponentModel.ISupportInitialize)WarningAccountantName_PictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox WarningAccountantName_PictureBox;
        private Label WarningAccountantName_Label;
        private Label AccountantName_Label;
        private Guna.UI2.WinForms.Guna2TextBox Accountant_TextBox;
        private Guna.UI2.WinForms.Guna2Button AddAccountant_Button;
        private Label AddAccountant_Label;
        private Guna.UI2.WinForms.Guna2TextBox Search_TextBox;
        private Label ShowingResultsFor_Label;
        public Label Total_Label;
    }
}