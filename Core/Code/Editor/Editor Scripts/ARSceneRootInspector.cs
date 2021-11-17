using UnityEngine;
using UnityEditor;
using Bridge.Core.App.AR.Manager;

namespace Bridge.Core.UnityEditor.AR.Manager
{
    [CustomEditor(typeof(ARSceneRoot), true)]
    public class ARSceneRootInspector : Editor
    {
      public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            if(GUILayout.Button("Open Content Manager", GUILayout.Height(50)))
            {
                ARSceneRootEditorWindow.OpenARManagerEditor();
            }
        }
    }
}
