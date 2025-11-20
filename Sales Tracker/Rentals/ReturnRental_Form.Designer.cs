using Sales_Tracker.UI;
using System.Windows.Forms;

namespace Sales_Tracker.Rentals
{
    partial class ReturnRental_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges17 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges18 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Title_Label = new Label();
            RentalDetails_Label = new Label();
            ReturnDate_Label = new Label();
            ReturnDate_Picker = new Guna.UI2.WinForms.Guna2DateTimePicker();
            Tax_Label = new Label();
            Tax_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Fee_Label = new Label();
            Fee_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Shipping_Label = new Label();
            Shipping_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Discount_Label = new Label();
            Discount_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Notes_Label = new Label();
            Notes_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            AmountCharged_Label = new Label();
            AmountCharged_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Return_Button = new Guna.UI2.WinForms.Guna2Button();
            Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // Title_Label
            // 
            Title_Label.AutoSize = true;
            Title_Label.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            Title_Label.Location = new Point(30, 30);
            Title_Label.Margin = new Padding(4, 0, 4, 0);
            Title_Label.Name = "Title_Label";
            Title_Label.Size = new Size(196, 38);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Return Rental";
            // 
            // RentalDetails_Label
            // 
            RentalDetails_Label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            RentalDetails_Label.BackColor = Color.FromArgb(240, 240, 240);
            RentalDetails_Label.BorderStyle = BorderStyle.FixedSingle;
            RentalDetails_Label.Font = new Font("Segoe UI", 10F);
            RentalDetails_Label.Location = new Point(30, 90);
            RentalDetails_Label.Margin = new Padding(4, 0, 4, 0);
            RentalDetails_Label.Name = "RentalDetails_Label";
            RentalDetails_Label.Padding = new Padding(15);
            RentalDetails_Label.Size = new Size(638, 260);
            RentalDetails_Label.TabIndex = 1;
            RentalDetails_Label.Text = "Rental Details";
            RentalDetails_Label.UseMnemonic = false;
            // 
            // ReturnDate_Label
            // 
            ReturnDate_Label.AutoSize = true;
            ReturnDate_Label.Font = new Font("Segoe UI", 11F);
            ReturnDate_Label.Location = new Point(30, 375);
            ReturnDate_Label.Margin = new Padding(4, 0, 4, 0);
            ReturnDate_Label.Name = "ReturnDate_Label";
            ReturnDate_Label.Size = new Size(127, 30);
            ReturnDate_Label.TabIndex = 2;
            ReturnDate_Label.Text = "Return Date";
            // 
            // ReturnDate_Picker
            // 
            ReturnDate_Picker.BackColor = Color.Transparent;
            ReturnDate_Picker.Checked = true;
            ReturnDate_Picker.CustomizableEdges = customizableEdges1;
            ReturnDate_Picker.FillColor = Color.White;
            ReturnDate_Picker.Font = new Font("Segoe UI", 10F);
            ReturnDate_Picker.Format = DateTimePickerFormat.Short;
            ReturnDate_Picker.Location = new Point(30, 415);
            ReturnDate_Picker.Margin = new Padding(4, 5, 4, 5);
            ReturnDate_Picker.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            ReturnDate_Picker.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            ReturnDate_Picker.Name = "ReturnDate_Picker";
            ReturnDate_Picker.ShadowDecoration.CustomizableEdges = customizableEdges2;
            ReturnDate_Picker.Size = new Size(300, 50);
            ReturnDate_Picker.TabIndex = 3;
            ReturnDate_Picker.Value = new DateTime(2025, 11, 19, 0, 0, 0, 0);
            // 
            // Tax_Label
            // 
            Tax_Label.AutoSize = true;
            Tax_Label.Font = new Font("Segoe UI", 11F);
            Tax_Label.Location = new Point(30, 492);
            Tax_Label.Margin = new Padding(4, 0, 4, 0);
            Tax_Label.Name = "Tax_Label";
            Tax_Label.Size = new Size(44, 30);
            Tax_Label.TabIndex = 4;
            Tax_Label.Text = "Tax";
            // 
            // Tax_TextBox
            // 
            Tax_TextBox.CustomizableEdges = customizableEdges3;
            Tax_TextBox.DefaultText = "";
            Tax_TextBox.Font = new Font("Segoe UI", 10F);
            Tax_TextBox.Location = new Point(30, 530);
            Tax_TextBox.Margin = new Padding(5, 8, 5, 8);
            Tax_TextBox.MaxLength = 10;
            Tax_TextBox.Name = "Tax_TextBox";
            Tax_TextBox.PlaceholderText = "0.00";
            Tax_TextBox.SelectedText = "";
            Tax_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Tax_TextBox.Size = new Size(150, 50);
            Tax_TextBox.TabIndex = 5;
            // 
            // Fee_Label
            // 
            Fee_Label.AutoSize = true;
            Fee_Label.Font = new Font("Segoe UI", 11F);
            Fee_Label.Location = new Point(190, 492);
            Fee_Label.Margin = new Padding(4, 0, 4, 0);
            Fee_Label.Name = "Fee_Label";
            Fee_Label.Size = new Size(48, 30);
            Fee_Label.TabIndex = 6;
            Fee_Label.Text = "Fee";
            // 
            // Fee_TextBox
            // 
            Fee_TextBox.CustomizableEdges = customizableEdges5;
            Fee_TextBox.DefaultText = "";
            Fee_TextBox.Font = new Font("Segoe UI", 10F);
            Fee_TextBox.Location = new Point(190, 530);
            Fee_TextBox.Margin = new Padding(5, 8, 5, 8);
            Fee_TextBox.MaxLength = 10;
            Fee_TextBox.Name = "Fee_TextBox";
            Fee_TextBox.PlaceholderText = "0.00";
            Fee_TextBox.SelectedText = "";
            Fee_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Fee_TextBox.Size = new Size(150, 50);
            Fee_TextBox.TabIndex = 7;
            // 
            // Shipping_Label
            // 
            Shipping_Label.AutoSize = true;
            Shipping_Label.Font = new Font("Segoe UI", 11F);
            Shipping_Label.Location = new Point(350, 492);
            Shipping_Label.Margin = new Padding(4, 0, 4, 0);
            Shipping_Label.Name = "Shipping_Label";
            Shipping_Label.Size = new Size(98, 30);
            Shipping_Label.TabIndex = 8;
            Shipping_Label.Text = "Shipping";
            // 
            // Shipping_TextBox
            // 
            Shipping_TextBox.CustomizableEdges = customizableEdges7;
            Shipping_TextBox.DefaultText = "";
            Shipping_TextBox.Font = new Font("Segoe UI", 10F);
            Shipping_TextBox.Location = new Point(350, 530);
            Shipping_TextBox.Margin = new Padding(5, 8, 5, 8);
            Shipping_TextBox.MaxLength = 10;
            Shipping_TextBox.Name = "Shipping_TextBox";
            Shipping_TextBox.PlaceholderText = "0.00";
            Shipping_TextBox.SelectedText = "";
            Shipping_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Shipping_TextBox.Size = new Size(150, 50);
            Shipping_TextBox.TabIndex = 9;
            // 
            // Discount_Label
            // 
            Discount_Label.AutoSize = true;
            Discount_Label.Font = new Font("Segoe UI", 11F);
            Discount_Label.Location = new Point(510, 492);
            Discount_Label.Margin = new Padding(4, 0, 4, 0);
            Discount_Label.Name = "Discount_Label";
            Discount_Label.Size = new Size(96, 30);
            Discount_Label.TabIndex = 10;
            Discount_Label.Text = "Discount";
            // 
            // Discount_TextBox
            // 
            Discount_TextBox.CustomizableEdges = customizableEdges9;
            Discount_TextBox.DefaultText = "";
            Discount_TextBox.Font = new Font("Segoe UI", 10F);
            Discount_TextBox.Location = new Point(510, 530);
            Discount_TextBox.Margin = new Padding(5, 8, 5, 8);
            Discount_TextBox.MaxLength = 10;
            Discount_TextBox.Name = "Discount_TextBox";
            Discount_TextBox.PlaceholderText = "0.00";
            Discount_TextBox.SelectedText = "";
            Discount_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges10;
            Discount_TextBox.Size = new Size(150, 50);
            Discount_TextBox.TabIndex = 11;
            // 
            // Notes_Label
            // 
            Notes_Label.AutoSize = true;
            Notes_Label.Font = new Font("Segoe UI", 11F);
            Notes_Label.Location = new Point(30, 721);
            Notes_Label.Margin = new Padding(4, 0, 4, 0);
            Notes_Label.Name = "Notes_Label";
            Notes_Label.Size = new Size(238, 30);
            Notes_Label.TabIndex = 12;
            Notes_Label.Text = "Return Notes (optional)";
            // 
            // Notes_TextBox
            // 
            Notes_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Notes_TextBox.Cursor = Cursors.IBeam;
            Notes_TextBox.CustomizableEdges = customizableEdges11;
            Notes_TextBox.DefaultText = "";
            Notes_TextBox.Font = new Font("Segoe UI", 10F);
            Notes_TextBox.Location = new Point(30, 759);
            Notes_TextBox.Margin = new Padding(5, 8, 5, 8);
            Notes_TextBox.Multiline = true;
            Notes_TextBox.Name = "Notes_TextBox";
            Notes_TextBox.PlaceholderText = "Enter any additional notes about the return...";
            Notes_TextBox.SelectedText = "";
            Notes_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges12;
            Notes_TextBox.Size = new Size(638, 110);
            Notes_TextBox.TabIndex = 13;
            // 
            // AmountCharged_Label
            // 
            AmountCharged_Label.AutoSize = true;
            AmountCharged_Label.Font = new Font("Segoe UI", 11F);
            AmountCharged_Label.Location = new Point(30, 607);
            AmountCharged_Label.Margin = new Padding(4, 0, 4, 0);
            AmountCharged_Label.Name = "AmountCharged_Label";
            AmountCharged_Label.Size = new Size(179, 30);
            AmountCharged_Label.TabIndex = 14;
            AmountCharged_Label.Text = "Amount Charged";
            // 
            // AmountCharged_TextBox
            // 
            AmountCharged_TextBox.CustomizableEdges = customizableEdges13;
            AmountCharged_TextBox.DefaultText = "";
            AmountCharged_TextBox.Font = new Font("Segoe UI", 10F);
            AmountCharged_TextBox.Location = new Point(30, 645);
            AmountCharged_TextBox.Margin = new Padding(5, 8, 5, 8);
            AmountCharged_TextBox.MaxLength = 10;
            AmountCharged_TextBox.Name = "AmountCharged_TextBox";
            AmountCharged_TextBox.PlaceholderText = "0.00";
            AmountCharged_TextBox.SelectedText = "";
            AmountCharged_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges14;
            AmountCharged_TextBox.Size = new Size(200, 50);
            AmountCharged_TextBox.TabIndex = 15;
            // 
            // Return_Button
            // 
            Return_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Return_Button.CustomizableEdges = customizableEdges15;
            Return_Button.Font = new Font("Segoe UI", 11F);
            Return_Button.ForeColor = Color.White;
            Return_Button.Location = new Point(468, 884);
            Return_Button.Margin = new Padding(4, 5, 4, 5);
            Return_Button.Name = "Return_Button";
            Return_Button.ShadowDecoration.CustomizableEdges = customizableEdges16;
            Return_Button.Size = new Size(200, 50);
            Return_Button.TabIndex = 16;
            Return_Button.Text = "Process Return";
            Return_Button.Click += Return_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.CustomizableEdges = customizableEdges17;
            Cancel_Button.Font = new Font("Segoe UI", 11F);
            Cancel_Button.ForeColor = Color.White;
            Cancel_Button.Location = new Point(260, 884);
            Cancel_Button.Margin = new Padding(4, 5, 4, 5);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges18;
            Cancel_Button.Size = new Size(200, 50);
            Cancel_Button.TabIndex = 17;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // ReturnRental_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(698, 964);
            Controls.Add(Title_Label);
            Controls.Add(RentalDetails_Label);
            Controls.Add(ReturnDate_Label);
            Controls.Add(ReturnDate_Picker);
            Controls.Add(Tax_Label);
            Controls.Add(Tax_TextBox);
            Controls.Add(Fee_Label);
            Controls.Add(Fee_TextBox);
            Controls.Add(Shipping_Label);
            Controls.Add(Shipping_TextBox);
            Controls.Add(Discount_Label);
            Controls.Add(Discount_TextBox);
            Controls.Add(Notes_Label);
            Controls.Add(Notes_TextBox);
            Controls.Add(AmountCharged_Label);
            Controls.Add(AmountCharged_TextBox);
            Controls.Add(Return_Button);
            Controls.Add(Cancel_Button);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(720, 1020);
            Name = "ReturnRental_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Return Rental";
            Shown += ReturnRental_Form_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Title_Label;
        private Label RentalDetails_Label;
        private Label ReturnDate_Label;
        private Guna.UI2.WinForms.Guna2DateTimePicker ReturnDate_Picker;
        private Label Tax_Label;
        private Guna.UI2.WinForms.Guna2TextBox Tax_TextBox;
        private Label Fee_Label;
        private Guna.UI2.WinForms.Guna2TextBox Fee_TextBox;
        private Label Shipping_Label;
        private Guna.UI2.WinForms.Guna2TextBox Shipping_TextBox;
        private Label Discount_Label;
        private Guna.UI2.WinForms.Guna2TextBox Discount_TextBox;
        private Label Notes_Label;
        private Guna.UI2.WinForms.Guna2TextBox Notes_TextBox;
        private Label AmountCharged_Label;
        private Guna.UI2.WinForms.Guna2TextBox AmountCharged_TextBox;
        private Guna.UI2.WinForms.Guna2Button Return_Button;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
    }
}
