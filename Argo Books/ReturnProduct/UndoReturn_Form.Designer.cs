using Guna.UI2.WinForms;

namespace Argo_Books
{
    partial class UndoReturn_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label Title_Label;
        private Label TransactionInfo_Label;
        private Label ReturnInfo_Label;
        private Label UndoReason_Label;
        private Guna2TextBox UndoReason_TextBox;
        private Guna2Button UndoReturn_Button;
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Title_Label = new Label();
            TransactionInfo_Label = new Label();
            ReturnInfo_Label = new Label();
            UndoReason_Label = new Label();
            UndoReason_TextBox = new Guna2TextBox();
            UndoReturn_Button = new Guna2Button();
            Cancel_Button = new Guna2Button();
            TransactionDetails_Label = new Label();
            ReturnInformation_Label = new Label();
            CharacterCount_Label = new Label();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Title_Label.Location = new Point(216, 30);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(320, 45);
            Title_Label.TabIndex = 1;
            Title_Label.Text = "Undo Product Return";
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
            // ReturnInfo_Label
            // 
            ReturnInfo_Label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ReturnInfo_Label.AutoEllipsis = true;
            ReturnInfo_Label.Font = new Font("Segoe UI", 11F);
            ReturnInfo_Label.Location = new Point(72, 325);
            ReturnInfo_Label.Name = "ReturnInfo_Label";
            ReturnInfo_Label.Size = new Size(609, 100);
            ReturnInfo_Label.TabIndex = 3;
            ReturnInfo_Label.Text = "Return info...";
            // 
            // UndoReason_Label
            // 
            UndoReason_Label.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            UndoReason_Label.AutoSize = true;
            UndoReason_Label.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UndoReason_Label.Location = new Point(42, 438);
            UndoReason_Label.Name = "UndoReason_Label";
            UndoReason_Label.Size = new Size(371, 30);
            UndoReason_Label.TabIndex = 4;
            UndoReason_Label.Text = "Reason for undoing return (optional):";
            // 
            // UndoReason_TextBox
            // 
            UndoReason_TextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UndoReason_TextBox.BorderRadius = 6;
            UndoReason_TextBox.Cursor = Cursors.IBeam;
            UndoReason_TextBox.CustomizableEdges = customizableEdges1;
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
            UndoReason_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            UndoReason_TextBox.Size = new Size(670, 120);
            UndoReason_TextBox.TabIndex = 5;
            UndoReason_TextBox.TextChanged += UndoReason_TextBox_TextChanged;
            // 
            // UndoReturn_Button
            // 
            UndoReturn_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            UndoReturn_Button.BorderColor = Color.LightGray;
            UndoReturn_Button.BorderRadius = 2;
            UndoReturn_Button.BorderThickness = 1;
            UndoReturn_Button.CustomizableEdges = customizableEdges3;
            UndoReturn_Button.DisabledState.BorderColor = Color.DarkGray;
            UndoReturn_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            UndoReturn_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            UndoReturn_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            UndoReturn_Button.Enabled = false;
            UndoReturn_Button.FillColor = Color.White;
            UndoReturn_Button.Font = new Font("Segoe UI", 10F);
            UndoReturn_Button.ForeColor = Color.Black;
            UndoReturn_Button.Location = new Point(540, 655);
            UndoReturn_Button.Margin = new Padding(4, 5, 4, 5);
            UndoReturn_Button.Name = "UndoReturn_Button";
            UndoReturn_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            UndoReturn_Button.Size = new Size(200, 45);
            UndoReturn_Button.TabIndex = 6;
            UndoReturn_Button.Text = "Undo Return";
            UndoReturn_Button.Click += UndoReturn_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.BorderColor = Color.LightGray;
            Cancel_Button.BorderRadius = 2;
            Cancel_Button.BorderThickness = 1;
            Cancel_Button.CustomizableEdges = customizableEdges5;
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
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
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
            // ReturnInformation_Label
            // 
            ReturnInformation_Label.AutoSize = true;
            ReturnInformation_Label.Font = new Font("Segoe UI", 11F);
            ReturnInformation_Label.Location = new Point(44, 290);
            ReturnInformation_Label.Name = "ReturnInformation_Label";
            ReturnInformation_Label.Size = new Size(199, 30);
            ReturnInformation_Label.TabIndex = 11;
            ReturnInformation_Label.Text = "Return information:";
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
            // UndoReturn_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(753, 714);
            Controls.Add(CharacterCount_Label);
            Controls.Add(ReturnInformation_Label);
            Controls.Add(TransactionDetails_Label);
            Controls.Add(Title_Label);
            Controls.Add(TransactionInfo_Label);
            Controls.Add(Cancel_Button);
            Controls.Add(ReturnInfo_Label);
            Controls.Add(UndoReturn_Button);
            Controls.Add(UndoReason_Label);
            Controls.Add(UndoReason_TextBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(775, 770);
            Name = "UndoReturn_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Shown += UndoReturn_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label TransactionDetails_Label;
        private Label ReturnInformation_Label;
        private Label CharacterCount_Label;
    }
}