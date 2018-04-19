using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace CarWifiBackground
{
    class Server
    {
        private string buffer;
        private Motor motor;

        public Server(Motor motor)
        {
            buffer = "";
            this.motor = motor;
        }
        public void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] info = new Byte[1];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntryAsync(Dns.GetHostName()).GetAwaiter().GetResult();
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            Debug.WriteLine(ipAddress.ToString());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 2001);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            Windows.Networking.Sockets.StreamSocketListener socket = new Windows.Networking.Sockets.StreamSocketListener();
            Windows.Networking.HostName hostName = new Windows.Networking.HostName("192.168.1.105");


            int rec_flag = 0;
            int i = 0;
            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(2);
                motor.StartLeds();
                // Start listening for connections.  
                while (true)
                {
                    Debug.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.AcceptAsync().GetAwaiter().GetResult();
                    string exitData = "";
                    Debug.WriteLine("Accepted connection");
                    info = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        info = new byte[1];
                        int bytesRec = handler.Receive(info);

                        string data = string.Concat(info.Select(b => b.ToString("X2")).ToArray());//Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        Debug.WriteLine("data: " + data);
                        if (data == "00")
                        {
                            exitData += data;
                        }
                        else
                        {
                            exitData = "";
                        }
                        if (exitData == "00000000")
                        {
                            break;
                        }

                        if (rec_flag == 0)
                        {
                            if (data == "FF")
                            {
                                buffer = "";
                                rec_flag = 1;
                                i = 0;

                            }

                        }
                        else
                        {
                            if (data == "FF")
                            {
                                rec_flag = 0;
                                if (i == 3)
                                {
                                    Debug.WriteLine("Got data " + data);
                                    Communication_Decode();
                                }
                                i = 0;
                            }
                            else
                            {
                                buffer += data;
                                i += 1;
                            }
                        }
                    }

                    // Show the data on the console.  
                    Debug.WriteLine("Text received : {0}", info);

                    handler.Shutdown(SocketShutdown.Both);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }
        public void Communication_Decode()
        {
            if (buffer.Substring(0, 2) == "00")
            {
                if (buffer.Substring(2, 2) == "01")
                {
                    motor.Forward();
                }
                else if (buffer.Substring(2, 2) == "02")
                {
                    motor.Backward();
                }
                else if (buffer.Substring(2, 2) == "03")
                {
                    motor.TurnLeft();
                }
                else if (buffer.Substring(2, 2) == "04")
                {
                    motor.TurnRight();
                }
                else if (buffer.Substring(2, 2) == "00")
                {
                    motor.Stop();
                }
            }

        }
    }
}
