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
        [SerializeField] private bool clientIsUp;
        [SerializeField] private bool initClientOnStart;
        
        [Header("Debug Related")] 
        [SerializeField] private bool debug;

        //Private variables
        private HttpClient _client;
        private string _formattedUrl;
        private Queue<HttpResponseMessage> _responses;

        private void Start()
        {
            if (initClientOnStart)
            {
                InitializeClient();
            }
        }
        
        public void InitializeClient()
        {
            if (clientIsUp) return;
            
            _client = new HttpClient();
            _formattedUrl = $"http://{url}:{port}";
            clientIsUp = true;
            
            DebugMessage($"Starting up client to {_formattedUrl}");
            
            _responses = new Queue<HttpResponseMessage>();
            StartCoroutine(ClientCoroutine());
        }
        
        /// <summary>
        /// Server coroutine. It runs together with Unity's thread and makes sure that the requests are handled correctly.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ClientCoroutine()
        {
            while (clientIsUp)
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
            DebugMessage($"Sent command: {uri}");
            _responses.Enqueue(await _client.GetAsync(uri));
        }

        public void SendToggleObject(int index)
        {
            var json = new JObject {{"index", index}};
            SendPost($"{_formattedUrl}/toggleObject", json);
        }

        private async void SendPost(string uri, JObject json)
        {
            var content = JsonConvert.SerializeObject(json);
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
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
