using Argo_Books.Classes;

namespace Argo_Books.Settings.Menus
{
    partial class LanguageSelection_Form
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
            Title_Label = new Label();
            Instructions_Label = new Label();
            LanguagesPanel = new Panel();
            SelectAll_Button = new Guna.UI2.WinForms.Guna2Button();
            SelectNone_Button = new Guna.UI2.WinForms.Guna2Button();
            BrowseFolder_Button = new Guna.UI2.WinForms.Guna2Button();
            FolderPath_Label = new Label();
            Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            Generate_Button = new Guna.UI2.WinForms.Guna2Button();
            Status_Label = new Label();
            SelectMissing_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold);
            Title_Label.Location = new Point(329, 25);
            Title_Label.Margin = new Padding(4, 0, 4, 0);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(301, 40);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Generate Translations";
            Title_Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Instructions_Label
            // 
            Instructions_Label.Anchor = AnchorStyles.Top;
            Instructions_Label.AutoSize = true;
            Instructions_Label.Font = new Font("Segoe UI", 10F);
            Instructions_Label.Location = new Point(208, 81);
            Instructions_Label.Margin = new Padding(4, 0, 4, 0);
            Instructions_Label.Name = "Instructions_Label";
            Instructions_Label.Size = new Size(542, 56);
            Instructions_Label.TabIndex = 1;
            Instructions_Label.Text = "Select the languages you want to generate translations for.\nOnly new or changed text will be translated to save API costs.";
            Instructions_Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LanguagesPanel
            // 
            LanguagesPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LanguagesPanel.AutoScroll = true;
            LanguagesPanel.BorderStyle = BorderStyle.FixedSingle;
            LanguagesPanel.Location = new Point(25, 239);
            LanguagesPanel.Margin = new Padding(4);
            LanguagesPanel.Name = "LanguagesPanel";
            LanguagesPanel.Size = new Size(907, 413);
            LanguagesPanel.TabIndex = 2;
            // 
            // SelectAll_Button
            // 
            SelectAll_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SelectAll_Button.BorderColor = Color.LightGray;
            SelectAll_Button.BorderRadius = 2;
            SelectAll_Button.BorderThickness = 1;
            SelectAll_Button.CustomizableEdges = customizableEdges1;
            SelectAll_Button.FillColor = Color.FromArgb(250, 250, 250);
            SelectAll_Button.Font = new Font("Segoe UI", 9F);
            SelectAll_Button.ForeColor = Color.Black;
            SelectAll_Button.Location = new Point(25, 665);
            SelectAll_Button.Margin = new Padding(4);
            SelectAll_Button.Name = "SelectAll_Button";
            SelectAll_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            SelectAll_Button.Size = new Size(150, 40);
            SelectAll_Button.TabIndex = 3;
            SelectAll_Button.Text = "Select all";
            SelectAll_Button.Click += SelectAll_Button_Click;
            // 
            // SelectNone_Button
            // 
            SelectNone_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SelectNone_Button.BorderColor = Color.LightGray;
            SelectNone_Button.BorderRadius = 2;
            SelectNone_Button.BorderThickness = 1;
            SelectNone_Button.CustomizableEdges = customizableEdges3;
            SelectNone_Button.FillColor = Color.FromArgb(250, 250, 250);
            SelectNone_Button.Font = new Font("Segoe UI", 9F);
            SelectNone_Button.ForeColor = Color.Black;
            SelectNone_Button.Location = new Point(183, 665);
            SelectNone_Button.Margin = new Padding(4);
            SelectNone_Button.Name = "SelectNone_Button";
            SelectNone_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            SelectNone_Button.Size = new Size(150, 40);
            SelectNone_Button.TabIndex = 4;
            SelectNone_Button.Text = "Select none";
            SelectNone_Button.Click += SelectNone_Button_Click;
            // 
            // BrowseFolder_Button
            // 
            BrowseFolder_Button.BorderColor = Color.LightGray;
            BrowseFolder_Button.BorderRadius = 2;
            BrowseFolder_Button.BorderThickness = 1;
            BrowseFolder_Button.CustomizableEdges = customizableEdges5;
            BrowseFolder_Button.FillColor = Color.FromArgb(250, 250, 250);
            BrowseFolder_Button.Font = new Font("Segoe UI", 9F);
            BrowseFolder_Button.ForeColor = Color.Black;
            BrowseFolder_Button.Location = new Point(25, 170);
            BrowseFolder_Button.Margin = new Padding(4);
            BrowseFolder_Button.Name = "BrowseFolder_Button";
            BrowseFolder_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            BrowseFolder_Button.Size = new Size(188, 44);
            BrowseFolder_Button.TabIndex = 5;
            BrowseFolder_Button.Text = "Browse Folder...";
            BrowseFolder_Button.Click += BrowseFolder_Button_Click;
            // 
            // FolderPath_Label
            // 
            FolderPath_Label.AutoSize = true;
            FolderPath_Label.Font = new Font("Segoe UI", 9F);
            FolderPath_Label.Location = new Point(225, 180);
            FolderPath_Label.Margin = new Padding(4, 0, 4, 0);
            FolderPath_Label.Name = "FolderPath_Label";
            FolderPath_Label.Size = new Size(157, 25);
            FolderPath_Label.TabIndex = 6;
            FolderPath_Label.Text = "No folder selected";
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.BorderColor = Color.LightGray;
            Cancel_Button.BorderRadius = 2;
            Cancel_Button.BorderThickness = 1;
            Cancel_Button.CustomizableEdges = customizableEdges7;
            Cancel_Button.FillColor = Color.FromArgb(250, 250, 250);
            Cancel_Button.Font = new Font("Segoe UI", 9F);
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(604, 726);
            Cancel_Button.Margin = new Padding(4);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Cancel_Button.Size = new Size(160, 50);
            Cancel_Button.TabIndex = 7;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // Generate_Button
            // 
            Generate_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Generate_Button.BorderColor = Color.LightGray;
            Generate_Button.BorderRadius = 2;
            Generate_Button.BorderThickness = 1;
            Generate_Button.CustomizableEdges = customizableEdges9;
            Generate_Button.FillColor = Color.FromArgb(250, 250, 250);
            Generate_Button.Font = new Font("Segoe UI", 9F);
            Generate_Button.ForeColor = Color.Black;
            Generate_Button.Location = new Point(772, 726);
            Generate_Button.Margin = new Padding(4);
            Generate_Button.Name = "Generate_Button";
            Generate_Button.ShadowDecoration.CustomizableEdges = customizableEdges10;
            Generate_Button.Size = new Size(160, 50);
            Generate_Button.TabIndex = 8;
            Generate_Button.Text = "Generate";
            Generate_Button.Click += Generate_Button_Click;
            // 
            // Status_Label
            // 
            Status_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Status_Label.AutoSize = true;
            Status_Label.Font = new Font("Segoe UI", 9F);
            Status_Label.Location = new Point(25, 727);
            Status_Label.Margin = new Padding(4, 0, 4, 0);
            Status_Label.Name = "Status_Label";
            Status_Label.Size = new Size(0, 25);
            Status_Label.TabIndex = 9;
            // 
            // SelectMissing_Button
            // 
            SelectMissing_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SelectMissing_Button.BorderColor = Color.LightGray;
            SelectMissing_Button.BorderRadius = 2;
            SelectMissing_Button.BorderThickness = 1;
            SelectMissing_Button.CustomizableEdges = customizableEdges11;
            SelectMissing_Button.FillColor = Color.FromArgb(250, 250, 250);
            SelectMissing_Button.Font = new Font("Segoe UI", 9F);
            SelectMissing_Button.ForeColor = Color.Black;
            SelectMissing_Button.Location = new Point(341, 665);
            SelectMissing_Button.Margin = new Padding(4);
            SelectMissing_Button.Name = "SelectMissing_Button";
            SelectMissing_Button.ShadowDecoration.CustomizableEdges = customizableEdges12;
            SelectMissing_Button.Size = new Size(150, 40);
            SelectMissing_Button.TabIndex = 10;
            SelectMissing_Button.Text = "Select missing";
            SelectMissing_Button.Click += SelectMissing_Button_Click;
            // 
            // LanguageSelection_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(958, 789);
            Controls.Add(SelectMissing_Button);
            Controls.Add(Status_Label);
            Controls.Add(Generate_Button);
            Controls.Add(Cancel_Button);
            Controls.Add(FolderPath_Label);
            Controls.Add(BrowseFolder_Button);
            Controls.Add(SelectNone_Button);
            Controls.Add(SelectAll_Button);
            Controls.Add(LanguagesPanel);
            Controls.Add(Instructions_Label);
            Controls.Add(Title_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(980, 845);
            Name = "LanguageSelection_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private Label Instructions_Label;
        private Panel LanguagesPanel;
        private Guna.UI2.WinForms.Guna2Button SelectAll_Button;
        private Guna.UI2.WinForms.Guna2Button SelectNone_Button;
        private Guna.UI2.WinForms.Guna2Button BrowseFolder_Button;
        private Label FolderPath_Label;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
        private Guna.UI2.WinForms.Guna2Button Generate_Button;
        private Label Status_Label;
        private Guna.UI2.WinForms.Guna2Button SelectMissing_Button;
    }
}