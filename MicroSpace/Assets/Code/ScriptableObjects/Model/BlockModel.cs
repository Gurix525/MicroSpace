using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "BlockModel",
        menuName = "ScriptableObjects/BlockModel")]
    public class BlockModel : Model
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private Sprite _sprite;

        private static List<BlockModel> _models = new();

        public int Id => _id;

        public Sprite Sprite => _sprite;

        public static BlockModel GetModel(int modelId)
        {
            return _models.Find(model => model.Id == modelId);
        }

        public static List<BlockModel> Models => _models;

#if UNITY_EDITOR

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
            bool areDuplicates = false;
            var duplicates = _models.GroupBy(model => model.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                areDuplicates = true;
                _models.FindAll(model => model.Id == duplicate.Id)
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

        public bool IsNotFullyCreated()
        {
            return name == string.Empty ||
                name == null ||
                _sprite == null ||
                _sprite.name == "BlockDefault";
        }

        public override string ToString()
        {
            return $"{_id} : {name} : {_sprite.name}";
        }

#endif
    }
}