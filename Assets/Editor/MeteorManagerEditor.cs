using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeteorManager))]
public class MeteorManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (GUILayout.Button("Spawn Meteors"))
        {
            var meteor_manager = (MeteorManager)target;
            meteor_manager.SpawnMeteors(5.0f);
        }
    }

}
