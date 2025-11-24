namespace Argo_Books.ReportGenerator.Elements
{
    /// <summary>
    /// Provides utility methods for converting StringAlignment enums to user-friendly display text and vice versa.
    /// Supports both horizontal (Left/Center/Right) and vertical (Top/Middle/Bottom) alignment translations.
    /// </summary>
    public static class AlignmentHelper
    {
        // Arrays for ComboBoxes
        public static readonly string[] HorizontalOptions = ["Left", "Center", "Right"];
        public static readonly string[] VerticalOptions = ["Top", "Middle", "Bottom"];

        /// <summary>
        /// Converts a StringAlignment enum value to display-friendly text.
        /// </summary>
        public static string ToDisplayText(StringAlignment alignment, bool isVertical = false)
        {
            return alignment switch
            {
                StringAlignment.Near => isVertical ? "Top" : "Left",
                StringAlignment.Center => isVertical ? "Middle" : "Center",
                StringAlignment.Far => isVertical ? "Bottom" : "Right",
                _ => isVertical ? "Middle" : "Left"
            };
        }

        /// <summary>
        /// Converts display-friendly text back to a StringAlignment enum value.
        /// </summary>
        public static StringAlignment FromDisplayText(string text)
        {
            return text switch
            {
                "Left" or "Top" => StringAlignment.Near,
                "Center" or "Middle" => StringAlignment.Center,
                "Right" or "Bottom" => StringAlignment.Far,
                _ => StringAlignment.Center
            };
        }
    }
}