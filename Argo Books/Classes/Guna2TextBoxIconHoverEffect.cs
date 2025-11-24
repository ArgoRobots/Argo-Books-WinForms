using Argo_Books.Theme;
using Guna.UI2.WinForms;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Provides hover effect functionality for the right icon in Guna2TextBox controls because Guna has not implemented it yet.
    /// </summary>
    public static class Guna2TextBoxIconHoverEffect
    {
        private static bool isIconHovered = false;
        private static Rectangle iconBounds;

        public static void Initialize(Guna2TextBox textbox)
        {
            // Calculate icon bounds once
            iconBounds = new Rectangle(
                textbox.Width - (textbox.IconRightSize.Width + 5 + textbox.IconRightOffset.X),
                textbox.Height / 2 - textbox.IconRightSize.Height / 2 + textbox.IconRightOffset.Y,
                textbox.IconRightSize.Width,
                textbox.IconRightSize.Height
            );

            textbox.Paint += (s, e) =>
            {
                if (isIconHovered)
                {
                    // Draw hover effect behind the icon by creating a larger area
                    Rectangle hoverEffectBounds = new(
                        iconBounds.X - 2,
                        iconBounds.Y - 2,
                        iconBounds.Width + 4,
                        iconBounds.Height + 4
                    );

                    using (SolidBrush brush = new(CustomColors.MouseHover))
                    {
                        e.Graphics.FillRectangle(brush, hoverEffectBounds);
                    }

                    // Redraw the icon after the hover effect to bring it to the front
                    e.Graphics.DrawImage(textbox.IconRight, iconBounds);
                }
            };

            textbox.MouseMove += (s, e) =>
            {
                bool wasHovered = isIconHovered;
                isIconHovered = iconBounds.Contains(e.Location);

                if (wasHovered != isIconHovered)
                {
                    textbox.Invalidate();
                }
            };

            textbox.MouseLeave += (_, _) =>
            {
                isIconHovered = false;
                textbox.Invalidate();
            };

            // Update icon bounds when control resizes
            textbox.Resize += (_, _) =>
            {
                iconBounds = new Rectangle(
                    textbox.Width - (textbox.IconRightSize.Width + 5 + textbox.IconRightOffset.X),
                    textbox.Height / 2 - textbox.IconRightSize.Height / 2 + textbox.IconRightOffset.Y,
                    textbox.IconRightSize.Width,
                    textbox.IconRightSize.Height
                );
            };

            textbox.IconRightClick += (_, _) =>
            {
                textbox.Clear();
            };
        }
    }
}