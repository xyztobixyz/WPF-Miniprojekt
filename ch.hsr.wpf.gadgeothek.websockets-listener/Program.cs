using System;
using System.Threading;
using System.Threading.Tasks;
using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;


namespace ch.hsr.wpf.gadgeothek.runner
{
    class Program
    {
        static void Main(string[] args)
        {



            /*
                This part demonstrates how to deal with live push notifications through Web sockets
            */

            Console.WriteLine("App Termination Notice:");
            Console.WriteLine("The app is listening for updates through web sockets");
            Console.WriteLine("You should see live updates for all changed data");
            Console.WriteLine("<Press CTRL + C to terminate the app>");


            var url = "http://localhost:8080";

            // web socket connection to listen to changes:
            var client = new websocket.WebSocketClient(url);
            client.NotificationReceived += (o, e) =>
            {
                Console.WriteLine("WebSocket::Notification: " + e.Notification.Target + " > " + e.Notification.Type);

                // demonstrate how these updates could be further used
                if (e.Notification.Target == typeof (Gadget).Name.ToLower())
                {
                    // deserialize the json representation of the data object to an object of type Gadget
                    var gadget = e.Notification.DataAs<Gadget>();
                    // now you can use it as usual...
                    //Console.WriteLine("Details: " + gadget);
                }
            };

            // spawn a new background thread in which the websocket client listens to notifications from the server
            var bgTask = client.ListenAsync();

            // make sure, the foreground thread of the console app does not terminate
            // (would stop the background thread, too)
            // press CTRL + C in the console window to stop execution
            Task.WaitAll(bgTask);
        }
    }
}
