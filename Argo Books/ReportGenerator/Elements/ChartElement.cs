using Argo_Books.Charts;
using Argo_Books.DataClasses;
using Argo_Books.GridView;
using Argo_Books.ReportGenerator;
using Guna.UI2.WinForms;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WinForms;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.Language;
using Argo_Books.ReportGenerator.Menus;
using SkiaSharp;
using System.Drawing.Drawing2D;

namespace Argo_Books.ReportGenerator.Elements
{
    /// <summary>
    /// Chart element for displaying graphs and charts.
    /// </summary>
    public class ChartElement : BaseElement, IDisposable
    {
        private static readonly Dictionary<MainMenu_Form.ChartDataType, (DateTime? StartDate, DateTime? EndDate, bool IncludeReturns, bool IncludeLosses)> _lastLoadedConfig = [];

        // Private backing fields
        private Control _chartControl;
        private MainMenu_Form.ChartDataType _chartType = MainMenu_Form.ChartDataType.TotalRevenue;
        private bool _showLegend = true;
        private bool _showTitle = true;
        private Color _borderColor = Color.Gray;
        private int _borderThickness = 1;
        private string _fontFamily = "Segoe UI";
        private float _titleFontSize = 12f;
        private float _legendFontSize = 11f;

        // Bitmap cache fields
        private Bitmap _cachedBitmap;
        private Size _cachedSize;
        private (DateTime? StartDate, DateTime? EndDate, bool IncludeReturns, bool IncludeLosses)? _cachedConfig;
        private float _cachedRenderScale;

