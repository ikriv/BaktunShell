using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Windows.Threading;

namespace PluginHost
{
    class Program
    {
        public static Dispatcher Dispatcher { get; private set; }

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: PluginHost name");
                return;
            }

            try
            {
                var name = args[0];
                int bits = IntPtr.Size * 8;
                Console.WriteLine("Starting PluginHost {0}, {1} bit", name, bits );

                Dispatcher = Dispatcher.CurrentDispatcher;

                var serverProvider = new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };
                var clientProvider = new BinaryClientFormatterSinkProvider();
                var properties = new Hashtable();
                properties["portName"] = name;

                var channel = new IpcChannel(properties, clientProvider, serverProvider);
                ChannelServices.RegisterChannel(channel, false);

                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(PluginLoader), "PluginLoader", WellKnownObjectMode.Singleton);

                SignalReady(name);

                Dispatcher.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static void SignalReady(string name)
        {
            var eventName = name + ".Ready";
            var readyEvent = EventWaitHandle.OpenExisting(eventName);
            readyEvent.Set();
        }
    }
}
