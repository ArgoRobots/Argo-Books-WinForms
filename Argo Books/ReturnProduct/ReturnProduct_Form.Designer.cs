using Guna.UI2.WinForms;

namespace Argo_Books.ReturnProduct
{
    partial class ReturnProduct_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label Title_Label;
        private Label TransactionInfo_Label;
        private Label ReturnReason_Label;
        private Guna2ComboBox ReturnReason_ComboBox;
        private Label AdditionalNotes_Label;
        private Guna2TextBox AdditionalNotes_TextBox;
        private Guna2Button ProcessReturn_Button;
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Title_Label = new Label();
            TransactionInfo_Label = new Label();
            ReturnReason_Label = new Label();
            ReturnReason_ComboBox = new Guna2ComboBox();
            AdditionalNotes_Label = new Label();
            AdditionalNotes_TextBox = new Guna2TextBox();
            ProcessReturn_Button = new Guna2Button();
            Cancel_Button = new Guna2Button();
            TransactionDetails_Label = new Label();
            CharacterCount_Label = new Label();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Title_Label.Location = new Point(260, 30);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(233, 45);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Return Product";
            // 
            // TransactionInfo_Label
            // 
            TransactionInfo_Label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TransactionInfo_Label.Font = new Font("Segoe UI", 11F);
            TransactionInfo_Label.Location = new Point(72, 125);
            TransactionInfo_Label.Name = "TransactionInfo_Label";
            TransactionInfo_Label.Size = new Size(609, 145);
            TransactionInfo_Label.TabIndex = 0;
            TransactionInfo_Label.Text = "Transaction info...";
            // 
            // ReturnReason_Label
            // 
            ReturnReason_Label.AutoSize = true;
            ReturnReason_Label.Font = new Font("Segoe UI", 11F);
            ReturnReason_Label.Location = new Point(42, 294);
            ReturnReason_Label.Name = "ReturnReason_Label";
            ReturnReason_Label.Size = new Size(186, 30);
            ReturnReason_Label.TabIndex = 0;
            ReturnReason_Label.Text = "Reason for return:";
            // 
            // ReturnReason_ComboBox
            // 
            ReturnReason_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ReturnReason_ComboBox.BackColor = Color.Transparent;
            ReturnReason_ComboBox.BorderColor = Color.FromArgb(213, 218, 223);
            ReturnReason_ComboBox.BorderRadius = 6;
            ReturnReason_ComboBox.CustomizableEdges = customizableEdges1;
            ReturnReason_ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            ReturnReason_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ReturnReason_ComboBox.FocusedColor = Color.FromArgb(94, 148, 255);
            ReturnReason_ComboBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            ReturnReason_ComboBox.Font = new Font("Segoe UI", 10F);
            ReturnReason_ComboBox.ForeColor = Color.FromArgb(68, 88, 112);
            ReturnReason_ComboBox.ItemHeight = 44;
            ReturnReason_ComboBox.Location = new Point(44, 329);
            ReturnReason_ComboBox.Margin = new Padding(4, 5, 4, 5);
            ReturnReason_ComboBox.Name = "ReturnReason_ComboBox";
            ReturnReason_ComboBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            ReturnReason_ComboBox.Size = new Size(664, 50);
            ReturnReason_ComboBox.TabIndex = 1;
            ReturnReason_ComboBox.SelectedIndexChanged += ReturnReason_ComboBox_SelectedIndexChanged;
            // 
            // AdditionalNotes_Label
            // 
            AdditionalNotes_Label.AutoSize = true;
            AdditionalNotes_Label.Font = new Font("Segoe UI", 11F);
            AdditionalNotes_Label.Location = new Point(42, 408);
            AdditionalNotes_Label.Name = "AdditionalNotes_Label";
            AdditionalNotes_Label.Size = new Size(274, 30);
            AdditionalNotes_Label.TabIndex = 0;
            AdditionalNotes_Label.Text = "Additional notes (optional):";
            // 
            // AdditionalNotes_TextBox
            // 
            AdditionalNotes_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            AdditionalNotes_TextBox.BorderRadius = 6;
            AdditionalNotes_TextBox.Cursor = Cursors.IBeam;
            AdditionalNotes_TextBox.CustomizableEdges = customizableEdges3;
            AdditionalNotes_TextBox.DefaultText = "";
            AdditionalNotes_TextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            AdditionalNotes_TextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            AdditionalNotes_TextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            AdditionalNotes_TextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            AdditionalNotes_TextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            AdditionalNotes_TextBox.Font = new Font("Segoe UI", 9F);
            AdditionalNotes_TextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            AdditionalNotes_TextBox.Location = new Point(42, 446);
            AdditionalNotes_TextBox.Margin = new Padding(6, 8, 6, 8);
            AdditionalNotes_TextBox.MaxLength = 500;
            AdditionalNotes_TextBox.Multiline = true;
            AdditionalNotes_TextBox.Name = "AdditionalNotes_TextBox";
            AdditionalNotes_TextBox.PlaceholderText = "";
            AdditionalNotes_TextBox.SelectedText = "";
            AdditionalNotes_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            AdditionalNotes_TextBox.Size = new Size(670, 120);
            AdditionalNotes_TextBox.TabIndex = 2;
            AdditionalNotes_TextBox.TextChanged += AdditionalNotes_TextBox_TextChanged;
            // 
            // ProcessReturn_Button
            // 
            ProcessReturn_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ProcessReturn_Button.BorderColor = Color.LightGray;
            ProcessReturn_Button.BorderRadius = 2;
            ProcessReturn_Button.BorderThickness = 1;
            ProcessReturn_Button.CustomizableEdges = customizableEdges5;
            ProcessReturn_Button.DisabledState.BorderColor = Color.DarkGray;
            ProcessReturn_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            ProcessReturn_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            ProcessReturn_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            ProcessReturn_Button.Enabled = false;
            ProcessReturn_Button.FillColor = Color.White;
            ProcessReturn_Button.Font = new Font("Segoe UI", 10F);
            ProcessReturn_Button.ForeColor = Color.Black;
            ProcessReturn_Button.Location = new Point(540, 625);
            ProcessReturn_Button.Margin = new Padding(4, 5, 4, 5);
            ProcessReturn_Button.Name = "ProcessReturn_Button";
            ProcessReturn_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            ProcessReturn_Button.Size = new Size(200, 45);
            ProcessReturn_Button.TabIndex = 4;
            ProcessReturn_Button.Text = "Process Return";
            ProcessReturn_Button.Click += ProcessReturn_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.BorderColor = Color.LightGray;
            Cancel_Button.BorderRadius = 2;
            Cancel_Button.BorderThickness = 1;
            Cancel_Button.CustomizableEdges = customizableEdges7;
            Cancel_Button.DisabledState.BorderColor = Color.DarkGray;
            Cancel_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Cancel_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Cancel_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Cancel_Button.FillColor = Color.White;
            Cancel_Button.Font = new Font("Segoe UI", 10F);
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(332, 625);
            Cancel_Button.Margin = new Padding(4, 5, 4, 5);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Cancel_Button.Size = new Size(200, 45);
            Cancel_Button.TabIndex = 3;
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
            TransactionDetails_Label.TabIndex = 0;
            TransactionDetails_Label.Text = "Transaction details:";
            // 
            // CharacterCount_Label
            // 
            CharacterCount_Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CharacterCount_Label.AutoSize = true;
            CharacterCount_Label.Font = new Font("Segoe UI", 10F);
            CharacterCount_Label.Location = new Point(648, 574);
            CharacterCount_Label.Name = "CharacterCount_Label";
            CharacterCount_Label.Size = new Size(64, 28);
            CharacterCount_Label.TabIndex = 0;
            CharacterCount_Label.Text = "0/500";
            // 
            // ReturnProduct_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(753, 684);
            Controls.Add(CharacterCount_Label);
            Controls.Add(TransactionDetails_Label);
            Controls.Add(Title_Label);
            Controls.Add(TransactionInfo_Label);
            Controls.Add(Cancel_Button);
            Controls.Add(ReturnReason_Label);
            Controls.Add(ProcessReturn_Button);
            Controls.Add(ReturnReason_ComboBox);
            Controls.Add(AdditionalNotes_TextBox);
            Controls.Add(AdditionalNotes_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(775, 740);
            Name = "ReturnProduct_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Shown += ReturnProduct_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label TransactionDetails_Label;
        private Label CharacterCount_Label;
    }
}