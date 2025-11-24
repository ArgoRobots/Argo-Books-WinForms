using Guna.UI2.WinForms;

namespace Argo_Books.UI
{
    public static class DpiHelper
    {
        // I designed this app with my Windows display scale set to 150%
        private static readonly float DESIGN_DPI_SCALE = 1.5f;

        private static float? _cachedDpiScale = null;

        /// <summary>
        /// Gets the relative DPI scale factor compared to the design baseline DPI.
        /// </summary>
        public static float GetRelativeDpiScale()
        {
            // Return cached value if already calculated
            if (_cachedDpiScale.HasValue)
            {
                return _cachedDpiScale.Value;
            }

            // Calculate and cache the DPI scale
            _cachedDpiScale = CalculateDpiScale();
            return _cachedDpiScale.Value;
        }

        /// <summary>
        /// Gets the current DPI scale factor.
        /// </summary>
        private static float CalculateDpiScale()
        {
            try
            {
                using Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
                float currentDpiScale = graphics.DpiX / 96f;
                return currentDpiScale / DESIGN_DPI_SCALE;
            }
            catch
            {
                return 1.0f;
            }
        }

        /// <summary>
        /// Scales the item height of a ComboBox based on the current DPI scale.
        /// </summary>
        public static void ScaleComboBox(Guna2ComboBox comboBox)
        {
            float scale = GetRelativeDpiScale();

            comboBox.ItemHeight = (int)(comboBox.ItemHeight * scale);
        }

        /// <summary>
        /// Scales the image size of a Button based on the current DPI scale.
        /// </summary>
        public static void ScaleImageSize(Guna2Button button)
        {
            float scale = GetRelativeDpiScale();
            int scaledWidth = (int)(button.ImageSize.Width * scale);
            int scaledHeight = (int)(button.ImageSize.Height * scale);

            button.ImageSize = new(scaledWidth, scaledHeight);
        }

        /// <summary>
        /// Scales the image size of an ImageButton based on the current DPI scale.
        /// </summary>
        public static void ScaleImageButton(Guna2ImageButton button)
        {
            float scale = GetRelativeDpiScale();
            int scaledImageSize = (int)(button.ImageSize.Width * scale);

            button.ImageSize = new(scaledImageSize, scaledImageSize);
        }

        /// <summary>
        /// Scales the top custom border thickness of a GroupBox based on the current DPI scale.
        /// </summary>
        public static void ScaleGroupBox(Guna2GroupBox groupBox)
        {
            float scale = GetRelativeDpiScale();
            int scaledTop = (int)(groupBox.CustomBorderThickness.Top * scale);
            groupBox.CustomBorderThickness = new Padding(
                groupBox.CustomBorderThickness.Left,
                scaledTop,
                groupBox.CustomBorderThickness.Right,
                groupBox.CustomBorderThickness.Bottom
            );
        }
    }
}