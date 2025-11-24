namespace Argo_Books.ReportGenerator.Menus
{
    partial class CustomTemplateManager_Form
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
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
            Templates_DataGridView = new Guna.UI2.WinForms.Guna2DataGridView();
            TemplateName = new DataGridViewTextBoxColumn();
            Load_Button = new Guna.UI2.WinForms.Guna2Button();
            Delete_Button = new Guna.UI2.WinForms.Guna2Button();
            Rename_Button = new Guna.UI2.WinForms.Guna2Button();
            Export_Button = new Guna.UI2.WinForms.Guna2Button();
            Import_Button = new Guna.UI2.WinForms.Guna2Button();
            Close_Button = new Guna.UI2.WinForms.Guna2Button();
            NoTemplates_Label = new Label();
            ((System.ComponentModel.ISupportInitialize)Templates_DataGridView).BeginInit();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            Title_Label.Location = new Point(204, 20);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(250, 38);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Custom Templates";
            // 
            // Templates_DataGridView
            // 
            Templates_DataGridView.AllowUserToAddRows = false;
            Templates_DataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.White;
            Templates_DataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            Templates_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Templates_DataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
            Templates_DataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.Padding = new Padding(10, 0, 0, 0);
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            Templates_DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            Templates_DataGridView.ColumnHeadersHeight = 60;
            Templates_DataGridView.ColumnHeadersVisible = false;
            Templates_DataGridView.Columns.AddRange(new DataGridViewColumn[] { TemplateName });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 12F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.Padding = new Padding(10, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            Templates_DataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            Templates_DataGridView.GridColor = Color.FromArgb(231, 229, 255);
            Templates_DataGridView.Location = new Point(30, 80);
            Templates_DataGridView.Name = "Templates_DataGridView";
            Templates_DataGridView.ReadOnly = true;
            Templates_DataGridView.RowHeadersVisible = false;
            Templates_DataGridView.RowHeadersWidth = 62;
            Templates_DataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            Templates_DataGridView.RowTemplate.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            Templates_DataGridView.RowTemplate.Height = 35;
            Templates_DataGridView.ScrollBars = ScrollBars.Vertical;
            Templates_DataGridView.Size = new Size(598, 294);
            Templates_DataGridView.TabIndex = 1;
            Templates_DataGridView.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            Templates_DataGridView.ThemeStyle.AlternatingRowsStyle.Font = null;
            Templates_DataGridView.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            Templates_DataGridView.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            Templates_DataGridView.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            Templates_DataGridView.ThemeStyle.BackColor = Color.White;
            Templates_DataGridView.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            Templates_DataGridView.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            Templates_DataGridView.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.Single;
            Templates_DataGridView.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Templates_DataGridView.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            Templates_DataGridView.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            Templates_DataGridView.ThemeStyle.HeaderStyle.Height = 60;
            Templates_DataGridView.ThemeStyle.ReadOnly = true;
            Templates_DataGridView.ThemeStyle.RowsStyle.BackColor = Color.White;
            Templates_DataGridView.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.None;
            Templates_DataGridView.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 12F);
            Templates_DataGridView.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            Templates_DataGridView.ThemeStyle.RowsStyle.Height = 35;
            Templates_DataGridView.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            Templates_DataGridView.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            Templates_DataGridView.CellDoubleClick += Templates_DataGridView_CellDoubleClick;
            Templates_DataGridView.SelectionChanged += Templates_DataGridView_SelectionChanged;
            // 
            // TemplateName
            // 
            TemplateName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            TemplateName.HeaderText = "Template Name";
            TemplateName.MinimumWidth = 8;
            TemplateName.Name = "TemplateName";
            TemplateName.ReadOnly = true;
            // 
            // Load_Button
            // 
            Load_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Load_Button.BorderRadius = 2;
            Load_Button.CustomizableEdges = customizableEdges1;
            Load_Button.DisabledState.BorderColor = Color.DarkGray;
            Load_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Load_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Load_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Load_Button.Enabled = false;
            Load_Button.Font = new Font("Segoe UI", 9.5F);
            Load_Button.ForeColor = Color.White;
            Load_Button.Location = new Point(30, 380);
            Load_Button.Name = "Load_Button";
            Load_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Load_Button.Size = new Size(130, 96);
            Load_Button.TabIndex = 3;
            Load_Button.Text = "Load";
            Load_Button.Click += Load_Button_Click;
            // 
            // Delete_Button
            // 
            Delete_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Delete_Button.BorderColor = Color.LightGray;
            Delete_Button.BorderRadius = 2;
            Delete_Button.BorderThickness = 1;
            Delete_Button.CustomizableEdges = customizableEdges3;
            Delete_Button.DisabledState.BorderColor = Color.DarkGray;
            Delete_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Delete_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Delete_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Delete_Button.Enabled = false;
            Delete_Button.FillColor = Color.White;
            Delete_Button.Font = new Font("Segoe UI", 9.5F);
            Delete_Button.ForeColor = Color.Black;
            Delete_Button.Location = new Point(166, 431);
            Delete_Button.Name = "Delete_Button";
            Delete_Button.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Delete_Button.Size = new Size(130, 45);
            Delete_Button.TabIndex = 4;
            Delete_Button.Text = "Delete";
            Delete_Button.Click += Delete_Button_Click;
            // 
            // Rename_Button
            // 
            Rename_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Rename_Button.BorderColor = Color.LightGray;
            Rename_Button.BorderRadius = 2;
            Rename_Button.BorderThickness = 1;
            Rename_Button.CustomizableEdges = customizableEdges5;
            Rename_Button.DisabledState.BorderColor = Color.DarkGray;
            Rename_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Rename_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Rename_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Rename_Button.Enabled = false;
            Rename_Button.FillColor = Color.White;
            Rename_Button.Font = new Font("Segoe UI", 9.5F);
            Rename_Button.ForeColor = Color.Black;
            Rename_Button.Location = new Point(302, 431);
            Rename_Button.Name = "Rename_Button";
            Rename_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Rename_Button.Size = new Size(130, 45);
            Rename_Button.TabIndex = 5;
            Rename_Button.Text = "Rename";
            Rename_Button.Click += Rename_Button_Click;
            // 
            // Export_Button
            // 
            Export_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Export_Button.BorderColor = Color.LightGray;
            Export_Button.BorderRadius = 2;
            Export_Button.BorderThickness = 1;
            Export_Button.CustomizableEdges = customizableEdges7;
            Export_Button.DisabledState.BorderColor = Color.DarkGray;
            Export_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Export_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Export_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Export_Button.Enabled = false;
            Export_Button.FillColor = Color.White;
            Export_Button.Font = new Font("Segoe UI", 9.5F);
            Export_Button.ForeColor = Color.Black;
            Export_Button.Location = new Point(166, 380);
            Export_Button.Name = "Export_Button";
            Export_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Export_Button.Size = new Size(130, 45);
            Export_Button.TabIndex = 7;
            Export_Button.Text = "Export";
            Export_Button.Click += Export_Button_Click;
            // 
            // Import_Button
            // 
            Import_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Import_Button.BorderColor = Color.LightGray;
            Import_Button.BorderRadius = 2;
            Import_Button.BorderThickness = 1;
            Import_Button.CustomizableEdges = customizableEdges9;
            Import_Button.DisabledState.BorderColor = Color.DarkGray;
            Import_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Import_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Import_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Import_Button.FillColor = Color.White;
            Import_Button.Font = new Font("Segoe UI", 9.5F);
            Import_Button.ForeColor = Color.Black;
            Import_Button.Location = new Point(302, 380);
            Import_Button.Name = "Import_Button";
            Import_Button.ShadowDecoration.CustomizableEdges = customizableEdges10;
            Import_Button.Size = new Size(130, 45);
            Import_Button.TabIndex = 8;
            Import_Button.Text = "Import";
            Import_Button.Click += Import_Button_Click;
            // 
            // Close_Button
            // 
            Close_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Close_Button.BorderColor = Color.LightGray;
            Close_Button.BorderRadius = 2;
            Close_Button.BorderThickness = 1;
            Close_Button.CustomizableEdges = customizableEdges11;
            Close_Button.DisabledState.BorderColor = Color.DarkGray;
            Close_Button.DisabledState.CustomBorderColor = Color.DarkGray;
            Close_Button.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            Close_Button.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            Close_Button.FillColor = Color.White;
            Close_Button.Font = new Font("Segoe UI", 9.5F);
            Close_Button.ForeColor = Color.Black;
            Close_Button.Location = new Point(498, 431);
            Close_Button.Name = "Close_Button";
            Close_Button.ShadowDecoration.CustomizableEdges = customizableEdges12;
            Close_Button.Size = new Size(130, 45);
            Close_Button.TabIndex = 6;
            Close_Button.Text = "Close";
            Close_Button.Click += Close_Button_Click;
            // 
            // NoTemplates_Label
            // 
            NoTemplates_Label.Anchor = AnchorStyles.None;
            NoTemplates_Label.AutoSize = true;
            NoTemplates_Label.BackColor = Color.White;
            NoTemplates_Label.Font = new Font("Segoe UI", 10F);
            NoTemplates_Label.Location = new Point(202, 223);
            NoTemplates_Label.Name = "NoTemplates_Label";
            NoTemplates_Label.Size = new Size(254, 28);
            NoTemplates_Label.TabIndex = 2;
            NoTemplates_Label.Text = "No custom templates saved";
            NoTemplates_Label.TextAlign = ContentAlignment.MiddleCenter;
            NoTemplates_Label.Visible = false;
            // 
            // CustomTemplateManager_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(658, 496);
            Controls.Add(NoTemplates_Label);
            Controls.Add(Close_Button);
            Controls.Add(Import_Button);
            Controls.Add(Export_Button);
            Controls.Add(Rename_Button);
            Controls.Add(Delete_Button);
            Controls.Add(Load_Button);
            Controls.Add(Templates_DataGridView);
            Controls.Add(Title_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(680, 520);
            Name = "CustomTemplateManager_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Custom Templates";
            Shown += CustomTemplateManager_Form_Shown;
            ((System.ComponentModel.ISupportInitialize)Templates_DataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private Guna.UI2.WinForms.Guna2DataGridView Templates_DataGridView;
        private DataGridViewTextBoxColumn TemplateName;
        private Label NoTemplates_Label;
        private Guna.UI2.WinForms.Guna2Button Load_Button;
        private Guna.UI2.WinForms.Guna2Button Delete_Button;
        private Guna.UI2.WinForms.Guna2Button Rename_Button;
        private Guna.UI2.WinForms.Guna2Button Export_Button;
        private Guna.UI2.WinForms.Guna2Button Import_Button;
        private Guna.UI2.WinForms.Guna2Button Close_Button;
    }
}
