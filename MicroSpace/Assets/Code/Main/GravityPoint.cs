using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class GravityPoint : MonoBehaviour
    {
        #region Fields

        private static readonly float _gravityConstant = 400000F;

        #endregion Fields

        #region Private

        private void ApplyGravity()
        {
            GameManager.ForEachSatellite(
                satellite => AddGravitationalForce(satellite));
        }

        private void AddGravitationalForce(Satellite satellite)
        {
            satellite.Rigidbody2D.AddForce(CalculateForce(satellite.Rigidbody2D));
        }

        private Vector2 CalculateForce(Rigidbody2D satellite)
        {
            var distance = Vector2.Distance(
                satellite.transform.position, transform.position);
            if (distance == 0)
                return Vector2.zero;
            else
                return (Vector2)(transform.position - satellite.transform.position).normalized *
                _gravityConstant * satellite.mass / (distance * distance);
        }

        #endregion Private

        #region Unity

        private void FixedUpdate()
        {
            ApplyGravity();
        }

        #endregion Unity
    }
}