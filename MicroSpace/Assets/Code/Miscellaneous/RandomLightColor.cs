using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RandomLightColor : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Light2D>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
}