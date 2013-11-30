using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Interfaces;
using System.Windows;

namespace Shell.Services
{
    class AssemblySearch
    {
        public string[] GetAssemblyNames()
        {
            var dir = Directory.GetCurrentDirectory();
            var dllFiles = Directory.GetFiles(dir, "*.dll");
            var exeFiles = Directory.GetFiles(dir, "*.exe");
            var files = dllFiles.Union(exeFiles);

            var exclusions = new[]
            {
                GetType().Assembly.GetName().Name, 
                typeof(IPluginLoader).Assembly.GetName().Name,
                "PluginHost32",
                "PluginHost64"
            };

            return 
                files
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Where(f=>!f.Contains(".vshost"))
                .Except(exclusions)
                .OrderBy(s=>s)
                .ToArray();
        }

        public string[] GetPluginClasses(string assembly)
        {
            var domain = AppDomain.CreateDomain("Test");

            try
            {
                var loader = (Loader)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().FullName, typeof(Loader).FullName);
                var result = loader.GetPluginClasses(assembly);
                return result;
            }
            catch
            {
                return new string[0];
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        public class Loader : MarshalByRefObject
        {
            public string[] GetPluginClasses(string assemblyName)
            {
                var assembly = AppDomain.CurrentDomain.Load(assemblyName);
                var types = assembly.GetTypes();

                return types.Where(t => t.IsSubclassOf(typeof(UserControl)))
                    .Select(t => t.FullName)
                    .OrderBy(n=>n)
                    .ToArray();
            }
        }

    }
}
