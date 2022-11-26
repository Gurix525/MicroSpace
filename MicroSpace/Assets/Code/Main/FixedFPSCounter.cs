using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixedFPSCounter : MonoBehaviour
{
    private TextMeshProUGUI _counter;
    private float _timer = 0;
    private float _frames = 0;

    private TextMeshProUGUI Counter =>
        _counter ??= GetComponent<TextMeshProUGUI>();

    private void Update()
    {
        if (_timer >= 1F)
        {
            Counter.text = $"FFPS: {System.Math.Round(1 / (_timer / _frames))}";
            _timer = 0;
            _frames = 0;
        }
        _timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        _frames++;
    }
}