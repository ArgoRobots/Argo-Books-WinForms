using System.Windows.Forms;

namespace Sales_Tracker
{
    partial class RentOutItem_Form
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
            ProductName_Label = new Label();
            AvailableQuantity_Label = new Label();
            Customer_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Quantity_NumericUpDown = new Guna.UI2.WinForms.Guna2NumericUpDown();
            RentalStartDate_DateTimePicker = new Guna.UI2.WinForms.Guna2DateTimePicker();
            Notes_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            RentOut_Button = new Guna.UI2.WinForms.Guna2Button();
            Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            TotalCost_Label = new Label();
            NoCustomers_Label = new Label();
            SelectCustomer_Label = new Label();
            Quantity_Label = new Label();
            StartDate_Label = new Label();
            Notes_Label = new Label();
            ((System.ComponentModel.ISupportInitialize)Quantity_NumericUpDown).BeginInit();
            SuspendLayout();
            // 
            // ProductName_Label
            // 
            ProductName_Label.AutoSize = true;
            ProductName_Label.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            ProductName_Label.Location = new Point(38, 31);
            ProductName_Label.Margin = new Padding(4, 0, 4, 0);
            ProductName_Label.Name = "ProductName_Label";
            ProductName_Label.Size = new Size(236, 45);
            ProductName_Label.TabIndex = 0;
            ProductName_Label.Text = "Product Name";
            // 
            // AvailableQuantity_Label
            // 
            AvailableQuantity_Label.AutoSize = true;
            AvailableQuantity_Label.Font = new Font("Segoe UI", 10F);
            AvailableQuantity_Label.Location = new Point(38, 94);
            AvailableQuantity_Label.Margin = new Padding(4, 0, 4, 0);
            AvailableQuantity_Label.Name = "AvailableQuantity_Label";
            AvailableQuantity_Label.Size = new Size(112, 28);
            AvailableQuantity_Label.TabIndex = 1;
            AvailableQuantity_Label.Text = "Available: 0";
            // 
            // Customer_TextBox
            // 
            Customer_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Customer_TextBox.Cursor = Cursors.IBeam;
            Customer_TextBox.CustomizableEdges = customizableEdges1;
            Customer_TextBox.DefaultText = "";
            Customer_TextBox.Font = new Font("Segoe UI", 10F);
            Customer_TextBox.Location = new Point(38, 203);
            Customer_TextBox.Margin = new Padding(4, 5, 4, 5);
            Customer_TextBox.Name = "Customer_TextBox";
            Customer_TextBox.PlaceholderText = "Search for customer...";
            Customer_TextBox.SelectedText = "";
            Customer_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges2;
            Customer_TextBox.Size = new Size(672, 56);
            Customer_TextBox.TabIndex = 3;
            Customer_TextBox.TextChanged += Customer_TextBox_TextChanged;
            // 
            // Quantity_NumericUpDown
            // 
            Quantity_NumericUpDown.BackColor = Color.Transparent;
            Quantity_NumericUpDown.CustomizableEdges = customizableEdges3;
            Quantity_NumericUpDown.Font = new Font("Segoe UI", 10F);
            Quantity_NumericUpDown.Location = new Point(38, 344);
            Quantity_NumericUpDown.Margin = new Padding(5, 8, 5, 8);
            Quantity_NumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            Quantity_NumericUpDown.Name = "Quantity_NumericUpDown";
            Quantity_NumericUpDown.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Quantity_NumericUpDown.Size = new Size(188, 56);
            Quantity_NumericUpDown.TabIndex = 6;
            Quantity_NumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            Quantity_NumericUpDown.ValueChanged += Quantity_NumericUpDown_ValueChanged;
            // 
            // RentalStartDate_DateTimePicker
            // 
            RentalStartDate_DateTimePicker.BackColor = Color.Transparent;
            RentalStartDate_DateTimePicker.Checked = true;
            RentalStartDate_DateTimePicker.CustomizableEdges = customizableEdges5;
            RentalStartDate_DateTimePicker.FillColor = Color.White;
            RentalStartDate_DateTimePicker.Font = new Font("Segoe UI", 10F);
            RentalStartDate_DateTimePicker.Format = DateTimePickerFormat.Short;
            RentalStartDate_DateTimePicker.Location = new Point(275, 344);
            RentalStartDate_DateTimePicker.Margin = new Padding(4, 5, 4, 5);
            RentalStartDate_DateTimePicker.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            RentalStartDate_DateTimePicker.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            RentalStartDate_DateTimePicker.Name = "RentalStartDate_DateTimePicker";
            RentalStartDate_DateTimePicker.ShadowDecoration.CustomizableEdges = customizableEdges6;
            RentalStartDate_DateTimePicker.Size = new Size(250, 56);
            RentalStartDate_DateTimePicker.TabIndex = 8;
            RentalStartDate_DateTimePicker.Value = new DateTime(2025, 11, 9, 17, 21, 22, 428);
            //
            // Notes_TextBox
            // 
            Notes_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Notes_TextBox.Cursor = Cursors.IBeam;
            Notes_TextBox.CustomizableEdges = customizableEdges7;
            Notes_TextBox.DefaultText = "";
            Notes_TextBox.Font = new Font("Segoe UI", 10F);
            Notes_TextBox.Location = new Point(38, 444);
            Notes_TextBox.Margin = new Padding(5, 8, 5, 8);
            Notes_TextBox.Multiline = true;
            Notes_TextBox.Name = "Notes_TextBox";
            Notes_TextBox.PlaceholderText = "Enter any additional notes...";
            Notes_TextBox.SelectedText = "";
            Notes_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Notes_TextBox.Size = new Size(673, 125);
            Notes_TextBox.TabIndex = 10;
            // 
            // RentOut_Button
            //
            RentOut_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            RentOut_Button.CustomizableEdges = customizableEdges7;
            RentOut_Button.Font = new Font("Segoe UI", 11F);
            RentOut_Button.ForeColor = Color.White;
            RentOut_Button.Location = new Point(510, 649);
            RentOut_Button.Margin = new Padding(4, 5, 4, 5);
            RentOut_Button.Name = "RentOut_Button";
            RentOut_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            RentOut_Button.Size = new Size(200, 50);
            RentOut_Button.TabIndex = 12;
            RentOut_Button.Text = "Rent Out";
            RentOut_Button.Click += RentOut_Button_Click;
            //
            // Cancel_Button
            //
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.CustomizableEdges = customizableEdges8;
            Cancel_Button.Font = new Font("Segoe UI", 11F);
            Cancel_Button.ForeColor = Color.White;
            Cancel_Button.Location = new Point(302, 649);
            Cancel_Button.Margin = new Padding(4, 5, 4, 5);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Cancel_Button.Size = new Size(200, 50);
            Cancel_Button.TabIndex = 13;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // TotalCost_Label
            //
            TotalCost_Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TotalCost_Label.AutoSize = true;
            TotalCost_Label.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            TotalCost_Label.Location = new Point(555, 592);
            TotalCost_Label.Margin = new Padding(4, 0, 4, 0);
            TotalCost_Label.Name = "TotalCost_Label";
            TotalCost_Label.Size = new Size(156, 36);
            TotalCost_Label.TabIndex = 11;
            TotalCost_Label.Text = "Total: $0.00";
            TotalCost_Label.TextAlign = ContentAlignment.MiddleRight;
            // 
            // NoCustomers_Label
            // 
            NoCustomers_Label.AutoSize = true;
            NoCustomers_Label.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            NoCustomers_Label.ForeColor = Color.Red;
            NoCustomers_Label.Location = new Point(38, 258);
            NoCustomers_Label.Margin = new Padding(4, 0, 4, 0);
            NoCustomers_Label.Name = "NoCustomers_Label";
            NoCustomers_Label.Size = new Size(392, 25);
            NoCustomers_Label.TabIndex = 4;
            NoCustomers_Label.Text = "No customers found. Please add a customer first.";
            NoCustomers_Label.Visible = false;
            // 
            // SelectCustomer_Label
            // 
            SelectCustomer_Label.AutoSize = true;
            SelectCustomer_Label.Font = new Font("Segoe UI", 11F);
            SelectCustomer_Label.Location = new Point(36, 168);
            SelectCustomer_Label.Margin = new Padding(4, 0, 4, 0);
            SelectCustomer_Label.Name = "SelectCustomer_Label";
            SelectCustomer_Label.Size = new Size(171, 30);
            SelectCustomer_Label.TabIndex = 2;
            SelectCustomer_Label.Text = "Select Customer";
            // 
            // Quantity_Label
            // 
            Quantity_Label.AutoSize = true;
            Quantity_Label.Font = new Font("Segoe UI", 11F);
            Quantity_Label.Location = new Point(38, 306);
            Quantity_Label.Margin = new Padding(4, 0, 4, 0);
            Quantity_Label.Name = "Quantity_Label";
            Quantity_Label.Size = new Size(95, 30);
            Quantity_Label.TabIndex = 5;
            Quantity_Label.Text = "Quantity";
            // 
            // StartDate_Label
            //
            StartDate_Label.AutoSize = true;
            StartDate_Label.Font = new Font("Segoe UI", 11F);
            StartDate_Label.Location = new Point(275, 306);
            StartDate_Label.Margin = new Padding(4, 0, 4, 0);
            StartDate_Label.Name = "StartDate_Label";
            StartDate_Label.Size = new Size(108, 30);
            StartDate_Label.TabIndex = 7;
            StartDate_Label.Text = "Start Date";
            //
            // Notes_Label
            //
            Notes_Label.AutoSize = true;
            Notes_Label.Font = new Font("Segoe UI", 11F);
            Notes_Label.Location = new Point(38, 406);
            Notes_Label.Margin = new Padding(4, 0, 4, 0);
            Notes_Label.Name = "Notes_Label";
            Notes_Label.Size = new Size(169, 30);
            Notes_Label.TabIndex = 9;
            Notes_Label.Text = "Notes (optional)";
            // 
            // RentOutItem_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(748, 727);
            Controls.Add(ProductName_Label);
            Controls.Add(AvailableQuantity_Label);
            Controls.Add(SelectCustomer_Label);
            Controls.Add(Customer_TextBox);
            Controls.Add(NoCustomers_Label);
            Controls.Add(Quantity_Label);
            Controls.Add(Quantity_NumericUpDown);
            Controls.Add(StartDate_Label);
            Controls.Add(RentalStartDate_DateTimePicker);
            Controls.Add(Notes_Label);
            Controls.Add(Notes_TextBox);
            Controls.Add(TotalCost_Label);
            Controls.Add(RentOut_Button);
            Controls.Add(Cancel_Button);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(770, 783);
            Name = "RentOutItem_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Rent Out Item";
            Shown += RentOutItem_Form_Shown;
            ((System.ComponentModel.ISupportInitialize)Quantity_NumericUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label ProductName_Label;
        private Label AvailableQuantity_Label;
        private Label SelectCustomer_Label;
        private Guna.UI2.WinForms.Guna2TextBox Customer_TextBox;
        private Label NoCustomers_Label;
        private Label Quantity_Label;
        private Guna.UI2.WinForms.Guna2NumericUpDown Quantity_NumericUpDown;
        private Label StartDate_Label;
        private Guna.UI2.WinForms.Guna2DateTimePicker RentalStartDate_DateTimePicker;
        private Label Notes_Label;
        private Guna.UI2.WinForms.Guna2TextBox Notes_TextBox;
        private Label TotalCost_Label;
        private Guna.UI2.WinForms.Guna2Button RentOut_Button;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
    }
}