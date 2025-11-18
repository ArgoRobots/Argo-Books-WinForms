using Sales_Tracker.Classes;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Passwords
{
    /// <summary>
    /// Form for managing application password settings.
    /// </summary>
    public partial class PasswordManager_Form : BaseForm
    {
        // Init.
        public PasswordManager_Form()
        {
            InitializeComponent();

            AddEventHandlers();
            Modify_RadioButton.Checked = true;
            ThemeManager.SetThemeForForm(this);
            LanguageManager.UpdateLanguageForControl(this);
            Message_Label.MaximumSize = new Size(ClientSize.Width - 40, 0);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlers()
        {
            TextBoxManager.Attach(false, CurrentPassword_TextBox);
            TextBoxManager.Attach(false, NewPassword_TextBox);

            Modify_RadioButton.Click += (_, _) => Modify_RadioButton.Checked = true;
            Remove_RadioButton.Click += (_, _) => Remove_RadioButton.Checked = true;
        }

        // Form event handlers
        private void PasswordManager_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void Modify_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetUpdateButton();
        }
        private void Remove_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetUpdateButton();
        }
        private void CurrentPassword_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (Remove_RadioButton.Checked)
                {
                    Update_Button.PerformClick();
                }
            }
        }
        private void NewPassword_TextBox_TextChanged(object sender, EventArgs e)
        {
            ValidateButton();
        }
        private void CurrentPassword_TextBox_TextChanged(object sender, EventArgs e)
        {
            Update_Button.Enabled = CurrentPassword_TextBox.Text != "";
        }
        private void Update_Button_Click(object sender, EventArgs e)
        {
            if (Modify_RadioButton.Checked)
            {
                if (PasswordManager.Password != CurrentPassword_TextBox.Text)
                {
                    CustomMessageBox.Show("Incorrect passowrd", "The current password is incorrect.", CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                    CurrentPassword_TextBox.Focus();
                    return;
                }

                PasswordManager.Password = NewPassword_TextBox.Text;
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 4, $"Removed updated");
                CustomMessageBox.Show("Password updated", "Password updated successfully.", CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);
                Close();
            }
            else  // Remove_RadioButton.Checked
            {
                if (PasswordManager.Password != CurrentPassword_TextBox.Text)
                {
                    CustomMessageBox.Show("Incorrect passowrd", "The password is incorrect.", CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                    CurrentPassword_TextBox.Focus();
                    return;
                }

                CustomMessageBoxResult result = CustomMessageBox.Show("Remove password",
                   "Removing your password will make your data vulnerable to unauthorized access. Are you sure you want to continue?",
                   CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.YesNo);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return;
                }

                PasswordManager.Password = null;
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.ThingsThatHaveChangedInFile, 4, $"Removed password");
                CustomMessageBox.Show("Password removed", "Password removed successfully.", CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);
                Close();
            }
        }

        // Methods
        private void SetUpdateButton()
        {
            if (Modify_RadioButton.Checked)
            {
                Update_Button.Text = "Update password";

                foreach (Control item in GetControls())
                {
                    Controls.Add(item);
                }
                Height = 820;
                Update_Button.Top = 685;
                ValidateButton();
            }
            else
            {
                Update_Button.Text = "Remove password";

                foreach (Control item in GetControls())
                {
                    Controls.Remove(item);
                }
                Height = 420;
                Update_Button.Top = 285;
            }
        }
        private List<Control> GetControls()
        {
            return
            [
                NewPassword_TextBox,
                LengthRequirement_Label,
                NumberRequirement_Label,
                UppercaseRequirement_Label,
                SpecialCharacterRequirement_Label,
                Message_Label
            ];
        }
        private void ValidateButton()
        {
            Update_Button.Enabled = PasswordManager.ValidatePassword(
                LengthRequirement_Label,
                UppercaseRequirement_Label,
                NumberRequirement_Label,
                SpecialCharacterRequirement_Label,
                NewPassword_TextBox.Text
            );
        }
    }
}