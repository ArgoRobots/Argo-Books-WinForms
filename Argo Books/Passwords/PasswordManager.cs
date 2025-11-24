using Argo_Books.Classes;
using Argo_Books.Encryption;
using Argo_Books.Theme;
using Guna.UI2.WinForms;
using Argo_Books;
using Argo_Books.Passwords;
using Argo_Books.Properties;
using Argo_Books.Theme;

namespace Argo_Books.Passwords
{
    /// <summary>
    /// Provides password management functionality, including password validation and prompting the user for password entry.
    /// </summary>
    internal class PasswordManager
    {
        // Getters and setters
        public static bool IsPasswordValid { get; set; }
        public static string Password { get; set; }
        public static bool HasPassword => !string.IsNullOrEmpty(Password);

        /// <summary>
        /// Validates the password based on specified requirements and updates the label colors.
        /// </summary>
        /// <returns>True if the password meets all the requirements, otherwise false.</returns>
        public static bool ValidatePassword(Label length, Label uppercase, Label digit, Label specialChar, string password)
        {
            PasswordValidationResult result = ValidatePasswordWithFlags(length, uppercase, digit, specialChar, password);
            return result.IsValid;
        }

        /// <summary>
        /// Validates the password based on specified requirements, updates the label colors, and returns flags for each requirement.
        /// </summary>
        /// <returns>PasswordValidationResult containing all validation flags.</returns>
        public static PasswordValidationResult ValidatePasswordWithFlags(Label length, Label uppercase, Label digit, Label specialChar, string password)
        {
            bool lengthValid = false;
            bool uppercaseValid = false;
            bool digitValid = false;
            bool specialCharValid = false;

            if (password.Length >= 8)
            {
                length.ForeColor = CustomColors.AccentGreen;
                lengthValid = true;
            }
            else
            {
                length.ForeColor = CustomColors.Text;
            }

            if (password.Any(char.IsUpper))
            {
                uppercase.ForeColor = CustomColors.AccentGreen;
                uppercaseValid = true;
            }
            else
            {
                uppercase.ForeColor = CustomColors.Text;
            }

            if (password.Any(char.IsDigit))
            {
                digit.ForeColor = CustomColors.AccentGreen;
                digitValid = true;
            }
            else
            {
                digit.ForeColor = CustomColors.Text;
            }

            if (password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                specialChar.ForeColor = CustomColors.AccentGreen;
                specialCharValid = true;
            }
            else
            {
                specialChar.ForeColor = CustomColors.Text;
            }

            bool isValid = lengthValid && uppercaseValid && digitValid && specialCharValid;
            return new PasswordValidationResult(isValid, lengthValid, uppercaseValid, digitValid, specialCharValid);
        }

        /// <summary>
        /// If a password is set, the user is prompted to enter it.
        /// If there are multiple accountants, the user is prompted to select one.
        /// If there's only one accountant, it's automatically selected.
        /// </summary>
        /// <returns>True if the password is entered correctly (or not required) and accountant is selected (if required). Otherwise, false.</returns>
        public static bool EnterPassword(bool allowWindowsHello = true)
        {
            List<string> accountants = FooterManager.GetAccountants(Directories.ArgoCompany_file);
            bool requiresAccountantSelection = accountants.Count >= 2 && allowWindowsHello;

            Password = FooterManager.GetPassword(Directories.ArgoCompany_file);
            bool requiresPassword = HasPassword;

            // Auto-select single accountant
            if (accountants.Count == 1)
            {
                MainMenu_Form.SelectedAccountant = accountants[0];
            }
            else if (accountants.Count == 0)
            {
                MainMenu_Form.SelectedAccountant = null;
            }

            if (requiresAccountantSelection || requiresPassword)
            {
                new EnterPassword_Form(allowWindowsHello, requiresAccountantSelection, requiresPassword).ShowDialog();

                // Check if both required inputs are valid
                bool accountantValid = !requiresAccountantSelection || !string.IsNullOrEmpty(MainMenu_Form.SelectedAccountant);
                bool passwordValid = !requiresPassword || IsPasswordValid;

                return accountantValid && passwordValid;
            }

            return true;
        }

        /// <summary>
        /// Toggles the visibility of the password characters in a <see cref="TextBox"/> by switching 
        /// between the plain text and a masked character. Also updates the associated eye icon 
        /// based on the current application theme.
        /// </summary>
        public static void TogglePasswordVisibility(Guna2TextBox textBox, Guna2CircleButton eyeButton)
        {
            bool isDarkTheme = ThemeManager.IsDarkTheme();

            // Toggle the password character
            if (textBox.PasswordChar == '\0')
            {
                textBox.PasswordChar = '•';
                eyeButton.Image = isDarkTheme ? Resources.ViewWhite : Resources.ViewBlack;
            }
            else
            {
                textBox.PasswordChar = '\0';
                eyeButton.Image = isDarkTheme ? Resources.HideWhite : Resources.HideBlack;
            }

            textBox.Focus();
        }
    }
}