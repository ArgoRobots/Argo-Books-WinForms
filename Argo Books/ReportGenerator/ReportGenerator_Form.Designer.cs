
namespace Argo_Books.ReportGenerator
{
    partial class ReportGenerator_Form
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
            StepTitle_Label = new Label();
            Progress_ProgressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            ProgressValue_Label = new Label();
            BottomNavigation_Panel = new Panel();
            Previous_Button = new Guna.UI2.WinForms.Guna2Button();
            Next_Button = new Guna.UI2.WinForms.Guna2Button();
            Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            Content_Panel = new Guna.UI2.WinForms.Guna2Panel();
            Top_Panel = new Guna.UI2.WinForms.Guna2Panel();
            BottomNavigation_Panel.SuspendLayout();
            Top_Panel.SuspendLayout();
            SuspendLayout();
            // 
            // StepTitle_Label
            // 
            StepTitle_Label.AutoSize = true;
            StepTitle_Label.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            StepTitle_Label.ForeColor = Color.Black;
            StepTitle_Label.Location = new Point(32, 13);
            StepTitle_Label.Margin = new Padding(6, 0, 6, 0);
            StepTitle_Label.Name = "StepTitle_Label";
            StepTitle_Label.Size = new Size(235, 45);
            StepTitle_Label.TabIndex = 0;
            StepTitle_Label.Text = "Data Selection";
            // 
            // Progress_ProgressBar
            // 
            Progress_ProgressBar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Progress_ProgressBar.BorderRadius = 8;
            Progress_ProgressBar.CustomizableEdges = customizableEdges1;
            Progress_ProgressBar.FillColor = Color.White;
            Progress_ProgressBar.Location = new Point(1144, 19);
            Progress_ProgressBar.Margin = new Padding(6, 5, 6, 5);
            Progress_ProgressBar.Name = "Progress_ProgressBar";
            Progress_ProgressBar.ProgressColor = Color.FromArgb(94, 148, 255);
            Progress_ProgressBar.ProgressColor2 = Color.FromArgb(94, 148, 255);
            Progress_ProgressBar.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Progress_ProgressBar.Size = new Size(214, 33);
            Progress_ProgressBar.TabIndex = 1;
            Progress_ProgressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            Progress_ProgressBar.Value = 33;
            // 
            // ProgressValue_Label
            // 
            ProgressValue_Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ProgressValue_Label.AutoSize = true;
            ProgressValue_Label.Font = new Font("Segoe UI", 10F);
            ProgressValue_Label.ForeColor = Color.Black;
            ProgressValue_Label.Location = new Point(1373, 21);
            ProgressValue_Label.Margin = new Padding(6, 0, 6, 0);
            ProgressValue_Label.Name = "ProgressValue_Label";
            ProgressValue_Label.Size = new Size(52, 28);
            ProgressValue_Label.TabIndex = 2;
            ProgressValue_Label.Text = "1 / 3";
            // 
            // BottomNavigation_Panel
            // 
            BottomNavigation_Panel.BackColor = Color.LightGray;
            BottomNavigation_Panel.Controls.Add(Previous_Button);
            BottomNavigation_Panel.Controls.Add(Next_Button);
            BottomNavigation_Panel.Controls.Add(Cancel_Button);
            BottomNavigation_Panel.Dock = DockStyle.Bottom;
            BottomNavigation_Panel.Location = new Point(0, 964);
            BottomNavigation_Panel.Margin = new Padding(6, 5, 6, 5);
            BottomNavigation_Panel.Name = "BottomNavigation_Panel";
            BottomNavigation_Panel.Size = new Size(1478, 80);
            BottomNavigation_Panel.TabIndex = 2;
            // 
            // Previous_Button
            // 
            Previous_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Previous_Button.BorderColor = Color.DimGray;
            Previous_Button.BorderRadius = 2;
            Previous_Button.BorderThickness = 1;
            Previous_Button.CustomizableEdges = customizableEdges3;
            Previous_Button.DisabledState.BorderColor = Color.DarkGray;
            Previous_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Previous_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Previous_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Previous_Button.FillColor = Color.White;
            Previous_Button.Font = new Font("Segoe UI", 11F);
            Previous_Button.ForeColor = Color.Black;
            Previous_Button.Location = new Point(976, 16);
            Previous_Button.Margin = new Padding(6, 5, 6, 5);
            Previous_Button.Name = "Previous_Button";
            Previous_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Previous_Button.Size = new Size(150, 50);
            Previous_Button.TabIndex = 0;
            Previous_Button.Text = "Previous";
            Previous_Button.Click += Previous_Button_Click;
            // 
            // Next_Button
            // 
            Next_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Next_Button.BorderColor = Color.DimGray;
            Next_Button.BorderRadius = 2;
            Next_Button.BorderThickness = 1;
            Next_Button.CustomizableEdges = customizableEdges5;
            Next_Button.DisabledState.BorderColor = Color.DarkGray;
            Next_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Next_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Next_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Next_Button.FillColor = Color.White;
            Next_Button.Font = new Font("Segoe UI", 11F);
            Next_Button.ForeColor = Color.Black;
            Next_Button.Location = new Point(1138, 16);
            Next_Button.Margin = new Padding(6, 5, 6, 5);
            Next_Button.Name = "Next_Button";
            Next_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Next_Button.Size = new Size(150, 50);
            Next_Button.TabIndex = 1;
            Next_Button.Text = "Next";
            Next_Button.Click += Next_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.BorderColor = Color.DimGray;
            Cancel_Button.BorderRadius = 2;
            Cancel_Button.BorderThickness = 1;
            Cancel_Button.CustomizableEdges = customizableEdges7;
            Cancel_Button.DisabledState.BorderColor = Color.DarkGray;
            Cancel_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Cancel_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Cancel_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Cancel_Button.FillColor = Color.White;
            Cancel_Button.Font = new Font("Segoe UI", 11F);
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(1300, 16);
            Cancel_Button.Margin = new Padding(6, 5, 6, 5);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Cancel_Button.Size = new Size(150, 50);
            Cancel_Button.TabIndex = 2;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // Content_Panel
            // 
            Content_Panel.BackColor = Color.Transparent;
            Content_Panel.CustomizableEdges = customizableEdges9;
            Content_Panel.Dock = DockStyle.Fill;
            Content_Panel.FillColor = SystemColors.Control;
            Content_Panel.Location = new Point(0, 70);
            Content_Panel.Margin = new Padding(6, 5, 6, 5);
            Content_Panel.Name = "Content_Panel";
            Content_Panel.ShadowDecoration.CustomizableEdges = customizableEdges10;
            Content_Panel.Size = new Size(1478, 894);
            Content_Panel.TabIndex = 1;
            // 
            // Top_Panel
            // 
            Top_Panel.BackColor = Color.Transparent;
            Top_Panel.Controls.Add(StepTitle_Label);
            Top_Panel.Controls.Add(ProgressValue_Label);
            Top_Panel.Controls.Add(Progress_ProgressBar);
            Top_Panel.CustomizableEdges = customizableEdges11;
            Top_Panel.Dock = DockStyle.Top;
            Top_Panel.FillColor = Color.LightGray;
            Top_Panel.Location = new Point(0, 0);
            Top_Panel.Margin = new Padding(6, 5, 6, 5);
            Top_Panel.Name = "Top_Panel";
            Top_Panel.ShadowDecoration.CustomizableEdges = customizableEdges12;
            Top_Panel.Size = new Size(1478, 70);
            Top_Panel.TabIndex = 0;
            // 
            // ReportGenerator_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1478, 1044);
            Controls.Add(Content_Panel);
            Controls.Add(BottomNavigation_Panel);
            Controls.Add(Top_Panel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MinimumSize = new Size(1300, 900);
            Name = "ReportGenerator_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Report Generator";
            FormClosing += ReportGenerator_Form_FormClosing;
            Shown += ReportGenerator_Form_Shown;
            Resize += ReportGenerator_Form_Resize;
            BottomNavigation_Panel.ResumeLayout(false);
            Top_Panel.ResumeLayout(false);
            Top_Panel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        public System.Windows.Forms.Label StepTitle_Label;
        public Guna.UI2.WinForms.Guna2ProgressBar Progress_ProgressBar;
        public System.Windows.Forms.Label ProgressValue_Label;
        public System.Windows.Forms.Panel BottomNavigation_Panel;
        public Guna.UI2.WinForms.Guna2Button Previous_Button;
        public Guna.UI2.WinForms.Guna2Button Next_Button;
        public Guna.UI2.WinForms.Guna2Button Cancel_Button;
        public Guna.UI2.WinForms.Guna2Panel Content_Panel;
        public Guna.UI2.WinForms.Guna2Panel Top_Panel;
    }
}