#if UNITY_EDITOR

using UnityEditor;
using Attributes;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ReadonlyInspectorAttribute))]
    public class ReadonlyInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}

#endif