namespace Sales_Tracker.ReportGenerator.Menus
{
    partial class SaveTemplate_Form
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
            Title_Label = new Label();
            UpdateExisting_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            UpdateExisting_Label = new Label();
            SaveAsNew_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            SaveAsNew_Label = new Label();
            TemplateName_Label = new Label();
            TemplateName_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Save_Button = new Guna.UI2.WinForms.Guna2Button();
            Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            Title_Label.Location = new Point(164, 20);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(200, 38);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Save Template";
            // 
            // UpdateExisting_RadioButton
            // 
            UpdateExisting_RadioButton.Animated = true;
            UpdateExisting_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            UpdateExisting_RadioButton.CheckedState.BorderThickness = 0;
            UpdateExisting_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            UpdateExisting_RadioButton.CheckedState.InnerColor = Color.White;
            UpdateExisting_RadioButton.Location = new Point(30, 81);
            UpdateExisting_RadioButton.Name = "UpdateExisting_RadioButton";
            UpdateExisting_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges1;
            UpdateExisting_RadioButton.Size = new Size(25, 25);
            UpdateExisting_RadioButton.TabIndex = 1;
            UpdateExisting_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            UpdateExisting_RadioButton.UncheckedState.BorderThickness = 2;
            UpdateExisting_RadioButton.UncheckedState.FillColor = Color.Transparent;
            UpdateExisting_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            UpdateExisting_RadioButton.Visible = false;
            UpdateExisting_RadioButton.CheckedChanged += UpdateExisting_RadioButton_CheckedChanged;
            // 
            // UpdateExisting_Label
            // 
            UpdateExisting_Label.AutoEllipsis = true;
            UpdateExisting_Label.AutoSize = true;
            UpdateExisting_Label.BackColor = Color.Transparent;
            UpdateExisting_Label.Cursor = Cursors.Hand;
            UpdateExisting_Label.Font = new Font("Segoe UI", 10F);
            UpdateExisting_Label.ForeColor = SystemColors.ControlText;
            UpdateExisting_Label.Location = new Point(52, 74);
            UpdateExisting_Label.Name = "UpdateExisting_Label";
            UpdateExisting_Label.Padding = new Padding(5);
            UpdateExisting_Label.Size = new Size(242, 38);
            UpdateExisting_Label.TabIndex = 2;
            UpdateExisting_Label.Text = "Update existing template";
            UpdateExisting_Label.Visible = false;
            // 
            // SaveAsNew_RadioButton
            // 
            SaveAsNew_RadioButton.Animated = true;
            SaveAsNew_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            SaveAsNew_RadioButton.CheckedState.BorderThickness = 0;
            SaveAsNew_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            SaveAsNew_RadioButton.CheckedState.InnerColor = Color.White;
            SaveAsNew_RadioButton.Location = new Point(30, 119);
            SaveAsNew_RadioButton.Name = "SaveAsNew_RadioButton";
            SaveAsNew_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges2;
            SaveAsNew_RadioButton.Size = new Size(25, 25);
            SaveAsNew_RadioButton.TabIndex = 3;
            SaveAsNew_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            SaveAsNew_RadioButton.UncheckedState.BorderThickness = 2;
            SaveAsNew_RadioButton.UncheckedState.FillColor = Color.Transparent;
            SaveAsNew_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            SaveAsNew_RadioButton.Visible = false;
            SaveAsNew_RadioButton.CheckedChanged += SaveAsNew_RadioButton_CheckedChanged;
            // 
            // SaveAsNew_Label
            // 
            SaveAsNew_Label.AutoSize = true;
            SaveAsNew_Label.BackColor = Color.Transparent;
            SaveAsNew_Label.Cursor = Cursors.Hand;
            SaveAsNew_Label.Font = new Font("Segoe UI", 10F);
            SaveAsNew_Label.ForeColor = SystemColors.ControlText;
            SaveAsNew_Label.Location = new Point(52, 112);
            SaveAsNew_Label.Name = "SaveAsNew_Label";
            SaveAsNew_Label.Padding = new Padding(5);
            SaveAsNew_Label.Size = new Size(209, 38);
            SaveAsNew_Label.TabIndex = 4;
            SaveAsNew_Label.Text = "Save as new template";
            SaveAsNew_Label.Visible = false;
            // 
            // TemplateName_Label
            // 
            TemplateName_Label.AutoSize = true;
            TemplateName_Label.Font = new Font("Segoe UI", 10F);
            TemplateName_Label.Location = new Point(30, 167);
            TemplateName_Label.Name = "TemplateName_Label";
            TemplateName_Label.Size = new Size(152, 28);
            TemplateName_Label.TabIndex = 5;
            TemplateName_Label.Text = "Template Name:";
            // 
            // TemplateName_TextBox
            // 
            TemplateName_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TemplateName_TextBox.BorderRadius = 4;
            TemplateName_TextBox.Cursor = Cursors.IBeam;
            TemplateName_TextBox.CustomizableEdges = customizableEdges3;
            TemplateName_TextBox.DefaultText = "";
            TemplateName_TextBox.Font = new Font("Segoe UI", 10F);
            TemplateName_TextBox.ForeColor = Color.Black;
            TemplateName_TextBox.Location = new Point(30, 200);
            TemplateName_TextBox.Margin = new Padding(4, 5, 4, 5);
            TemplateName_TextBox.MaxLength = 100;
            TemplateName_TextBox.Name = "TemplateName_TextBox";
            TemplateName_TextBox.PlaceholderText = "Enter template name";
            TemplateName_TextBox.SelectedText = "";
            TemplateName_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            TemplateName_TextBox.ShortcutsEnabled = false;
            TemplateName_TextBox.Size = new Size(468, 50);
            TemplateName_TextBox.TabIndex = 6;
            TemplateName_TextBox.KeyDown += TemplateName_TextBox_KeyDown;
            // 
            // Save_Button
            // 
            Save_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Save_Button.BorderRadius = 2;
            Save_Button.CustomizableEdges = customizableEdges5;
            Save_Button.DisabledState.BorderColor = Color.DarkGray;
            Save_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Save_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Save_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Save_Button.Font = new Font("Segoe UI", 9.5F);
            Save_Button.ForeColor = Color.White;
            Save_Button.Location = new Point(270, 287);
            Save_Button.Name = "Save_Button";
            Save_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Save_Button.Size = new Size(120, 45);
            Save_Button.TabIndex = 7;
            Save_Button.Text = "Save";
            Save_Button.Click += Save_Button_Click;
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
            Cancel_Button.Font = new Font("Segoe UI", 9.5F);
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(396, 287);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Cancel_Button.Size = new Size(120, 45);
            Cancel_Button.TabIndex = 8;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // SaveTemplate_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(528, 344);
            Controls.Add(SaveAsNew_RadioButton);
            Controls.Add(UpdateExisting_RadioButton);
            Controls.Add(Cancel_Button);
            Controls.Add(Save_Button);
            Controls.Add(TemplateName_TextBox);
            Controls.Add(TemplateName_Label);
            Controls.Add(SaveAsNew_Label);
            Controls.Add(UpdateExisting_Label);
            Controls.Add(Title_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(460, 373);
            Name = "SaveTemplate_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Save Template";
            Shown += SaveTemplate_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private Guna.UI2.WinForms.Guna2CustomRadioButton UpdateExisting_RadioButton;
        private Label UpdateExisting_Label;
        private Guna.UI2.WinForms.Guna2CustomRadioButton SaveAsNew_RadioButton;
        private Label SaveAsNew_Label;
        private Label TemplateName_Label;
        private Guna.UI2.WinForms.Guna2TextBox TemplateName_TextBox;
        private Guna.UI2.WinForms.Guna2Button Save_Button;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
    }
}
