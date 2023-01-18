using UnityEngine;

namespace Miscellaneous
{
    public class Pointers : MonoBehaviour
    {
        private void Start()
        {
            References.Pointers = (RectTransform)transform;
        }
    }
}