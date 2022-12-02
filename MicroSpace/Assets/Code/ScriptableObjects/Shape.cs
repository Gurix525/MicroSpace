using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Shape",
        menuName = "ScriptableObjects/Shape")]
    public class Shape : ScriptableObject
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private GameObject _prefab;

        private static List<Shape> _shapes = new();

        public int Id => _id;

        public GameObject Prefab => _prefab;

        public Sprite Sprite => _prefab.GetComponent<SpriteMask>().sprite;

        public static Shape GetShape(int modelId)
        {
            return _shapes.Find(model => model.Id == modelId);
        }

        public static List<Shape> Shapes => _shapes;

#if UNITY_EDITOR

        private void Awake()
        {
            OnValidate();
        }

        public bool IsNotFullyCreated()
        {
            return name == string.Empty ||
                name == null ||
                _prefab == null ||
                !_prefab.GetComponent<SpriteMask>() ||
                !_prefab.GetComponent<PolygonCollider2D>();
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
            Debug.Log($"{string.Join("\n", _shapes)}\n{_shapes.Count}");
        }

        private static void CheckForIdDuplicates()
        {
            bool areDuplicates = false;
            var duplicates = _shapes.GroupBy(model => model.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                areDuplicates = true;
                _shapes.FindAll(model => model.Id == duplicate.Id)
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

        public override string ToString()
        {
            return $"{_id} : {name}";
        }
    }

#endif
}