using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
            fileName = "GasModel",
            menuName = "ScriptableObjects/GasModel")]
    public class GasModel : ScriptableObject
    {
        [SerializeField]
        private int _id;

        public int Id => _id;

        private static List<GasModel> GasModels = new();

        public static GasModel GetModel(int modelId)
        {
            return GasModels.Find(block => block.Id == modelId);
        }

        public override string ToString()
        {
            return $"{_id} : {name}";
        }

        public bool IsNotFullyCreated()
        {
            return name == string.Empty ||
                name == null;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (GasModels.Find(model => model == this) == null)
                GasModels.Add(this);
            GasModels = GasModels
                .OrderBy(model => model.Id)
                .ToList();
            CheckIfModelFinished();
            CheckForIdDuplicates();
            Debug.Log($"{string.Join("\n", GasModels)}\n{GasModels.Count}");
        }

        private static void CheckForIdDuplicates()
        {
            bool areDuplicates = false;
            var duplicates = GasModels.GroupBy(model => model.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                areDuplicates = true;
                GasModels.FindAll(model => model.Id == duplicate.Id)
                    .ForEach(model => Debug.LogError($"Zduplikowane ID w gazie {model}"));
            });
            if (areDuplicates)
                EditorApplication.isPlaying = false;
        }

        private void CheckIfModelFinished()
        {
            if (IsNotFullyCreated())
                Debug.LogWarning($"Blok {this} wymaga dodatkowych informacji");
        }

#endif
    }
}