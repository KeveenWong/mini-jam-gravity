using UnityEngine;
using UnityEditor;

namespace FastMesh_Example
{
    [ExecuteInEditMode]
    public class SceneViewText : MonoBehaviour
    {
        public bool isShow = true;
        string text2 = "These 3D models, all created with \"Fast Mesh - 3D Asset Creation Tool\" (click)"; 
        Color backgroundColor = Color.white;
        Color textColor = Color.black; 
    
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
    
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    
        private void OnSceneGUI(SceneView sceneView)
        {
            if (isShow == false) return;
            
            Handles.BeginGUI();
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 20;
            style.normal.textColor = textColor;

            GUI.backgroundColor = backgroundColor;
            GUI.Label(new Rect(Screen.width / 2 - 400, 20, 800, 60), text2, style);
            Handles.EndGUI();
        }
    }
}
