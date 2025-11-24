using Argo_Books.Theme;
using Guna.UI2.WinForms;

namespace Argo_Books.UI
{
    public static class TextBoxTooltip
    {
        // Properties
        private static readonly Dictionary<Control, Guna2HtmlToolTip> _tooltips = [];
        private static readonly Dictionary<Control, bool> _tooltipStates = [];

        // Methods
        public static void SetOverflowTooltip(Guna2TextBox textBox)
        {
            if (!_tooltips.TryGetValue(textBox, out Guna2HtmlToolTip tooltip))
            {
                tooltip = CreateTooltip();
                _tooltips[textBox] = tooltip;
                _tooltipStates[textBox] = false;

                textBox.MouseHover += TextBox_MouseHover;
                textBox.MouseLeave += HandleMouseLeave;
                textBox.TextChanged += UpdateTooltip;
            }
        }
        private static Guna2HtmlToolTip CreateTooltip()
        {
            return new Guna2HtmlToolTip
            {
                TitleFont = new Font("Segoe UI", 10, FontStyle.Bold),
                TitleForeColor = CustomColors.Text,
                BackColor = CustomColors.ControlBack,
                ForeColor = CustomColors.Text,
                BorderColor = CustomColors.ControlPanelBorder,
                Font = new Font("Segoe UI", 10),
                InitialDelay = 500,
                ReshowDelay = 100,
            };
        }
        private static void TextBox_MouseHover(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;

            if (IsTextOverflowing(textBox) && !_tooltipStates[textBox])
            {
                _tooltips[textBox].Show(textBox.Text, textBox);
                _tooltipStates[textBox] = true;
            }
        }
        private static void HandleMouseLeave(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;
            _tooltips[textBox].Hide(textBox);
            _tooltipStates[textBox] = false;
        }
        private static void UpdateTooltip(object sender, EventArgs e)
        {
            Guna2TextBox textBox = (Guna2TextBox)sender;

            if (IsTextOverflowing(textBox))
            {
                _tooltips[textBox].SetToolTip(textBox, textBox.Text);
            }
            else
            {
                _tooltips[textBox].Hide(textBox);
                _tooltipStates[textBox] = false;
                _tooltips[textBox].SetToolTip(textBox, "");
            }
        }
        private static bool IsTextOverflowing(Guna2TextBox textBox)
        {
            using Graphics g = textBox.CreateGraphics();
            float textWidth = g.MeasureString(textBox.Text, textBox.Font).Width;
            int availableWidth = textBox.ClientSize.Width - textBox.Padding.Horizontal - 10;
            return textWidth > availableWidth;
        }
    }
}