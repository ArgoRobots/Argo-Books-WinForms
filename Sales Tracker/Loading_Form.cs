using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker
{
    /// <summary>
    /// Loading screen form displayed during long-running operations.
    /// </summary>
    public partial class Loading_Form : Form
    {
        // Static members to track form instance and operations
        private static Loading_Form _instance;
        private static int _activeOperations = 0;
        private static readonly List<string> _activeMessages = [];
        private static readonly Dictionary<string, CancellationTokenSource> _operationCancellationTokens = [];

        // Events
        public static event EventHandler<string> OperationCancelled;

        // Init.
        public Loading_Form() : this("") { }  // This is needed for TranslationGenerator.GenerateAllLanguageTranslationFiles()
        private Loading_Form(string message)
        {
            InitializeComponent();
            LoadingPanel.ShowLoadingScreen(this, message, true);
            LoadingPanel.CancelRequested += OnCancelRequested;
            ThemeManager.SetThemeForForm(this);
        }

        // Form event handlers
        private void Loading_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadingPanel.CancelRequested -= OnCancelRequested;

            // Reset when form is closed
            LoadingPanel.HideLoadingScreen(this);
            _activeOperations = 0;
            _activeMessages.Clear();

            // Cancel all remaining operations
            foreach (CancellationTokenSource? tokenSource in _operationCancellationTokens.Values)
            {
                tokenSource?.Cancel();
                tokenSource?.Dispose();
            }
            _operationCancellationTokens.Clear();

            _instance = null;
        }
        private void OnCancelRequested(object sender, EventArgs e)
        {
            if (_activeMessages.Count > 0)
            {
                CancelAllOperations();
            }
        }

        // Static methods to manage operations
        /// <summary>
        /// Shows the loading form or updates the existing one with a new operation.
        /// </summary>
        public static void ShowLoading(string message, CancellationTokenSource cancellationTokenSource = null)
        {
            // Store the cancellation token source for this operation
            if (cancellationTokenSource != null)
            {
                _operationCancellationTokens[message] = cancellationTokenSource;
            }

            // Create form if needed or get existing
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new Loading_Form(message);

                // Show the form on a separate thread to prevent blocking
                Task.Run(() =>
                {
                    _instance.Invoke(new Action(() => _instance.Show()));
                });
            }

            // Add to operations count
            _activeOperations++;
            _activeMessages.Add(message);
            UpdateLoadingMessage();
        }

        /// <summary>
        /// Completes an operation and updates or closes the form.
        /// </summary>
        public static void CompleteOperation(string message = null)
        {
            if (_instance == null || _instance.IsDisposed)
            {
                return;
            }

            // Decrement operation count
            _activeOperations = Math.Max(0, _activeOperations - 1);

            // Remove the message and its cancellation token
            if (message != null && _activeMessages.Contains(message))
            {
                _activeMessages.Remove(message);
                if (_operationCancellationTokens.TryGetValue(message, out CancellationTokenSource? value))
                {
                    value?.Dispose();
                    _operationCancellationTokens.Remove(message);
                }
            }
            else if (_activeMessages.Count > 0)
            {
                // Remove the oldest message if none specified
                string oldestMessage = _activeMessages[0];
                _activeMessages.RemoveAt(0);
                if (_operationCancellationTokens.TryGetValue(oldestMessage, out CancellationTokenSource? value))
                {
                    value?.Dispose();
                    _operationCancellationTokens.Remove(oldestMessage);
                }
            }

            // Update or close the form
            if (_activeOperations <= 0)
            {
                _instance.Invoke(new Action(_instance.Close));
            }
            else
            {
                UpdateLoadingMessage();
            }
        }

        /// <summary>
        /// Cancels all active operations.
        /// </summary>
        private static void CancelAllOperations()
        {
            foreach (KeyValuePair<string, CancellationTokenSource> kvp in _operationCancellationTokens)
            {
                kvp.Value?.Cancel();
                OperationCancelled?.Invoke(null, kvp.Key);
            }

            // Clear all operations
            _activeOperations = 0;
            _activeMessages.Clear();
            _operationCancellationTokens.Clear();

            if (_instance != null && !_instance.IsDisposed)
            {
                _instance.Invoke(new Action(_instance.Close));
            }
        }

        /// <summary>
        /// Updates the loading message on the form to show how many operations are in progress.
        /// </summary>
        private static void UpdateLoadingMessage()
        {
            if (_instance == null || _instance.IsDisposed)
            {
                return;
            }

            // Use UI thread to update
            _instance.Invoke(new Action(() =>
            {
                string baseMessage = _activeMessages.Count > 0 ? _activeMessages[0] : "Loading...";
                string displayMessage = baseMessage;

                if (_activeOperations > 1)
                {
                    displayMessage = $"{baseMessage} ({_activeOperations} operations in progress)";
                }

                // Get the cancellation token source for the current operation
                CancellationTokenSource currentTokenSource = null;
                if (_activeMessages.Count > 0 && _operationCancellationTokens.TryGetValue(_activeMessages[0], out CancellationTokenSource? value))
                {
                    currentTokenSource = value;
                }

                LoadingPanel.ShowLoadingScreen(_instance, displayMessage, true, currentTokenSource);
            }));
        }
    }
}