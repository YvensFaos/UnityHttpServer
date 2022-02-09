using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Example related")] 
        [SerializeField] private List<Button> toggleObjects;
        
        [Header("Debug Related")] 
        [SerializeField] private bool debug;

        //Private variables
        private HttpClient _client;
        private string _formattedUrl;
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
        }
        
        public void StopClient()
        {
            if (!ClientIsUp) return;
            
            _client = null;
            _clientIsUp = false;
        }

        #region Client Commands
        public void SendPing()
        {
            SendGet($"{_formattedUrl}/ping", DefaultResponse);
        }
        public void SendExecuteEvents()
        {
            SendGet($"{_formattedUrl}/executeEvents", DefaultResponse);
        }

        /// <summary>
        /// Send a get request to the server and execute the callback when the response is received.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="callback"></param>
        private async void SendGet(string uri, Action<HttpResponseMessage> callback)
        {
            if (_client == null) return; 
            
            DebugMessage($"Sent command: {uri}");
            try
            {
                var response = await _client.GetAsync(uri);
                callback(response);
            }
            catch (Exception e)
            {
                //Fall into this catch if a time out or connection issue happens
                DebugMessage($"Error! Message: {e.Message}");
            }
        }

        /// <summary>
        /// Default reply for a message method in which the response is simply printed to the debug console.
        /// </summary>
        /// <param name="response"></param>
        private void DefaultResponse(HttpResponseMessage response)
        {
            DebugMessage(response.ToString());
        }

        public void SendToggleObject(int index)
        {
            //Create a JSON with the data that should be sent
            var json = JsonUtility.ToJson(new ToggleObject {index = index});
            
            SendPost($"{_formattedUrl}/toggleObject", json, (async (HttpResponseMessage httpResponseMessage) =>
            {
                //Read the content of the response message
                var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();
                DebugMessage(readAsStringAsync);
                
                //Get the response message as a ToggleStatus struct
                var toggleStatus = JsonUtility.FromJson<ToggleStatus>(readAsStringAsync);
                
                //Apply the changes to the game screen
                toggleObjects[toggleStatus.index].image.color = toggleStatus.active ? Color.green : Color.red;
            }));
        }

        private async void SendPost(string uri, string content, Action<HttpResponseMessage> callback)
        {
            if (_client == null) return;
            
            //Encode the content as bytes, serializing the JSON
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            //Mark the content type as JSON so the application knows how to deal with it
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            try
            {
                var response = await _client.PostAsync(uri, byteContent);
                callback(response);
            }
            catch (Exception e)
            {
                //Fall into this catch if a time out or connection issue happens
                DebugMessage($"Error! Message: {e.Message}");
            }
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
