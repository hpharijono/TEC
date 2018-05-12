using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemporaryEmploymentCorp.Helpers.ReportViewer
{
    /// <summary>
    /// Acts as a wraper to a window to create only a single instance of that window.
    /// if the window is not yet created, a new instance will be created and shown/activated
    /// if the window is already created, it will become the active window.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleInstanceWindowViewer<T> where T : System.Windows.Window, new()
    {
        private bool _isWindowActive;
        private T _window;

        /// <summary>
        /// Gets the associated window of this instance.
        /// Make sure not to call the associated Window's Show method.
        /// </summary>
        public T Instance { get; private set; }

        public void Close()
        {
            _window?.Close();
        }

        public void Show()
        {
            if (!_isWindowActive) //if window is not active
            {
                _window = new T
                {
                    Topmost = false,
                    ShowActivated = true
                };
                Instance = _window;
                _window.Closed += OnClosed;

            }
            _window?.Show();
            _window?.Activate();
            _isWindowActive = true;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _isWindowActive = false;
            _window.Closed -= OnClosed;
        }
    }
}
