using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Shell.Util;
using Shell.Services;
using System.Linq;
using System.Collections.Generic;

namespace Shell
{
    class MainViewModel : INotifyPropertyChanged
    {
        private readonly AssemblySearch _assemblySearch = new AssemblySearch();

        public MainViewModel()
        {
            Plugins = new ObservableCollection<Plugin>();
            LoadCommand = new DelegateCommand(Load);

            Assemblies = _assemblySearch.GetAssemblyNames();
            AssemblyName = Assemblies.FirstOrDefault();

            LoadClassNames();
        }

        public ICommand LoadCommand { get; private set; }

        public ObservableCollection<Plugin> Plugins { get; private set; }

        public Plugin SelectedPlugin
        {
            get { return _selectedPlugin; }
            set { _selectedPlugin = value; RaisePropertyChanged("SelectedPlugin"); }
        }
        private Plugin _selectedPlugin;

        public IEnumerable<string> Assemblies { get; private set; }

        public string AssemblyName 
        { 
            get { return _assemblyName; }
            set { _assemblyName = value; LoadClassNames(); }
        }
        private string _assemblyName;

        public IEnumerable<string> ClassNames 
        {
            get { return _classNames; }
            set { _classNames = value; RaisePropertyChanged("ClassNames"); }
        }
        private IEnumerable<string> _classNames;

        public string ClassName
        {
            get { return _className; }
            set { _className = value; RaisePropertyChanged("ClassName"); }
        }
        private string _className;

        public int Bitness
        {
            get { return _bitness;  }
            set { _bitness = value; RaisePropertyChanged("Bitness"); }
        }
        private int _bitness = 32;

        private void Load()
        {
            try
            {
                var pluginHost = new PluginHostProxy { Is64Bit = (Bitness == 64) };
                var plugin = pluginHost.LoadPlugin(AssemblyName, ClassName);
                Plugins.Add(plugin);
                SelectedPlugin = plugin;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CloseTab(Plugin plugin)
        {
            Plugins.Remove(plugin);
            plugin.Dispose();
        }

        private void LoadClassNames()
        {
            if (String.IsNullOrEmpty(AssemblyName))
            {
                ClassNames = new string[0];
            }
            else
            {
                ClassNames = _assemblySearch.GetPluginClasses(AssemblyName);
            }

            ClassName = ClassNames.FirstOrDefault();
        }

        public void OnWindowClosed()
        {
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.Dispose();
                }
                catch { }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
