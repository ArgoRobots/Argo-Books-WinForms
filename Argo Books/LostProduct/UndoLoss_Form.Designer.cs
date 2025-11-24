using Guna.UI2.WinForms;

namespace Argo_Books.LostProduct
{
    partial class UndoLoss_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label Title_Label;
        private Label TransactionInfo_Label;
        private Label LossInfo_Label;
        private Label UndoReason_Label;
        private Guna2TextBox UndoReason_TextBox;
        private Guna2Button UndoLoss_Button;
        private Guna2Button Cancel_Button;

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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Title_Label = new Label();
            TransactionInfo_Label = new Label();
            LossInfo_Label = new Label();
            UndoReason_Label = new Label();
            UndoReason_TextBox = new Guna2TextBox();
            UndoLoss_Button = new Guna2Button();
            Cancel_Button = new Guna2Button();
            TransactionDetails_Label = new Label();
            LossInformation_Label = new Label();
            CharacterCount_Label = new Label();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Title_Label.Location = new Point(232, 30);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(289, 45);
            Title_Label.TabIndex = 1;
            Title_Label.Text = "Undo Product Loss";
            // 
            // TransactionInfo_Label
            // 
            TransactionInfo_Label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TransactionInfo_Label.Font = new Font("Segoe UI", 11F);
            TransactionInfo_Label.Location = new Point(72, 125);
            TransactionInfo_Label.Name = "TransactionInfo_Label";
            TransactionInfo_Label.Size = new Size(609, 145);
            TransactionInfo_Label.TabIndex = 2;
            TransactionInfo_Label.Text = "Transaction info...";
            // 
            // LossInfo_Label
            // 
            LossInfo_Label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            LossInfo_Label.AutoEllipsis = true;
            LossInfo_Label.Font = new Font("Segoe UI", 11F);
            LossInfo_Label.Location = new Point(72, 325);
            LossInfo_Label.Name = "LossInfo_Label";
            LossInfo_Label.Size = new Size(609, 100);
            LossInfo_Label.TabIndex = 3;
            LossInfo_Label.Text = "Loss info...";
            // 
            // UndoReason_Label
            // 
            UndoReason_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UndoReason_Label.AutoSize = true;
            UndoReason_Label.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UndoReason_Label.Location = new Point(42, 438);
            UndoReason_Label.Name = "UndoReason_Label";
            UndoReason_Label.Size = new Size(348, 30);
            UndoReason_Label.TabIndex = 4;
            UndoReason_Label.Text = "Reason for undoing loss (optional):";
            // 
            // UndoReason_TextBox
            // 
            UndoReason_TextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UndoReason_TextBox.BorderRadius = 6;
            UndoReason_TextBox.Cursor = Cursors.IBeam;
            UndoReason_TextBox.CustomizableEdges = customizableEdges7;
            UndoReason_TextBox.DefaultText = "";
            UndoReason_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            UndoReason_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            UndoReason_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            UndoReason_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            UndoReason_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            UndoReason_TextBox.Font = new Font("Segoe UI", 9F);
            UndoReason_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            UndoReason_TextBox.Location = new Point(42, 476);
            UndoReason_TextBox.Margin = new Padding(6, 8, 6, 8);
            UndoReason_TextBox.MaxLength = 500;
            UndoReason_TextBox.Multiline = true;
            UndoReason_TextBox.Name = "UndoReason_TextBox";
            UndoReason_TextBox.PlaceholderText = "";
            UndoReason_TextBox.SelectedText = "";
            UndoReason_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges8;
            UndoReason_TextBox.Size = new Size(670, 120);
            UndoReason_TextBox.TabIndex = 5;
            UndoReason_TextBox.TextChanged += UndoReason_TextBox_TextChanged;
            // 
            // UndoLoss_Button
            // 
            UndoLoss_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            UndoLoss_Button.BorderColor = Color.LightGray;
            UndoLoss_Button.BorderRadius = 2;
            UndoLoss_Button.BorderThickness = 1;
            UndoLoss_Button.CustomizableEdges = customizableEdges9;
            UndoLoss_Button.DisabledState.BorderColor = Color.DarkGray;
            UndoLoss_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            UndoLoss_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            UndoLoss_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            UndoLoss_Button.Enabled = false;
            UndoLoss_Button.FillColor = Color.White;
            UndoLoss_Button.Font = new Font("Segoe UI", 10F);
            UndoLoss_Button.ForeColor = Color.Black;
            UndoLoss_Button.Location = new Point(540, 655);
            UndoLoss_Button.Margin = new Padding(4, 5, 4, 5);
            UndoLoss_Button.Name = "UndoLoss_Button";
            UndoLoss_Button.ShadowDecoration.CustomizableEdges = customizableEdges10;
            UndoLoss_Button.Size = new Size(200, 45);
            UndoLoss_Button.TabIndex = 6;
            UndoLoss_Button.Text = "Undo Loss";
            UndoLoss_Button.Click += UndoLoss_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.BorderColor = Color.LightGray;
            Cancel_Button.BorderRadius = 2;
            Cancel_Button.BorderThickness = 1;
            Cancel_Button.CustomizableEdges = customizableEdges11;
            Cancel_Button.DisabledState.BorderColor = Color.DarkGray;
            Cancel_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Cancel_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Cancel_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Cancel_Button.FillColor = Color.White;
            Cancel_Button.Font = new Font("Segoe UI", 10F);
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(332, 655);
            Cancel_Button.Margin = new Padding(4, 5, 4, 5);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges12;
            Cancel_Button.Size = new Size(200, 45);
            Cancel_Button.TabIndex = 7;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // TransactionDetails_Label
            // 
            TransactionDetails_Label.AutoSize = true;
            TransactionDetails_Label.Font = new Font("Segoe UI", 11F);
            TransactionDetails_Label.Location = new Point(42, 90);
            TransactionDetails_Label.Name = "TransactionDetails_Label";
            TransactionDetails_Label.Size = new Size(194, 30);
            TransactionDetails_Label.TabIndex = 10;
            TransactionDetails_Label.Text = "Transaction details:";
            // 
            // LossInformation_Label
            // 
            LossInformation_Label.AutoSize = true;
            LossInformation_Label.Font = new Font("Segoe UI", 11F);
            LossInformation_Label.Location = new Point(44, 290);
            LossInformation_Label.Name = "LossInformation_Label";
            LossInformation_Label.Size = new Size(177, 30);
            LossInformation_Label.TabIndex = 11;
            LossInformation_Label.Text = "Loss information:";
            // 
            // CharacterCount_Label
            // 
            CharacterCount_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CharacterCount_Label.AutoSize = true;
            CharacterCount_Label.Font = new Font("Segoe UI", 10F);
            CharacterCount_Label.Location = new Point(648, 604);
            CharacterCount_Label.Name = "CharacterCount_Label";
            CharacterCount_Label.Size = new Size(64, 28);
            CharacterCount_Label.TabIndex = 12;
            CharacterCount_Label.Text = "0/500";
            // 
            // UndoLoss_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(753, 714);
            Controls.Add(CharacterCount_Label);
            Controls.Add(LossInformation_Label);
            Controls.Add(TransactionDetails_Label);
            Controls.Add(Title_Label);
            Controls.Add(TransactionInfo_Label);
            Controls.Add(Cancel_Button);
            Controls.Add(LossInfo_Label);
            Controls.Add(UndoLoss_Button);
            Controls.Add(UndoReason_Label);
            Controls.Add(UndoReason_TextBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(775, 770);
            Name = "UndoLoss_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Shown += UndoLoss_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label TransactionDetails_Label;
        private Label LossInformation_Label;
        private Label CharacterCount_Label;
    }
}