using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ch.hsr.wpf.gadgeothek.websocket
{
    public class WebSocketClientNotification
    {
        public string Target { get; set; }

        public WebSocketClientNotificationTypeEnum Type { get; set; }

        public object Data { get; set; }

        public T DataAs<T>()
        {
            return JsonConvert.DeserializeObject<T>(Data.ToString());
        }
    }

    public enum WebSocketClientNotificationTypeEnum
    {
        Add,
        Update,
        Delete,
    }
}

