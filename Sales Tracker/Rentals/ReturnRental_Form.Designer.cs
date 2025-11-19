using Sales_Tracker.UI;
using System.Windows.Forms;

namespace Sales_Tracker
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
            Title_Label = new Label();
            RentalDetails_Label = new Label();
            ReturnDate_Label = new Label();
            ReturnDate_Picker = new Guna.UI2.WinForms.Guna2DateTimePicker();
            Notes_Label = new Label();
            Notes_TextBox = new Guna.UI2.WinForms.Guna2TextBox();
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
            Title_Label.Size = new Size(209, 38);
            Title_Label.TabIndex = 0;
            Title_Label.Text = "Return Rental";
            //
            // RentalDetails_Label
            //
            RentalDetails_Label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            RentalDetails_Label.AutoSize = false;
            RentalDetails_Label.BackColor = Color.FromArgb(240, 240, 240);
            RentalDetails_Label.BorderStyle = BorderStyle.FixedSingle;
            RentalDetails_Label.Font = new Font("Segoe UI", 10F);
            RentalDetails_Label.Location = new Point(30, 90);
            RentalDetails_Label.Margin = new Padding(4, 0, 4, 0);
            RentalDetails_Label.Name = "RentalDetails_Label";
            RentalDetails_Label.Padding = new Padding(15);
            RentalDetails_Label.Size = new Size(640, 260);
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
            ReturnDate_Label.Size = new Size(124, 30);
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
            // Notes_Label
            //
            Notes_Label.AutoSize = true;
            Notes_Label.Font = new Font("Segoe UI", 11F);
            Notes_Label.Location = new Point(30, 490);
            Notes_Label.Margin = new Padding(4, 0, 4, 0);
            Notes_Label.Name = "Notes_Label";
            Notes_Label.Size = new Size(232, 30);
            Notes_Label.TabIndex = 4;
            Notes_Label.Text = "Return Notes (optional)";
            //
            // Notes_TextBox
            //
            Notes_TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Notes_TextBox.Cursor = Cursors.IBeam;
            Notes_TextBox.CustomizableEdges = customizableEdges3;
            Notes_TextBox.DefaultText = "";
            Notes_TextBox.Font = new Font("Segoe UI", 10F);
            Notes_TextBox.Location = new Point(30, 530);
            Notes_TextBox.Margin = new Padding(5, 8, 5, 8);
            Notes_TextBox.Multiline = true;
            Notes_TextBox.Name = "Notes_TextBox";
            Notes_TextBox.PlaceholderText = "Enter any additional notes about the return...";
            Notes_TextBox.SelectedText = "";
            Notes_TextBox.ShadowDecoration.CustomizableEdges = customizableEdges4;
            Notes_TextBox.Size = new Size(640, 100);
            Notes_TextBox.TabIndex = 5;
            //
            // Return_Button
            //
            Return_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Return_Button.CustomizableEdges = customizableEdges5;
            Return_Button.Font = new Font("Segoe UI", 11F);
            Return_Button.ForeColor = Color.White;
            Return_Button.Location = new Point(470, 660);
            Return_Button.Margin = new Padding(4, 5, 4, 5);
            Return_Button.Name = "Return_Button";
            Return_Button.ShadowDecoration.CustomizableEdges = customizableEdges6;
            Return_Button.Size = new Size(200, 50);
            Return_Button.TabIndex = 6;
            Return_Button.Text = "Process Return";
            Return_Button.Click += Return_Button_Click;
            //
            // Cancel_Button
            //
            Cancel_Button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Cancel_Button.CustomizableEdges = customizableEdges7;
            Cancel_Button.Font = new Font("Segoe UI", 11F);
            Cancel_Button.ForeColor = Color.White;
            Cancel_Button.Location = new Point(262, 660);
            Cancel_Button.Margin = new Padding(4, 5, 4, 5);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.ShadowDecoration.CustomizableEdges = customizableEdges8;
            Cancel_Button.Size = new Size(200, 50);
            Cancel_Button.TabIndex = 7;
            Cancel_Button.Text = "Cancel";
            Cancel_Button.Click += Cancel_Button_Click;
            //
            // ReturnRental_Form
            //
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 740);
            Controls.Add(Title_Label);
            Controls.Add(RentalDetails_Label);
            Controls.Add(ReturnDate_Label);
            Controls.Add(ReturnDate_Picker);
            Controls.Add(Notes_Label);
            Controls.Add(Notes_TextBox);
            Controls.Add(Return_Button);
            Controls.Add(Cancel_Button);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ReturnRental_Form";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
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
        private Label Notes_Label;
        private Guna.UI2.WinForms.Guna2TextBox Notes_TextBox;
        private Guna.UI2.WinForms.Guna2Button Return_Button;
        private Guna.UI2.WinForms.Guna2Button Cancel_Button;
    }
}
