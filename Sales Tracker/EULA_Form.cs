using Sales_Tracker.Classes;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using System.Runtime.InteropServices;

namespace Sales_Tracker
{
    /// <summary>
    /// End User License Agreement form for application acceptance.
    /// </summary>
    public partial class EULA_Form : BaseForm
    {
        private bool _userActionTaken = false;

        // Init.
        public EULA_Form()
        {
            InitializeComponent();
            UpdateTheme();
            LoadEULAText();

            // Hide caret
            EULA_RichTextBox.MouseDown += (_, _) =>
            {
                HideCaret(EULA_RichTextBox.Handle);
            };

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.CustomizeScrollBar(EULA_RichTextBox);
            ThemeManager.MakeGButtonBluePrimary(Accept_Button);
            ThemeManager.MakeGButtonBlueSecondary(Decline_Button);
        }
        private void LoadEULAText()
        {
            // Add padding by setting a margin
            EULA_RichTextBox.SelectionIndent = 20;  // Left margin in pixels
            EULA_RichTextBox.SelectionRightIndent = 20;  // Right margin in pixels

            // Define text styles
            Font titleFont = new("Segoe UI", 14f, FontStyle.Bold);
            Font headingFont = new("Segoe UI", 11f, FontStyle.Bold);
            Font subheadingFont = new("Segoe UI", 10f, FontStyle.Bold);
            Font regularFont = new("Segoe UI", 9.75f);
            Font emphasisFont = new("Segoe UI", 9.75f, FontStyle.Bold);

            // First, add extra space at the top for visual padding
            EULA_RichTextBox.AppendText("\n");

            // Add title (center-aligned)
            AppendFormattedText("End User License Agreement for Argo Sales Tracker", titleFont, CustomColors.AccentBlue, true, HorizontalAlignment.Center);
            EULA_RichTextBox.AppendText("\n");

            // Reset to left alignment for the rest of the document
            EULA_RichTextBox.SelectionAlignment = HorizontalAlignment.Left;

            // Add important notice
            AppendFormattedText("Important - Read Carefully", headingFont);
            EULA_RichTextBox.AppendText("\n");

            // Introduction paragraph
            AppendFormattedText("This End User License Agreement (\"Agreement\") is a legal agreement between you (either an individual or a single entity) and Argo (\"Licensor\") for the software product Argo Sales Tracker, which includes computer software and may include associated media, printed materials, and online or electronic documentation (\"Software\").", regularFont);
            EULA_RichTextBox.AppendText("\n");

            // Bold agreement statement
            AppendFormattedText("By installing, copying, or otherwise using the Software, you agree to be bound by the terms of this Agreement. If you do not agree to the terms of this Agreement, do not install or use the Software.", emphasisFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 1
            AppendFormattedText("1. Grant of License", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("1.1 Free Version", subheadingFont);
            AppendFormattedText("If you are using the Free Version of the Software, Licensor grants you a non-exclusive, non-transferable license to use the Software with limited functionality as defined in the product documentation, subject to the terms and conditions of this Agreement.", regularFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("1.2 Paid Version", subheadingFont);
            AppendFormattedText("If you have purchased a license key for the Paid Version of the Software, Licensor grants you a non-exclusive, non-transferable license to use the Software with full functionality, subject to the terms and conditions of this Agreement.", regularFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("1.3 License Scope", subheadingFont);
            AppendFormattedText("This license permits you to:", regularFont);
            // Bullet points with indentation
            AppendFormattedText("   • Install and use the Software on one computer at a time", regularFont);
            AppendFormattedText("   • Make one copy of the Software for backup or archival purposes", regularFont);
            AppendFormattedText("   • Transfer the Software from one computer to another, provided it is used on only one computer at a time", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 2
            AppendFormattedText("2. License Restrictions", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("You may NOT:", regularFont);
            AppendFormattedText("   • Rent, lease, lend, sell, redistribute, sublicense or provide commercial hosting services with the Software", regularFont);
            AppendFormattedText("   • Copy, decompile, reverse engineer, disassemble, attempt to derive the source code of, modify, or create derivative works of the Software", regularFont);
            AppendFormattedText("   • Remove, alter, or obscure any copyright, trademark, or other proprietary rights notices from the Software", regularFont);
            AppendFormattedText("   • Use the Software in any manner that violates any applicable local, state, national, or international law", regularFont);
            AppendFormattedText("   • Share or distribute license keys for the Paid Version", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 3
            AppendFormattedText("3. Intellectual Property Rights", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("The Software is protected by copyright and other intellectual property laws and treaties. Licensor owns all title, copyright, and other intellectual property rights in and to the Software. This Agreement does not grant you any rights to trademarks or service marks of Licensor.", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 4
            AppendFormattedText("4. Anonymous Data Collection", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("4.1 Usage Data", subheadingFont);
            AppendFormattedText("The Software collects anonymous usage statistics to help improve the quality and performance of the Software. This data collection is enabled by default but can be disabled in the Settings panel. Data collected includes:", regularFont);
            AppendFormattedText("   • Export operations (type, duration, file size)", regularFont);
            AppendFormattedText("   • API usage (type, duration, tokens)", regularFont);
            AppendFormattedText("   • Language features (translation duration, amount of text)", regularFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("4.2 Storage and Control", subheadingFont);
            AppendFormattedText("All anonymous data is stored locally on your device. No data is sent to our servers without your explicit permission. You can export and delete this data at any time through the Settings panel.", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 5
            AppendFormattedText("5. Third-Party Services", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("The Software may use third-party services for certain functionality, including but not limited to Google Sheets, OpenAI, Microsoft Translator, and Open Exchange Rates. Your use of these services through the Software is subject to the respective terms and conditions of those services.", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 6
            AppendFormattedText("6. Warranty Disclaimer", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("The Software is provided \"as is\" without warranty of any kind, either express or implied, including, without limitation, the implied warranties of merchantability, fitness for a particular purpose, or non-infringement. Licensor does not warrant that the functions contained in the Software will meet your requirements or that the operation of the Software will be uninterrupted or error-free.", emphasisFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 7
            AppendFormattedText("7. Limitation of Liability", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("In no event shall Licensor be liable for any special, incidental, indirect, or consequential damages whatsoever (including, without limitation, damages for loss of business profits, business interruption, loss of business information, or any other pecuniary loss) arising out of the use of or inability to use the Software, even if Licensor has been advised of the possibility of such damages.", emphasisFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 8
            AppendFormattedText("8. Term and Termination", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("8.1 Term", subheadingFont);
            AppendFormattedText("This Agreement is effective until terminated.", regularFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("8.2 Termination", subheadingFont);
            AppendFormattedText("Your rights under this Agreement will terminate automatically without notice if you fail to comply with any of its terms. Upon termination, you must cease all use of the Software and destroy all copies, full or partial, of the Software.", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 9
            AppendFormattedText("9. Export Regulations", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("You agree to comply with all applicable international and national laws that apply to the Software, including the export regulations of your country.", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 10
            AppendFormattedText("10. Governing Law", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("This Agreement shall be governed by the laws of Canada, excluding its conflicts of law rules. Any disputes arising under or in connection with this Agreement shall be subject to the exclusive jurisdiction of the courts located in Canada.", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 11
            AppendFormattedText("11. Entire Agreement", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("This Agreement constitutes the entire agreement between you and Licensor concerning the Software and supersedes all prior or contemporaneous oral or written communications, proposals, and representations with respect to the Software or any other subject matter covered by this Agreement.", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Section 12
            AppendFormattedText("12. Contact Information", headingFont);
            EULA_RichTextBox.AppendText("\n");

            AppendFormattedText("If you have any questions about this Agreement, please contact:", regularFont);
            AppendFormattedText("   • Email: contact@argorobots.com", regularFont);
            EULA_RichTextBox.AppendText("\n\n");

            // Center-align the last updated info
            AppendFormattedText("Last updated: May 20, 2025", new Font("Segoe UI", 9f, FontStyle.Italic), null, true, HorizontalAlignment.Center);

            // Add some extra space at the bottom for padding
            EULA_RichTextBox.AppendText("\n");

            // Reset the selection to the beginning so the RichTextBox doesn't open with text selected
            EULA_RichTextBox.SelectionStart = 0;
            EULA_RichTextBox.SelectionLength = 0;
        }

        /// <summary>
        /// Helper function to append text with specific formatting.
        /// </summary>
        private void AppendFormattedText(string text, Font font, Color? color = null, bool addNewLine = true, HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            color ??= CustomColors.Text;

            EULA_RichTextBox.SelectionFont = font;
            EULA_RichTextBox.SelectionColor = color.Value;
            EULA_RichTextBox.SelectionAlignment = alignment;

            EULA_RichTextBox.AppendText(text + (addNewLine ? "\n" : ""));
        }

        // Form event handlers
        private void EULA_Form_Shown(object sender, EventArgs e)
        {
            Title_Label.Focus();  // Remove the caret (blinking text cursor)
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void EULA_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Skip the confirmation if user has already clicked Accept or Decline
            if (_userActionTaken)
            {
                return;
            }

            // User is trying to close the form using the X button or Alt+F4
            CustomMessageBoxResult result = CustomMessageBox.Show(
                  "Application Closing",
                  "The End User License Agreement must be accepted to use Argo Sales Tracker. The application will now exit.",
                  CustomMessageBoxIcon.Info, CustomMessageBoxButtons.OkCancel);

            if (result == CustomMessageBoxResult.Ok)
            {
                DialogResult = DialogResult.Cancel;
                _userActionTaken = true;
                Application.Exit();
            }
            else
            {
                e.Cancel = true;  // Prevent the form from closing
            }
        }

        // Event handlers
        private void AcceptLabel_Click(object sender, EventArgs e)
        {
            Accept_CheckBox.Checked = !Accept_CheckBox.Checked;
        }
        private void Accept_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Accept_Button.Enabled = Accept_CheckBox.Checked;
        }
        private void Accept_Button_Click(object sender, EventArgs e)
        {
            DataFileManager.SetValue(GlobalAppDataSettings.EULAAccepted, bool.TrueString);
            Log.Write(2, "User accepted the EULA");
            _userActionTaken = true;
            DialogResult = DialogResult.OK;
            Close();
        }
        private void Decline_Button_Click(object sender, EventArgs e)
        {
            // Show confirmation message before declining
            CustomMessageBoxResult result = CustomMessageBox.Show(
                "Decline License Agreement",
                "If you decline the License Agreement, Argo Sales Tracker will exit. Are you sure you want to decline?",
                CustomMessageBoxIcon.Question, CustomMessageBoxButtons.YesNo);

            if (result == CustomMessageBoxResult.Yes)
            {
                // User confirmed they want to decline
                DataFileManager.SetValue(GlobalAppDataSettings.EULAAccepted, bool.FalseString);
                Log.Write(2, "User declined the EULA");
                _userActionTaken = true;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        // Caret
        [LibraryImport("user32.dll", EntryPoint = "HideCaret")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool HideCaret(IntPtr hWnd);
    }
}