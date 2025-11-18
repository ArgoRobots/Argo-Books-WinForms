using Sales_Tracker.Classes;
using Sales_Tracker.Theme;

namespace Sales_Tracker.ImportSpreadsheet
{
    /// <summary>
    /// Setup form for importing spreadsheet data.
    /// </summary>
    public partial class Setup_Form : BaseForm
    {
        // Properties
        private static Setup_Form _instance;

        // Getters
        public static Setup_Form Instance => _instance;
        public Form FormTutorial { get; }
        public Form FormImport { get; }

        // Init.
        public Setup_Form()
        {
            InitializeComponent();
            _instance = this;

            FormTutorial = new Tutorial_Form();
            FormImport = new ImportSpreadsheet_Form();

            ThemeManager.SetThemeForForm(this);
            AddForms();
        }
        private void AddForms()
        {
            FormTutorial.TopLevel = false;
            FormTutorial.Dock = DockStyle.Fill;
            FormTutorial.FormBorderStyle = FormBorderStyle.None;
            FormTutorial.Visible = true;
            FormTutorial.MinimumSize = new Size(0, 0);
            FormBack_Panel.Controls.Add(FormTutorial);

            FormImport.TopLevel = false;
            FormImport.Dock = DockStyle.Fill;
            FormImport.FormBorderStyle = FormBorderStyle.None;
            FormImport.Visible = true;
            FormImport.MinimumSize = new Size(0, 0);
            FormBack_Panel.Controls.Add(FormImport);

            FormTutorial.BringToFront();
        }
    }
}