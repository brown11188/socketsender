using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Mateo.UILogic.SocketMessage;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SocketSender
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StreamSocket socket;
        public MainPage()
        {
            this.InitializeComponent();
            
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {

            try
            {
                //Create the StreamSocket and establish a connection to the echo server.
                Windows.Networking.Sockets.StreamSocket socket = new Windows.Networking.Sockets.StreamSocket();

                //The server hostname that we will be establishing a connection to. We will be running the server and client locally,
                //so we will use localhost as the hostname.
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName("113.160.225.76");

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 1337.
                string serverPort = "2099";
                await socket.ConnectAsync(serverHost, serverPort);

                //Write data to the echo server.
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut);
                CommandMessage message =  new CommandMessage()
                {
                    CommandType = "MotorStartCommand",
                    Message = "Start",
                    TimeStamp = DateTime.UtcNow.ToString(),
                    CompanyName = "Enclave",
                    Username = "Harold"
                };
                string request = JsonConvert.SerializeObject(message);
                await writer.WriteLineAsync(request);
                await writer.FlushAsync();

                //Read data from the echo server.
                Stream streamIn = socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(streamIn);
                string response = await reader.ReadLineAsync();
                Result.Text = response;
            }
            catch (Exception ex)
            {
                //Handle exception here.            
            }
        }
    }
}
