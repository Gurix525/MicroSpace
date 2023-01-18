using Entities;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(SatelliteGenerator))]
public class SatelliteGeneratorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate", GUILayout.Height(24)))
            (target as SatelliteGenerator).Generate();
        base.OnInspectorGUI();
    }
}

#endif