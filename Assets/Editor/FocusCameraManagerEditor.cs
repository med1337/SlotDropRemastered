using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FocusCameraManager))]
public class FocusCameraManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Focus on Monitor"))
        {
            if (!EditorApplication.isPlaying)
                return;

            var manager = (FocusCameraManager)target;
            manager.Focus(GameManager.scene.pc_manager.transform.position, 9, 1.5f);
        }

        if (GUILayout.Button("Focus on Mouse"))
        {
            if (!EditorApplication.isPlaying)
                return;

            var manager = (FocusCameraManager)target;
            manager.Focus(GameObject.Find("Mouse").transform.position, 5, 1.5f);
        }
    }

}
