using UnityEngine;
using UnityEditor;
using ItsCalls.System;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator map = target as MapGenerator;

        if (GUI.changed)
        {
            map.GenerateMap();
        }
    }
}