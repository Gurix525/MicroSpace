using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "BlockModel",
        menuName = "ScriptableObjects/BlockModel")]
    public class BlockModel : Model
    {
        #region Fields

        [SerializeField]
        private int _id;

        [SerializeField]
        private Sprite _sprite;

        [Header("Przedmioty wymagane do wykonania polecenia")]
        [SerializeField]
        private ItemModel[] _itemModels;

        [Header("Ilości tych przedmiotów")]
        [SerializeField]
        private float[] _itemAmounts;

        private static List<BlockModel> _models = new();

        #endregion Fields

        #region Properties

        public int Id => _id;

        public Sprite Sprite => _sprite;

        public ItemModel[] ItemModels => _itemModels;

        public float[] ItemAmounts => _itemAmounts;

        public static List<BlockModel> Models => _models;

        #endregion Properties

        #region Public

        public static BlockModel GetModel(int modelId)
        {
            return _models.Find(model => model.Id == modelId);
        }

        public override string ToString()
        {
            return $"{_id} : {name} : {_sprite.name}";
        }

        #endregion Public

        #region Unity

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
            CheckItemArraysLength();
            //Debug.Log($"{string.Join("\n", _models)}\n{_models.Count}");
        }

        #endregion Unity

        #region Private

        private void CheckItemArraysLength()
        {
            if (_itemModels.Length != _itemAmounts.Length)
                Debug.LogError("ItemModels and ItemAmounts muszą posiadać tyle" +
                    "samo elementów");
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

        private bool IsNotFullyCreated()
        {
            return name == string.Empty ||
                name == null ||
                _sprite == null ||
                _sprite.name == "BlockDefault";
        }

        #endregion Private
    }
}