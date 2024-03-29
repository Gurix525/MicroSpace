﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Shape",
        menuName = "ScriptableObjects/Shape")]
    public class Shape : Model
    {
        #region Fields

        [SerializeField]
        private int _id;

        [SerializeField]
        private GameObject _prefab;

        private static List<Shape> _shapes = new();

        #endregion Fields

        #region Properties

        public int Id => _id;

        public GameObject Prefab => _prefab;

        public Sprite MaskSprite => _prefab.GetComponent<SpriteMask>().sprite;
        public Sprite ButtonSprite => _prefab.GetComponent<SpriteRenderer>().sprite;
        public static List<Shape> Shapes => _shapes;

        #endregion Properties

        #region Public

        public static Shape GetShape(int modelId)
        {
            return _shapes.Find(model => model.Id == modelId);
        }

        public override string ToString()
        {
            return $"{_id} : {name}";
        }

        #endregion Public

        #region Unity

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (_shapes.Find(model => model == this) == null)
                _shapes.Add(this);
            _shapes = _shapes
                .OrderBy(model => model.Id)
                .ToList();
            CheckIfModelFinished();
            CheckForIdDuplicates();
            //Debug.Log($"{string.Join("\n", _shapes)}\n{_shapes.Count}");
        }

        #endregion Unity

        #region Private

        private static void CheckForIdDuplicates()
        {
            var duplicates = _shapes.GroupBy(model => model.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                _shapes.FindAll(model => model.Id == duplicate.Id)
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
                _prefab == null ||
                !_prefab.GetComponent<SpriteMask>() ||
                !_prefab.GetComponent<PolygonCollider2D>();
        }

        #endregion Private
    }
}