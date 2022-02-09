using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Client
{
    public class UnityHttpClient : MonoBehaviour
    {
        //Singleton variables
        private static UnityHttpClient _instance;
        //Singleton public variable to allow global access
        public static UnityHttpClient Instance;

        [Header("Server related information")] 
        [SerializeField] private string url = $"localhost";
        [SerializeField] private int port = 8000;
        
        [SerializeField] private bool initClientOnStart;
        
        [Header("Debug Related")] 
        [SerializeField] private bool debug;

        //Private variables
        private HttpClient _client;
        private string _formattedUrl;
        private Queue<HttpResponseMessage> _responses;
        private bool _clientIsUp;

        public bool ClientIsUp => _clientIsUp;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);
        }
        
        private void Start()
        {
            if (initClientOnStart)
            {
                InitializeClient();
            }
        }
        
        public void InitializeClient()
        {
            if (ClientIsUp) return;
            
            //Initialize client and stores the URL that is going to be used
            _client = new HttpClient();
            _formattedUrl = $"http://{url}:{port}";
            _clientIsUp = true;
            DebugMessage($"Starting up client to {_formattedUrl}");
            
            //Initialize the queue of responses. Every new response is added to the queue and handled in the order in
            //which they entered the queue (First In, First Out).
            _responses = new Queue<HttpResponseMessage>();
            
            //Initialize the client coroutine
            StartCoroutine(ClientCoroutine());
        }
        
        public void StopClient()
        {
            if (!ClientIsUp) return;
            
            //Stop the server and kill the server thread and coroutine.
            StopAllCoroutines();
            _client = null;
            _clientIsUp = false;
        }
        
        /// <summary>
        /// Server coroutine. It runs together with Unity's thread and makes sure that the requests are handled correctly.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ClientCoroutine()
        {
            while (ClientIsUp)
            {
                //Waits until there is at least 1 response to be handled.
                yield return new WaitUntil(() => _responses.Count > 0);
                //Iterates over the lists of responses removing them one by one until the queue is empty.
                while (_responses.Count > 0)
                {
                    //Resolves the responses in the queue.
                    ResolveResponse(_responses.Dequeue());
                }
            }
        }
        
        private void ResolveResponse(HttpResponseMessage response)
        {
            //For now it is simply printing the response. You could expand this code to read the response's answer code
            //and content and deal with each response separately according to your application.
            DebugMessage(response.ToString());            
        }
        
        #region Client Commands
        public void SendPing()
        {
            SendGet($"{_formattedUrl}/ping");
        }
        public void SendExecuteEvents()
        {
            SendGet($"{_formattedUrl}/executeEvents");
        }

        private async void SendGet(string uri)
        {
            if (_client == null) return; 
            
            DebugMessage($"Sent command: {uri}");
            //Enqueue the async command to the get URL received
            _responses.Enqueue(await _client.GetAsync(uri));
        }

        public void SendToggleObject(int index)
        {
            //Create a JSON with the data that should be sent
            var json = new JObject {{"index", index}};
            SendPost($"{_formattedUrl}/toggleObject", json);
        }

        private async void SendPost(string uri, JObject json)
        {
            if (_client == null) return;
            
            //Encode the content as bytes, serializing the JSON
            var content = JsonConvert.SerializeObject(json);
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            //Mark the content type as JSON so the application knows how to deal with it
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            //Enqueue the async command to the URL received
            _responses.Enqueue(await _client.PostAsync(uri, byteContent));
        }
        #endregion
        
        #region Debug Related Code
        private void DebugMessage(string message)
        {
            if (!debug) return;
            Debug.Log(message);
        }
        #endregion
    }
}
