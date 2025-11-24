using DocumentFormat.OpenXml.Wordprocessing;
using LiveChartsCore.Measure;

namespace Argo_Books.UI
{
    partial class ViewTransactionDetails_Form
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
            TransactionDetails_Label = new Label();
            TransactionInfo_Label = new Label();
            DetailsHeader_Label = new Label();
            DetailsInfo_Label = new Label();
            ItemsHeader_Label = new Label();
            ItemsInfo_Label = new Label();
            Close_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // TransactionDetails_Label
            // 
            TransactionDetails_Label.AutoSize = true;
            TransactionDetails_Label.Font = new System.Drawing.Font("Segoe UI", 13F, FontStyle.Bold);
            TransactionDetails_Label.Location = new Point(43, 50);
            TransactionDetails_Label.Margin = new Padding(4, 0, 4, 0);
            TransactionDetails_Label.Name = "TransactionDetails_Label";
            TransactionDetails_Label.Size = new Size(250, 36);
            TransactionDetails_Label.TabIndex = 0;
            TransactionDetails_Label.Text = "Transaction Details:";
            // 
            // TransactionInfo_Label
            // 
            TransactionInfo_Label.AutoSize = true;
            TransactionInfo_Label.Font = new System.Drawing.Font("Segoe UI", 11F);
            TransactionInfo_Label.Location = new Point(43, 90);
            TransactionInfo_Label.Margin = new Padding(4, 0, 4, 0);
            TransactionInfo_Label.MaximumSize = new Size(743, 0);
            TransactionInfo_Label.Name = "TransactionInfo_Label";
            TransactionInfo_Label.Size = new Size(153, 30);
            TransactionInfo_Label.TabIndex = 1;
            TransactionInfo_Label.Text = "Transaction ID:";
            // 
            // DetailsHeader_Label
            // 
            DetailsHeader_Label.AutoSize = true;
            DetailsHeader_Label.Font = new System.Drawing.Font("Segoe UI", 13F, FontStyle.Bold);
            DetailsHeader_Label.Location = new Point(43, 241);
            DetailsHeader_Label.Margin = new Padding(4, 0, 4, 0);
            DetailsHeader_Label.Name = "DetailsHeader_Label";
            DetailsHeader_Label.Size = new Size(193, 36);
            DetailsHeader_Label.TabIndex = 2;
            DetailsHeader_Label.Text = "Return Details:";
            // 
            // DetailsInfo_Label
            // 
            DetailsInfo_Label.AutoSize = true;
            DetailsInfo_Label.Font = new System.Drawing.Font("Segoe UI", 11F);
            DetailsInfo_Label.Location = new Point(43, 282);
            DetailsInfo_Label.Margin = new Padding(4, 0, 4, 0);
            DetailsInfo_Label.MaximumSize = new Size(743, 0);
            DetailsInfo_Label.Name = "DetailsInfo_Label";
            DetailsInfo_Label.Size = new Size(132, 30);
            DetailsInfo_Label.TabIndex = 3;
            DetailsInfo_Label.Text = "Return Date:";
            // 
            // ItemsHeader_Label
            // 
            ItemsHeader_Label.AutoSize = true;
            ItemsHeader_Label.Font = new System.Drawing.Font("Segoe UI", 13F, FontStyle.Bold);
            ItemsHeader_Label.Location = new Point(43, 432);
            ItemsHeader_Label.Margin = new Padding(4, 0, 4, 0);
            ItemsHeader_Label.Name = "ItemsHeader_Label";
            ItemsHeader_Label.Size = new Size(200, 36);
            ItemsHeader_Label.TabIndex = 4;
            ItemsHeader_Label.Text = "Affected Items:";
            ItemsHeader_Label.Visible = false;
            // 
            // ItemsInfo_Label
            // 
            ItemsInfo_Label.AutoSize = true;
            ItemsInfo_Label.Font = new System.Drawing.Font("Segoe UI", 11F);
            ItemsInfo_Label.Location = new Point(43, 472);
            ItemsInfo_Label.Margin = new Padding(4, 0, 4, 0);
            ItemsInfo_Label.MaximumSize = new Size(743, 0);
            ItemsInfo_Label.Name = "ItemsInfo_Label";
            ItemsInfo_Label.Size = new Size(71, 30);
            ItemsInfo_Label.TabIndex = 5;
            ItemsInfo_Label.Text = "Items:";
            ItemsInfo_Label.Visible = false;
            // 
            // Close_Button
            // 
            Close_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Close_Button.CustomizableEdges = customizableEdges1;
            Close_Button.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            Close_Button.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            Close_Button.DisabledState.FillColor = System.Drawing.Color.FromArgb(169, 169, 169);
            Close_Button.DisabledState.ForeColor = System.Drawing.Color.FromArgb(141, 141, 141);
            Close_Button.Font = new System.Drawing.Font("Segoe UI", 11F);
            Close_Button.ForeColor = System.Drawing.Color.White;
            Close_Button.Location = new Point(385, 630);
            Close_Button.Margin = new Padding(4, 5, 4, 5);
            Close_Button.Name = "Close_Button";
            Close_Button.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Close_Button.Size = new Size(180, 50);
            Close_Button.TabIndex = 6;
            Close_Button.Text = "Close";
            Close_Button.Click += Close_Button_Click;
            // 
            // ViewTransactionDetails_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(578, 694);
            Controls.Add(Close_Button);
            Controls.Add(ItemsInfo_Label);
            Controls.Add(ItemsHeader_Label);
            Controls.Add(DetailsInfo_Label);
            Controls.Add(DetailsHeader_Label);
            Controls.Add(TransactionInfo_Label);
            Controls.Add(TransactionDetails_Label);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ViewTransactionDetails_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Transaction Details";
            Shown += ViewTransactionDetails_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label TransactionDetails_Label;
        private Label TransactionInfo_Label;
        private Label DetailsHeader_Label;
        private Label DetailsInfo_Label;
        private Label ItemsHeader_Label;
        private Label ItemsInfo_Label;
        private Guna.UI2.WinForms.Guna2Button Close_Button;
    }
}