using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ch.hsr.wpf.gadgeothek.websocket
{
    public class WebSocketClient
    {

        public string ServerUrl { get; set; }


        public event EventHandler<WebSocketClientNotificationEventArgs> NotificationReceived;


        public WebSocketClient(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        public void Listen()
        {
            ListenAsync().Wait();
        }


        public async Task ListenAsync()
        {
            var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri(ServerUrl.Replace("http://", "ws://")), CancellationToken.None);

            // listen for updates
            await Receive(socket);
        }

        private async Task Receive(ClientWebSocket socket)
        {
            var buffer = new byte[2048];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var json = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                    var notification = JsonConvert.DeserializeObject<WebSocketClientNotification>(json);

                    NotificationReceived?.Invoke(this, new WebSocketClientNotificationEventArgs(notification));

                    Array.Clear(buffer,0,buffer.Length);
                }
            }
        }
    }

    public class WebSocketClientNotificationEventArgs : EventArgs
    {
        public WebSocketClientNotification Notification { get; set; }

        public WebSocketClientNotificationEventArgs(WebSocketClientNotification notification)
        {
            Notification = notification;
        }
    }
}
