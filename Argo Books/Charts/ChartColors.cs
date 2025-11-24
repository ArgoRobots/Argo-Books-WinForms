using Argo_Books.Theme;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

namespace Argo_Books.Charts
{
    /// <summary>
    /// Provides configuration for theming of LiveCharts.
    /// Configures colors and styles to match the application's current theme.
    /// </summary>
    internal class ChartColors
    {
        /// <summary>
        /// Applies theming to a CartesianChart.
        /// </summary>
        public static void ApplyTheme(CartesianChart chart)
        {
            SKColor foreColor = ToSKColor(CustomColors.Text);
            SKColor backgroundColor = ToSKColor(CustomColors.ContentPanelBackground);

            chart.BackColor = CustomColors.ContentPanelBackground;
            chart.LegendTextPaint = new SolidColorPaint(foreColor);
            chart.ForeColor = CustomColors.Text;

            chart.TooltipTextPaint = new SolidColorPaint(foreColor);
            chart.TooltipBackgroundPaint = new SolidColorPaint(backgroundColor);
            chart.TooltipTextSize = 18;
        }

        /// <summary>
        /// Applies theming to a PieChart.
        /// </summary>
        public static void ApplyTheme(PieChart chart)
        {
            SKColor foreColor = ToSKColor(CustomColors.Text);
            SKColor backgroundColor = ToSKColor(CustomColors.ContentPanelBackground);

            chart.BackColor = CustomColors.ContentPanelBackground;
            chart.LegendTextPaint = new SolidColorPaint(foreColor);
            chart.ForeColor = CustomColors.Text;

            chart.TooltipTextPaint = new SolidColorPaint(foreColor);
            chart.TooltipBackgroundPaint = new SolidColorPaint(backgroundColor);
            chart.TooltipTextSize = 18;
        }

        /// <summary>
        /// Applies theming to a GeoMap.
        /// </summary>
        public static void ApplyTheme(GeoMap geoMap)
        {
            geoMap.BackColor = CustomColors.ContentPanelBackground;
        }

        /// <summary>
        /// Converts Color to SKColor.
        /// </summary>
        public static SKColor ToSKColor(Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Creates a solid color paint for chart elements using SKColor.
        /// </summary>
        public static SolidColorPaint CreateSolidColorPaint(SKColor color, float strokeThickness = 1)
        {
            return new SolidColorPaint(color) { StrokeThickness = strokeThickness };
        }
    }
}