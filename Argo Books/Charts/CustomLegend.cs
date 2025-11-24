using Argo_Books.ReportGenerator;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.SKCharts;
using Padding = LiveChartsCore.Drawing.Padding;

namespace Argo_Books.Charts
{
    /// <summary>
    /// A custom legend for LiveCharts that dynamically adjusts its layout, padding, and size
    /// based on the chart height to improve visual fit and avoid overflow.
    /// Supports multiline wrapping for bottom legends.
    /// </summary>
    public class CustomLegend(bool forReport = false, float legendFontSize = 11f) : SKDefaultLegend
    {
        private StackLayout _existingLayout;
        private StackLayout _existingBottomLayout;

        protected override Layout<SkiaSharpDrawingContext> GetLayout(Chart chart)
        {
            float chartHeight = GetChartHeight(chart);
            float chartWidth = GetChartWidth(chart);

            // Calculate dynamic padding based on chart height
            (float horizontalPadding, float verticalPadding) = CalculateDynamicPadding(chartHeight, forReport);

            // Determine legend position
            LegendPosition legendPosition = chart.LegendPosition;

            // For bottom or top legends, use a wrapping layout
            if (legendPosition == LegendPosition.Bottom || legendPosition == LegendPosition.Top)
            {
                return CreateCachedWrappingLayout(chart, chartWidth, chartHeight, horizontalPadding, verticalPadding);
            }

            // For other positions, use vertical stack layout
            _existingLayout ??= new StackLayout
            {
                Orientation = ContainerOrientation.Vertical,
                HorizontalAlignment = Align.Start,
                VerticalAlignment = Align.Middle
            };

            _existingLayout.Children.Clear();
            _existingLayout.Padding = new Padding(horizontalPadding, verticalPadding + 50, horizontalPadding, verticalPadding);

            // Add fresh legend items
            foreach (ISeries series in chart.Series.Where(x => x.IsVisibleAtLegend))
            {
                _existingLayout.Children.Add(new CustomLegendItem(series, chartHeight, forReport, legendFontSize));
            }

            return _existingLayout;
        }
        private Layout<SkiaSharpDrawingContext> CreateCachedWrappingLayout(Chart chart, float chartWidth, float chartHeight, float horizontalPadding, float verticalPadding)
        {
            // Create or reuse the bottom layout
            _existingBottomLayout ??= new StackLayout
            {
                Orientation = ContainerOrientation.Vertical,
                HorizontalAlignment = Align.Middle,
                VerticalAlignment = Align.Start
            };

            // Clear existing children to prevent duplication
            _existingBottomLayout.Children.Clear();
            _existingBottomLayout.Padding = new Padding(horizontalPadding, verticalPadding + 20, horizontalPadding, verticalPadding);

            List<CustomLegendItem> legendItems = chart.Series
                .Where(x => x.IsVisibleAtLegend)
                .Select(series => new CustomLegendItem(series, chartHeight, forReport, legendFontSize))
                .ToList();

            if (legendItems.Count == 0)
            {
                return _existingBottomLayout;
            }

            // Calculate available width for legend with renderScale applied
            float renderScale = forReport ? ReportRenderer.RenderScale : 1;
            float availableWidth = (chartWidth - (horizontalPadding * 2) - 40) * renderScale;
            float currentRowWidth = 0;
            StackLayout currentRow = CreateNewRow();

            foreach (CustomLegendItem item in legendItems)
            {
                // Estimate item width (this is approximate)
                float estimatedItemWidth = EstimateLegendItemWidth(item, chartHeight);

                // Check if item fits in current row
                if (currentRow.Children.Count > 0 && currentRowWidth + estimatedItemWidth > availableWidth)
                {
                    // Start new row
                    _existingBottomLayout.Children.Add(currentRow);
                    currentRow = CreateNewRow();
                    currentRowWidth = 0;
                }

                currentRow.Children.Add(item);
                currentRowWidth += estimatedItemWidth;
            }

            // Add the last row
            if (currentRow.Children.Count > 0)
            {
                _existingBottomLayout.Children.Add(currentRow);
            }

            return _existingBottomLayout;
        }
        private static StackLayout CreateNewRow()
        {
            return new StackLayout
            {
                Orientation = ContainerOrientation.Horizontal,
                HorizontalAlignment = Align.Middle,
                VerticalAlignment = Align.Start
            };
        }
        private float EstimateLegendItemWidth(CustomLegendItem item, float chartHeight)
        {
            // Estimate width based on text length and chart height
            // This is an approximation - actual width depends on font metrics
            (float bulletSize, float textSize, float textPadding) = CalculateDynamicSizes(chartHeight, forReport, legendFontSize);

            // Apply renderScale to bullet size
            float renderScale = forReport ? ReportRenderer.RenderScale : 1;
            float scaledBulletSize = bulletSize * renderScale;

            // Rough estimation: bullet + padding + text width
            // Assume average character width is about 60% of text size
            string text = item.Children.OfType<LabelGeometry>().FirstOrDefault()?.Text ?? "";
            float estimatedTextWidth = text.Length * textSize * 0.6f;

            return scaledBulletSize + textPadding + estimatedTextWidth + 20;  // Add some extra margin
        }
        public void ClearLayout()
        {
            _existingLayout?.Children.Clear();
            _existingLayout = null;

            _existingBottomLayout?.Children.Clear();
            _existingBottomLayout = null;
        }

