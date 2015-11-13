using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Http;


namespace ch.hsr.wpf.gadgeothek.service
{
    public abstract class ServiceBase
    {
        public static LoginToken Token { get; set; }

        private static string _serverUrl;
        public static string ServerUrl
        {
            get
            {
                return _serverUrl;
            }
            set
            {
                Debug.WriteLine("Setting server to " + value);
                _serverUrl = value;
            }
        }


        private static string TokenAsString => Newtonsoft.Json.JsonConvert.SerializeObject(Token);




        protected static void GetList<T>(string uriPath, Action<List<T>> onResponse, Action<string> onError)
        {
            CheckToken();
            var parameter = PrepareDictionaryWithToken();

            CallApi<List<T>>(uriPath, HttpMethod.Get, parameter, onResponse, onError);
        }

        protected static Dictionary<string, string> PrepareDictionaryWithToken()
        {
            var parameter = new Dictionary<string, string>();
            parameter.Add("token", TokenAsString);
            return parameter;
        }

        protected static void CheckToken()
        {
            if (Token == null)
                throw new UnauthorizedAccessException("Not logged in");
        }





        public static void CallApi<T>(string uriPath, HttpMethod method, Dictionary<string, string> parameters, Action<T> onResponse, Action<string> onError)
        {


            // prepare params (but don't consider the "token" param which is set in the http header further below)
            var data = string.Join("&", parameters.Keys.Where(x => x != "token").Select(x => x + "=" + System.Web.HttpUtility.UrlEncode(parameters[x])));

            // if HTTP GET: append query string to uri
            var url = ServerUrl + uriPath + (method == HttpMethod.Get ? "?" + data : "");

            var request = WebRequest.CreateHttp(url);
            request.Method = method.Method;

            // additionally, add the params to the body
            // in case of GET: addition
            var bytes = Encoding.ASCII.GetBytes(data);

            if (Token != null)
            {
                request.Headers.Add("X-Security-Token", Token.SecurityToken);
                request.Headers.Add("X-Customer-Id", Token.CustomerId);
            }

            request.ContentLength = bytes.Length;
            if (request.Method != HttpMethod.Get.Method)
            {
                request.ContentType = "application/x-www-form-urlencoded";

                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bytes, 0, bytes.Length);
                }
            }


            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = GetResponseString(response);
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString);

                onResponse?.Invoke(obj);
            }
            catch (WebException wex)
            {
                var httpResponse = wex.Response as HttpWebResponse;
                if (httpResponse != null)
                {
                    onError?.Invoke($"Remote server call {method} {url} resulted in a http error {httpResponse.StatusCode} {httpResponse.StatusDescription}.");
                }
                else
                {
                    onError?.Invoke($"Remote server call {method} {url} resulted in an error: {wex.Message}.");
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke($"Remote server call {method} {url} resulted in an error: {ex.Message}.");
            }

        }

        protected static string GetResponseString(HttpWebResponse response)
        {
            using (var rstream = response.GetResponseStream())
            {
                using (var rsr = new System.IO.StreamReader(rstream))
                {
                    var content = rsr.ReadToEnd();
                    return content;
                }
            }
        }
    }
}
