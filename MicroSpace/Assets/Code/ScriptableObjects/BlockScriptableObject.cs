using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Block",
        menuName = "ScriptableObjects/Block")]
    public class BlockScriptableObject : ScriptableObject
    {
        [SerializeField]
        private int _id;

        [SerializeField]
        private Sprite _sprite;

        public int Id => _id;

        public Sprite Sprite => _sprite;

        public override string ToString()
        {
            return $"{_id} : {name} : {_sprite.name}";
        }

        public bool IsNotFullyCreated()
        {
            return _id == 0 ||
                name == string.Empty ||
                name == null ||
                _sprite == null ||
                _sprite.name == "BlockDefault";
        }
    }
}