        /// <summary>
        /// Gets the chart height from the chart view control.
        /// </summary>
        private static float GetChartHeight(Chart chart)
        {
            // Try to get the chart view from the chart's view property
            if (chart.View != null)
            {
                // Access the actual chart control (CartesianChart, PieChart, etc.)
                if (chart.View is Control chartControl)
                {
                    return chartControl.Height;
                }
            }

            // Default height if we can't access the control
            return 400f;
        }

        /// <summary>
        /// Gets the chart width from the chart view control.
        /// </summary>
        private static float GetChartWidth(Chart chart)
        {
            // Try to get the chart view from the chart's view property
            if (chart.View != null)
            {
                // Access the actual chart control (CartesianChart, PieChart, etc.)
                if (chart.View is Control chartControl)
                {
                    return chartControl.Width;
                }
            }

            // Default width if we can't access the control
            return 600f;
        }

        /// <summary>
        /// Calculates dynamic padding based on chart height to prevent legend overflow.
        /// </summary>
        private static (float horizontal, float vertical) CalculateDynamicPadding(float chartHeight, bool forReport)
        {
            float renderScale = forReport ? ReportRenderer.RenderScale : 1;

            // Define height thresholds and corresponding padding values
            float minHeight = 200f * renderScale;
            float normalHeight = 400f * renderScale;

            // Default padding values
            float defaultHorizontalPadding = 15f * renderScale;
            float defaultVerticalPadding = 4f * renderScale;

            // Minimum padding values to maintain readability
            float minHorizontalPadding = 8f * renderScale;
            float minVerticalPadding = 2f * renderScale;

            if (chartHeight >= normalHeight)
            {
                // Use default padding for normal-sized charts
                return (defaultHorizontalPadding, defaultVerticalPadding);
            }
            else if (chartHeight <= minHeight)
            {
                // Use minimum padding for very small charts
                return (minHorizontalPadding, minVerticalPadding);
            }
            else
            {
                // Linearly interpolate padding based on height
                float ratio = (chartHeight - minHeight) / (normalHeight - minHeight);
                float horizontalPadding = minHorizontalPadding + (defaultHorizontalPadding - minHorizontalPadding) * ratio;
                float verticalPadding = minVerticalPadding + (defaultVerticalPadding - minVerticalPadding) * ratio;

                return (horizontalPadding, verticalPadding);
            }
        }

        /// <summary>
        /// Calculates dynamic sizes for legend elements based on chart height.
        /// </summary>
        public static (float bulletSize, float textSize, float textPadding) CalculateDynamicSizes(float chartHeight, bool forReport, float legendFontSize)
        {
            float renderScale = forReport ? ReportRenderer.RenderScale : 1;

            // Define height thresholds
            float minHeight = 200f * renderScale;
            float normalHeight = 400f * renderScale;

            // Default sizes
            float defaultBulletSize = 14f * renderScale;
            float defaultTextSize = legendFontSize * renderScale;
            float defaultTextPadding = 10f * renderScale;

            // Minimum sizes
            float minBulletSize = 10f * renderScale;
            float minTextSize = legendFontSize * renderScale;

            if (chartHeight >= normalHeight)
            {
                return (defaultBulletSize, defaultTextSize, defaultTextPadding);
            }
            else if (chartHeight <= minHeight)
            {
                return (minBulletSize, minTextSize, defaultTextPadding);
            }
            else
            {
                // Linearly interpolate sizes based on height
                float ratio = (chartHeight - minHeight) / (normalHeight - minHeight);
                float bulletSize = minBulletSize + (defaultBulletSize - minBulletSize) * ratio;
                float textSize = minTextSize + (defaultTextSize - minTextSize) * ratio;

                return (bulletSize, textSize, defaultTextPadding);
            }
        }
    }
}