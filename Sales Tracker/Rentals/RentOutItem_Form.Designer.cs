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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges18 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges19 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges20 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges21 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges22 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges23 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges24 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges25 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges26 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges27 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges28 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges29 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges30 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges31 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges32 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges33 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges34 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            ProductName_Label = new Label();
            AvailableQuantity_Label = new Label();
            Customer_ComboBox = new Guna.UI2.WinForms.Guna2ComboBox();
            Quantity_NumericUpDown = new Guna.UI2.WinForms.Guna2NumericUpDown();
            RentalStartDate_DateTimePicker = new Guna.UI2.WinForms.Guna2DateTimePicker();
            DailyRate_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            DailyRate_Label = new Label();
            WeeklyRate_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            WeeklyRate_Label = new Label();
            MonthlyRate_RadioButton = new Guna.UI2.WinForms.Guna2CustomRadioButton();
            MonthlyRate_Label = new Label();
            SecurityDeposit_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            Notes_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
            RentOut_Button = new Guna.UI2.WinForms.Guna2Button();
            Cancel_Button = new Guna.UI2.WinForms.Guna2Button();
            TotalCost_Label = new Label();
            NoCustomers_Label = new Label();
            SelectCustomer_Label = new Label();
            Quantity_Label = new Label();
            StartDate_Label = new Label();
            RateType_Label = new Label();
            SecurityDeposit_Label = new Label();
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
            // Customer_ComboBox
            // 
            Customer_ComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Customer_ComboBox.BackColor = Color.Transparent;
            Customer_ComboBox.CustomizableEdges = customizableEdges18;
            Customer_ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            Customer_ComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            Customer_ComboBox.FocusedColor = Color.Empty;
            Customer_ComboBox.Font = new Font("Segoe UI", 10F);
            Customer_ComboBox.ForeColor = Color.FromArgb(68, 88, 112);
            Customer_ComboBox.ItemHeight = 44;
            Customer_ComboBox.Location = new Point(38, 203);
            Customer_ComboBox.Margin = new Padding(4, 5, 4, 5);
            Customer_ComboBox.Name = "Customer_ComboBox";
            Customer_ComboBox.ShadowDecoration.CustomizableEdges = customizableEdges19;
            Customer_ComboBox.Size = new Size(672, 50);
            Customer_ComboBox.TabIndex = 3;
            Customer_ComboBox.SelectedIndexChanged += Customer_ComboBox_SelectedIndexChanged;
            // 
            // Quantity_NumericUpDown
            // 
            Quantity_NumericUpDown.BackColor = Color.Transparent;
            Quantity_NumericUpDown.CustomizableEdges = customizableEdges20;
            Quantity_NumericUpDown.Font = new Font("Segoe UI", 10F);
            Quantity_NumericUpDown.Location = new Point(38, 344);
            Quantity_NumericUpDown.Margin = new Padding(5, 8, 5, 8);
            Quantity_NumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            Quantity_NumericUpDown.Name = "Quantity_NumericUpDown";
            Quantity_NumericUpDown.ShadowDecoration.CustomizableEdges = customizableEdges21;
            Quantity_NumericUpDown.Size = new Size(188, 56);
            Quantity_NumericUpDown.TabIndex = 6;
            Quantity_NumericUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            Quantity_NumericUpDown.ValueChanged += Quantity_NumericUpDown_ValueChanged;
            // 
            // RentalStartDate_DateTimePicker
            // 
            RentalStartDate_DateTimePicker.BackColor = Color.Transparent;
            RentalStartDate_DateTimePicker.Checked = true;
            RentalStartDate_DateTimePicker.CustomizableEdges = customizableEdges22;
            RentalStartDate_DateTimePicker.FillColor = Color.White;
            RentalStartDate_DateTimePicker.Font = new Font("Segoe UI", 10F);
            RentalStartDate_DateTimePicker.Format = DateTimePickerFormat.Short;
            RentalStartDate_DateTimePicker.Location = new Point(275, 344);
            RentalStartDate_DateTimePicker.Margin = new Padding(4, 5, 4, 5);
            RentalStartDate_DateTimePicker.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            RentalStartDate_DateTimePicker.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            RentalStartDate_DateTimePicker.Name = "RentalStartDate_DateTimePicker";
            RentalStartDate_DateTimePicker.ShadowDecoration.CustomizableEdges = customizableEdges23;
            RentalStartDate_DateTimePicker.Size = new Size(250, 56);
            RentalStartDate_DateTimePicker.TabIndex = 8;
            RentalStartDate_DateTimePicker.Value = new DateTime(2025, 11, 9, 17, 21, 22, 428);
            // 
            // DailyRate_RadioButton
            // 
            DailyRate_RadioButton.Animated = true;
            DailyRate_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            DailyRate_RadioButton.CheckedState.BorderThickness = 0;
            DailyRate_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            DailyRate_RadioButton.CheckedState.InnerColor = Color.White;
            DailyRate_RadioButton.Location = new Point(38, 477);
            DailyRate_RadioButton.Margin = new Padding(4, 5, 4, 5);
            DailyRate_RadioButton.Name = "DailyRate_RadioButton";
            DailyRate_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges24;
            DailyRate_RadioButton.Size = new Size(25, 31);
            DailyRate_RadioButton.TabIndex = 10;
            DailyRate_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            DailyRate_RadioButton.UncheckedState.BorderThickness = 2;
            DailyRate_RadioButton.UncheckedState.FillColor = Color.Transparent;
            DailyRate_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            DailyRate_RadioButton.CheckedChanged += RateType_CheckedChanged;
            // 
            // DailyRate_Label
            // 
            DailyRate_Label.AutoSize = true;
            DailyRate_Label.Cursor = Cursors.Hand;
            DailyRate_Label.Font = new Font("Segoe UI", 10F);
            DailyRate_Label.Location = new Point(69, 478);
            DailyRate_Label.Margin = new Padding(4, 0, 4, 0);
            DailyRate_Label.Name = "DailyRate_Label";
            DailyRate_Label.Size = new Size(113, 28);
            DailyRate_Label.TabIndex = 11;
            DailyRate_Label.Text = "Daily: $0.00";
            // 
            // WeeklyRate_RadioButton
            // 
            WeeklyRate_RadioButton.Animated = true;
            WeeklyRate_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            WeeklyRate_RadioButton.CheckedState.BorderThickness = 0;
            WeeklyRate_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            WeeklyRate_RadioButton.CheckedState.InnerColor = Color.White;
            WeeklyRate_RadioButton.Location = new Point(250, 477);
            WeeklyRate_RadioButton.Margin = new Padding(4, 5, 4, 5);
            WeeklyRate_RadioButton.Name = "WeeklyRate_RadioButton";
            WeeklyRate_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges25;
            WeeklyRate_RadioButton.Size = new Size(25, 31);
            WeeklyRate_RadioButton.TabIndex = 12;
            WeeklyRate_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            WeeklyRate_RadioButton.UncheckedState.BorderThickness = 2;
            WeeklyRate_RadioButton.UncheckedState.FillColor = Color.Transparent;
            WeeklyRate_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            WeeklyRate_RadioButton.CheckedChanged += RateType_CheckedChanged;
            // 
            // WeeklyRate_Label
            // 
            WeeklyRate_Label.AutoSize = true;
            WeeklyRate_Label.Cursor = Cursors.Hand;
            WeeklyRate_Label.Font = new Font("Segoe UI", 10F);
            WeeklyRate_Label.Location = new Point(281, 478);
            WeeklyRate_Label.Margin = new Padding(4, 0, 4, 0);
            WeeklyRate_Label.Name = "WeeklyRate_Label";
            WeeklyRate_Label.Size = new Size(132, 28);
            WeeklyRate_Label.TabIndex = 13;
            WeeklyRate_Label.Text = "Weekly: $0.00";
            // 
            // MonthlyRate_RadioButton
            // 
            MonthlyRate_RadioButton.Animated = true;
            MonthlyRate_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            MonthlyRate_RadioButton.CheckedState.BorderThickness = 0;
            MonthlyRate_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            MonthlyRate_RadioButton.CheckedState.InnerColor = Color.White;
            MonthlyRate_RadioButton.Location = new Point(475, 477);
            MonthlyRate_RadioButton.Margin = new Padding(4, 5, 4, 5);
            MonthlyRate_RadioButton.Name = "MonthlyRate_RadioButton";
            MonthlyRate_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges26;
            MonthlyRate_RadioButton.Size = new Size(25, 31);
            MonthlyRate_RadioButton.TabIndex = 14;
            MonthlyRate_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            MonthlyRate_RadioButton.UncheckedState.BorderThickness = 2;
            MonthlyRate_RadioButton.UncheckedState.FillColor = Color.Transparent;
            MonthlyRate_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            MonthlyRate_RadioButton.CheckedChanged += RateType_CheckedChanged;
            // 
            // MonthlyRate_Label
            // 
            MonthlyRate_Label.AutoSize = true;
            MonthlyRate_Label.Cursor = Cursors.Hand;
            MonthlyRate_Label.Font = new Font("Segoe UI", 10F);
            MonthlyRate_Label.Location = new Point(506, 478);
            MonthlyRate_Label.Margin = new Padding(4, 0, 4, 0);
            MonthlyRate_Label.Name = "MonthlyRate_Label";
            MonthlyRate_Label.Size = new Size(143, 28);
            MonthlyRate_Label.TabIndex = 15;
            MonthlyRate_Label.Text = "Monthly: $0.00";
            // 
            // SecurityDeposit_TextBox
            // 
            SecurityDeposit_TextBox.Cursor = Cursors.IBeam;
            SecurityDeposit_TextBox.CustomizableEdges = customizableEdges27;
            SecurityDeposit_TextBox.DefaultText = "0.00";
            SecurityDeposit_TextBox.Font = new Font("Segoe UI", 10F);
            SecurityDeposit_TextBox.Location = new Point(38, 586);
            SecurityDeposit_TextBox.Margin = new Padding(5, 8, 5, 8);
            SecurityDeposit_TextBox.Name = "SecurityDeposit_TextBox";
            SecurityDeposit_TextBox.PlaceholderText = "0.00";
            SecurityDeposit_TextBox.SelectedText = "";
            SecurityDeposit_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges28;
            SecurityDeposit_TextBox.Size = new Size(250, 56);
            SecurityDeposit_TextBox.TabIndex = 17;
            SecurityDeposit_TextBox.TextChanged += SecurityDeposit_TextBox_TextChanged;
            // 
            // Notes_TextBox
            // 
            Notes_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Notes_TextBox.Cursor = Cursors.IBeam;
            Notes_TextBox.CustomizableEdges = customizableEdges29;
            Notes_TextBox.DefaultText = "";
            Notes_TextBox.Font = new Font("Segoe UI", 10F);
            Notes_TextBox.Location = new Point(38, 711);
            Notes_TextBox.Margin = new Padding(5, 8, 5, 8);
            Notes_TextBox.Multiline = true;
            Notes_TextBox.Name = "Notes_TextBox";
            Notes_TextBox.PlaceholderText = "Enter any additional notes...";
            Notes_TextBox.SelectedText = "";
            Notes_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges30;
            Notes_TextBox.Size = new Size(673, 125);
            Notes_TextBox.TabIndex = 19;
            // 
            // RentOut_Button
            // 
            RentOut_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            RentOut_Button.CustomizableEdges = customizableEdges31;
            RentOut_Button.Font = new Font("Segoe UI", 11F);
            RentOut_Button.ForeColor = Color.White;
            RentOut_Button.Location = new Point(510, 916);
            RentOut_Button.Margin = new Padding(4, 5, 4, 5);
            RentOut_Button.Name = "RentOut_Button";
            RentOut_Button.ShadowDecoration.CustomizableEdges = customizableEdges32;
            RentOut_Button.Size = new Size(200, 50);
            RentOut_Button.TabIndex = 21;
            RentOut_Button.Text = "Rent Out";
            RentOut_Button.Click += RentOut_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.CustomizableEdges = customizableEdges33;
            Cancel_Button.Font = new Font("Segoe UI", 11F);
            Cancel_Button.ForeColor = Color.White;
            Cancel_Button.Location = new Point(302, 916);
            Cancel_Button.Margin = new Padding(4, 5, 4, 5);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges34;
            Cancel_Button.Size = new Size(200, 50);
            Cancel_Button.TabIndex = 22;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // TotalCost_Label
            // 
            TotalCost_Label.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TotalCost_Label.AutoSize = true;
            TotalCost_Label.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            TotalCost_Label.Location = new Point(555, 859);
            TotalCost_Label.Margin = new Padding(4, 0, 4, 0);
            TotalCost_Label.Name = "TotalCost_Label";
            TotalCost_Label.Size = new Size(156, 36);
            TotalCost_Label.TabIndex = 20;
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
            // RateType_Label
            // 
            RateType_Label.AutoSize = true;
            RateType_Label.Font = new Font("Segoe UI", 11F);
            RateType_Label.Location = new Point(38, 422);
            RateType_Label.Margin = new Padding(4, 0, 4, 0);
            RateType_Label.Name = "RateType_Label";
            RateType_Label.Size = new Size(121, 30);
            RateType_Label.TabIndex = 9;
            RateType_Label.Text = "Rental Rate";
            // 
            // SecurityDeposit_Label
            // 
            SecurityDeposit_Label.AutoSize = true;
            SecurityDeposit_Label.Font = new Font("Segoe UI", 11F);
            SecurityDeposit_Label.Location = new Point(38, 548);
            SecurityDeposit_Label.Margin = new Padding(4, 0, 4, 0);
            SecurityDeposit_Label.Name = "SecurityDeposit_Label";
            SecurityDeposit_Label.Size = new Size(170, 30);
            SecurityDeposit_Label.TabIndex = 16;
            SecurityDeposit_Label.Text = "Security Deposit";
            // 
            // Notes_Label
            // 
            Notes_Label.AutoSize = true;
            Notes_Label.Font = new Font("Segoe UI", 11F);
            Notes_Label.Location = new Point(38, 673);
            Notes_Label.Margin = new Padding(4, 0, 4, 0);
            Notes_Label.Name = "Notes_Label";
            Notes_Label.Size = new Size(169, 30);
            Notes_Label.TabIndex = 18;
            Notes_Label.Text = "Notes (optional)";
            // 
            // RentOutItem_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(748, 994);
            Controls.Add(ProductName_Label);
            Controls.Add(AvailableQuantity_Label);
            Controls.Add(SelectCustomer_Label);
            Controls.Add(Customer_ComboBox);
            Controls.Add(NoCustomers_Label);
            Controls.Add(Quantity_Label);
            Controls.Add(Quantity_NumericUpDown);
            Controls.Add(StartDate_Label);
            Controls.Add(RentalStartDate_DateTimePicker);
            Controls.Add(RateType_Label);
            Controls.Add(DailyRate_RadioButton);
            Controls.Add(DailyRate_Label);
            Controls.Add(WeeklyRate_RadioButton);
            Controls.Add(WeeklyRate_Label);
            Controls.Add(MonthlyRate_RadioButton);
            Controls.Add(MonthlyRate_Label);
            Controls.Add(SecurityDeposit_Label);
            Controls.Add(SecurityDeposit_TextBox);
            Controls.Add(Notes_Label);
            Controls.Add(Notes_TextBox);
            Controls.Add(TotalCost_Label);
            Controls.Add(RentOut_Button);
            Controls.Add(Cancel_Button);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(770, 1050);
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
        private Guna.UI2.WinForms.Guna2ComboBox Customer_ComboBox;
        private Label NoCustomers_Label;
        private Label Quantity_Label;
        private Guna.UI2.WinForms.Guna2NumericUpDown Quantity_NumericUpDown;
        private Label StartDate_Label;
        private Guna.UI2.WinForms.Guna2DateTimePicker RentalStartDate_DateTimePicker;
        private Label RateType_Label;
        private Guna.UI2.WinForms.Guna2CustomRadioButton DailyRate_RadioButton;
        private Label DailyRate_Label;
        private Guna.UI2.WinForms.Guna2CustomRadioButton WeeklyRate_RadioButton;
        private Label WeeklyRate_Label;
        private Guna.UI2.WinForms.Guna2CustomRadioButton MonthlyRate_RadioButton;
        private Label MonthlyRate_Label;
        private Label SecurityDeposit_Label;
        private Guna.UI2.WinForms.Guna2TextBox SecurityDeposit_TextBox;
        private Label Notes_Label;
        private Guna.UI2.WinForms.Guna2TextBox Notes_TextBox;
        private Label TotalCost_Label;
        private Guna.UI2.WinForms.Guna2Button RentOut_Button;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
    }
}