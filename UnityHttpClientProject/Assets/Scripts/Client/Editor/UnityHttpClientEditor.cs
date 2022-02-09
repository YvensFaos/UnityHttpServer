using UnityEngine;

namespace Client.Editor
{
    [UnityEditor.CustomEditor(typeof(UnityHttpClient))]
    public class UnityHttpClientEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var unityHttpClient = (UnityHttpClient) target;
            
            base.OnInspectorGUI();

            if (GUILayout.Button("Start Client"))
            {
                unityHttpClient.InitializeClient();
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