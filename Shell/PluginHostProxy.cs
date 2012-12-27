using System;
using System.AddIn.Pipeline;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Windows;
using Interfaces;

namespace Shell
{
    class PluginHostProxy
    {
        private string _name;
        private EventWaitHandle _readyEvent;
        private int _refCount;
        private Process _process;
        private IPluginLoader _pluginLoader;

        class IpcChannelRegistration
        {
            private static object _lock = new object();
            private static bool _registered;

            public static void RegisterChannel()
            {
                lock (_lock)
                {
                    if (_registered) return;
                    var channel = new IpcChannel();
                    ChannelServices.RegisterChannel(channel, false);
                    _registered = true;
                }

            }
        }

        public bool Is64Bit { get; set; }

        public Plugin LoadPlugin(string assemblyName, string typeName)
        {
            Start();
            OpenPluginLoader();
            var contract = _pluginLoader.LoadPlugin(assemblyName, typeName);
            var remoteControl = FrameworkElementAdapters.ContractToViewAdapter(contract);

            var plugin = new Plugin(remoteControl) { Title = GetTitle(typeName) };
            ++_refCount;
            plugin.Disposed += OnPluginDisposed;

            return plugin;
        }

        private static string GetTitle(string typeName)
        {
            int dot = typeName.IndexOf('.');
            if (dot < 0 || dot >= typeName.Length-1) return typeName;
            return typeName.Substring(dot+1);
        }

        private void Start()
        {
            if (_process != null) return;
            _name = "PluginHost." + Guid.NewGuid().ToString();
            var eventName = _name + ".Ready";
            _readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, eventName);

            var processName = Is64Bit ? "PluginHost64.exe" : "PluginHost32.exe";

            var info = new ProcessStartInfo
            {
                Arguments = _name,
#if !DEBUG
                CreateNoWindow = true,
#endif
                UseShellExecute = false,
                FileName = processName
            };

            _process = Process.Start(info);
        }

        private void OpenPluginLoader()
        {
            if (_pluginLoader != null) return;

            if (!_readyEvent.WaitOne(5000))
            {
                throw new InvalidOperationException("Plugin host process not ready");
            }

            IpcChannelRegistration.RegisterChannel();

            var url = "ipc://" + _name + "/PluginLoader";
            _pluginLoader = (IPluginLoader)Activator.GetObject(typeof(IPluginLoader), url);
        }

        private void OnPluginDisposed(object sender, EventArgs args)
        {
            --_refCount;
            var plugin = sender as Plugin;
            if (plugin != null) plugin.Disposed -= OnPluginDisposed;

            if (_refCount == 0)
            {
                _pluginLoader.Terminate();
                _pluginLoader = null;
                _process = null;
            }
        }
    }
}
