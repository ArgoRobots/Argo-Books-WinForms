using Guna.UI2.WinForms;

namespace Sales_Tracker
{
    partial class DateRange_Form
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges19 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges20 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges21 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges22 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            TimeRange_Label = new Label();
            From_DateTimePicker = new Guna2DateTimePicker();
            From_Label = new Label();
            To_DateTimePicker = new Guna2DateTimePicker();
            To_Label = new Label();
            Apply_Button = new Guna2Button();
            Cancel_Button = new Guna2Button();
            AllTime_RadioButton = new Guna2CustomRadioButton();
            AllTime_Label = new Label();
            Last24Hours_RadioButton = new Guna2CustomRadioButton();
            Last24Hours_Label = new Label();
            Last48Hours_RadioButton = new Guna2CustomRadioButton();
            Last48Hours_Label = new Label();
            Last3Days_RadioButton = new Guna2CustomRadioButton();
            Last3Days_Label = new Label();
            Last5Days_RadioButton = new Guna2CustomRadioButton();
            Last5Days_Label = new Label();
            Last10Days_RadioButton = new Guna2CustomRadioButton();
            Last10Days_Label = new Label();
            Custom_RadioButton = new Guna2CustomRadioButton();
            Custom_Label = new Label();
            Last5Years_RadioButton = new Guna2CustomRadioButton();
            Last5Years_Label = new Label();
            Last2Years_RadioButton = new Guna2CustomRadioButton();
            Last2Years_Label = new Label();
            LastYear_RadioButton = new Guna2CustomRadioButton();
            LastYear_Label = new Label();
            Last100Days_RadioButton = new Guna2CustomRadioButton();
            Last100Days_Label = new Label();
            Last30Days_RadioButton = new Guna2CustomRadioButton();
            Last30Days_Label = new Label();
            Bottom_Separator = new Guna2Separator();
            Main_Panel = new Guna2Panel();
            Main_Panel.SuspendLayout();
            SuspendLayout();
            // 
            // TimeRange_Label
            // 
            TimeRange_Label.AutoSize = true;
            TimeRange_Label.Font = new Font("Segoe UI", 12F);
            TimeRange_Label.Location = new Point(12, 9);
            TimeRange_Label.Name = "TimeRange_Label";
            TimeRange_Label.Size = new Size(135, 32);
            TimeRange_Label.TabIndex = 1;
            TimeRange_Label.Text = "Time range";
            // 
            // From_DateTimePicker
            // 
            From_DateTimePicker.Checked = true;
            From_DateTimePicker.CustomizableEdges = customizableEdges1;
            From_DateTimePicker.FillColor = Color.White;
            From_DateTimePicker.Font = new Font("Segoe UI", 9F);
            From_DateTimePicker.Format = DateTimePickerFormat.Long;
            From_DateTimePicker.Location = new Point(110, 358);
            From_DateTimePicker.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            From_DateTimePicker.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            From_DateTimePicker.Name = "From_DateTimePicker";
            From_DateTimePicker.ShadowDecoration.CustomizableEdges = customizableEdges2;
            From_DateTimePicker.Size = new Size(300, 50);
            From_DateTimePicker.TabIndex = 8;
            From_DateTimePicker.Value = new DateTime(2024, 6, 6, 19, 37, 49, 128);
            // 
            // From_Label
            // 
            From_Label.AutoSize = true;
            From_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            From_Label.Location = new Point(110, 324);
            From_Label.Name = "From_Label";
            From_Label.Size = new Size(66, 31);
            From_Label.TabIndex = 7;
            From_Label.Text = "From";
            // 
            // To_DateTimePicker
            // 
            To_DateTimePicker.Checked = true;
            To_DateTimePicker.CustomizableEdges = customizableEdges3;
            To_DateTimePicker.FillColor = Color.White;
            To_DateTimePicker.Font = new Font("Segoe UI", 9F);
            To_DateTimePicker.Format = DateTimePickerFormat.Long;
            To_DateTimePicker.Location = new Point(110, 445);
            To_DateTimePicker.MaxDate = new DateTime(9998, 12, 31, 0, 0, 0, 0);
            To_DateTimePicker.MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);
            To_DateTimePicker.Name = "To_DateTimePicker";
            To_DateTimePicker.ShadowDecoration.CustomizableEdges = customizableEdges4;
            To_DateTimePicker.Size = new Size(300, 50);
            To_DateTimePicker.TabIndex = 10;
            To_DateTimePicker.Value = new DateTime(2024, 6, 6, 19, 37, 49, 128);
            // 
            // To_Label
            // 
            To_Label.AutoSize = true;
            To_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            To_Label.Location = new Point(110, 411);
            To_Label.Name = "To_Label";
            To_Label.Size = new Size(37, 31);
            To_Label.TabIndex = 9;
            To_Label.Text = "To";
            // 
            // Apply_Button
            // 
            Apply_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Apply_Button.BackColor = Color.Transparent;
            Apply_Button.BorderColor = Color.LightGray;
            Apply_Button.BorderRadius = 2;
            Apply_Button.BorderThickness = 1;
            Apply_Button.CustomizableEdges = customizableEdges5;
            Apply_Button.FillColor = Color.White;
            Apply_Button.Font = new Font("Segoe UI", 10F);
            Apply_Button.ForeColor = Color.Black;
            Apply_Button.Location = new Point(348, 563);
            Apply_Button.Name = "Apply_Button";
            Apply_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Apply_Button.Size = new Size(160, 45);
            Apply_Button.TabIndex = 14;
            Apply_Button.Tag = "";
            Apply_Button.Text = "Apply";
            Apply_Button.Click += Apply_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.BackColor = Color.Transparent;
            Cancel_Button.BorderColor = Color.LightGray;
            Cancel_Button.BorderRadius = 2;
            Cancel_Button.BorderThickness = 1;
            Cancel_Button.CustomizableEdges = customizableEdges7;
            Cancel_Button.FillColor = Color.White;
            Cancel_Button.Font = new Font("Segoe UI", 10F);
            Cancel_Button.ForeColor = Color.Black;
            Cancel_Button.Location = new Point(182, 563);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Cancel_Button.Size = new Size(160, 45);
            Cancel_Button.TabIndex = 15;
            Cancel_Button.Tag = "";
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            // 
            // AllTime_RadioButton
            // 
            AllTime_RadioButton.Animated = true;
            AllTime_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            AllTime_RadioButton.CheckedState.BorderThickness = 0;
            AllTime_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            AllTime_RadioButton.CheckedState.InnerColor = Color.White;
            AllTime_RadioButton.Location = new Point(66, 68);
            AllTime_RadioButton.Name = "AllTime_RadioButton";
            AllTime_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges9;
            AllTime_RadioButton.Size = new Size(25, 25);
            AllTime_RadioButton.TabIndex = 53;
            AllTime_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            AllTime_RadioButton.UncheckedState.BorderThickness = 2;
            AllTime_RadioButton.UncheckedState.FillColor = Color.Transparent;
            AllTime_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // AllTime_Label
            // 
            AllTime_Label.AutoSize = true;
            AllTime_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AllTime_Label.Location = new Point(89, 58);
            AllTime_Label.Name = "AllTime_Label";
            AllTime_Label.Padding = new Padding(5);
            AllTime_Label.Size = new Size(103, 41);
            AllTime_Label.TabIndex = 54;
            AllTime_Label.Text = "All time";
            // 
            // Last24Hours_RadioButton
            // 
            Last24Hours_RadioButton.Animated = true;
            Last24Hours_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last24Hours_RadioButton.CheckedState.BorderThickness = 0;
            Last24Hours_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last24Hours_RadioButton.CheckedState.InnerColor = Color.White;
            Last24Hours_RadioButton.Location = new Point(66, 109);
            Last24Hours_RadioButton.Name = "Last24Hours_RadioButton";
            Last24Hours_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges10;
            Last24Hours_RadioButton.Size = new Size(25, 25);
            Last24Hours_RadioButton.TabIndex = 55;
            Last24Hours_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last24Hours_RadioButton.UncheckedState.BorderThickness = 2;
            Last24Hours_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last24Hours_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last24Hours_Label
            // 
            Last24Hours_Label.AutoSize = true;
            Last24Hours_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last24Hours_Label.Location = new Point(89, 99);
            Last24Hours_Label.Name = "Last24Hours_Label";
            Last24Hours_Label.Padding = new Padding(5);
            Last24Hours_Label.Size = new Size(158, 41);
            Last24Hours_Label.TabIndex = 56;
            Last24Hours_Label.Text = "Last 24 hours";
            // 
            // Last48Hours_RadioButton
            // 
            Last48Hours_RadioButton.Animated = true;
            Last48Hours_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last48Hours_RadioButton.CheckedState.BorderThickness = 0;
            Last48Hours_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last48Hours_RadioButton.CheckedState.InnerColor = Color.White;
            Last48Hours_RadioButton.Location = new Point(66, 150);
            Last48Hours_RadioButton.Name = "Last48Hours_RadioButton";
            Last48Hours_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges11;
            Last48Hours_RadioButton.Size = new Size(25, 25);
            Last48Hours_RadioButton.TabIndex = 57;
            Last48Hours_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last48Hours_RadioButton.UncheckedState.BorderThickness = 2;
            Last48Hours_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last48Hours_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last48Hours_Label
            // 
            Last48Hours_Label.AutoSize = true;
            Last48Hours_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last48Hours_Label.Location = new Point(89, 140);
            Last48Hours_Label.Name = "Last48Hours_Label";
            Last48Hours_Label.Padding = new Padding(5);
            Last48Hours_Label.Size = new Size(158, 41);
            Last48Hours_Label.TabIndex = 58;
            Last48Hours_Label.Text = "Last 48 hours";
            // 
            // Last3Days_RadioButton
            // 
            Last3Days_RadioButton.Animated = true;
            Last3Days_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last3Days_RadioButton.CheckedState.BorderThickness = 0;
            Last3Days_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last3Days_RadioButton.CheckedState.InnerColor = Color.White;
            Last3Days_RadioButton.Location = new Point(66, 191);
            Last3Days_RadioButton.Name = "Last3Days_RadioButton";
            Last3Days_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges12;
            Last3Days_RadioButton.Size = new Size(25, 25);
            Last3Days_RadioButton.TabIndex = 59;
            Last3Days_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last3Days_RadioButton.UncheckedState.BorderThickness = 2;
            Last3Days_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last3Days_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last3Days_Label
            // 
            Last3Days_Label.AutoSize = true;
            Last3Days_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last3Days_Label.Location = new Point(89, 181);
            Last3Days_Label.Name = "Last3Days_Label";
            Last3Days_Label.Padding = new Padding(5);
            Last3Days_Label.Size = new Size(136, 41);
            Last3Days_Label.TabIndex = 60;
            Last3Days_Label.Text = "Last 3 days";
            // 
            // Last5Days_RadioButton
            // 
            Last5Days_RadioButton.Animated = true;
            Last5Days_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last5Days_RadioButton.CheckedState.BorderThickness = 0;
            Last5Days_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last5Days_RadioButton.CheckedState.InnerColor = Color.White;
            Last5Days_RadioButton.Location = new Point(66, 232);
            Last5Days_RadioButton.Name = "Last5Days_RadioButton";
            Last5Days_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges13;
            Last5Days_RadioButton.Size = new Size(25, 25);
            Last5Days_RadioButton.TabIndex = 61;
            Last5Days_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last5Days_RadioButton.UncheckedState.BorderThickness = 2;
            Last5Days_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last5Days_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last5Days_Label
            // 
            Last5Days_Label.AutoSize = true;
            Last5Days_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last5Days_Label.Location = new Point(89, 222);
            Last5Days_Label.Name = "Last5Days_Label";
            Last5Days_Label.Padding = new Padding(5);
            Last5Days_Label.Size = new Size(136, 41);
            Last5Days_Label.TabIndex = 62;
            Last5Days_Label.Text = "Last 5 days";
            // 
            // Last10Days_RadioButton
            // 
            Last10Days_RadioButton.Animated = true;
            Last10Days_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last10Days_RadioButton.CheckedState.BorderThickness = 0;
            Last10Days_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last10Days_RadioButton.CheckedState.InnerColor = Color.White;
            Last10Days_RadioButton.Location = new Point(66, 273);
            Last10Days_RadioButton.Name = "Last10Days_RadioButton";
            Last10Days_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges14;
            Last10Days_RadioButton.Size = new Size(25, 25);
            Last10Days_RadioButton.TabIndex = 63;
            Last10Days_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last10Days_RadioButton.UncheckedState.BorderThickness = 2;
            Last10Days_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last10Days_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last10Days_Label
            // 
            Last10Days_Label.AutoSize = true;
            Last10Days_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last10Days_Label.Location = new Point(89, 263);
            Last10Days_Label.Name = "Last10Days_Label";
            Last10Days_Label.Padding = new Padding(5);
            Last10Days_Label.Size = new Size(148, 41);
            Last10Days_Label.TabIndex = 64;
            Last10Days_Label.Text = "Last 10 days";
            // 
            // Custom_RadioButton
            // 
            Custom_RadioButton.Animated = true;
            Custom_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Custom_RadioButton.CheckedState.BorderThickness = 0;
            Custom_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Custom_RadioButton.CheckedState.InnerColor = Color.White;
            Custom_RadioButton.Location = new Point(271, 273);
            Custom_RadioButton.Name = "Custom_RadioButton";
            Custom_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges15;
            Custom_RadioButton.Size = new Size(25, 25);
            Custom_RadioButton.TabIndex = 75;
            Custom_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Custom_RadioButton.UncheckedState.BorderThickness = 2;
            Custom_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Custom_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            Custom_RadioButton.CheckedChanged += Custom_RadioButton_CheckedChanged;
            // 
            // Custom_Label
            // 
            Custom_Label.AutoSize = true;
            Custom_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Custom_Label.Location = new Point(294, 263);
            Custom_Label.Name = "Custom_Label";
            Custom_Label.Padding = new Padding(5);
            Custom_Label.Size = new Size(102, 41);
            Custom_Label.TabIndex = 76;
            Custom_Label.Text = "Custom";
            // 
            // Last5Years_RadioButton
            // 
            Last5Years_RadioButton.Animated = true;
            Last5Years_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last5Years_RadioButton.CheckedState.BorderThickness = 0;
            Last5Years_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last5Years_RadioButton.CheckedState.InnerColor = Color.White;
            Last5Years_RadioButton.Location = new Point(271, 232);
            Last5Years_RadioButton.Name = "Last5Years_RadioButton";
            Last5Years_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges16;
            Last5Years_RadioButton.Size = new Size(25, 25);
            Last5Years_RadioButton.TabIndex = 73;
            Last5Years_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last5Years_RadioButton.UncheckedState.BorderThickness = 2;
            Last5Years_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last5Years_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last5Years_Label
            // 
            Last5Years_Label.AutoSize = true;
            Last5Years_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last5Years_Label.Location = new Point(294, 222);
            Last5Years_Label.Name = "Last5Years_Label";
            Last5Years_Label.Padding = new Padding(5);
            Last5Years_Label.Size = new Size(142, 41);
            Last5Years_Label.TabIndex = 74;
            Last5Years_Label.Text = "Last 5 years";
            // 
            // Last2Years_RadioButton
            // 
            Last2Years_RadioButton.Animated = true;
            Last2Years_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last2Years_RadioButton.CheckedState.BorderThickness = 0;
            Last2Years_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last2Years_RadioButton.CheckedState.InnerColor = Color.White;
            Last2Years_RadioButton.Location = new Point(271, 191);
            Last2Years_RadioButton.Name = "Last2Years_RadioButton";
            Last2Years_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges17;
            Last2Years_RadioButton.Size = new Size(25, 25);
            Last2Years_RadioButton.TabIndex = 71;
            Last2Years_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last2Years_RadioButton.UncheckedState.BorderThickness = 2;
            Last2Years_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last2Years_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last2Years_Label
            // 
            Last2Years_Label.AutoSize = true;
            Last2Years_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last2Years_Label.Location = new Point(294, 181);
            Last2Years_Label.Name = "Last2Years_Label";
            Last2Years_Label.Padding = new Padding(5);
            Last2Years_Label.Size = new Size(142, 41);
            Last2Years_Label.TabIndex = 72;
            Last2Years_Label.Text = "Last 2 years";
            // 
            // LastYear_RadioButton
            // 
            LastYear_RadioButton.Animated = true;
            LastYear_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            LastYear_RadioButton.CheckedState.BorderThickness = 0;
            LastYear_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            LastYear_RadioButton.CheckedState.InnerColor = Color.White;
            LastYear_RadioButton.Location = new Point(271, 150);
            LastYear_RadioButton.Name = "LastYear_RadioButton";
            LastYear_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges18;
            LastYear_RadioButton.Size = new Size(25, 25);
            LastYear_RadioButton.TabIndex = 69;
            LastYear_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            LastYear_RadioButton.UncheckedState.BorderThickness = 2;
            LastYear_RadioButton.UncheckedState.FillColor = Color.Transparent;
            LastYear_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // LastYear_Label
            // 
            LastYear_Label.AutoSize = true;
            LastYear_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LastYear_Label.Location = new Point(294, 140);
            LastYear_Label.Name = "LastYear_Label";
            LastYear_Label.Padding = new Padding(5);
            LastYear_Label.Size = new Size(114, 41);
            LastYear_Label.TabIndex = 70;
            LastYear_Label.Text = "Last year";
            // 
            // Last100Days_RadioButton
            // 
            Last100Days_RadioButton.Animated = true;
            Last100Days_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last100Days_RadioButton.CheckedState.BorderThickness = 0;
            Last100Days_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last100Days_RadioButton.CheckedState.InnerColor = Color.White;
            Last100Days_RadioButton.Location = new Point(271, 109);
            Last100Days_RadioButton.Name = "Last100Days_RadioButton";
            Last100Days_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges19;
            Last100Days_RadioButton.Size = new Size(25, 25);
            Last100Days_RadioButton.TabIndex = 67;
            Last100Days_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last100Days_RadioButton.UncheckedState.BorderThickness = 2;
            Last100Days_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last100Days_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last100Days_Label
            // 
            Last100Days_Label.AutoSize = true;
            Last100Days_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last100Days_Label.Location = new Point(294, 99);
            Last100Days_Label.Name = "Last100Days_Label";
            Last100Days_Label.Padding = new Padding(5);
            Last100Days_Label.Size = new Size(160, 41);
            Last100Days_Label.TabIndex = 68;
            Last100Days_Label.Text = "Last 100 days";
            // 
            // Last30Days_RadioButton
            // 
            Last30Days_RadioButton.Animated = true;
            Last30Days_RadioButton.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            Last30Days_RadioButton.CheckedState.BorderThickness = 0;
            Last30Days_RadioButton.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            Last30Days_RadioButton.CheckedState.InnerColor = Color.White;
            Last30Days_RadioButton.Location = new Point(271, 68);
            Last30Days_RadioButton.Name = "Last30Days_RadioButton";
            Last30Days_RadioButton.ShadowDecoration.CustomizableEdges = customizableEdges20;
            Last30Days_RadioButton.Size = new Size(25, 25);
            Last30Days_RadioButton.TabIndex = 65;
            Last30Days_RadioButton.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            Last30Days_RadioButton.UncheckedState.BorderThickness = 2;
            Last30Days_RadioButton.UncheckedState.FillColor = Color.Transparent;
            Last30Days_RadioButton.UncheckedState.InnerColor = Color.Transparent;
            // 
            // Last30Days_Label
            // 
            Last30Days_Label.AutoSize = true;
            Last30Days_Label.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Last30Days_Label.Location = new Point(294, 58);
            Last30Days_Label.Name = "Last30Days_Label";
            Last30Days_Label.Padding = new Padding(5);
            Last30Days_Label.Size = new Size(148, 41);
            Last30Days_Label.TabIndex = 66;
            Last30Days_Label.Text = "Last 30 days";
            // 
            // Bottom_Separator
            // 
            Bottom_Separator.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Bottom_Separator.Location = new Point(1, 541);
            Bottom_Separator.Name = "Bottom_Separator";
            Bottom_Separator.Size = new Size(518, 16);
            Bottom_Separator.TabIndex = 77;
            // 
            // Main_Panel
            // 
            Main_Panel.BorderThickness = 1;
            Main_Panel.Controls.Add(Custom_RadioButton);
            Main_Panel.Controls.Add(Last5Years_RadioButton);
            Main_Panel.Controls.Add(Last2Years_RadioButton);
            Main_Panel.Controls.Add(LastYear_RadioButton);
            Main_Panel.Controls.Add(Last100Days_RadioButton);
            Main_Panel.Controls.Add(Last30Days_RadioButton);
            Main_Panel.Controls.Add(AllTime_RadioButton);
            Main_Panel.Controls.Add(Last24Hours_RadioButton);
            Main_Panel.Controls.Add(Last48Hours_RadioButton);
            Main_Panel.Controls.Add(Last10Days_RadioButton);
            Main_Panel.Controls.Add(Last3Days_RadioButton);
            Main_Panel.Controls.Add(Last5Days_RadioButton);
            Main_Panel.Controls.Add(TimeRange_Label);
            Main_Panel.Controls.Add(Bottom_Separator);
            Main_Panel.Controls.Add(From_Label);
            Main_Panel.Controls.Add(From_DateTimePicker);
            Main_Panel.Controls.Add(Custom_Label);
            Main_Panel.Controls.Add(To_Label);
            Main_Panel.Controls.Add(To_DateTimePicker);
            Main_Panel.Controls.Add(Last5Years_Label);
            Main_Panel.Controls.Add(Apply_Button);
            Main_Panel.Controls.Add(Cancel_Button);
            Main_Panel.Controls.Add(Last2Years_Label);
            Main_Panel.Controls.Add(AllTime_Label);
            Main_Panel.Controls.Add(LastYear_Label);
            Main_Panel.Controls.Add(Last24Hours_Label);
            Main_Panel.Controls.Add(Last100Days_Label);
            Main_Panel.Controls.Add(Last48Hours_Label);
            Main_Panel.Controls.Add(Last30Days_Label);
            Main_Panel.Controls.Add(Last3Days_Label);
            Main_Panel.Controls.Add(Last10Days_Label);
            Main_Panel.Controls.Add(Last5Days_Label);
            Main_Panel.CustomizableEdges = customizableEdges21;
            Main_Panel.Location = new Point(0, 0);
            Main_Panel.Name = "Main_Panel";
            Main_Panel.ShadowDecoration.CustomizableEdges = customizableEdges22;
            Main_Panel.Size = new Size(520, 620);
            Main_Panel.TabIndex = 78;
            // 
            // DateRange_Form
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 620);
            Controls.Add(Main_Panel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "DateRange_Form";
            Shown += DateRange_Form_Shown;
            Main_Panel.ResumeLayout(false);
            Main_Panel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label TimeRange_Label;
        private Label From_Label;
        private Label To_Label;
        private Guna.UI2.WinForms.Guna2Button Apply_Button;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
        private Guna.UI2.WinForms.Guna2DateTimePicker From_DateTimePicker;
        private Guna.UI2.WinForms.Guna2DateTimePicker To_DateTimePicker;
        public Guna.UI2.WinForms.Guna2CustomRadioButton AllTime_RadioButton;
        private Label AllTime_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last24Hours_RadioButton;
        private Label Last24Hours_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last48Hours_RadioButton;
        private Label Last48Hours_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last3Days_RadioButton;
        private Label Last3Days_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last5Days_RadioButton;
        private Label Last5Days_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last10Days_RadioButton;
        private Label Last10Days_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Custom_RadioButton;
        private Label Custom_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last5Years_RadioButton;
        private Label Last5Years_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last2Years_RadioButton;
        private Label Last2Years_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton LastYear_RadioButton;
        private Label LastYear_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last100Days_RadioButton;
        private Label Last100Days_Label;
        public Guna.UI2.WinForms.Guna2CustomRadioButton Last30Days_RadioButton;
        private Label Last30Days_Label;
        private Guna.UI2.WinForms.Guna2Separator Bottom_Separator;
        public Guna2Panel Main_Panel;
    }
}