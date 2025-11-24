using Argo_Books.Classes;
using Argo_Books.Theme;
using Guna.UI2.WinForms;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.Language;
using System.Drawing.Drawing2D;

namespace Argo_Books.UI
{
    public static class CompanyLogo
    {
        // Properties
        private static PictureBox _companyLogo;
        private static bool _isLogoHovered = false;
        private static Bitmap _cameraIcon;
        private static Guna2Button _removeLogo_Button, _changeLogo_Button;

        // Getters
        public static Guna2Panel CompanyLogoRightClick_Panel { get; private set; }

        // Public methods
        public static void ConstructRightClickMenu()
        {
            CompanyLogoRightClick_Panel = CustomControls.ConstructPanelForMenu(
                new Size(CustomControls.PanelWidth - 50, 2 * CustomControls.PanelButtonHeight + CustomControls.SpaceForPanel),
                "companyLogoRightClick_Panel"
            );

            FlowLayoutPanel flowPanel = (FlowLayoutPanel)CompanyLogoRightClick_Panel.Controls[0];
            int btnWidth = CustomControls.PanelBtnWidth - 50;

            // Change Logo button
            _changeLogo_Button = CustomControls.ConstructBtnForMenu("Change logo", btnWidth, flowPanel);
            _changeLogo_Button.Click += (sender, e) =>
            {
                BrowseForCompanyLogo();
            };

            // Remove Logo button (will be shown/hidden based on whether there's a custom logo)
            _removeLogo_Button = CustomControls.ConstructBtnForMenu("Remove logo", btnWidth, flowPanel);
            _removeLogo_Button.Click += (sender, e) =>
            {
                RemoveCompanyLogo();
            };
        }
        public static void SetCompanyLogo()
        {
            if (Properties.Settings.Default.ShowCompanyLogo)
            {
                CreateLogoIfNeeded();
                LoadCompanyLogoImage();
                UpdateLogoPosition();
            }
            else
            {
                RemoveLogoControl();
            }
        }
        public static void Hide()
        {
            CompanyLogoRightClick_Panel?.Parent?.Controls.Remove(CompanyLogoRightClick_Panel);
        }

        // Private methods
        private static void CreateLogoIfNeeded()
        {
            if (_companyLogo != null) { return; }

            float scale = DpiHelper.GetRelativeDpiScale();
            int leftMargin = (int)(18 * scale);

            // Calculate logo size as 80% of MainTop_Panel height
            int logoSize = (int)(MainMenu_Form.Instance.MainTop_Panel.Height * 0.8f);

            _companyLogo = new PictureBox
            {
                Size = new Size(logoSize, logoSize),
                Location = new Point(leftMargin, (MainMenu_Form.Instance.MainTop_Panel.Height - logoSize) / 2),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            // Set up event handlers
            _companyLogo.MouseEnter += CompanyLogo_MouseEnter;
            _companyLogo.MouseLeave += CompanyLogo_MouseLeave;
            _companyLogo.Click += CompanyLogo_Click;
            _companyLogo.MouseDown += CompanyLogo_MouseDown;
            _companyLogo.Paint += CompanyLogo_Paint;

            CustomTooltip.SetToolTip(_companyLogo, "", "Click to change company logo • Right-click for options");
            MainMenu_Form.Instance.MainTop_Panel.Controls.Add(_companyLogo);
        }
        private static void RemoveLogoControl()
        {
            if (_companyLogo == null) { return; }

            MainMenu_Form.Instance.MainTop_Panel.Controls.Remove(_companyLogo);
            _companyLogo.Image?.Dispose();
            _companyLogo.Dispose();
            _companyLogo = null;

            // Move company name back to original position with scaled margin
            float scale = DpiHelper.GetRelativeDpiScale();
            MainMenu_Form.Instance.CompanyName_Label.Left = (int)(18 * scale);
            MainMenu_Form.Instance.SetEditButtonLocation();
        }
        private static void UpdateLogoPosition()
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            MainMenu_Form.Instance.CompanyName_Label.Left = _companyLogo.Right + (int)(10 * scale);
            MainMenu_Form.Instance.SetEditButtonLocation();
        }
        private static void LoadCompanyLogoImage()
        {
            if (_companyLogo == null) { return; }

            // Dispose previous image
            _companyLogo.Image?.Dispose();

            string logoPath = Properties.Settings.Default.CompanyLogoPath;

            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                try
                {
                    using MemoryStream ms = new(File.ReadAllBytes(logoPath));
                    _companyLogo.Image = Image.FromStream(ms);
                }
                catch (Exception ex)
                {
                    Log.Error_WriteToFile($"Failed to load company logo: {ex.Message}");
                    _companyLogo.Image = CreateDefaultLogoImage();
                }
            }
            else
            {
                _companyLogo.Image = CreateDefaultLogoImage();
            }
        }
        private static Bitmap CreateDefaultLogoImage()
        {
            // Get current logo size dynamically
            int logoSize = _companyLogo?.Width ?? (int)(MainMenu_Form.Instance.MainTop_Panel.Height * 0.8f);

            Bitmap defaultLogo = new(logoSize, logoSize);

            using (Graphics g = Graphics.FromImage(defaultLogo))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (SolidBrush backgroundBrush = new(CustomColors.AccentBlue))
                {
                    g.FillRectangle(backgroundBrush, 0, 0, logoSize, logoSize);
                }

                string initials = GetCompanyInitials(Directories.CompanyName);

                // Scale font size based on logo size (roughly 25% of logo size)
                float fontSize = logoSize * 0.25f;
                using Font font = new("Segoe UI", fontSize, FontStyle.Bold);
                using SolidBrush textBrush = new(Color.White);
                SizeF textSize = g.MeasureString(initials, font);
                float x = (logoSize - textSize.Width) / 2;
                float y = (logoSize - textSize.Height) / 2;
                g.DrawString(initials, font, textBrush, x, y);
            }

