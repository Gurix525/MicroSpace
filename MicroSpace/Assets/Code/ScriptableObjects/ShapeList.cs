using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "ShapeList",
        menuName = "ScriptableObjects/ShapeList")]
    public class ShapeList : ScriptableObject
    {
        [SerializeField]
        private List<Shape> _shapes;

        public IReadOnlyList<Shape> Shapes => _shapes;

        public Shape GetShape(int shapeId)
        {
            return _shapes.Find(shape => shape.Id == shapeId);
        }

        private static List<Shape> GetAllShapes()
        {
            string[] guids = AssetDatabase
                .FindAssets("t:" + typeof(Shape).Name);
            var shapes = new Shape[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                shapes[i] = AssetDatabase.LoadAssetAtPath<Shape>(path);
            }
            return shapes.OrderBy(shape => shape.Id).ToList();
        }

        private void OnValidate()
        {
            _shapes = GetAllShapes();
            CheckForUnfinishedShapes();
            CheckForIdDuplicates();
        }

        private void CheckForIdDuplicates()
        {
            bool areDuplicates = false;
            var duplicates = _shapes.GroupBy(shape => shape.Id)
                            .SelectMany(group => group.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                areDuplicates = true;
                _shapes.FindAll(shape => shape.Id == duplicate.Id)
                    .ForEach(shape => Debug.LogError($"Zduplikowane ID w kształcie {shape}"));
            });
            if (areDuplicates)
                EditorApplication.isPlaying = false;
        }

        private void CheckForUnfinishedShapes()
        {
            _shapes.ForEach(shape =>
            {
                if (shape.IsNotFullyCreated())
                    Debug.LogWarning($"Kształt {shape} wymaga dodatkowych informacji");
            });
        }
    }
}