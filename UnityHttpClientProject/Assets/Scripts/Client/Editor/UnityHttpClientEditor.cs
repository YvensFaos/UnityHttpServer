using UnityEditor;
using UnityEngine;

namespace Client.Editor
{
    [CustomEditor(typeof(UnityHttpClient))]
    public class UnityHttpClientEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var unityHttpClient = (UnityHttpClient) target;
            
            EditorGUILayout.Toggle("Client is up", unityHttpClient.ClientIsUp);
            
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Start Client"))
            {
                unityHttpClient.InitializeClient();
            }
            if (GUILayout.Button("Stop Client"))
            {
                unityHttpClient.StopClient();
            }
            
            GUILayout.Space(15);
            
            if (GUILayout.Button("Ping"))
            {
                unityHttpClient.SendPing();
            }
            
            if (GUILayout.Button("Execute Events"))
            {
                unityHttpClient.SendExecuteEvents();
            }

            GUILayout.Space(15);
            
            for (int i = 0; i < 3; i++)
            {
                if (GUILayout.Button($"Toggle Object [{i}]"))
                {
                    unityHttpClient.SendToggleObject(i);
                }
            }
        }
    }
}