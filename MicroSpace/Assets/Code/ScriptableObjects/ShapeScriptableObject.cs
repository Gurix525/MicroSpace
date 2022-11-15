using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Shape",
        menuName = "ScriptableObjects/Shape")]
    public class ShapeScriptableObject : ScriptableObject
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private GameObject _prefab;

        public int Id => _id;

        public GameObject Prefab => _prefab;

        public bool IsNotFullyCreated()
        {
            return name == string.Empty ||
                name == null ||
                _prefab == null ||
                !_prefab.GetComponent<SpriteMask>() ||
                !_prefab.GetComponent<PolygonCollider2D>();
        }
    }
}