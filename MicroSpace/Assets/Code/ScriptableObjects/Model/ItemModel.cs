using System.Collections.Generic;
using System.Linq;
using Miscellaneous;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "ItemModel",
        menuName = "ScriptableObjects/ItemModel")]
    public class ItemModel : Model
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private ToolType _toolType;

        public int Id => _id;

        public ToolType ToolType => _toolType;

        private static List<ItemModel> _models = new();

        public static ItemModel GetModel(int modelId)
        {
            return _models.Find(model => model.Id == modelId);
        }

        public static List<ItemModel> Models => _models;

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
        }

        private static void CheckForIdDuplicates()
        {
            var duplicates = _models.GroupBy(model => model.Id)
                .SelectMany(g => g.Skip(1))
                .ToList();
            duplicates.ForEach(duplicate =>
            {
                _models.FindAll(model => model.Id == duplicate.Id)
                .ForEach(model => Debug.LogError($"Zduplikowane ID w przedmiocie {model}"));
            });
        }

        private void CheckIfModelFinished()
        {
            if (IsNotFullyCreated())
                Debug.LogWarning($"Przedmiot {this} wymaga dodatkowych informacji");
        }

        private bool IsNotFullyCreated()
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