using Argo_Books.DataClasses;
using Argo_Books.Theme;
using Argo_Books;
using Argo_Books.Language;

namespace Argo_Books.UI
{
    /// <summary>
    /// Manages the display and positioning of labels in the Sales Tracker application, ensuring that labels
    /// for showing results and totals maintain consistent wording across language changes.
    /// </summary>
    public class LabelManager
    {
        // Properties
        private static readonly string _noDataLabelName = "NoData_Label";

        // Methods
        /// <summary>
        /// Updates the label text to show the specified results message while preserving the original base text.
        /// This method ensures that the label’s prefix remains consistent even if the application language changes.
        /// The label is then centered horizontally within the specified parent control.
        /// </summary>
        public static void ShowLabelWithBaseText(Label label, string text)
        {
            // Keep the first part (before ":") the same in case it's been translated
            string[] parts = label.Text.Split(':');
            string baseText = parts[0].Trim();
            label.Text = $"{baseText}: {text}";

            // Center the label horizontally
            label.Left = (label.Parent.ClientSize.Width - label.Width) / 2;

            label.Visible = true;
        }

        /// <summary>
        /// Updates the total label text to display the row count of the specified DataGridView while preserving
        /// the original base text to maintain consistency across language changes. The label is positioned near
        /// the bottom of the DataGridView, either to the left or right based on the positionOnLeft parameter.
        /// </summary>
        /// <example>
        /// Input text: "Total: 5"
        /// Output text: "Total: 6"
        /// </example>
        public static void ShowTotalLabel(Label totalLabel, DataGridView dataGridView, bool positionOnLeft = false)
        {
            if (MainMenu_Form.IsProgramLoading) { return; }

            // Extract the word "total" (or its translation) from the existing text
            string[] parts = totalLabel.Text.Split(':');
            string baseText = parts[0].Trim();
            int count = dataGridView.Rows.Cast<DataGridViewRow>().Count(r => r.Visible);
            totalLabel.Text = $"{baseText}: {count}";

            // Position the label near the bottom of the DataGridView
            int xPosition = positionOnLeft
                ? dataGridView.Left
                : dataGridView.Right - totalLabel.Width;
            totalLabel.Location = new Point(xPosition, dataGridView.Bottom + 10);
        }

        /// <summary>
        /// Updates the totals label text to display the row count with "transactions" text while preserving
        /// the original base text and transaction word to maintain consistency across language changes.
        /// </summary>
        /// <example>
        /// Input text: "Totals: (5 transactions)"
        /// Output text: "Total: (6 transactions)"
        /// </example>
        public static void ShowTotalsWithTransactions(Label totalsLabel, DataGridView dataGridView)
        {
            string baseText = totalsLabel.Text.Split(['(', ':'])[0].Trim();

            // Extract the word "transactions" (or its translation) from the existing text
            string transactionText = "transactions";  // default fallback
            if (totalsLabel.Text.Contains('('))
            {
                string match = totalsLabel.Text.Split('(')[1];
                if (match.Contains(' '))
                {
                    transactionText = match.Split(' ')[1].TrimEnd(')');
                }
            }

            // Get count of visible rows
            int count = dataGridView.Rows.Cast<DataGridViewRow>().Count(r => r.Visible);

            // Format with preserved translations
            totalsLabel.Text = $"{baseText}: ({count} {transactionText})";
        }

        /// <summary>
        /// If there is no data, then it adds a Label to the control.
        /// </summary>
        /// <returns>True if there is any data, otherwise false.</returns>
        public static bool ManageNoDataLabelOnControl(bool hasData, Control control)
        {
            Label existingLabel = control.Controls.OfType<Label>().FirstOrDefault(label =>
                label.Name?.ToString() == _noDataLabelName);

            if (!hasData)
            {
                // If there's no data and the label doesn't exist, create and add it
                if (existingLabel == null)
                {
                    Label label = new()
                    {
                        Font = new Font("Segoe UI", 12),
                        ForeColor = CustomColors.Text,
                        Text = LanguageManager.TranslateString("No data"),
                        AutoSize = true,
                        AccessibleDescription = AccessibleDescriptionManager.DoNotCache,
                        BackColor = Color.Transparent,
                        Name = _noDataLabelName
                    };

                    control.Controls.Add(label);
                    CenterNoDataLabelInControl(control);

                    control.Resize += (_, _) => CenterNoDataLabelInControl(control);
                    label.BringToFront();
                }
                return false;
            }
            else
            {
                // If there's data and the label exists, remove it
                if (existingLabel != null)
                {
                    control.Controls.Remove(existingLabel);
                    existingLabel.Dispose();
                }
                return true;
            }
        }
        private static void CenterNoDataLabelInControl(Control control)
        {
            Label label = control.Controls.OfType<Label>().FirstOrDefault(label =>
                label.Name?.ToString() == _noDataLabelName);

            if (label != null)
            {
                label.Location = new Point((control.Width - label.Width) / 2, (control.Height - label.Height) / 2);
            }
        }
        public static void AddNoRecentlyOpenedCompanies(Control parent, int labelWidth, int labelHeight)
        {
            float scale = DpiHelper.GetRelativeDpiScale();

            int scaledWidth = (int)(labelWidth * scale);
            int scaledHeight = (int)(labelHeight * scale);

            parent.Controls.Add(new Label
            {
                Text = LanguageManager.TranslateString("No recently opened companies"),
                Name = "NoRecentlyOpenedCompanies_Label",
                Size = new Size(scaledWidth, scaledHeight),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = CustomColors.Text
            });
        }
    }
}