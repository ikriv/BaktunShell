using System.AddIn.Contract;
using System.Runtime.Remoting.Messaging;

namespace Interfaces
{
    public interface IPluginLoader
    {
        INativeHandleContract LoadPlugin(string assembly, string typeName);

        [OneWay]
        void Terminate();
    }
}