            return defaultLogo;
        }
        private static string GetCompanyInitials(string companyName)
        {
            string[] words = companyName.Split([' ', '.', '-'], StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0) { return "AC"; }  // Default initials if name is empty

            // Find first alphanumeric character in first word
            char? firstInitial = words[0].FirstOrDefault(char.IsLetterOrDigit);

            if (words.Length == 1)
            {
                return firstInitial.HasValue ? char.ToUpperInvariant(firstInitial.Value).ToString() : "AC";
            }

            // Find first alphanumeric character in second word
            char? secondInitial = words[1].FirstOrDefault(char.IsLetterOrDigit);

            // Build result with only valid alphanumeric characters
            string result = "";
            if (firstInitial.HasValue) { result += char.ToUpperInvariant(firstInitial.Value); }
            if (secondInitial.HasValue) { result += char.ToUpperInvariant(secondInitial.Value); }

            return !string.IsNullOrEmpty(result) ? result : "AC";
        }
        private static void CompanyLogo_MouseEnter(object sender, EventArgs e)
        {
            _isLogoHovered = true;
            _companyLogo.Invalidate();

            // Create camera icon on first hover with appropriate size
            if (_cameraIcon == null)
            {
                int iconSize = (int)(_companyLogo.Width * 0.5f);
                _cameraIcon = CreateCameraIcon(iconSize);
            }
        }
        private static void CompanyLogo_MouseLeave(object sender, EventArgs e)
        {
            _isLogoHovered = false;
            _companyLogo.Invalidate();
        }
        private static void CompanyLogo_Paint(object sender, PaintEventArgs e)
        {
            if (!_isLogoHovered || _cameraIcon == null) { return; }

            SolidBrush overlayBrush = new(Color.FromArgb(128, 0, 0, 0));

            // Draw semi-transparent overlay using cached brush
            e.Graphics.FillRectangle(overlayBrush, _companyLogo.ClientRectangle);

            // Scale camera icon based on logo size (roughly 50% of logo size)
            int iconSize = (int)(_companyLogo.Width * 0.5f);
            int x = (_companyLogo.Width - iconSize) / 2;
            int y = (_companyLogo.Height - iconSize) / 2;

            // Create scaled camera icon if current one doesn't match size
            if (_cameraIcon.Width != iconSize)
            {
                _cameraIcon?.Dispose();
                _cameraIcon = CreateCameraIcon(iconSize);
            }

            e.Graphics.DrawImage(_cameraIcon, x, y, iconSize, iconSize);
        }
        private static Bitmap CreateCameraIcon(int size = 32)
        {
            Bitmap icon = new(size, size);

            using (Graphics g = Graphics.FromImage(icon))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Scale pen width and measurements based on icon size
                float scale = size / 32f;
                float penWidth = 2f * scale;

                using Pen whitePen = new(Color.White, penWidth);
                using SolidBrush whiteBrush = new(Color.White);

                // Scale all measurements proportionally
                float bodyX = 4 * scale;
                float bodyY = 12 * scale;
                float bodyWidth = 24 * scale;
                float bodyHeight = 14 * scale;

                float topX = 9 * scale;
                float topY = 8 * scale;
                float topWidth = 8 * scale;
                float topHeight = 4 * scale;

                float lensX = 12 * scale;
                float lensY = 16 * scale;
                float lensSize = 8 * scale;

                float centerX = 14 * scale;
                float centerY = 18 * scale;
                float centerSize = 4 * scale;

                // Draw camera body
                g.DrawRectangle(whitePen, bodyX, bodyY, bodyWidth, bodyHeight);
                // Draw camera top
                g.DrawRectangle(whitePen, topX, topY, topWidth, topHeight);
                // Draw lens
                g.DrawEllipse(whitePen, lensX, lensY, lensSize, lensSize);
                // Draw lens center
                g.FillEllipse(whiteBrush, centerX, centerY, centerSize, centerSize);
            }

            return icon;
        }
        private static void CompanyLogo_Click(object sender, EventArgs e)
        {
            BrowseForCompanyLogo();
        }
        private static void CompanyLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ShowLogoRightClickPanel(e.Location);
            }
        }
        private static void ShowLogoRightClickPanel(Point location)
        {
            // Update button text based on logo state
            bool hasCustomLogo = !string.IsNullOrEmpty(Properties.Settings.Default.CompanyLogoPath);
            _changeLogo_Button.Text = hasCustomLogo
                ? LanguageManager.TranslateString("Change logo")
                : LanguageManager.TranslateString("Add logo");

            // Toggle Remove Logo button visibility
            _removeLogo_Button.Visible = !string.IsNullOrEmpty(Properties.Settings.Default.CompanyLogoPath);

            CustomControls.SetRightClickMenuHeight(CompanyLogoRightClick_Panel);

            // Position and show menu
            Point screenLocation = _companyLogo.PointToScreen(location);
            CompanyLogoRightClick_Panel.Location = MainMenu_Form.Instance.PointToClient(screenLocation);

            MainMenu_Form.Instance.Controls.Add(CompanyLogoRightClick_Panel);
            CompanyLogoRightClick_Panel.BringToFront();
        }
        private static void BrowseForCompanyLogo()
        {
            Hide();

            using OpenFileDialog dialog = new();

            dialog.Title = "Select Company Logo";
            dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.ico|" +
                            "PNG Files|*.png|" +
                            "JPEG Files|*.jpg;*.jpeg|" +
                            "Bitmap Files|*.bmp|" +
                            "GIF Files|*.gif|" +
                            "Icon Files|*.ico|" +
                            "All Files|*.*";
            dialog.FilterIndex = 1;

            if (dialog.ShowDialog() != DialogResult.OK) { return; }

            try
            {
                // Copy the logo file to application data directory
                string logoFileName = $"company_logo{Path.GetExtension(dialog.FileName)}";
                string logoDestinationPath = Path.Combine(Directories.TempCompany_dir, logoFileName);

                // Remove old logo file if it exists
                RemoveOldLogoFile();

                // Copy the new logo file
                File.Copy(dialog.FileName, logoDestinationPath, true);

                // Update settings with the copied file path
                string oldLogoPath = Properties.Settings.Default.CompanyLogoPath;
                Properties.Settings.Default.CompanyLogoPath = logoDestinationPath;
                Properties.Settings.Default.Save();

                LogLogoChange(oldLogoPath, dialog.FileName);  // Log with original filename for user clarity
                LoadCompanyLogoImage();
            }
            catch
            {
                ShowImageError();
            }
        }
        private static void RemoveOldLogoFile()
        {
            string currentLogoPath = Properties.Settings.Default.CompanyLogoPath;
            if (!string.IsNullOrEmpty(currentLogoPath) && File.Exists(currentLogoPath))
            {
                try
                {
                    File.Delete(currentLogoPath);
                }
                catch (Exception ex)
                {
                    Log.Write(1, $"Could not delete old logo file: {ex.Message}");
                }
            }
        }
        private static void LogLogoChange(string oldLogoPath, string newLogoPath)
        {
            string message = string.IsNullOrEmpty(oldLogoPath)
                ? $"Company logo set to: {Path.GetFileName(newLogoPath)}"
                : $"Company logo changed to: {Path.GetFileName(newLogoPath)}";

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(
                MainMenu_Form.SettingsThatHaveChangedInFile, 3, message);
            Log.Write(2, $"Company logo updated: {newLogoPath}");
        }
        private static void ShowImageError()
        {
            CustomMessageBox.ShowWithFormat("Invalid Image",
                "The selected file is not a valid image or cannot be loaded.",
                CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);

            Log.Error_WriteToFile("Failed to load selected logo image");
        }
        private static void RemoveCompanyLogo()
        {
            Hide();
            RemoveOldLogoFile();

            Properties.Settings.Default.CompanyLogoPath = "";
            Properties.Settings.Default.Save();

            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(
                MainMenu_Form.SettingsThatHaveChangedInFile, 3, "Company logo removed");
            LoadCompanyLogoImage();

            Log.Write(2, "Company logo removed");
        }
    }
}