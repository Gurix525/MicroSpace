using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetComponentTest : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < 1000000; i++)
        {
            GetComponent<Transform>();
        }
    }
}