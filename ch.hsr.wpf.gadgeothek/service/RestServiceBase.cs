using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Diagnostics;
using System.IO;
using ch.hsr.wpf.gadgeothek.domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestSharp.Serializers;


namespace ch.hsr.wpf.gadgeothek.service
{
    /// <summary>
    /// simple client for a rest api using a security token
    /// </summary>
    public class RestServiceBase
    {
        /// <summary>
        /// flag to turn low-level logging on/off
        /// </summary>
        public static bool IsLogging = false;


        /// <summary>
        /// custom settings: make sure all types and properties are return in camelCase notation
        /// instead of PascalCase
        /// </summary>
        private JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };


        /// <summary>
        /// the current security token
        /// </summary>
        public LoginToken Token { get; set; }

        private string _serverUrl;

        /// <summary>
        /// the server url (incl. port)
        /// </summary>
        public string ServerUrl
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

        /// <summary>
        /// json representation of the security token
        /// </summary>
        private string TokenAsString => SerializeObject(Token);


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="serverUrl">the server url (incl. base path, if any)</param>
        public RestServiceBase(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        /// <summary>
        /// gets a list of objects using the REST API
        /// </summary>
        /// <typeparam name="T">type of the objects</typeparam>
        /// <param name="mustCheckToken">flag to tell if a security token is needed
        /// <param name="uriPath">path to the corresponding REST endpoint
        /// (or null in which case, the path will be automatically derive from the type name)</param>
        /// to access the REST endpoint (client-side check, prevents unnecessary
        /// round-trip to server if no token given)</param>
        /// <returns>list of objects</returns>
        public List<T> GetList<T>(bool mustCheckToken = true, string uriPath = null)
        {
            if (mustCheckToken)
                CheckToken();

            var parameter = PrepareDictionaryWithToken();

            return CallRestApi<List<T>>(uriPath ?? GetRestEndpoint<T>(), Method.GET, parameter);
        }

        /// <summary>
        /// gets an item using the REST API
        /// </summary>
        /// <typeparam name="T">type of the item</typeparam>
        /// <param name="id">the item's id</param>
        /// <param name="path">optional path to the item (if null, tries to automatically determine the path based on the type name)</param>
        /// <returns>true on successfull call of the POST method in the REST API</returns>
        /// <remarks>note for .NET newbies: type T is implicitly derived from the passed
        /// object by the compiler, so the type name does not need to be mentioned when
        /// calling this method</remarks>
        public T GetItem<T>(string id, string path = null)
            where T : class, new()
        {
            return CallRestApi<T, T>(path ?? GetRestResourcePath<T>(id), Method.GET, null);
        }


        /// <summary>
        /// adds an item using the REST API
        /// </summary>
        /// <typeparam name="T">type of the item</typeparam>
        /// <param name="obj">the item</param>
        /// <param name="path">optional path to the item (if null, tries to automatically determine the path based on the type name)</param>
        /// <returns>true on successfull call of the POST method in the REST API</returns>
        /// <remarks>note for .NET newbies: type T is implicitly derived from the passed
        /// object by the compiler, so the type name does not need to be mentioned when
        /// calling this method</remarks>
        public bool AddItem<T>(T obj, string path = null)
            where T : class
        {
            return CallRestApi<bool, T>(path ?? GetRestEndpoint<T>(), Method.POST, obj);
        }

        /// <summary>
        /// updates an item using the REST API
        /// </summary>
        /// <typeparam name="T">type of the item</typeparam>
        /// <param name="obj">the item</param>
        /// <param name="id">the item's id</param>
        /// <param name="path">optional path to the item (if null, tries to automatically determine the path based on the type name)</param>
        /// <returns>true on successfull call of the POST method in the REST API</returns>
        /// <remarks>note for .NET newbies: type T is implicitly derived from the passed
        /// object by the compiler, so the type name does not need to be mentioned when
        /// calling this method</remarks>
        public bool UpdateItem<T>(T obj, string id, string path = null)
            where T: class, new()
        {
            // the sample REST API returns the updated object (or the boolean value false if not found)
            var updatedObj = CallRestApi<T, T>(path ?? GetRestResourcePath<T>(id), Method.POST, obj);
            return updatedObj != null;
        }

        /// <summary>
        /// deletes an item using the rest api
        /// </summary>
        /// <typeparam name="T">type of the item</typeparam>
        /// <param name="obj">the item</param>
        /// <param name="id">the item's id</param>
        /// <param name="path">optional path to the item (if null, tries to automatically determine the path based on the type name)</param>
        /// <returns>true on successfull call of the DELETE method in the REST API</returns>
        /// <remarks>note for .NET newbies: type T is implicitly derived from the passed
        /// object by the compiler, so the type name does not need to be mentioned when
        /// calling this method</remarks>
        public bool DeleteItem<T>(T obj, string id, string path = null)
            where T:class
        {
            return CallRestApi<bool, T>(path ?? GetRestResourcePath<T>(id), Method.DELETE, obj);
        }

        /// <summary>
        /// returns a new dictionary that alreday contains the json representation
        /// of the security token (key "token")
        /// </summary>
        /// <returns>the dictionary</returns>
        protected Dictionary<string, string> PrepareDictionaryWithToken()
        {
            var parameter = new Dictionary<string, string>();
            parameter.Add("token", TokenAsString);
            return parameter;
        }

        /// <summary>
        /// raises an exception if no security token is given
        /// </summary>
        protected void CheckToken()
        {
            if (Token == null)
                throw new UnauthorizedAccessException("Not logged in");
        }

        /// <summary>
        /// calls the REST API at the given endpoint using the specified
        /// method and object in json representation
        /// </summary>
        /// <typeparam name="TR">result type of the REST call</typeparam>
        /// <typeparam name="TO">type of the object passed to the REST API</typeparam>
        /// <param name="uriPath">path to the REST endpoint</param>
        /// <param name="method">HTTP method</param>
        /// <param name="obj">object to be passed along</param>
        /// <param name="key">form param (key) in which the object should be transferred</param>
        /// <returns>result object of the given result type</returns>
        public TR CallRestApi<TR, TO>(string uriPath, Method method, TO obj = null, string key = "value")
            where TR : new()
            where TO : class 
        {
            return CallRestApi<TR>(uriPath, method, 
                obj == null 
                    ? null 
                    : new Dictionary<string, string>()
                    {
                        { key, SerializeObject(obj) }
                    });
        }


        /// <summary>
        /// calls the REST API at the given endpoint using the specified
        /// method and set of parameters
        /// </summary>
        /// <typeparam name="T">result type of the REST call</typeparam>
        /// <param name="uriPath">path to the REST endpoint</param>
        /// <param name="method">HTTP method</param>
        /// <param name="parameters">objects to be passed along given as key/value pairs (dictionary)</param>
        /// <returns>result object of the given result type</returns>
        public T CallRestApi<T>(string uriPath, Method method, Dictionary<string, string> parameters)
            where T:new()           
        {
            var client = new RestClient(ServerUrl);
            var req = new RestRequest(uriPath, method);

            // set security related information to the Http Header
            if (Token != null)
            {
                req.AddHeader("X-Security-Token", Token.SecurityToken);
                req.AddHeader("X-Customer-Id", Token.CustomerId);
            }
            if (parameters != null)
            {
                // Add all parameters to the request
                // but don't consider the token again
                foreach (var p in parameters.Where(x => x.Key != "token"))
                {
                    req.AddParameter(p.Key, p.Value);
                }
            }

            if (IsLogging)
            {
                Console.WriteLine($"REST API CALL: {req.Resource}: {req.Method} - PAYLOAD: {JsonConvert.SerializeObject(req.Parameters)}");
            }

            var resp = client.Execute<T>(req);
            return resp.Data;
        }


        

        /// <summary>
        /// builds a path of the appropriate REST endpoint (e.g. '/customers') 
        /// out of the type name
        /// </summary>
        /// <typeparam name="T">type of the domain object</typeparam>
        /// <returns>the path name of the REST endpoint, e.g. '/customers' for domain objects of type <see cref="Customer"/></returns>
        private string GetRestEndpoint<T>()
        {
            var typeName = typeof(T).Name;
            return $"/{typeName.ToLower()}s";
        }

        /// <summary>
        /// builds a path to the appropriate REST resource (e.g. '/customers/5') 
        /// out of the type name and the id of the given object
        /// </summary>
        /// <typeparam name="T">type of the domain object</typeparam>
        /// <param name="id">id of the object to be used as the identifier in the url</param>
        /// <returns>the path name of the REST resource, e.g. '/customers/5' for a domain object of type <see cref="Customer"/> with id 5</returns>
        private string GetRestResourcePath<T>(string id)
        {
            var typeName = typeof(T).Name;
            return $"/{typeName.ToLower()}s/{id}";
        }


        /// <summary>
        /// ensures proper json serialization of the given object
        /// </summary>
        /// <typeparam name="T">type name of the given object</typeparam>
        /// <param name="obj">object to serialize</param>
        /// <returns>object representation as json string</returns>
        protected string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }
    }

    public class UpdateResult
    {
        public string Data { get; set; }
    }
}
