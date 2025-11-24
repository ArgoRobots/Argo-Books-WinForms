using Argo_Books.Classes;
using Argo_Books.DataClasses;
using Argo_Books.Theme;
using Guna.UI2.WinForms;
using Argo_Books.Language;
using Argo_Books.Theme;
using Timer = System.Windows.Forms.Timer;

namespace Argo_Books.UI
{
    /// <summary>
    /// Provides a loading animation panel for better UX during data-intensive operations.
    /// This prevents the screen from flashing white while the Form is loading, especially in dark theme scenarios.
    /// </summary>
    public class LoadingPanel
    {
        // Properties
        private static Label _messageLabel;
        private static Timer _animationTimer;
        private static Guna2WinProgressIndicator _progressIndicator;
        private static Panel _animationPanel;
        private static Guna2Button _cancelButton;
        private static CancellationTokenSource _cancellationTokenSource;
        private static Control _currentControl;

        // Events
        public static event EventHandler CancelRequested;

        // Getters and setters
        public static Panel BlankLoadingPanelInstance { get; private set; }
        public static Panel LoadingPanelInstance { get; private set; }

        public static void InitBlankLoadingPanel()
        {
            BlankLoadingPanelInstance = new Panel
            {
                BackColor = CustomColors.MainBackground,
                Dock = DockStyle.Fill
            };
        }
        public static void InitLoadingPanel()
        {
            LoadingPanelInstance = new Panel
            {
                BackColor = CustomColors.MainBackground,
                Dock = DockStyle.Fill
            };

            // Create a separate panel for the animation to run on its own thread
            _animationPanel = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(100, 100)
            };

            _progressIndicator = new Guna2WinProgressIndicator
            {
                ProgressColor = CustomColors.AccentBlue,
                AnimationSpeed = 80
            };

            _messageLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Text = "Loading...",
                AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create cancel button
            _cancelButton = new Guna2Button
            {
                Text = "Cancel",
                Size = new Size(200, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Regular)
            };
            _cancelButton.Click += CancelButton_Click;
            ThemeManager.MakeGButtonBlueSecondary(_cancelButton);

            // Create a timer for smooth animation
            _animationTimer = new Timer
            {
                Interval = 100,
                Enabled = false
            };
            _animationTimer.Tick += AnimationTimer_Tick;

            _animationPanel.Controls.Add(_progressIndicator);
            LoadingPanelInstance.Controls.Add(_animationPanel);
            LoadingPanelInstance.Controls.Add(_messageLabel);
            LoadingPanelInstance.Controls.Add(_cancelButton);
        }
        private static void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_progressIndicator != null && _progressIndicator.IsHandleCreated)
            {
                _progressIndicator.BeginInvoke(new Action(() =>
                {
                    _progressIndicator.Invalidate();
                    _progressIndicator.Update();
                }));
            }
        }
        private static void CancelButton_Click(object sender, EventArgs e)
        {
            // Cancel the operation
            _cancellationTokenSource?.Cancel();

            // Hide the loading panel from the current control
            if (_currentControl != null)
            {
                HideLoadingScreen(_currentControl);
            }

            // Fire the cancel requested event for any additional handling
            CancelRequested?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Displays a loading screen with a progress indicator and a message over a specified control.
        /// </summary>
        public static void ShowLoadingScreen(Control control, string message, bool showCancelButton = false, CancellationTokenSource cancellationTokenSource = null)
        {
            string translatedMessage = LanguageManager.TranslateString(message);
            _cancellationTokenSource = cancellationTokenSource;
            _currentControl = control;

            control.Controls.Add(LoadingPanelInstance);
            LoadingPanelInstance.BringToFront();

            // Position the animation panel
            _animationPanel.Location = new Point(
                (LoadingPanelInstance.Width - _animationPanel.Width) / 2,
                (LoadingPanelInstance.Height - _animationPanel.Height) / 2
            );

            _progressIndicator.Location = new Point(
                (_animationPanel.Width - _progressIndicator.Width) / 2,
                (_animationPanel.Height - _progressIndicator.Height) / 2
            );

            _messageLabel.Text = translatedMessage;
            _messageLabel.Location = new Point(
                (LoadingPanelInstance.Width - _messageLabel.Width) / 2,
                _animationPanel.Top - 70
            );
            _messageLabel.ForeColor = CustomColors.Text;

            _cancelButton.Visible = showCancelButton;
            if (showCancelButton)
            {
                _cancelButton.Location = new Point(
                    (LoadingPanelInstance.Width - _cancelButton.Width) / 2,
                    _animationPanel.Bottom + 80
                );
            }

            // Start the progress indicator animation
            _progressIndicator.Start();
            _animationTimer.Start();
        }
        public static void ShowBlankLoadingPanel(Control control, Color? color = null)
        {
            BlankLoadingPanelInstance.InvokeIfRequired(() =>
            {
                BlankLoadingPanelInstance.BackColor = color ?? CustomColors.MainBackground;
                control.Controls.Add(BlankLoadingPanelInstance);
                BlankLoadingPanelInstance.BringToFront();
            });
        }
        public static void HideLoadingScreen(Control control)
        {
            _animationTimer.Stop();
            _progressIndicator.Stop();
            _currentControl = null;

            control.InvokeIfRequired(() =>
            {
                control.Controls.Remove(LoadingPanelInstance);
            });
        }
        public static void HideBlankLoadingPanel(Control control)
        {
            control.InvokeIfRequired(() =>
            {
                control.Controls.Remove(BlankLoadingPanelInstance);
            });
        }
        public static void UpdateTheme()
        {
            LoadingPanelInstance.BackColor = CustomColors.MainBackground;
            BlankLoadingPanelInstance.BackColor = CustomColors.MainBackground;
            _progressIndicator.ProgressColor = CustomColors.AccentBlue;
            _messageLabel.ForeColor = CustomColors.Text;
        }
    }
}