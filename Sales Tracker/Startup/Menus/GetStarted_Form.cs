using Guna.UI2.WinForms;
using Microsoft.VisualBasic.FileIO;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Passwords;
using Sales_Tracker.Properties;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.Startup.Menus
{
    /// <summary>
    /// Getting started form for opening or creating companies.
    /// </summary>
    public partial class GetStarted_Form : BaseForm
    {
        // Properties
        private static GetStarted_Form _instance;
        private readonly Dictionary<string, FileSystemWatcher> _fileWatchers;

        // Getters
        public static GetStarted_Form Instance => _instance;

        // Init.
        public GetStarted_Form()
        {
            InitializeComponent();
            _instance = this;
            _fileWatchers = [];

            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);

            ConstructRightClickOpenRecentMenu();
            LoadListOfRecentCompanies();
            SetFlowLayoutPanel();
            UpdateTheme();
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void SetAccessibleDescriptions()
        {
            ArgoSalesTracker_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }
        private void SetFlowLayoutPanel()
        {
            // Enable vertical scroll
            OpenRecent_FlowLayoutPanel.HorizontalScroll.Maximum = 0;
            OpenRecent_FlowLayoutPanel.HorizontalScroll.Enabled = false;
            OpenRecent_FlowLayoutPanel.HorizontalScroll.Visible = false;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.UpdateThemeForPanel([RightClickOpenRecent_Panel]);
            _rightClickOpenRecent_DeleteBtn.ForeColor = CustomColors.AccentRed;

            if (ThemeManager.IsDarkTheme())
            {
                CreateCompany_Button.Image = Resources.CreateFileWhite;
                OpenCompany_Button.Image = Resources.OpenFolderWhite;
            }
            else
            {
                CreateCompany_Button.Image = Resources.CreateFileBlack;
                OpenCompany_Button.Image = Resources.OpenFolderGray;
            }
        }

        // Form event handlers
        private void GetStarted_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
            ArgoCompany.RecoverUnsavedWork();
        }

        // Recent companies
        private void LoadListOfRecentCompanies()
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledButtonHeight = (int)(60 * scale);

            List<string> validCompanyDirs = ArgoCompany.GetValidRecentCompanyPaths(false);

            if (validCompanyDirs.Count == 0)
            {
                LabelManager.AddNoRecentlyOpenedCompanies(OpenRecent_FlowLayoutPanel, CalculateButtonWidth(), scaledButtonHeight);
                return;
            }

            // Construct a button for each company
            foreach (string companyDir in validCompanyDirs)
            {
                Guna2Button btn = new()
                {
                    BackColor = CustomColors.ControlBack,
                    FillColor = CustomColors.ControlBack,
                    Size = new Size(CalculateButtonWidth(), scaledButtonHeight),
                    Text = Path.GetFileNameWithoutExtension(companyDir),
                    Font = new Font("Segoe UI", 11),
                    Tag = companyDir,
                    AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate
                };
                btn.MouseDown += Btn_MouseDown;
                btn.MouseUp += Btn_MouseUp;
                OpenRecent_FlowLayoutPanel.Controls.Add(btn);

                // Initialize file watcher for the directory
                string directory = Path.GetDirectoryName(companyDir);
                InitializeFileWatcher(directory);
            }
        }
        private void Btn_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }
            Guna2Button button = (Guna2Button)sender;

            string companyName = Path.GetFileNameWithoutExtension(button.Tag.ToString());
            string newDir = Directory.GetParent(button.Tag.ToString()).FullName;
            string filePath = newDir + @"\" + companyName + ArgoFiles.ArgoCompanyFileExtension;

            Directories.SetDirectories(newDir, companyName);

            if (!VersionCompatibilityChecker.HandleFileVersionCompatibility(filePath))
            {
                return;
            }

            if (!PasswordManager.EnterPassword())
            {
                return;
            }

            if (!ArgoCompany.OnlyAllowOneInstanceOfACompany(companyName))
            {
                return;
            }

            DataFileManager.AppendValue(GlobalAppDataSettings.RecentCompanies, filePath);

            List<string> listOfDirectories = Directories.GetListOfAllDirectoryNamesInDirectory(Directories.AppData_dir);
            Directories.ImportArgoTarFile(Directories.ArgoCompany_file, Directories.AppData_dir, listOfDirectories, false);
            DataFileManager.SetValue(AppDataSettings.ChangesMade, false.ToString());

            ShowMainMenu();
        }
        private void Btn_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Guna2Button button = (Guna2Button)sender;

                // Position and show the right click panel
                RightClickOpenRecent_Panel.Location = new Point(e.X, OpenRecent_FlowLayoutPanel.Top + button.Top + button.Height);
                RightClickOpenRecent_Panel.Tag = button;
                Controls.Add(RightClickOpenRecent_Panel);
                RightClickOpenRecent_Panel.BringToFront();
            }
        }
        private int CalculateButtonWidth()
        {
            int scrollBarWidth = OpenRecent_FlowLayoutPanel.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;
            return OpenRecent_FlowLayoutPanel.Width - scrollBarWidth - 8;
        }
        private void InitializeFileWatcher(string directory)
        {
            if (Directory.Exists(directory) && !_fileWatchers.ContainsKey(directory))
            {
                FileSystemWatcher fileWatcher = new()
                {
                    Path = directory,
                    NotifyFilter = NotifyFilters.FileName
                };
                fileWatcher.Deleted += FileDeleted;
                fileWatcher.EnableRaisingEvents = true;

                _fileWatchers[directory] = fileWatcher;
            }
        }
        private void FileDeleted(object sender, FileSystemEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                List<Guna2Button> itemsToRemove = [];
                foreach (Control control in OpenRecent_FlowLayoutPanel.Controls)
                {
                    if (control is Guna2Button btn)
                    {
                        if (btn.Tag.ToString() == e.FullPath)
                        {
                            itemsToRemove.Add(btn);
                        }
                    }
                }

                foreach (Guna2Button item in itemsToRemove)
                {
                    OpenRecent_FlowLayoutPanel.Controls.Remove(item);
                }

                // Remove the watcher if no more files are being watched in this directory
                string? directory = Path.GetDirectoryName(e.FullPath);
                if (directory != null)
                {
                    bool directoryStillInUse = OpenRecent_FlowLayoutPanel.Controls.OfType<Guna2Button>()
                        .Any(btn => Path.GetDirectoryName(btn.Tag.ToString()) == directory);

                    if (!directoryStillInUse && _fileWatchers.TryGetValue(directory, out FileSystemWatcher? value))
                    {
                        value.Dispose();
                        _fileWatchers.Remove(directory);
                    }
                }
            });
        }
        private void DisposeAllFileWatchers()
        {
            foreach (FileSystemWatcher watcher in _fileWatchers.Values)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _fileWatchers.Clear();
        }

        // Event handlers
        private void CreateNewCompany_Click(object sender, EventArgs e)
        {
            Startup_Form.Instance.SwitchMainForm(Startup_Form.Instance.FormConfigureCompany);
        }
        private void OpenCompany_Button_Click(object sender, EventArgs e)
        {
            ArgoCompany.OpenCompanyFromDialog();
        }
        private void OpenRecent_FlowLayoutPanel_Resize(object sender, EventArgs e)
        {
            foreach (Control control in OpenRecent_FlowLayoutPanel.Controls)
            {
                if (control is Guna2Button)
                {
                    control.Width = CalculateButtonWidth();
                }
            }

            // Prevents the horizontal scrollbar from flashing in some cases
            OpenRecent_FlowLayoutPanel.PerformLayout();
        }

        // Right click open recent menu
        public static Guna2Panel RightClickOpenRecent_Panel { get; private set; }
        private Guna2Button _rightClickOpenRecent_DeleteBtn;
        private void ConstructRightClickOpenRecentMenu()
        {
            RightClickOpenRecent_Panel = CustomControls.ConstructPanelForMenu(new Size(CustomControls.PanelWidth, 4 * CustomControls.PanelButtonHeight + CustomControls.SpaceForPanel), "rightClickOpenRecent_Panel");
            FlowLayoutPanel flowPanel = (FlowLayoutPanel)RightClickOpenRecent_Panel.Controls[0];

            Guna2Button menuBtn = CustomControls.ConstructBtnForMenu("Show in folder", CustomControls.PanelBtnWidth, flowPanel);
            menuBtn.Click += ShowInFolder;

            menuBtn = CustomControls.ConstructBtnForMenu("Rename company", CustomControls.PanelBtnWidth, flowPanel);
            menuBtn.Click += Rename;

            menuBtn = CustomControls.ConstructBtnForMenu("Remove from list", CustomControls.PanelBtnWidth, flowPanel);
            menuBtn.Click += RemoveFromList;

            _rightClickOpenRecent_DeleteBtn = CustomControls.ConstructBtnForMenu("Delete", CustomControls.PanelBtnWidth, flowPanel);
            _rightClickOpenRecent_DeleteBtn.Click += Delete;

            LanguageManager.UpdateLanguageForControl(RightClickOpenRecent_Panel);
        }
        private void ShowInFolder(object sender, EventArgs e)
        {
            if (RightClickOpenRecent_Panel.Tag is Guna2Button btn)
            {
                string companyFile = btn.Tag.ToString();
                if (File.Exists(companyFile))
                {
                    Tools.ShowFileInFolder(companyFile);
                }
            }
        }
        private void Rename(object sender, EventArgs e)
        {
            Controls.Remove(RightClickOpenRecent_Panel);

            Guna2Button button = (Guna2Button)RightClickOpenRecent_Panel.Tag;

            CustomControls.Rename_TextBox.Location = new Point(
                OpenRecent_FlowLayoutPanel.Left + button.Left + 1,
                OpenRecent_FlowLayoutPanel.Top + button.Top);

            if (CustomControls.Rename_TextBox.Font.Size != button.Font.Size)
            {
                CustomControls.Rename_TextBox.Font = new Font(CustomControls.Rename_TextBox.Font.FontFamily, button.Font.Size);
            }

            CustomControls.Rename_TextBox.Text = button.Text;
            CustomControls.Rename_TextBox.Size = button.Size;
            Controls.Add(CustomControls.Rename_TextBox);
            CustomControls.Rename_TextBox.Focus();
            CustomControls.Rename_TextBox.SelectAll();
            CustomControls.Rename_TextBox.BringToFront();
        }
        private void RemoveFromList(object sender, EventArgs e)
        {
            if (RightClickOpenRecent_Panel.Tag is Guna2Button button)
            {
                string companyDir = button.Tag.ToString();

                DataFileManager.RemoveValue(GlobalAppDataSettings.RecentCompanies, companyDir);
                OpenRecent_FlowLayoutPanel.Controls.Remove(button);
            }
        }
        private void Delete(object sender, EventArgs e)
        {
            if (RightClickOpenRecent_Panel.Tag is Guna2Button btn)
            {
                string companyDir = btn.Tag.ToString();
                string companyName = Path.GetFileNameWithoutExtension(companyDir);

                if (File.Exists(companyDir))
                {
                    CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                        "Delete company",
                        "Are you sure you want to delete {0}? It will be moved to the recycle bin.",
                        CustomMessageBoxIcon.Exclamation, CustomMessageBoxButtons.YesNo,
                        companyName);

                    if (result == CustomMessageBoxResult.Yes)
                    {
                        FileSystem.DeleteFile(
                            companyDir,
                            UIOption.OnlyErrorDialogs,
                            RecycleOption.SendToRecycleBin);

                        OpenRecent_FlowLayoutPanel.Controls.Remove(btn);
                    }
                }
                else { RemoveFromList(null, null); }
            }
        }

        // Methods
        public void RenameCompany()
        {
            if (!Controls.Contains(CustomControls.Rename_TextBox))
            {
                return;
            }
            Controls.Remove(CustomControls.Rename_TextBox);

            Guna2Button button = (Guna2Button)RightClickOpenRecent_Panel.Tag;

            // If the name did not change
            if (CustomControls.Rename_TextBox.Text == button.Text)
            {
                return;
            }

            button.Text = CustomControls.Rename_TextBox.Text;

            string companyDir = button.Tag.ToString();
            string newCompanyDir = Path.Combine(Path.GetDirectoryName(companyDir), CustomControls.Rename_TextBox.Text + ArgoFiles.ArgoCompanyFileExtension);

            if (Directories.MoveFile(companyDir, newCompanyDir))
            {
                button.Tag = newCompanyDir;
            }

            CustomControls.Rename_TextBox.Clear();
        }
        public void ShowMainMenu()
        {
            DisposeAllFileWatchers();
            Startup_Form.CanExitApp = false;
            Startup_Form.Instance.Close();

            MainMenu_Form formMainMenu = new();
            formMainMenu.Show();
        }
    }
}