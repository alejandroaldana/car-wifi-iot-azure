using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace CarWifiBackground
{
    public sealed class StartupTask : IBackgroundTask
    {
        Motor motor;
        Server server;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Debug.WriteLine("Inicia");
            motor = new Motor();
            server = new Server(motor);
            server.StartListening();
        }
    }
}
