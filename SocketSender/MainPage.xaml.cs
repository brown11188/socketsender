using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
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
                //Write data to the echo server.
                //DataWriter writer = new DataWriter(socket.OutputStream);
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
                //writer.WriteUInt32(writer.MeasureString(request));
                //writer.WriteString(request);

                //try
                //{
                //    await writer.StoreAsync();
                //}
                //catch (Exception exception)
                //{
                //    // If this is an unknown status it means that the error if fatal and retry will likely fail.
                //    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                //    {
                //        throw;
                //    }

                //}

                await writer.WriteLineAsync(request);
                await writer.FlushAsync().ConfigureAwait(true);

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

        private HostName serverHost;

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Create the StreamSocket and establish a connection to the echo server.
                socket = new StreamSocket();

                //The server hostname that we will be establishing a connection to. We will be running the server and client locally,
                //so we will use localhost as the hostname.
                serverHost = new HostName("113.160.225.76");

                string serverPort = "2099";
                await socket.ConnectAsync(serverHost, serverPort);

                //string certInformation = GetCertificateInformation(
                //    socket.Information.ServerCertificate,
                //    socket.Information.ServerIntermediateCertificates);
                //Result.Text = certInformation;
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                // Could retry the connection, but for this simple example
                // just close the socket.

                socket.Dispose();
                socket = null;
            }
        }

        private string GetCertificateInformation(
            Certificate serverCert,
            IReadOnlyList<Certificate> intermediateCertificates)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("\tFriendly Name: " + serverCert.FriendlyName);
            stringBuilder.AppendLine("\tSubject: " + serverCert.Subject);
            stringBuilder.AppendLine("\tIssuer: " + serverCert.Issuer);
            stringBuilder.AppendLine("\tValidity: " + serverCert.ValidFrom + " - " + serverCert.ValidTo);

            // Enumerate the entire certificate chain.
            if (intermediateCertificates.Count > 0)
            {
                stringBuilder.AppendLine("\tCertificate chain: ");
                foreach (var cert in intermediateCertificates)
                {
                    stringBuilder.AppendLine("\t\tIntermediate Certificate Subject: " + cert.Subject);
                }
            }
            else
            {
                stringBuilder.AppendLine("\tNo certificates within the intermediate chain.");
            }

            return stringBuilder.ToString();
        }

        private async void Upgrade_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Try to upgrade to SSL
                await socket.UpgradeToSslAsync(SocketProtectionLevel.Tls12, serverHost);

            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }


                socket.Dispose();
                socket = null;
                return;
            }
        }
    }
}
