namespace Argo_Books
{
    partial class ItemsInTransaction_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemsInTransaction_Form));
            Items_DataGridView = new Guna.UI2.WinForms.Guna2DataGridView();
            Title_Label = new Label();
            ((System.ComponentModel.ISupportInitialize)Items_DataGridView).BeginInit();
            SuspendLayout();
            // 
            // Items_DataGridView
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            Items_DataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            Items_DataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(100, 88, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            Items_DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            Items_DataGridView.ColumnHeadersHeight = 4;
            Items_DataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            Items_DataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            Items_DataGridView.GridColor = Color.FromArgb(231, 229, 255);
            Items_DataGridView.Location = new Point(18, 104);
            Items_DataGridView.Name = "Items_DataGridView";
            Items_DataGridView.RowHeadersVisible = false;
            Items_DataGridView.RowHeadersWidth = 62;
            Items_DataGridView.RowTemplate.Height = 25;
            Items_DataGridView.Size = new Size(1740, 420);
            Items_DataGridView.TabIndex = 11;
            Items_DataGridView.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            Items_DataGridView.ThemeStyle.AlternatingRowsStyle.Font = null;
            Items_DataGridView.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            Items_DataGridView.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            Items_DataGridView.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            Items_DataGridView.ThemeStyle.BackColor = Color.White;
            Items_DataGridView.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            Items_DataGridView.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(100, 88, 255);
            Items_DataGridView.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            Items_DataGridView.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F);
            Items_DataGridView.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            Items_DataGridView.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            Items_DataGridView.ThemeStyle.HeaderStyle.Height = 4;
            Items_DataGridView.ThemeStyle.ReadOnly = false;
            Items_DataGridView.ThemeStyle.RowsStyle.BackColor = Color.White;
            Items_DataGridView.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            Items_DataGridView.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            Items_DataGridView.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            Items_DataGridView.ThemeStyle.RowsStyle.Height = 25;
            Items_DataGridView.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            Items_DataGridView.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // Title_Label
            // 
            Title_Label.Anchor = AnchorStyles.Top;
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Title_Label.Location = new Point(756, 30);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(271, 45);
            Title_Label.TabIndex = 12;
            Title_Label.Text = "Items in purchase";
            // 
            // ItemsInTransaction_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1776, 542);
            Controls.Add(Title_Label);
            Controls.Add(Items_DataGridView);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1789, 497);
            Name = "ItemsInTransaction_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosed += ItemsInTransaction_Form_FormClosed;
            Shown += ItemsInTransaction_Form_Shown;
            ((System.ComponentModel.ISupportInitialize)Items_DataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView Items_DataGridView;
        private Label Title_Label;
    }
}