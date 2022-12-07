using System.Collections.Generic;
using UnityEngine;
using Attributes;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace ScriptableObjects
{
    [CreateAssetMenu(
            fileName = "ModelsArray",
            menuName = "ScriptableObjects/ModelsArray")]
    public class ModelsArray : ScriptableObject
    {
        [SerializeField, ReadonlyInspector]
        private ScriptableObject[] _scriptableObjects;

#if UNITY_EDITOR

        private void OnValidate()
        {
            _scriptableObjects = GetAllScriptableObjects();
        }

        private static ScriptableObject[] GetAllScriptableObjects()
        {
            string[] guids = AssetDatabase
                .FindAssets("t:" + typeof(ScriptableObject).Name);
            var scriptableObjects = new ScriptableObject[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                scriptableObjects[i] = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }
            return scriptableObjects;
        }

#endif
    }
}