using Microsoft.Win32;
using Argo_Books.Classes;

namespace Argo_Books.Theme
{
    /// <summary>
    /// Monitors Windows theme registry keys and raises events when modifications are detected.
    /// </summary>
    internal class RegistryWatcher(RegistryHive hive, string keyPath)
    {
        // Properties
        private readonly RegistryHive _hive = hive;
        private readonly string _keyPath = keyPath;
        private readonly ManualResetEvent _stopEvent = new(false);
        private Thread _thread;

        // Getter
        public event EventHandler RegChanged;

        // Methods
        public void Start()
        {
            _stopEvent.Reset();
            _thread = new Thread(WatchForChanges)
            {
                IsBackground = true,
                Name = "RegistryWatcherThread"
            };
            _thread.Start();
        }
        public void Stop()
        {
            _stopEvent.Set();
            _thread?.Join(1000);
        }
        private void WatchForChanges()
        {
            object previousValue = null;
            bool firstRun = true;

            while (!_stopEvent.WaitOne(1000))
            {
                try
                {
                    using RegistryKey key = RegistryKey.OpenBaseKey(_hive, RegistryView.Default).OpenSubKey(_keyPath, false);
                    if (key == null) { continue; }

                    // Read the specific value we care about
                    object currentValue = key.GetValue("AppsUseLightTheme");

                    // If the value has changed
                    if (!firstRun && !Equals(currentValue, previousValue))
                    {
                        RegChanged?.Invoke(this, EventArgs.Empty);
                        Log.WriteWithFormat(1, "Windows theme changed to: {0}", (currentValue?.ToString() == "0" ? "Dark" : "Light"));
                    }

                    // Update tracking variables
                    if (firstRun)
                    {
                        firstRun = false;
                    }
                    previousValue = currentValue;
                }
                catch (Exception ex)
                {
                    Log.Error_RegistryWatcher($"{ex.Message}");
                }
            }
        }
    }
}