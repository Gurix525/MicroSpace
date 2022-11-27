using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    public abstract class Model : ScriptableObject, IModel
    {
        private static List<IModel> Models = new();

        [SerializeField]
        protected int _id;

        public int Id => _id;

        protected static IModel GetModelById(int modelId)
        {
            return Models.Find(model => model.Id == modelId);
        }

#if UNITY_EDITOR

        protected abstract bool IsNotFullyCreated();

        private static void CheckForIdDuplicates()
        {
            bool areDuplicates = false;
            var duplicates = Models.GroupBy(model => model.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                areDuplicates = true;
                Models.FindAll(model => model.Id == duplicate.Id)
                    .ForEach(model => Debug.LogError($"Zduplikowane ID w modelu {model}"));
            });
            if (areDuplicates)
                EditorApplication.isPlaying = false;
        }

        private void CheckIfModelFinished()
        {
            if (IsNotFullyCreated())
                Debug.LogWarning($"Model {this} wymaga dodatkowych informacji");
        }

        private void OnValidate()
        {
            if (Models.Find(model => model == this) == null)
                Models.Add(this);
            Models = Models
                .OrderBy(model => model.Id)
                .ToList();
            CheckIfModelFinished();
            CheckForIdDuplicates();
            Debug.Log($"{string.Join("\n", Models)}\n{Models.Count}");
        }

#endif
    }
}