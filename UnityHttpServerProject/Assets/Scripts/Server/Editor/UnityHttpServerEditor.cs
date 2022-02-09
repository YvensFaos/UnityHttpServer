using UnityEngine;

namespace Server.Editor
{
    [UnityEditor.CustomEditor(typeof(UnityHttpServer))]
    public class UnityHttpServerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var unityHttpServer = (UnityHttpServer) target;
            
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