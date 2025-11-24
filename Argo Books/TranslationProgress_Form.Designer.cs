namespace Argo_Books
{
    partial class TranslationProgress_Form
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
            Title_Label = new Label();
            Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            Operation_Label = new Label();
            CurrentLanguage_Label = new Label();
            ProgressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            ProgressStats_Label = new Label();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Title_Label.Location = new Point(238, 19);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(350, 45);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Generating translations";
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.BackColor = Color.Transparent;
            Cancel_Button.BorderColor = Color.LightGray;
            Cancel_Button.BorderRadius = 2;
            Cancel_Button.BorderThickness = 1;
            Cancel_Button.CustomizableEdges = customizableEdges1;
            Cancel_Button.FillColor = Color.White;
            Cancel_Button.Font = new Font("Segoe UI", 10F);
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(626, 467);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Cancel_Button.Size = new Size(200, 45);
            Cancel_Button.TabIndex = 1;
            Cancel_Button.Tag = "";
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += CancelButton_Click;
            // 
            // Operation_Label
            // 
            Operation_Label.Anchor = AnchorStyles.Top;
            Operation_Label.AutoSize = true;
            Operation_Label.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Operation_Label.Location = new Point(89, 164);
            Operation_Label.Name = "Operation_Label";
            Operation_Label.Size = new Size(122, 30);
            Operation_Label.TabIndex = 0;
            Operation_Label.Text = "Preparing...";
            // 
            // CurrentLanguage_Label
            // 
            CurrentLanguage_Label.Anchor = AnchorStyles.Top;
            CurrentLanguage_Label.AutoSize = true;
            CurrentLanguage_Label.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CurrentLanguage_Label.Location = new Point(89, 210);
            CurrentLanguage_Label.Name = "CurrentLanguage_Label";
            CurrentLanguage_Label.Size = new Size(310, 30);
            CurrentLanguage_Label.TabIndex = 0;
            CurrentLanguage_Label.Text = "Initializing translation process...";
            // 
            // ProgressBar
            // 
            ProgressBar.Anchor = AnchorStyles.Top;
            ProgressBar.BorderRadius = 3;
            ProgressBar.CustomizableEdges = customizableEdges3;
            ProgressBar.Location = new Point(89, 261);
            ProgressBar.Name = "ProgressBar";
            ProgressBar.ShadowDecoration.CustomizableEdges = customizableEdges4;
            ProgressBar.Size = new Size(646, 30);
            ProgressBar.TabIndex = 0;
            ProgressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // ProgressStats_Label
            // 
            ProgressStats_Label.Anchor = AnchorStyles.Top;
            ProgressStats_Label.AutoSize = true;
            ProgressStats_Label.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ProgressStats_Label.Location = new Point(89, 304);
            ProgressStats_Label.Name = "ProgressStats_Label";
            ProgressStats_Label.Size = new Size(283, 30);
            ProgressStats_Label.TabIndex = 2;
            ProgressStats_Label.Text = "0 / 0 translations completed";
            // 
            // TranslationProgress_Form
            // 
            AutoScaleDimensions = new SizeF(144F, 144F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(838, 524);
            Controls.Add(ProgressStats_Label);
            Controls.Add(ProgressBar);
            Controls.Add(CurrentLanguage_Label);
            Controls.Add(Operation_Label);
            Controls.Add(Cancel_Button);
            Controls.Add(Title_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(860, 580);
            Name = "TranslationProgress_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            FormClosing += TranslationProgress_Form_FormClosing;
            Shown += TranslationProgress_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
        private Label Operation_Label;
        private Label CurrentLanguage_Label;
        private Guna.UI2.WinForms.Guna2ProgressBar ProgressBar;
        private Label ProgressStats_Label;
    }
}