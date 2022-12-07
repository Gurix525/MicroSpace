using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
            fileName = "GasModel",
            menuName = "ScriptableObjects/GasModel")]
    public class GasModel : Model
    {
        [SerializeField]
        private int _id;

        public int Id => _id;

        private static List<GasModel> _models = new();

        public static GasModel GetModel(int modelId)
        {
            return _models.Find(model => model.Id == modelId);
        }

        public static List<GasModel> Models => _models;

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (_models.Find(model => model == this) == null)
                _models.Add(this);
            _models = _models
                .OrderBy(model => model.Id)
                .ToList();
            CheckIfModelFinished();
            CheckForIdDuplicates();
            Debug.Log($"{string.Join("\n", _models)}\n{_models.Count}");
        }

        private static void CheckForIdDuplicates()
        {
            var duplicates = _models.GroupBy(model => model.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                _models.FindAll(model => model.Id == duplicate.Id)
                    .ForEach(model => Debug.LogError($"Zduplikowane ID w gazie {model}"));
            });
        }

        private void CheckIfModelFinished()
        {
            if (IsNotFullyCreated())
                Debug.LogWarning($"Blok {this} wymaga dodatkowych informacji");
        }

        public bool IsNotFullyCreated()
        {
            return name == string.Empty ||
                name == null;
        }

        public override string ToString()
        {
            return $"{_id} : {name}";
        }
    }
}