using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPoint : MonoBehaviour
{
    private static readonly float _gravityConstant = 400000F;

    private void FixedUpdate()
    {
        foreach (Transform child in transform.parent)
        {
            var rigidbody = child.GetComponent<Rigidbody2D>();
            if (child != transform && rigidbody != null)
            {
                rigidbody.AddForce(CalculateForce(child));
            }
        }
    }

    private Vector2 CalculateForce(Transform child)
    {
        var distance = Vector2.Distance(child.position, transform.position);
        return (transform.position - child.position).normalized *
            _gravityConstant / (distance * distance);
    }
}