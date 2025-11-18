using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using Timer = System.Windows.Forms.Timer;

namespace Sales_Tracker
{
    /// <summary>
    /// Form showing progress of language translation operations.
    /// </summary>
    public partial class TranslationProgress_Form : Form
    {
        // Properties
        private readonly CancellationTokenSource _cancellationTokenSource;
        private int _totalLanguages;
        private int _totalTranslationsPerLanguage;
        private int _currentLanguageIndex;
        private int _currentTranslationIndex;

        // Time estimation properties
        private DateTime _operationStartTime;
        private readonly Queue<DateTime> _progressTimestamps = new();
        private readonly Queue<int> _progressValues = new();

        // Getters
        public bool IsCancelled { get; private set; } = false;
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        // Init.
        public TranslationProgress_Form()
        {
            InitializeComponent();

            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBlueSecondary(Cancel_Button);

            _cancellationTokenSource = new CancellationTokenSource();
            _operationStartTime = DateTime.Now;

            LoadingPanel.ShowBlankLoadingPanel(this);
        }

        // Form event handlers
        private void TranslationProgress_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void TranslationProgress_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsCancelled && Cancel_Button.Text != "Close")
            {
                IsCancelled = true;
                _cancellationTokenSource.Cancel();
            }
        }

        // Methods
        public void Initialize(int totalLanguageCount, int translationsPerLanguage)
        {
            _totalLanguages = totalLanguageCount;
            _totalTranslationsPerLanguage = translationsPerLanguage;
            _currentLanguageIndex = 0;
            _currentTranslationIndex = 0;
            _operationStartTime = DateTime.Now;

            // Clear previous samples
            _progressTimestamps.Clear();
            _progressValues.Clear();

            UpdateProgressDisplay();
        }
        public void UpdateLanguage(string languageName, int languageIndex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateLanguage(languageName, languageIndex)));
                return;
            }

            _currentLanguageIndex = languageIndex;
            _currentTranslationIndex = 0;

            CurrentLanguage_Label.Text = $"Processing: {languageName} ({languageIndex + 1} of {_totalLanguages})";
            Operation_Label.Text = "Collecting source texts...";

            UpdateProgressDisplay();
        }
        public void UpdateTranslationProgress(int translationIndex, string operation = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateTranslationProgress(translationIndex, operation)));
                return;
            }

            _currentTranslationIndex = translationIndex;

            if (!string.IsNullOrEmpty(operation))
                Operation_Label.Text = operation;

            UpdateProgressDisplay();
        }
        public void UpdateBatchProgress(int batchIndex, int totalBatches, int textsInBatch)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateBatchProgress(batchIndex, totalBatches, textsInBatch)));
                return;
            }

            Operation_Label.Text = $"Translating batch {batchIndex + 1} of {totalBatches} ({textsInBatch} texts)";

            // Calculate progress within current language
            int progressWithinLanguage = (int)((double)batchIndex / totalBatches * _totalTranslationsPerLanguage);
            _currentTranslationIndex = progressWithinLanguage;

            UpdateProgressDisplay();
        }
        private void UpdateProgressDisplay()
        {
            // Calculate overall progress
            int totalPossibleTranslations = _totalLanguages * _totalTranslationsPerLanguage;
            int completedTranslations = _currentLanguageIndex * _totalTranslationsPerLanguage + _currentTranslationIndex;

            int progressPercentage = totalPossibleTranslations > 0
                ? (int)((double)completedTranslations / totalPossibleTranslations * 100)
                : 0;

            ProgressBar.Value = Math.Min(progressPercentage, 100);

            ProgressStats_Label.Text = $"{completedTranslations:N0} / {totalPossibleTranslations:N0} translations completed";
        }
        public void CompleteProgress()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CompleteProgress));
                return;
            }

            ProgressBar.Value = 100;

            // Calculate total time taken
            TimeSpan totalTime = DateTime.Now - _operationStartTime;
            string totalTimeFormatted;

            if (totalTime.TotalHours >= 1)
            {
                totalTimeFormatted = $"{(int)totalTime.TotalHours}h {totalTime.Minutes}m {totalTime.Seconds}s";
            }
            else if (totalTime.TotalMinutes >= 1)
            {
                totalTimeFormatted = $"{(int)totalTime.TotalMinutes}m {totalTime.Seconds}s";
            }
            else
            {
                totalTimeFormatted = $"{(int)totalTime.TotalSeconds}s";
            }

            Operation_Label.Text = $"Translation generation complete! (Total time: {totalTimeFormatted})";
            Cancel_Button.Text = "Close";

            // Auto-close after 3 seconds
            Timer closeTimer = new()
            {
                Interval = 3000
            };
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                DialogResult = DialogResult.OK;
                Close();
            };
            closeTimer.Start();
        }
        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (Cancel_Button.Text == "Close")
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            IsCancelled = true;
            _cancellationTokenSource.Cancel();

            Cancel_Button.Enabled = false;
            Cancel_Button.Text = "Cancelling...";
            Operation_Label.Text = "Cancelling operation...";
        }
    }
}