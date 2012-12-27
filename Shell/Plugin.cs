using System.Windows;
using System;

namespace Shell
{
    class Plugin : IDisposable
    {
        public Plugin(FrameworkElement view)
        {
            View = view;
        }

        public FrameworkElement View { get; private set; }

        public string Title { get; set; }

        public void Dispose()
        {
            if (Disposed != null) Disposed(this, EventArgs.Empty);
        }

        public event EventHandler Disposed;
    }
}
