using Argo_Books.Theme;
using Guna.UI2.WinForms;
using Argo_Books.Language;

namespace Argo_Books.UI
{
    /// <summary>
    /// Provides custom tooltip functionality using Guna2HtmlToolTip.
    /// </summary>
    public static class CustomTooltip
    {
        // Properties - one tooltip per form
        private static readonly Dictionary<Form, Guna2HtmlToolTip> _tooltips = [];
        private static readonly Dictionary<Control, (string title, string message)> _tooltipData = [];

        // Methods
        /// <summary>
        /// Gets or creates the tooltip instance for a specific form.
        /// </summary>
        private static Guna2HtmlToolTip GetTooltipForForm(Form form)
        {
            if (!_tooltips.TryGetValue(form, out Guna2HtmlToolTip tooltip))
            {
                tooltip = new Guna2HtmlToolTip
                {
                    TitleFont = new Font("Segoe UI", 10, FontStyle.Bold),
                    TitleForeColor = CustomColors.Text,
                    BackColor = CustomColors.ControlBack,
                    ForeColor = CustomColors.Text,
                    BorderColor = CustomColors.ControlPanelBorder,
                    Font = new Font("Segoe UI", 10),
                    InitialDelay = 500,
                    ReshowDelay = 100
                };

                // Only show the tooltip if the setting is true
                tooltip.Popup += (_, e) => e.Cancel = !Properties.Settings.Default.ShowTooltips;

                _tooltips[form] = tooltip;

                // Clean up when form is disposed
                form.Disposed += (_, _) =>
                {
                    if (_tooltips.TryGetValue(form, out Guna2HtmlToolTip t))
                    {
                        t.Dispose();
                        _tooltips.Remove(form);
                    }
                };
            }
            return tooltip;
        }

        /// <summary>
        /// Sets or updates a tooltip for a control with optional warning formatting.
        /// Only shows tooltips if enabled in application settings.
        /// </summary>
        public static void SetToolTip(Control control, string title, string message)
        {
            // Find the parent form
            Form parentForm = control.FindForm();
            if (parentForm == null) { return; }

            // Store the ORIGINAL (untranslated) tooltip data for this control
            _tooltipData[control] = (title, message);

            // Translate and set the tooltip
            string translatedTitle = LanguageManager.TranslateString(title);
            string translatedMessage = LanguageManager.TranslateString(message);

            Guna2HtmlToolTip tooltip = GetTooltipForForm(parentForm);
            tooltip.ToolTipTitle = translatedTitle;
            tooltip.SetToolTip(control, translatedMessage);
        }

        /// <summary>
        /// Updates all existing tooltips to match the current theme colors.
        /// </summary>
        public static void UpdateAllToolTipThemes()
        {
            foreach (Guna2HtmlToolTip tooltip in _tooltips.Values)
            {
                tooltip.TitleForeColor = CustomColors.Text;
                tooltip.BackColor = CustomColors.ControlBack;
                tooltip.ForeColor = CustomColors.Text;
                tooltip.BorderColor = CustomColors.ControlPanelBorder;
            }
        }

        /// <summary>
        /// Updates all existing tooltips with translations for the current language.
        /// </summary>
        public static void UpdateAllToolTipTranslations(string targetLanguageAbbreviation)
        {
            foreach (KeyValuePair<Control, (string title, string message)> entry in _tooltipData)
            {
                Control control = entry.Key;
                string originalTitle = entry.Value.title;
                string originalMessage = entry.Value.message;

                // Find the parent form
                Form parentForm = control.FindForm();
                if (parentForm == null || !_tooltips.ContainsKey(parentForm)) { continue; }

                // Translate and update the tooltip
                string translatedTitle = LanguageManager.TranslateString(originalTitle, targetLanguageAbbreviation);
                string translatedMessage = LanguageManager.TranslateString(originalMessage, targetLanguageAbbreviation);

                Guna2HtmlToolTip tooltip = _tooltips[parentForm];
                tooltip.ToolTipTitle = translatedTitle;
                tooltip.SetToolTip(control, translatedMessage);
            }
        }
    }
}