        // Public properties with cache invalidation
        public MainMenu_Form.ChartDataType ChartType
        {
            get => _chartType;
            set
            {
                if (_chartType != value)
                {
                    _chartType = value;
                    InvalidateCache();
                }
            }
        }
        public bool ShowLegend
        {
            get => _showLegend;
            set
            {
                if (_showLegend != value)
                {
                    _showLegend = value;
                    InvalidateCache();
                }
            }
        }
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                if (_showTitle != value)
                {
                    _showTitle = value;
                    InvalidateCache();
                }
            }
        }
        public string FontFamily
        {
            get => _fontFamily;
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    InvalidateCache();
                }
            }
        }
        public float TitleFontSize
        {
            get => _titleFontSize;
            set
            {
                if (Math.Abs(_titleFontSize - value) > 0.01f)
                {
                    _titleFontSize = value;
                    InvalidateCache();
                }
            }
        }
        public float LegendFontSize
        {
            get => _legendFontSize;
            set
            {
                if (Math.Abs(_legendFontSize - value) > 0.01f)
                {
                    _legendFontSize = value;
                    InvalidateCache();
                }
            }
        }
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    InvalidateCache();
                }
            }
        }
        public int BorderThickness
        {
            get => _borderThickness;
            set
            {
                if (_borderThickness != value)
                {
                    _borderThickness = value;
                    InvalidateCache();
                }
            }
        }

        // Overrides
        public override byte MinimumSize => 80;
        public override string DisplayName => LanguageManager.TranslateString("chart");
        public override ReportElementType GetElementType() => ReportElementType.Chart;
        public override BaseElement Clone()
        {
            return new ChartElement
            {
                Id = Id,
                Bounds = Bounds,
                ZOrder = ZOrder,
                IsVisible = IsVisible,
                ChartType = ChartType,
                ShowLegend = ShowLegend,
                ShowTitle = ShowTitle,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness,
                FontFamily = FontFamily,
                TitleFontSize = TitleFontSize,
                LegendFontSize = LegendFontSize
            };
        }
        public override void RenderElement(Graphics graphics, ReportConfiguration config, float renderScale)
        {
            try
            {
                // Get the current graphics transform
                float scaleX = graphics.Transform.Elements[0];
                float scaleY = graphics.Transform.Elements[3];

                // Check if we need to regenerate the cached bitmap
                bool needsRegeneration = ShouldRegenerateBitmap(config, renderScale);

                if (needsRegeneration)
                {
                    // Check if configuration has changed and reload chart data if necessary
                    bool needsReload = ShouldReloadChart(ChartType, config);

                    if (_chartControl == null || needsReload)
                    {
                        // Dispose of old control if reloading
                        if (_chartControl != null && needsReload)
                        {
                            _chartControl.Dispose();
                            _chartControl = null;
                        }

                        LoadChartData(ChartType, config);
                    }

                    // Clear old cached bitmap
                    if (_cachedBitmap != null)
                    {
                        _cachedBitmap.Dispose();
                        _cachedBitmap = null;
                    }

                    // Generate and cache the new bitmap
                    if (_chartControl != null && _chartControl is CartesianChart cartesianChart)
                    {
                        _cachedBitmap = GenerateCartesianChartImage(cartesianChart, renderScale);
                    }
                    else if (_chartControl != null && _chartControl is PieChart pieChart)
                    {
                        _cachedBitmap = GeneratePieChartImage(pieChart, renderScale);
                    }
                    else if (_chartControl != null && _chartControl is GeoMap geoMap)
                    {
                        _cachedBitmap = GenerateGeoMapImage(geoMap, renderScale);
                    }

                    // Update cache metadata
                    if (_cachedBitmap != null)
                    {
                        _cachedSize = Bounds.Size;
                        _cachedRenderScale = renderScale;

                        if (config?.Filters != null)
                        {
                            _cachedConfig = (
                                config.Filters.StartDate,
                                config.Filters.EndDate,
                                config.Filters.IncludeReturns,
                                config.Filters.IncludeLosses
                            );
                        }
                    }
                }

                // Render the cached bitmap
                if (_cachedBitmap != null)
                {
                    GraphicsState state = graphics.Save();
                    try
                    {
                        graphics.ResetTransform();
                        PointF[] points = [new PointF(Bounds.X, Bounds.Y)];
                        Matrix tempTransform = new(scaleX, 0, 0, scaleY, graphics.Transform.Elements[4], graphics.Transform.Elements[5]);
                        tempTransform.TransformPoints(points);

                        Rectangle targetRect = new(
                            (int)points[0].X,
                            (int)points[0].Y,
                            (int)(Bounds.Width * scaleX),
                            (int)(Bounds.Height * scaleY)
                        );

                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;

                        graphics.DrawImage(_cachedBitmap, targetRect);
                    }
                    finally
                    {
                        graphics.Restore(state);
                    }
                }
                else
                {
                    // Fallback to placeholder if chart control is not available
                    RenderPlaceholder(graphics, $"Chart: {ChartType}");
                }

                // Draw border if needed (drawn after bitmap)
                if (_borderColor != Color.Transparent && _borderThickness > 0)
                {
                    using Pen borderPen = new(_borderColor, _borderThickness);

                    // Adjust rectangle to account for border thickness to prevent clipping
                    Rectangle borderRect = Bounds;
                    if (_borderThickness > 1)
                    {
                        int offset = _borderThickness / 2;
                        borderRect = new Rectangle(
                            Bounds.X + offset,
                            Bounds.Y + offset,
                            Bounds.Width - _borderThickness,
                            Bounds.Height - _borderThickness
                        );
                    }

                    graphics.DrawRectangle(borderPen, borderRect);
                }
            }
            catch
            {
                RenderError(graphics);
            }
        }
        private bool ShouldRegenerateBitmap(ReportConfiguration config, float renderScale)
        {
            // No cached bitmap yet
            if (_cachedBitmap == null)
            {
                return true;
            }

            // Size changed
            if (_cachedSize != Bounds.Size)
            {
                return true;
            }

            // Render scale changed
            if (Math.Abs(_cachedRenderScale - renderScale) > 0.001f)
            {
                return true;
            }

            // Configuration changed
            if (config?.Filters != null)
            {
                var currentConfig = (
                    config.Filters.StartDate,
                    config.Filters.EndDate,
                    config.Filters.IncludeReturns,
                    config.Filters.IncludeLosses
                );

                if (_cachedConfig == null || _cachedConfig.Value != currentConfig)
                {
                    return true;
                }
            }

            return false;
        }
        private static bool ShouldReloadChart(MainMenu_Form.ChartDataType chartType, ReportConfiguration config)
        {
            if (config?.Filters == null) { return false; }

            if (!_lastLoadedConfig.TryGetValue(chartType, out (DateTime? StartDate, DateTime? EndDate, bool IncludeReturns, bool IncludeLosses) lastConfig))
            {
                return true;  // Never loaded before
            }

            // Use tuple comparison for cleaner code
            (DateTime? StartDate, DateTime? EndDate, bool IncludeReturns, bool IncludeLosses) currentConfig = (
                config.Filters.StartDate,
                config.Filters.EndDate,
                config.Filters.IncludeReturns,
                config.Filters.IncludeLosses
            );

            return lastConfig != currentConfig;
        }

        // Load chart methods
        private void LoadChartData(MainMenu_Form.ChartDataType chartType, ReportConfiguration config = null)
        {
            bool isLine = MainMenu_Form.Instance.LineChart_ToggleSwitch.Checked;

            try
            {
                // Update the tracking dictionary with the current configuration
                if (config?.Filters != null)
                {
                    _lastLoadedConfig[chartType] = (
                        config.Filters.StartDate,
                        config.Filters.EndDate,
                        config.Filters.IncludeReturns,
                        config.Filters.IncludeLosses
                    );
                }
                else
                {
                    _lastLoadedConfig.Remove(chartType);
                }

                CreateIndependentChartControl(chartType);

                // Create filtered clones of the DataGridViews
                Guna2DataGridView salesDataGridView = CreateFilteredDataGridView(
                    MainMenu_Form.Instance.Sale_DataGridView,
                    config?.Filters?.StartDate,
                    config?.Filters?.EndDate);

                Guna2DataGridView purchasesDataGridView = CreateFilteredDataGridView(
                    MainMenu_Form.Instance.Purchase_DataGridView,
                    config?.Filters?.StartDate,
                    config?.Filters?.EndDate);

                // Load charts with filtered DataGridViews
                switch (chartType)
                {
                    case MainMenu_Form.ChartDataType.TotalRevenue:
                        LoadChart.LoadTotalsIntoChart(salesDataGridView, (CartesianChart)_chartControl, isLine);
                        break;

                    case MainMenu_Form.ChartDataType.TotalExpenses:
                        LoadChart.LoadTotalsIntoChart(purchasesDataGridView, (CartesianChart)_chartControl, isLine);
                        break;

                    case MainMenu_Form.ChartDataType.RevenueDistribution:
                        LoadChart.LoadDistributionIntoChart(salesDataGridView, (PieChart)_chartControl, PieChartGrouping.Top12);
                        break;

                    case MainMenu_Form.ChartDataType.ExpensesDistribution:
                        LoadChart.LoadDistributionIntoChart(purchasesDataGridView, (PieChart)_chartControl, PieChartGrouping.Top12);
                        break;

                    case MainMenu_Form.ChartDataType.TotalProfits:
                        LoadChart.LoadProfitsIntoChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.CountriesOfOrigin:
                        LoadChart.LoadCountriesOfOriginChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.CompaniesOfOrigin:
                        LoadChart.LoadCompaniesOfOriginChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.CountriesOfDestination:
                        LoadChart.LoadCountriesOfDestinationChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.AccountantsTransactions:
                        LoadChart.LoadAccountantsIntoChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.SalesVsExpenses:
                        LoadChart.LoadSalesVsExpensesChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.AverageTransactionValue:
                        LoadChart.LoadAverageTransactionValueChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.TotalTransactions:
                        LoadChart.LoadTotalTransactionsChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.AverageShippingCosts:
                        LoadChart.LoadAverageShippingCostsChart((CartesianChart)_chartControl, isLine,
                            includeZeroShipping: true,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.GrowthRates:
                        LoadChart.LoadGrowthRateChart((CartesianChart)_chartControl,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.ReturnsOverTime:
                        LoadChart.LoadReturnsOverTimeChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.ReturnReasons:
                        LoadChart.LoadReturnReasonsChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.ReturnFinancialImpact:
                        LoadChart.LoadReturnFinancialImpactChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.ReturnsByCategory:
                        LoadChart.LoadReturnsByCategoryChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.ReturnsByProduct:
                        LoadChart.LoadReturnsByProductChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.PurchaseVsSaleReturns:
                        LoadChart.LoadPurchaseVsSaleReturnsChart((PieChart)_chartControl,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.WorldMap:
                        LoadChart.LoadWorldMapChart((GeoMap)_chartControl, MainMenu_Form.GeoMapDataType.Combined,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.LossesOverTime:
                        LoadChart.LoadLossesOverTimeChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.LossReasons:
                        LoadChart.LoadLossReasonsChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.LossFinancialImpact:
                        LoadChart.LoadLossFinancialImpactChart((CartesianChart)_chartControl, isLine,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.LossesByCategory:
                        LoadChart.LoadLossesByCategoryChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.LossesByProduct:
                        LoadChart.LoadLossesByProductChart((PieChart)_chartControl, PieChartGrouping.Top8,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;

                    case MainMenu_Form.ChartDataType.PurchaseVsSaleLosses:
                        LoadChart.LoadPurchaseVsSaleLossesChart((PieChart)_chartControl,
                            purchasesDataGridView: purchasesDataGridView,
                            salesDataGridView: salesDataGridView);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Write(0, $"Error loading chart data for ChartElement: {ex.Message}");
            }
        }
        private static Guna2DataGridView CreateFilteredDataGridView(Guna2DataGridView sourceGrid, DateTime? startDate, DateTime? endDate)
        {
            // Create new DataGridView
            Guna2DataGridView filteredGrid = new();

            // Determine if this is a purchase or sale grid and initialize with appropriate columns
            bool isPurchaseGrid = sourceGrid == MainMenu_Form.Instance.Purchase_DataGridView;

            if (isPurchaseGrid)
            {
                DataGridViewManager.InitializeDataGridView(
                    filteredGrid,
                    "FilteredPurchases_DataGridView",
                    MainMenu_Form.Instance.PurchaseColumnHeaders,
                    null,
                    null
                );
            }
            else
            {
                DataGridViewManager.InitializeDataGridView(
                    filteredGrid,
                    "FilteredSales_DataGridView",
                    MainMenu_Form.Instance.SalesColumnHeaders,
                    null,
                    null
                );
            }

            // Get date range
            DateTime filterStart = startDate ?? DateTime.MinValue;
            DateTime filterEnd = endDate ?? DateTime.MaxValue;

            // Copy rows that fall within date range
            foreach (DataGridViewRow sourceRow in sourceGrid.Rows)
            {
                if (sourceRow.Cells[ReadOnlyVariables.Date_column].Value != null &&
                    DateTime.TryParse(sourceRow.Cells[ReadOnlyVariables.Date_column].Value.ToString(), out DateTime rowDate))
                {
                    // Check if row is within date range
                    if (rowDate >= filterStart && rowDate <= filterEnd)
                    {
                        DataGridViewRow newRow = (DataGridViewRow)sourceRow.Clone();

                        // Copy cell values
                        for (int i = 0; i < sourceRow.Cells.Count; i++)
                        {
                            newRow.Cells[i].Value = sourceRow.Cells[i].Value;
                        }

                        newRow.Tag = sourceRow.Tag;

                        filteredGrid.Rows.Add(newRow);
                    }
                }
            }

            return filteredGrid;
        }
        private void CreateIndependentChartControl(MainMenu_Form.ChartDataType chartType)
        {
            // Determine chart type and create appropriate control
            if (IsCartesianChartType(chartType))
            {
                _chartControl = new CartesianChart();
            }
            else if (IsPieChartType(chartType))
            {
                _chartControl = new PieChart();
            }
            else if (IsGeoMapType(chartType))
            {
                _chartControl = new GeoMap();
            }
        }
        private static bool IsCartesianChartType(MainMenu_Form.ChartDataType chartType)
        {
            return chartType is
                MainMenu_Form.ChartDataType.TotalRevenue or
                MainMenu_Form.ChartDataType.TotalExpenses or
                MainMenu_Form.ChartDataType.TotalProfits or
                MainMenu_Form.ChartDataType.SalesVsExpenses or
                MainMenu_Form.ChartDataType.AverageTransactionValue or
                MainMenu_Form.ChartDataType.TotalTransactions or
                MainMenu_Form.ChartDataType.AverageShippingCosts or
                MainMenu_Form.ChartDataType.GrowthRates or
                MainMenu_Form.ChartDataType.ReturnsOverTime or
                MainMenu_Form.ChartDataType.ReturnFinancialImpact or
                MainMenu_Form.ChartDataType.PurchaseVsSaleReturns or
                MainMenu_Form.ChartDataType.LossesOverTime or
                MainMenu_Form.ChartDataType.LossFinancialImpact or
                MainMenu_Form.ChartDataType.PurchaseVsSaleLosses;
        }
        private static bool IsPieChartType(MainMenu_Form.ChartDataType chartType)
        {
            return chartType is
                MainMenu_Form.ChartDataType.RevenueDistribution or
                MainMenu_Form.ChartDataType.ExpensesDistribution or
                MainMenu_Form.ChartDataType.CountriesOfOrigin or
                MainMenu_Form.ChartDataType.CompaniesOfOrigin or
                MainMenu_Form.ChartDataType.CountriesOfDestination or
                MainMenu_Form.ChartDataType.AccountantsTransactions or
                MainMenu_Form.ChartDataType.ReturnReasons or
                MainMenu_Form.ChartDataType.ReturnsByCategory or
                MainMenu_Form.ChartDataType.ReturnsByProduct or
                MainMenu_Form.ChartDataType.LossReasons or
                MainMenu_Form.ChartDataType.LossesByCategory or
                MainMenu_Form.ChartDataType.LossesByProduct;
        }
        private static bool IsGeoMapType(MainMenu_Form.ChartDataType chartType)
        {
            return chartType is MainMenu_Form.ChartDataType.WorldMap;
        }

        // Render chart methods
        private Bitmap GenerateCartesianChartImage(CartesianChart sourceChart, float renderScale = 1.0f)
        {
            // Render at much higher resolution for quality
            int renderWidth = (int)(Bounds.Width * renderScale);
            int renderHeight = (int)(Bounds.Height * renderScale);

            SKCartesianChart skChart = new()
            {
                Width = renderWidth,
                Height = renderHeight,
                Background = SKColors.White
            };

            // Create SKTypeface for font family
            SKTypeface typeface = SKTypeface.FromFamilyName(FontFamily);

            // Show title if enabled
            if (ShowTitle)
            {
                string titleText = TranslatedChartTitles.GetChartDisplayName(ChartType);

                SolidColorPaint titlePaint = new(SKColors.Black)
                {
                    SKTypeface = typeface
                };

                skChart.Title = new LabelVisual
                {
                    Text = titleText,
                    TextSize = (TitleFontSize + 3) * renderScale,
                    Paint = titlePaint,
                    Padding = new LiveChartsCore.Drawing.Padding(5 * renderScale)
                };
            }

            if (sourceChart.Series != null)
            {
                skChart.Series = sourceChart.Series.Select(series =>
                {
                    if (series is LineSeries<double> lineSeries)
                    {
                        return new LineSeries<double>
                        {
                            Values = lineSeries.Values,
                            Name = lineSeries.Name,
                            GeometrySize = lineSeries.GeometrySize * renderScale * 0.75,
                            GeometryFill = new SolidColorPaint(SKColors.White),
                            GeometryStroke = lineSeries.GeometryStroke != null && lineSeries.GeometryStroke is SolidColorPaint gsPaint
                                ? new SolidColorPaint(gsPaint.Color)
                                {
                                    StrokeThickness = (float)(gsPaint.StrokeThickness * renderScale * 0.75)
                                }
                                : lineSeries.GeometryStroke,
                            Fill = lineSeries.Fill != null && lineSeries.Fill is SolidColorPaint fillPaint
                                ? new SolidColorPaint(fillPaint.Color)
                                {
                                    StrokeThickness = (float)(fillPaint.StrokeThickness * renderScale)
                                }
                                : lineSeries.Fill,
                            Stroke = lineSeries.Stroke != null && lineSeries.Stroke is SolidColorPaint strokePaint
                                ? new SolidColorPaint(strokePaint.Color)
                                {
                                    StrokeThickness = (float)(strokePaint.StrokeThickness * renderScale * 0.75)
                                }
                                : lineSeries.Stroke,
                            LineSmoothness = lineSeries.LineSmoothness,
                            DataLabelsSize = lineSeries.DataLabelsSize * renderScale,
                            DataLabelsPaint = lineSeries.DataLabelsPaint != null && lineSeries.DataLabelsPaint is SolidColorPaint dlPaint
                                ? new SolidColorPaint(dlPaint.Color)
                                {
                                    SKTypeface = typeface
                                }
                                : lineSeries.DataLabelsPaint,
                            DataLabelsPosition = lineSeries.DataLabelsPosition
                        };
                    }
                    else if (series is ColumnSeries<double> columnSeries)
                    {
                        return new ColumnSeries<double>
                        {
                            Values = columnSeries.Values,
                            Name = columnSeries.Name,
                            MaxBarWidth = columnSeries.MaxBarWidth * renderScale,
                            Padding = columnSeries.Padding * renderScale,
                            Rx = columnSeries.Rx > 0 ? columnSeries.Rx * renderScale : 2 * renderScale,
                            Ry = columnSeries.Ry > 0 ? columnSeries.Ry * renderScale : 2 * renderScale,
                            Fill = columnSeries.Fill != null && columnSeries.Fill is SolidColorPaint fillPaint
                                ? new SolidColorPaint(fillPaint.Color)
                                {
                                    StrokeThickness = (float)(fillPaint.StrokeThickness * renderScale)
                                }
                                : columnSeries.Fill,
                            Stroke = columnSeries.Stroke != null && columnSeries.Stroke is SolidColorPaint strokePaint
                                ? new SolidColorPaint(strokePaint.Color)
                                {
                                    StrokeThickness = (float)(strokePaint.StrokeThickness * renderScale)
                                }
                                : columnSeries.Stroke,
                            DataLabelsSize = columnSeries.DataLabelsSize * renderScale,
                            DataLabelsPaint = columnSeries.DataLabelsPaint != null && columnSeries.DataLabelsPaint is SolidColorPaint dlPaint
                                ? new SolidColorPaint(dlPaint.Color)
                                {
                                    SKTypeface = typeface
                                }
                                : columnSeries.DataLabelsPaint,
                            DataLabelsPosition = columnSeries.DataLabelsPosition
                        };
                    }
                    return series;
                }).ToArray();
            }

            // Copy axis configuration with font properties
            if (sourceChart.XAxes != null)
            {
                skChart.XAxes = sourceChart.XAxes.Select(axis =>
                {
                    SolidColorPaint labelPaint = new(SKColors.Black)
                    {
                        SKTypeface = typeface
                    };

                    Axis newAxis = new()
                    {
                        Labels = axis.Labels,
                        Labeler = axis.Labeler,
                        MinLimit = axis.MinLimit,
                        MaxLimit = axis.MaxLimit,
                        LabelsPaint = labelPaint,
                        TextSize = LegendFontSize * renderScale
                    };
                    return newAxis;
                }).ToArray();
            }

            if (sourceChart.YAxes != null)
            {
                skChart.YAxes = sourceChart.YAxes.Select(axis =>
                {
                    SolidColorPaint labelPaint = new(SKColors.Black)
                    {
                        SKTypeface = typeface
                    };

                    Axis newAxis = new()
                    {
                        Labels = axis.Labels,
                        Labeler = axis.Labeler,
                        MinLimit = axis.MinLimit,
                        MaxLimit = axis.MaxLimit,
                        LabelsPaint = labelPaint,
                        SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                        {
                            StrokeThickness = 1 * renderScale
                        },
                        TextSize = LegendFontSize * renderScale
                    };
                    return newAxis;
                }).ToArray();
            }

            skChart.Legend = new CustomLegend(true, LegendFontSize);

            skChart.LegendPosition = ShowLegend
                ? LegendPosition.Top
                : LegendPosition.Hidden;

            SolidColorPaint legendPaint = new(SKColors.Black)
            {
                SKTypeface = typeface
            };
            skChart.LegendTextPaint = legendPaint;
            skChart.LegendTextSize = (LegendFontSize + 1) * renderScale;

            using SKImage image = skChart.GetImage();
            using SKData data = image.Encode();
            using MemoryStream stream = new(data.ToArray());
            return new Bitmap(stream);
        }
        private Bitmap GeneratePieChartImage(PieChart sourceChart, float renderScale)
        {
            // Render at much higher resolution for quality
            int renderWidth = (int)(Bounds.Width * renderScale);
            int renderHeight = (int)(Bounds.Height * renderScale);

            SKPieChart skChart = new()
            {
                Width = renderWidth,
                Height = renderHeight,
                Background = SKColors.White
            };

            // Create SKTypeface for font family
            SKTypeface typeface = SKTypeface.FromFamilyName(FontFamily);

            // Show title if enabled
            if (ShowTitle)
            {
                string titleText = TranslatedChartTitles.GetChartDisplayName(ChartType);

                SolidColorPaint titlePaint = new(SKColors.Black)
                {
                    SKTypeface = typeface
                };

                skChart.Title = new LabelVisual
                {
                    Text = titleText,
                    TextSize = (TitleFontSize + 3) * renderScale,
                    Paint = titlePaint,
                    Padding = new LiveChartsCore.Drawing.Padding(5 * renderScale)
                };
            }

            // Clone and scale series for pie charts
            if (sourceChart.Series != null)
            {
                skChart.Series = sourceChart.Series.Select(series =>
                {
                    if (series is PieSeries<double> pieSeries)
                    {
                        PieSeries<double> newPieSeries = new()
                        {
                            Values = pieSeries.Values,
                            Name = pieSeries.Name,
                            Fill = pieSeries.Fill,
                            Stroke = pieSeries.Stroke != null ?
                                new SolidColorPaint(((SolidColorPaint)pieSeries.Stroke).Color)
                                {
                                    StrokeThickness = (float)(pieSeries.Stroke.StrokeThickness * renderScale)
                                } : null,
                            Pushout = pieSeries.Pushout * renderScale,
                            InnerRadius = pieSeries.InnerRadius * renderScale,
                            HoverPushout = pieSeries.HoverPushout * renderScale,
                            DataLabelsSize = pieSeries.DataLabelsSize * renderScale,
                            DataLabelsPaint = pieSeries.DataLabelsPaint,
                            DataLabelsPosition = pieSeries.DataLabelsPosition
                        };
                        return newPieSeries;
                    }
                    return series;
                }).ToArray();
            }

            skChart.Legend = new CustomLegend(true, LegendFontSize);

            skChart.LegendPosition = ShowLegend
                ? LegendPosition.Right
                : LegendPosition.Hidden;

            using SKImage image = skChart.GetImage();
            using SKData data = image.Encode();
            using MemoryStream stream = new(data.ToArray());
            return new Bitmap(stream);
        }
        private Bitmap GenerateGeoMapImage(GeoMap sourceMap, float renderScale = 1.0f)
        {
            // Render at much higher resolution for quality
            int renderWidth = (int)(Bounds.Width * renderScale);
            int renderHeight = (int)(Bounds.Height * renderScale);

            // Create an SK map with the same data
            SKGeoMap skMap = new()
            {
                Width = renderWidth,
                Height = renderHeight,
                Background = SKColors.White
            };

            // Copy and scale series data from source map to SK map
            if (sourceMap.Series != null)
            {
                skMap.Series = sourceMap.Series.Select(series =>
                {
                    if (series is HeatLandSeries heatSeries)
                    {
                        HeatLandSeries newHeatSeries = new()
                        {
                            Lands = heatSeries.Lands,
                            Name = heatSeries.Name,
                            HeatMap = heatSeries.HeatMap
                        };
                        return newHeatSeries;
                    }
                    return series;
                }).ToArray();
            }

            // Copy map projection
            skMap.MapProjection = sourceMap.MapProjection;

            // Generate image
            using SKImage image = skMap.GetImage();
            using SKData data = image.Encode();
            using MemoryStream stream = new(data.ToArray());
            return new Bitmap(stream);
        }
        protected override int CreateElementSpecificControls(Panel container, int yPosition, Action onPropertyChanged)
        {
            // Get undo manager for recording property changes
            UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
            string text;

            // Chart type selection
            text = LanguageManager.TranslateString("Chart Type") + ":";
            Label chartTypeLabel = AddPropertyLabel(container, text, yPosition);

            Guna2TextBox chartTypeTextBox = AddPropertySearchBox(
                container,
                TranslatedChartTitles.GetChartDisplayName(ChartType),
                yPosition,
                GetChartTypeSearchResults,
                value =>
                {
                    MainMenu_Form.ChartDataType newChartType = GetChartTypeFromDisplayName(value);
                    if (ChartType != newChartType)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ChartType),
                            ChartType,
                            newChartType,
                            onPropertyChanged));
                        ChartType = newChartType;
                        onPropertyChanged();
                    }
                },
                chartTypeLabel);

            CacheControl("ChartType", chartTypeTextBox,
                () => chartTypeTextBox.Text = TranslatedChartTitles.GetChartDisplayName(ChartType));
            yPosition += ControlRowHeight;

            // Font Family (only for non-geomap charts)
            if (!IsGeoMapType(ChartType))
            {
                text = LanguageManager.TranslateString("Font") + ":";
                Label fontLabel = AddPropertyLabel(container, text, yPosition);

                Guna2TextBox fontTextBox = AddPropertySearchBox(
                    container,
                    FontFamily,
                    yPosition,
                    GetFontSearchResults,
                    value =>
                    {
                        if (FontFamily != value)
                        {
                            undoRedoManager?.RecordAction(new PropertyChangeAction(
                                this,
                                nameof(FontFamily),
                                FontFamily,
                                value,
                                onPropertyChanged));
                            FontFamily = value;
                            onPropertyChanged();
                        }
                    },
                    fontLabel);

                CacheControl("FontFamily", fontTextBox, () => fontTextBox.Text = FontFamily);
                yPosition += ControlRowHeight;

                // Make both TextBoxes the same size
                if (chartTypeTextBox.Width < fontTextBox.Width)
                {
                    fontTextBox.Width = chartTypeTextBox.Width;
                    fontTextBox.Left = chartTypeTextBox.Left;
                }
                else
                {
                    chartTypeTextBox.Width = fontTextBox.Width;
                    chartTypeTextBox.Left = fontTextBox.Left;
                }
            }

            // Title font size (only for non-geomap charts)
            if (!IsGeoMapType(ChartType))
            {
                text = LanguageManager.TranslateString("Title Size") + ":";
                AddPropertyLabel(container, text, yPosition, false, NumericUpDownWidth);
                Guna2NumericUpDown titleNumericUpDown = AddPropertyNumericUpDown(container, (decimal)TitleFontSize, yPosition,
                    value =>
                    {
                        float newTitleSize = (float)value;
                        if (Math.Abs(TitleFontSize - newTitleSize) > 0.01f)
                        {
                            undoRedoManager?.RecordAction(new PropertyChangeAction(
                                this,
                                nameof(TitleFontSize),
                                TitleFontSize,
                                newTitleSize,
                                onPropertyChanged));
                            TitleFontSize = newTitleSize;
                            onPropertyChanged();
                        }
                    }, 8, 20);
                CacheControl("TitleFontSize", titleNumericUpDown, () => titleNumericUpDown.Value = (decimal)TitleFontSize);
                yPosition += ControlRowHeight;
            }

            // Legend font size (only for non-geomap charts)
            if (!IsGeoMapType(ChartType))
            {
                text = LanguageManager.TranslateString("Legend Size") + ":";
                AddPropertyLabel(container, text, yPosition, false, NumericUpDownWidth);
                Guna2NumericUpDown legendNumericUpDown = AddPropertyNumericUpDown(container, (decimal)LegendFontSize, yPosition,
                    value =>
                    {
                        float newLegendSize = (float)value;
                        if (Math.Abs(LegendFontSize - newLegendSize) > 0.01f)
                        {
                            undoRedoManager?.RecordAction(new PropertyChangeAction(
                                this,
                                nameof(LegendFontSize),
                                LegendFontSize,
                                newLegendSize,
                                onPropertyChanged));
                            LegendFontSize = newLegendSize;
                            onPropertyChanged();
                        }
                    }, 8, 20);
                CacheControl("LegendFontSize", legendNumericUpDown, () => legendNumericUpDown.Value = (decimal)LegendFontSize);
                yPosition += ControlRowHeight;
            }

            // Border thickness
            text = LanguageManager.TranslateString("Border thickness") + ":";
            AddPropertyLabel(container, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown thicknessNumeric = AddPropertyNumericUpDown(container, _borderThickness, yPosition,
                value =>
                {
                    int newThickness = (int)value;
                    if (_borderThickness != newThickness)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(_borderThickness),
                            _borderThickness,
                            newThickness,
                            onPropertyChanged));
                        _borderThickness = newThickness;
                        onPropertyChanged();
                    }
                }, 0, 20);
            CacheControl("_borderThickness", thicknessNumeric, () => thicknessNumeric.Value = _borderThickness);
            yPosition += ControlRowHeight;

            // Border color
            text = LanguageManager.TranslateString("Border color") + ":";
            AddPropertyLabel(container, text, yPosition, false, ColorPickerWidth);
            Panel _borderColorPanel = AddColorPicker(container, yPosition, _borderColor,
                newColor =>
                {
                    if (_borderColor.ToArgb() != newColor.ToArgb())
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(_borderColor),
                            _borderColor,
                            newColor,
                            onPropertyChanged));
                        _borderColor = newColor;
                        onPropertyChanged();
                    }
                });

            CacheControl("_borderColor", _borderColorPanel, () => _borderColorPanel.BackColor = _borderColor);
            yPosition += ControlRowHeight;

            // Show legend checkbox (only for non-geomap charts)
            if (!IsGeoMapType(ChartType))
            {
                text = LanguageManager.TranslateString("Show Legend");
                Guna2CustomCheckBox legendCheck = AddPropertyCheckBoxWithLabel(container, text, ShowLegend, yPosition,
                    value =>
                    {
                        if (ShowLegend != value)
                        {
                            undoRedoManager?.RecordAction(new PropertyChangeAction(
                                this,
                                nameof(ShowLegend),
                                ShowLegend,
                                value,
                                onPropertyChanged));
                            ShowLegend = value;
                            onPropertyChanged();
                        }
                    });
                CacheControl("ShowLegend", legendCheck, () => legendCheck.Checked = ShowLegend);
                yPosition += CheckBoxRowHeight;
            }

            // Show title checkbox (only for non-geomap charts)
            if (!IsGeoMapType(ChartType))
            {
                text = LanguageManager.TranslateString("Show Title");
                Guna2CustomCheckBox titleCheck = AddPropertyCheckBoxWithLabel(container, text, ShowTitle, yPosition,
                    value =>
                    {
                        if (ShowTitle != value)
                        {
                            undoRedoManager?.RecordAction(new PropertyChangeAction(
                                this,
                                nameof(ShowTitle),
                                ShowTitle,
                                value,
                                onPropertyChanged));
                            ShowTitle = value;
                            onPropertyChanged();
                        }
                    });
                CacheControl("ShowTitle", titleCheck, () => titleCheck.Checked = ShowTitle);
                yPosition += CheckBoxRowHeight;
            }

            return yPosition;
        }
        private static List<SearchResult> GetChartTypeSearchResults()
        {
            return Enum.GetValues<MainMenu_Form.ChartDataType>()
                .Select(type => new SearchResult(
                    TranslatedChartTitles.GetChartDisplayName(type),
                    null,
                    0))
                .ToList();
        }
        private static MainMenu_Form.ChartDataType GetChartTypeFromDisplayName(string displayName)
        {
            foreach (MainMenu_Form.ChartDataType type in Enum.GetValues<MainMenu_Form.ChartDataType>())
            {
                if (TranslatedChartTitles.GetChartDisplayName(type) == displayName)
                {
                    return type;
                }
            }
            return MainMenu_Form.ChartDataType.TotalRevenue;  // Default fallback
        }
        private void RenderPlaceholder(Graphics graphics, string text)
        {
            using SolidBrush brush = new(Color.LightBlue);
            using Pen pen = new(Color.Gray, 1);
            using Font font = new("Segoe UI", 10);
            using SolidBrush textBrush = new(Color.Black);

            graphics.FillRectangle(brush, Bounds);
            graphics.DrawRectangle(pen, Bounds);

            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            graphics.DrawString(text, font, textBrush, Bounds, format);
        }
        public void InvalidateCache()
        {
            if (_cachedBitmap != null)
            {
                _cachedBitmap.Dispose();
                _cachedBitmap = null;
            }
        }

        // Dispose
        public void Dispose()
        {
            if (_chartControl != null)
            {
                _chartControl.Dispose();
                _chartControl = null;
            }

            if (_cachedBitmap != null)
            {
                _cachedBitmap.Dispose();
                _cachedBitmap = null;
            }

            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _chartControl?.Dispose();
                _chartControl = null;
            }
        }
        ~ChartElement()
        {
            Dispose(false);
        }
    }
}