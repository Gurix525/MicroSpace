using Miscellaneous;
using UnityEngine;

public class World : MonoBehaviour
{
    private void Awake()
    {
        References.SetWorldTransform(transform);
    }
}