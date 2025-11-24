using Argo_Books.Classes;
using Argo_Books.ReportGenerator.Elements;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.ReportGenerator.Menus;
using System.Formats.Tar;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Argo_Books.ReportGenerator
{
    /// <summary>
    /// Handles storage and retrieval of custom report templates.
    /// </summary>
    public static class CustomTemplateStorage
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new ColorConverter(),
                new MarginsConverter()
            }
        };
        private static readonly string[] sourceArray = [".png", ".jpg", ".jpeg", ".svg"];

        static CustomTemplateStorage()
        {
            // Ensure templates directory exists
            if (!Directory.Exists(Directories.ReportTemplates_dir))
            {
                Directory.CreateDirectory(Directories.ReportTemplates_dir);
            }
        }

        /// <summary>
        /// Saves a custom template with the specified name.
        /// </summary>
        public static bool SaveTemplate(string templateName, ReportConfiguration config)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(templateName))
                {
                    return false;
                }

                // Create a copy of the configuration to save
                CustomTemplate template = new()
                {
                    Name = templateName,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    PageSize = config.PageSize,
                    PageOrientation = config.PageOrientation,
                    ShowHeader = config.ShowHeader,
                    ShowFooter = config.ShowFooter,
                    ShowPageNumbers = config.ShowPageNumbers,
                    BackgroundColor = config.BackgroundColor,
                    PageMargins = new Margins(
                        config.PageMargins.Left,
                        config.PageMargins.Top,
                        config.PageMargins.Right,
                        config.PageMargins.Bottom
                    ),
                    Filters = new ReportFilters
                    {
                        TransactionType = config.Filters.TransactionType,
                        IncludeReturns = config.Filters.IncludeReturns,
                        IncludeLosses = config.Filters.IncludeLosses,
                        DatePresetName = config.Filters.DatePresetName,
                        StartDate = config.Filters.StartDate,
                        EndDate = config.Filters.EndDate,
                        SelectedChartTypes = [.. config.Filters.SelectedChartTypes]
                    },
                    Elements = config.Elements.Select(e => SerializeElement(e)).ToList()
                };

                string fileName = SanitizeFileName(templateName) + ArgoFiles.JsonFileExtension;
                string filePath = Path.Combine(Directories.ReportTemplates_dir, fileName);

                string json = JsonSerializer.Serialize(template, _jsonOptions);
                File.WriteAllText(filePath, json);

                CleanupUnusedImages();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Loads a custom template by name.
        /// </summary>
        public static ReportConfiguration? LoadTemplate(string templateName)
        {
            try
            {
                string fileName = SanitizeFileName(templateName) + ArgoFiles.JsonFileExtension;
                string filePath = Path.Combine(Directories.ReportTemplates_dir, fileName);

                if (!File.Exists(filePath))
                {
                    return null;
                }

                string json = File.ReadAllText(filePath);
                CustomTemplate template = JsonSerializer.Deserialize<CustomTemplate>(json, _jsonOptions);

                if (template == null)
                {
                    return null;
                }

                // Convert back to ReportConfiguration
                ReportConfiguration config = new()
                {
                    Title = template.Name,
                    PageSize = template.PageSize,
                    PageOrientation = template.PageOrientation,
                    ShowHeader = template.ShowHeader,
                    ShowFooter = template.ShowFooter,
                    ShowPageNumbers = template.ShowPageNumbers,
                    BackgroundColor = template.BackgroundColor,
                    PageMargins = new Margins(
                        template.PageMargins.Left,
                        template.PageMargins.Top,
                        template.PageMargins.Right,
                        template.PageMargins.Bottom
                    ),
                    Filters = new ReportFilters
                    {
                        TransactionType = template.Filters.TransactionType,
                        IncludeReturns = template.Filters.IncludeReturns,
                        IncludeLosses = template.Filters.IncludeLosses,
                        DatePresetName = template.Filters.DatePresetName,
                        StartDate = template.Filters.StartDate,
                        EndDate = template.Filters.EndDate,
                        SelectedChartTypes = [.. template.Filters.SelectedChartTypes]
                    }
                };

                // Deserialize elements
                foreach (SerializedElement serializedElement in template.Elements)
                {
                    BaseElement element = DeserializeElement(serializedElement);
                    if (element != null)
                    {
                        config.AddElement(element);
                    }
                }

                return config;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a list of all saved custom template names.
        /// </summary>
        public static List<string> GetCustomTemplateNames()
        {
            try
            {
                if (!Directory.Exists(Directories.ReportTemplates_dir))
                {
                    return [];
                }

                return Directory.GetFiles(Directories.ReportTemplates_dir, "*.json")
                   .Select(Path.GetFileNameWithoutExtension)
                   .Where(name => name != null)
                   .OrderBy(name => name)
                   .ToList()!;
            }
            catch
            {
                return [];
            }
        }

        /// <summary>
        /// Deletes a custom template by name.
        /// </summary>
        public static bool DeleteTemplate(string templateName)
        {
            try
            {
                string fileName = SanitizeFileName(templateName) + ArgoFiles.JsonFileExtension;
                string filePath = Path.Combine(Directories.ReportTemplates_dir, fileName);

                if (File.Exists(filePath))
                {
                    Directories.DeleteFile(filePath);
                    CleanupUnusedImages();

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Renames a custom template.
        /// </summary>
        public static bool RenameTemplate(string oldTemplateName, string newTemplateName)
        {
            try
            {
                string oldFileName = SanitizeFileName(oldTemplateName) + ArgoFiles.JsonFileExtension;
                string newFileName = SanitizeFileName(newTemplateName) + ArgoFiles.JsonFileExtension;
                string oldFilePath = Path.Combine(Directories.ReportTemplates_dir, oldFileName);
                string newFilePath = Path.Combine(Directories.ReportTemplates_dir, newFileName);

                if (!File.Exists(oldFilePath))
                {
                    return false;
                }

                if (File.Exists(newFilePath))
                {
                    return false;  // New name already exists
                }

                // Load the template to update its internal Name property
                CustomTemplate template = JsonSerializer.Deserialize<CustomTemplate>(
                    File.ReadAllText(oldFilePath), _jsonOptions);

                if (template == null)
                {
                    return false;
                }

                // Update the template name and save to new file
                template.Name = newTemplateName;
                template.LastModified = DateTime.Now;

                string json = JsonSerializer.Serialize(template, _jsonOptions);
                File.WriteAllText(newFilePath, json);

                // Delete the old file
                Directories.DeleteFile(oldFilePath);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a template with the given name already exists.
        /// </summary>
        public static bool TemplateExists(string templateName)
        {
            string fileName = SanitizeFileName(templateName) + ArgoFiles.JsonFileExtension;
            string filePath = Path.Combine(Directories.ReportTemplates_dir, fileName);
            return File.Exists(filePath);
        }
        public static string SanitizeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }

        /// <summary>
        /// Exports a template with its images to a .ArgoSalesTemplate file (TAR archive).
        /// </summary>
        public static bool ExportTemplateWithImages(string templateName, string destinationPath)
        {
            try
            {
                // Load the template
                string sourceFileName = SanitizeFileName(templateName) + ArgoFiles.JsonFileExtension;
                string sourceFilePath = Path.Combine(Directories.ReportTemplates_dir, sourceFileName);

                if (!File.Exists(sourceFilePath))
                {
                    return false;
                }

                // Read the template JSON
                string templateJson = File.ReadAllText(sourceFilePath);
                CustomTemplate template = JsonSerializer.Deserialize<CustomTemplate>(templateJson, _jsonOptions);

                if (template == null)
                {
                    return false;
                }

                // Create a temporary directory to build the archive
                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                try
                {
                    // Save the template JSON
                    string templateJsonPath = Path.Combine(tempDir, "template.json");
                    File.WriteAllText(templateJsonPath, templateJson);

                    // Collect and copy all images
                    string imagesDir = Path.Combine(tempDir, "images");
                    Directory.CreateDirectory(imagesDir);

                    HashSet<string> copiedImages = [];
                    foreach (SerializedElement element in template.Elements)
                    {
                        if (element.ElementType == nameof(ImageElement))
                        {
                            if (element.Properties.TryGetValue("ImagePath", out object imagePathObj))
                            {
                                string imagePath = GetStringValue(imagePathObj, "");
                                if (!string.IsNullOrEmpty(imagePath))
                                {
                                    // Resolve the image path (handle both absolute and relative paths)
                                    string resolvedPath = imagePath;
                                    if (!Path.IsPathRooted(imagePath))
                                    {
                                        // If it's a relative path, look in the template images directory
                                        resolvedPath = Path.Combine(Directories.ReportTemplateImages_dir, imagePath);
                                    }

                                    if (File.Exists(resolvedPath))
                                    {
                                        string fileName = Path.GetFileName(imagePath);
                                        string destPath = Path.Combine(imagesDir, fileName);

                                        // Handle duplicate filenames
                                        int counter = 1;
                                        while (copiedImages.Contains(fileName))
                                        {
                                            string nameWithoutExt = Path.GetFileNameWithoutExtension(imagePath);
                                            string ext = Path.GetExtension(imagePath);
                                            fileName = $"{nameWithoutExt}_{counter}{ext}";
                                            destPath = Path.Combine(imagesDir, fileName);
                                            counter++;
                                        }

                                        File.Copy(resolvedPath, destPath, true);
                                        copiedImages.Add(fileName);
                                    }
                                }
                            }
                        }
                    }

                    // Create TAR archive
                    using FileStream tarStream = new(destinationPath, FileMode.Create, FileAccess.Write);
                    TarFile.CreateFromDirectory(tempDir, tarStream, includeBaseDirectory: false);

                    return true;
                }
                finally
                {
                    // Clean up temp directory
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Imports a template from a .ArgoSalesTemplate file (TAR archive) and restores images.
        /// Returns the template name if successful, null otherwise.
        /// </summary>
        public static string? ImportTemplateWithImages(string sourceFilePath, bool overwriteExisting = true, string newTemplateName = null)
        {
            try
            {
                // Create a temporary directory to extract the archive
                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                try
                {
                    // Extract TAR archive
                    using FileStream tarStream = new(sourceFilePath, FileMode.Open, FileAccess.Read);
                    TarFile.ExtractToDirectory(tarStream, tempDir, overwriteFiles: true);

                    // Read the template JSON
                    string templateJsonPath = Path.Combine(tempDir, "template.json");
                    if (!File.Exists(templateJsonPath))
                    {
                        return null;
                    }

                    string templateJson = File.ReadAllText(templateJsonPath);
                    CustomTemplate template = JsonSerializer.Deserialize<CustomTemplate>(templateJson, _jsonOptions);

                    if (template == null || string.IsNullOrWhiteSpace(template.Name))
                    {
                        return null;
                    }

                    // Use the new template name if provided
                    if (!string.IsNullOrWhiteSpace(newTemplateName))
                    {
                        template.Name = newTemplateName;
                    }

                    // Check if template already exists
                    string destinationFileName = SanitizeFileName(template.Name) + ArgoFiles.JsonFileExtension;
                    string destinationFilePath = Path.Combine(Directories.ReportTemplates_dir, destinationFileName);

                    if (File.Exists(destinationFilePath) && !overwriteExisting)
                    {
                        return null;
                    }

                    // Create images directory for templates
                    string templateImagesDir = Directories.ReportTemplateImages_dir;
                    Directory.CreateDirectory(templateImagesDir);

                    // Copy images and update paths
                    string extractedImagesDir = Path.Combine(tempDir, "images");
                    if (Directory.Exists(extractedImagesDir))
                    {
                        foreach (SerializedElement element in template.Elements)
                        {
                            if (element.ElementType == nameof(ImageElement))
                            {
                                if (element.Properties.TryGetValue("ImagePath", out object imagePathObj))
                                {
                                    string originalImagePath = GetStringValue(imagePathObj, "");
                                    if (!string.IsNullOrEmpty(originalImagePath))
                                    {
                                        string fileName = Path.GetFileName(originalImagePath);
                                        string extractedImagePath = Path.Combine(extractedImagesDir, fileName);

                                        if (File.Exists(extractedImagePath))
                                        {
                                            // Copy to permanent storage
                                            string newImagePath = Path.Combine(templateImagesDir, fileName);

                                            // Handle duplicate filenames
                                            if (File.Exists(newImagePath))
                                            {
                                                string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                                                string ext = Path.GetExtension(fileName);
                                                int counter = 1;
                                                do
                                                {
                                                    fileName = $"{nameWithoutExt}_{counter}{ext}";
                                                    newImagePath = Path.Combine(templateImagesDir, fileName);
                                                    counter++;
                                                } while (File.Exists(newImagePath));
                                            }

                                            File.Copy(extractedImagePath, newImagePath, true);

                                            // Update the image path in the template (store as relative path - just filename)
                                            element.Properties["ImagePath"] = fileName;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Save the template with updated image paths
                    string updatedJson = JsonSerializer.Serialize(template, _jsonOptions);
                    File.WriteAllText(destinationFilePath, updatedJson);

                    return template.Name;
                }
                finally
                {
                    // Clean up temp directory
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the template name from a .ArgoSalesTemplate file without importing it.
        /// </summary>
        public static string? GetTemplateNameFromFile(string sourceFilePath)
        {
            try
            {
                // Create a temporary directory to extract the archive
                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                try
                {
                    // Extract TAR archive
                    using FileStream tarStream = new(sourceFilePath, FileMode.Open, FileAccess.Read);
                    TarFile.ExtractToDirectory(tarStream, tempDir, overwriteFiles: true);

                    // Read the template JSON
                    string templateJsonPath = Path.Combine(tempDir, "template.json");
                    if (!File.Exists(templateJsonPath))
                    {
                        return null;
                    }

                    string templateJson = File.ReadAllText(templateJsonPath);
                    CustomTemplate template = JsonSerializer.Deserialize<CustomTemplate>(templateJson, _jsonOptions);

                    return template?.Name;
                }
                finally
                {
                    // Clean up temp directory
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        private static SerializedElement SerializeElement(BaseElement element)
        {
            SerializedElement serialized = new()
            {
                ElementType = element.GetType().Name,
                Bounds = element.Bounds,
                ZOrder = element.ZOrder,
                IsVisible = element.IsVisible,

                // Serialize type-specific properties
                Properties = element switch
                {
                    ChartElement chart => new Dictionary<string, object>
                    {
                        ["ChartType"] = chart.ChartType.ToString(),
                        ["ShowLegend"] = chart.ShowLegend,
                        ["ShowTitle"] = chart.ShowTitle,
                        ["FontFamily"] = chart.FontFamily ?? "Segoe UI",
                        ["TitleFontSize"] = chart.TitleFontSize,
                        ["LegendFontSize"] = chart.LegendFontSize,
                        ["BorderColor"] = ColorToHex(chart.BorderColor),
                        ["BorderThickness"] = chart.BorderThickness
                    },
                    LabelElement label => new Dictionary<string, object>
                    {
                        ["Text"] = label.Text ?? "",
                        ["FontFamily"] = label.FontFamily ?? "Segoe UI",
                        ["FontSize"] = label.FontSize,
                        ["FontStyle"] = label.FontStyle.ToString(),
                        ["TextColor"] = ColorToHex(label.TextColor),
                        ["HAlignment"] = label.HAlignment.ToString(),
                        ["VAlignment"] = label.VAlignment.ToString()
                    },
                    DateRangeElement dateRange => new Dictionary<string, object>
                    {
                        ["DateFormat"] = dateRange.DateFormat ?? "yyyy-MM-dd",
                        ["TextColor"] = ColorToHex(dateRange.TextColor),
                        ["FontSize"] = dateRange.FontSize,
                        ["FontStyle"] = dateRange.FontStyle.ToString(),
                        ["FontFamily"] = dateRange.FontFamily ?? "Segoe UI",
                        ["HAlignment"] = dateRange.HAlignment.ToString(),
                        ["VAlignment"] = dateRange.VAlignment.ToString()
                    },
                    SummaryElement summary => new Dictionary<string, object>
                    {
                        ["TransactionType"] = summary.TransactionType.ToString(),
                        ["IncludeReturns"] = summary.IncludeReturns,
                        ["IncludeLosses"] = summary.IncludeLosses,
                        ["ShowTotalSales"] = summary.ShowTotalSales,
                        ["ShowTotalTransactions"] = summary.ShowTotalTransactions,
                        ["ShowAverageValue"] = summary.ShowAverageValue,
                        ["ShowGrowthRate"] = summary.ShowGrowthRate,
                        ["BackgroundColor"] = ColorToHex(summary.BackgroundColor),
                        ["BorderThickness"] = summary.BorderThickness,
                        ["BorderColor"] = ColorToHex(summary.BorderColor),
                        ["FontFamily"] = summary.FontFamily ?? "Segoe UI",
                        ["FontSize"] = summary.FontSize,
                        ["FontStyle"] = summary.FontStyle.ToString(),
                        ["Alignment"] = summary.Alignment.ToString(),
                        ["VerticalAlignment"] = summary.VerticalAlignment.ToString()
                    },
                    TableElement table => new Dictionary<string, object>
                    {
                        ["TransactionType"] = table.TransactionType.ToString(),
                        ["IncludeReturns"] = table.IncludeReturns,
                        ["IncludeLosses"] = table.IncludeLosses,
                        ["DataSelection"] = table.DataSelection.ToString(),
                        ["SortOrder"] = table.SortOrder.ToString(),
                        ["MaxRows"] = table.MaxRows,
                        ["ShowHeaders"] = table.ShowHeaders,
                        ["AlternateRowColors"] = table.AlternateRowColors,
                        ["ShowGridLines"] = table.ShowGridLines,
                        ["ShowTotalsRow"] = table.ShowTotalsRow,
                        ["AutoSizeColumns"] = table.AutoSizeColumns,
                        ["FontSize"] = table.FontSize,
                        ["FontFamily"] = table.FontFamily ?? "Segoe UI",
                        ["DataRowHeight"] = table.DataRowHeight,
                        ["HeaderRowHeight"] = table.HeaderRowHeight,
                        ["CellPadding"] = table.CellPadding,
                        ["HeaderBackgroundColor"] = ColorToHex(table.HeaderBackgroundColor),
                        ["HeaderTextColor"] = ColorToHex(table.HeaderTextColor),
                        ["DataRowTextColor"] = ColorToHex(table.DataRowTextColor),
                        ["GridLineColor"] = ColorToHex(table.GridLineColor),
                        ["BaseRowColor"] = ColorToHex(table.BaseRowColor),
                        ["AlternateRowColor"] = ColorToHex(table.AlternateRowColor),
                        ["ShowDateColumn"] = table.ShowDateColumn,
                        ["ShowTransactionIdColumn"] = table.ShowTransactionIdColumn,
                        ["ShowCompanyColumn"] = table.ShowCompanyColumn,
                        ["ShowProductColumn"] = table.ShowProductColumn,
                        ["ShowQuantityColumn"] = table.ShowQuantityColumn,
                        ["ShowUnitPriceColumn"] = table.ShowUnitPriceColumn,
                        ["ShowTotalColumn"] = table.ShowTotalColumn,
                        ["ShowStatusColumn"] = table.ShowStatusColumn,
                        ["ShowAccountantColumn"] = table.ShowAccountantColumn,
                        ["ShowShippingColumn"] = table.ShowShippingColumn
                    },
                    ImageElement image => new Dictionary<string, object>
                    {
                        ["ImagePath"] = image.ImagePath ?? "",
                        ["ScaleMode"] = image.ScaleMode.ToString(),
                        ["BackgroundColor"] = ColorToHex(image.BackgroundColor),
                        ["BorderColor"] = ColorToHex(image.BorderColor),
                        ["BorderThickness"] = image.BorderThickness,
                        ["CornerRadius_Percent"] = image.CornerRadius_Percent,
                        ["Opacity"] = image.Opacity
                    },
                    _ => []
                }
            };

            return serialized;
        }
        private static BaseElement? DeserializeElement(SerializedElement serialized)
        {
            BaseElement element = serialized.ElementType switch
            {
                nameof(ChartElement) => new ChartElement
                {
                    ChartType = Enum.Parse<MainMenu_Form.ChartDataType>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("ChartType", "TotalRevenue"), "TotalRevenue")),
                    ShowLegend = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowLegend", true), true),
                    ShowTitle = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowTitle", true), true),
                    FontFamily = GetStringValue(serialized.Properties.GetValueOrDefault("FontFamily", "Segoe UI"), "Segoe UI"),
                    TitleFontSize = GetFloatValue(serialized.Properties.GetValueOrDefault("TitleFontSize", 12f), 12f),
                    LegendFontSize = GetFloatValue(serialized.Properties.GetValueOrDefault("LegendFontSize", 11f), 11f),
                    BorderColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("BorderColor", "#808080"), "#808080")),
                    BorderThickness = GetIntValue(serialized.Properties.GetValueOrDefault("BorderThickness", 1), 1)
                },
                nameof(LabelElement) => new LabelElement
                {
                    Text = GetStringValue(serialized.Properties.GetValueOrDefault("Text", ""), ""),
                    FontFamily = GetStringValue(
                        serialized.Properties.GetValueOrDefault("FontFamily",
                        serialized.Properties.GetValueOrDefault("FontName", "Segoe UI")), "Segoe UI"),
                    FontSize = GetFloatValue(serialized.Properties.GetValueOrDefault("FontSize", 12f), 12f),
                    FontStyle = Enum.Parse<FontStyle>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("FontStyle", "Regular"), "Regular")),
                    TextColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("TextColor", "#000000"), "#000000")),
                    HAlignment = Enum.Parse<StringAlignment>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("HAlignment",
                        serialized.Properties.GetValueOrDefault("Alignment", "Center")), "Center")),
                    VAlignment = Enum.Parse<StringAlignment>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("VAlignment", "Center"), "Center"))
                },
                nameof(DateRangeElement) => new DateRangeElement
                {
                    DateFormat = GetStringValue(serialized.Properties.GetValueOrDefault("DateFormat", "yyyy-MM-dd"), "yyyy-MM-dd"),
                    TextColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("TextColor", "#808080"), "#808080")),
                    FontSize = GetFloatValue(serialized.Properties.GetValueOrDefault("FontSize", 10f), 10f),
                    FontStyle = Enum.Parse<FontStyle>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("FontStyle", "Italic"), "Italic")),
                    FontFamily = GetStringValue(serialized.Properties.GetValueOrDefault("FontFamily", "Segoe UI"), "Segoe UI"),
                    HAlignment = Enum.Parse<StringAlignment>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("HAlignment", "Center"), "Center")),
                    VAlignment = Enum.Parse<StringAlignment>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("VAlignment", "Center"), "Center"))
                },
                nameof(SummaryElement) => new SummaryElement
                {
                    TransactionType = Enum.Parse<TransactionType>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("TransactionType", "Both"), "Both")),
                    IncludeReturns = GetBoolValue(serialized.Properties.GetValueOrDefault("IncludeReturns", true), true),
                    IncludeLosses = GetBoolValue(serialized.Properties.GetValueOrDefault("IncludeLosses", true), true),
                    ShowTotalSales = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowTotalSales", true), true),
                    ShowTotalTransactions = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowTotalTransactions", true), true),
                    ShowAverageValue = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowAverageValue", true), true),
                    ShowGrowthRate = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowGrowthRate", true), true),
                    BackgroundColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("BackgroundColor", "#F5F5F5"), "#F5F5F5")),
                    BorderThickness = GetIntValue(serialized.Properties.GetValueOrDefault("BorderThickness", 1), 1),
                    BorderColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("BorderColor", "#D3D3D3"), "#D3D3D3")),
                    FontFamily = GetStringValue(serialized.Properties.GetValueOrDefault("FontFamily", "Segoe UI"), "Segoe UI"),
                    FontSize = GetFloatValue(serialized.Properties.GetValueOrDefault("FontSize", 10f), 10f),
                    FontStyle = Enum.Parse<FontStyle>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("FontStyle", "Regular"), "Regular")),
                    Alignment = Enum.Parse<StringAlignment>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("Alignment", "Near"), "Near")),
                    VerticalAlignment = Enum.Parse<StringAlignment>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("VerticalAlignment", "Near"), "Near"))
                },
                nameof(TableElement) => new TableElement
                {
                    TransactionType = Enum.Parse<TransactionType>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("TransactionType", "Both"), "Both")),
                    IncludeReturns = GetBoolValue(serialized.Properties.GetValueOrDefault("IncludeReturns", true), true),
                    IncludeLosses = GetBoolValue(serialized.Properties.GetValueOrDefault("IncludeLosses", true), true),
                    DataSelection = Enum.Parse<TableDataSelection>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("DataSelection", "All"), "All")),
                    SortOrder = Enum.Parse<TableSortOrder>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("SortOrder", "DateDescending"), "DateDescending")),
                    MaxRows = GetIntValue(serialized.Properties.GetValueOrDefault("MaxRows", 10), 10),
                    ShowHeaders = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowHeaders", true), true),
                    AlternateRowColors = GetBoolValue(serialized.Properties.GetValueOrDefault("AlternateRowColors", true), true),
                    ShowGridLines = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowGridLines", true), true),
                    ShowTotalsRow = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowTotalsRow", false), false),
                    AutoSizeColumns = GetBoolValue(serialized.Properties.GetValueOrDefault("AutoSizeColumns", true), true),
                    FontSize = GetFloatValue(serialized.Properties.GetValueOrDefault("FontSize", 8f), 8f),
                    FontFamily = GetStringValue(serialized.Properties.GetValueOrDefault("FontFamily", "Segoe UI"), "Segoe UI"),
                    DataRowHeight = GetIntValue(serialized.Properties.GetValueOrDefault("DataRowHeight", 20), 20),
                    HeaderRowHeight = GetIntValue(serialized.Properties.GetValueOrDefault("HeaderRowHeight", 25), 25),
                    CellPadding = GetIntValue(serialized.Properties.GetValueOrDefault("CellPadding", 3), 3),
                    HeaderBackgroundColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("HeaderBackgroundColor", "#5E94FF"), "#5E94FF")),
                    HeaderTextColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("HeaderTextColor", "#FFFFFF"), "#FFFFFF")),
                    DataRowTextColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("DataRowTextColor", "#000000"), "#000000")),
                    GridLineColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("GridLineColor", "#D3D3D3"), "#D3D3D3")),
                    BaseRowColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("BaseRowColor", "#FFFFFF"), "#FFFFFF")),
                    AlternateRowColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("AlternateRowColor", "#F8F8F8"), "#F8F8F8")),
                    ShowDateColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowDateColumn", true), true),
                    ShowTransactionIdColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowTransactionIdColumn", true), true),
                    ShowCompanyColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowCompanyColumn", true), true),
                    ShowProductColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowProductColumn", true), true),
                    ShowQuantityColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowQuantityColumn", true), true),
                    ShowUnitPriceColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowUnitPriceColumn", false), false),
                    ShowTotalColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowTotalColumn", true), true),
                    ShowStatusColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowStatusColumn", false), false),
                    ShowAccountantColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowAccountantColumn", false), false),
                    ShowShippingColumn = GetBoolValue(serialized.Properties.GetValueOrDefault("ShowShippingColumn", false), false)
                },
                nameof(ImageElement) => new ImageElement
                {
                    ImagePath = GetStringValue(serialized.Properties.GetValueOrDefault("ImagePath", ""), ""),
                    ScaleMode = Enum.Parse<ImageScaleMode>(
                        GetStringValue(serialized.Properties.GetValueOrDefault("ScaleMode", "Fit"), "Fit")),
                    BackgroundColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("BackgroundColor", "#00FFFFFF"), "#00FFFFFF")),
                    BorderColor = HexToColor(GetStringValue(serialized.Properties.GetValueOrDefault("BorderColor", "#00FFFFFF"), "#00FFFFFF")),
                    BorderThickness = GetIntValue(serialized.Properties.GetValueOrDefault("BorderThickness", 1), 1),
                    CornerRadius_Percent = GetIntValue(serialized.Properties.GetValueOrDefault("CornerRadius_Percent", 0), 0),
                    Opacity = (byte)GetIntValue(serialized.Properties.GetValueOrDefault("Opacity", 255), 255)
                },
                _ => null
            };

            if (element != null)
            {
                element.Bounds = serialized.Bounds;
                element.ZOrder = serialized.ZOrder;
                element.IsVisible = serialized.IsVisible;
            }

            return element;
        }

        /// <summary>
        /// Helper methods to safely extract values from Properties dictionary,
        /// handling both JsonElement and regular objects.
        /// </summary>
        private static string GetStringValue(object value, string defaultValue = "")
        {
            if (value == null) { return defaultValue; }

            if (value is JsonElement jsonElement)
            {
                return jsonElement.ValueKind == JsonValueKind.String
                    ? jsonElement.GetString() ?? defaultValue
                    : jsonElement.ToString();
            }

            return value.ToString() ?? defaultValue;
        }
        private static bool GetBoolValue(object value, bool defaultValue = false)
        {
            if (value == null) { return defaultValue; }

            if (value is JsonElement jsonElement)
            {
                return jsonElement.ValueKind == JsonValueKind.True || jsonElement.ValueKind == JsonValueKind.False
                    ? jsonElement.GetBoolean()
                    : defaultValue;
            }

            if (value is bool boolValue) { return boolValue; }

            return bool.TryParse(value.ToString(), out bool result) ? result : defaultValue;
        }
        private static float GetFloatValue(object value, float defaultValue = 0f)
        {
            if (value == null) { return defaultValue; }

            if (value is JsonElement jsonElement)
            {
                return jsonElement.ValueKind == JsonValueKind.Number
                    ? jsonElement.GetSingle()
                    : defaultValue;
            }

            if (value is float floatValue) { return floatValue; }
            if (value is double doubleValue) { return (float)doubleValue; }
            if (value is int intValue) { return intValue; }

            return float.TryParse(value.ToString(), out float result) ? result : defaultValue;
        }
        private static int GetIntValue(object value, int defaultValue = 0)
        {
            if (value == null) { return defaultValue; }

            if (value is JsonElement jsonElement)
            {
                return jsonElement.ValueKind == JsonValueKind.Number
                    ? jsonElement.GetInt32()
                    : defaultValue;
            }

            if (value is int intValue) { return intValue; }
            if (value is long longValue) { return (int)longValue; }
            if (value is double doubleValue) { return (int)doubleValue; }
            if (value is float floatValue) { return (int)floatValue; }

            return int.TryParse(value.ToString(), out int result) ? result : defaultValue;
        }
        private static string ColorToHex(Color color)
        {
            // Include alpha channel if not fully opaque
            if (color.A < 255)
            {
                return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        private static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");

            // Support both RGB (6 chars) and ARGB (8 chars) formats
            if (hex.Length == 8)
            {
                return Color.FromArgb(
                    Convert.ToInt32(hex.Substring(0, 2), 16),  // Alpha
                    Convert.ToInt32(hex.Substring(2, 2), 16),  // Red
                    Convert.ToInt32(hex.Substring(4, 2), 16),  // Green
                    Convert.ToInt32(hex.Substring(6, 2), 16)   // Blue
                );
            }
            else if (hex.Length == 6)
            {
                return Color.FromArgb(
                    255,  // Fully opaque
                    Convert.ToInt32(hex.Substring(0, 2), 16),  // Red
                    Convert.ToInt32(hex.Substring(2, 2), 16),  // Green
                    Convert.ToInt32(hex.Substring(4, 2), 16)   // Blue
                );
            }

            return Color.Black;  // Fallback
        }

        /// <summary>
        /// Gets all image paths currently used across all templates.
        /// </summary>
        private static HashSet<string> GetAllUsedImagePaths()
        {
            HashSet<string> usedPaths = [];

            try
            {
                List<string> templateNames = ReportTemplates.GetAvailableTemplates();

                foreach (string templateName in templateNames)
                {
                    ReportConfiguration config = LoadTemplate(templateName);
                    if (config?.Elements != null)
                    {
                        foreach (BaseElement element in config.Elements)
                        {
                            if (element is ImageElement imageElement && !string.IsNullOrEmpty(imageElement.ImagePath))
                            {
                                // Store just the filename for comparison
                                string fileName = Path.GetFileName(imageElement.ImagePath);
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    usedPaths.Add(fileName);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors during scanning
            }

            return usedPaths;
        }

        /// <summary>
        /// Cleans up unused image files from the Images directory.
        /// Only deletes images that are not referenced by any template.
        /// </summary>
        public static void CleanupUnusedImages()
        {
            try
            {
                if (!Directory.Exists(Directories.ReportTemplateImages_dir))
                {
                    return;
                }

                ReportLayoutDesigner_Form.DisposeCachedImages();

                // Get all currently used image paths
                HashSet<string> usedPaths = GetAllUsedImagePaths();

                // Get all image files in the directory
                string[] imageFiles = Directory.GetFiles(Directories.ReportTemplateImages_dir, "*.*")
                    .Where(f => sourceArray.Contains(Path.GetExtension(f).ToLowerInvariant()))
                    .ToArray();

                // Delete unused images
                foreach (string imageFile in imageFiles)
                {
                    string fileName = Path.GetFileName(imageFile);
                    if (!usedPaths.Contains(fileName))
                    {
                        try
                        {
                            File.Delete(imageFile);
                        }
                        catch
                        {
                            // Ignore errors deleting individual files
                        }
                    }
                }
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }

        // Custom JSON converter for Color
        private class ColorConverter : JsonConverter<Color>
        {
            public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string hex = reader.GetString();
                return HexToColor(hex);
            }
            public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(ColorToHex(value));
            }
        }

        // Custom JSON converter for Margins
        private class MarginsConverter : JsonConverter<Margins>
        {
            public override Margins Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                int left = 0, top = 0, right = 0, bottom = 0;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return new Margins(left, top, right, bottom);
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        string propertyName = reader.GetString();
                        reader.Read();

                        switch (propertyName)
                        {
                            case "Left":
                                left = reader.GetInt32();
                                break;
                            case "Top":
                                top = reader.GetInt32();
                                break;
                            case "Right":
                                right = reader.GetInt32();
                                break;
                            case "Bottom":
                                bottom = reader.GetInt32();
                                break;
                        }
                    }
                }

                throw new JsonException();
            }
            public override void Write(Utf8JsonWriter writer, Margins value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("Left", value.Left);
                writer.WriteNumber("Top", value.Top);
                writer.WriteNumber("Right", value.Right);
                writer.WriteNumber("Bottom", value.Bottom);
                writer.WriteEndObject();
            }
        }

        // Internal classes for serialization
        private class CustomTemplate
        {
            public string Name { get; set; }
            public DateTime Created { get; set; }
            public DateTime LastModified { get; set; }
            public PageSize PageSize { get; set; }
            public PageOrientation PageOrientation { get; set; }
            public bool ShowHeader { get; set; }
            public bool ShowFooter { get; set; }
            public bool ShowPageNumbers { get; set; }
            public Color BackgroundColor { get; set; }
            public Margins PageMargins { get; set; }
            public ReportFilters Filters { get; set; }
            public List<SerializedElement> Elements { get; set; }
        }

        private class SerializedElement
        {
            public string ElementType { get; set; }
            public Rectangle Bounds { get; set; }
            public int ZOrder { get; set; }
            public bool IsVisible { get; set; }
            public Dictionary<string, object> Properties { get; set; }
        }
    }
}
