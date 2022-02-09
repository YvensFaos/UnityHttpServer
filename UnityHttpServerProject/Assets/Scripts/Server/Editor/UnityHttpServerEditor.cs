using UnityEditor;
using UnityEngine;

namespace Server.Editor
{
    [CustomEditor(typeof(UnityHttpServer))]
    public class UnityHttpServerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var unityHttpServer = (UnityHttpServer) target;

            EditorGUILayout.Toggle("Server is up", unityHttpServer.ServerIsUp);
            
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Start Server"))
            {
                unityHttpServer.InitializeServer();
            }
            if (GUILayout.Button("Stop Server"))
            {
                unityHttpServer.StopServer();
            }
        }
    }
}