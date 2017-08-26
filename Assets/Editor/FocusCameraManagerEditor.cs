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
        
        if (GUILayout.Button("Focus Test"))
        {
            if (!EditorApplication.isPlaying)
                return;

            var manager = (FocusCameraManager)target;
            //manager.Focus(GameManager.scene.pc_manager.transform.position, 9, 3);
            manager.Focus(GameObject.Find("Mouse").transform.position, 9, 3);
        }
    }

}
