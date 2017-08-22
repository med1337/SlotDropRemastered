using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnAreaCircle))]
public class SpawnAreaCircleEditor : Editor
{
    private SpawnAreaCircle circle;

    public void OnSceneGUI()
    {
        circle = this.target as SpawnAreaCircle;
        Handles.color = Color.red;
        Handles.DrawWireDisc(circle.transform.position, circle.transform.up, circle.spawn_radius);
    }

}
