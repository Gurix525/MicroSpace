using Ships;
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
            GameManager.ForEachShip(
                ship => AddGravitationalForce(ship));
        }

        private void AddGravitationalForce(Ship ship)
        {
            ship.Rigidbody2D.AddForce(CalculateForce(ship.Rigidbody2D));
        }

        private Vector2 CalculateForce(Rigidbody2D ship)
        {
            var distance = Vector2.Distance(
                ship.transform.position, transform.position);
            if (distance == 0)
                return Vector2.zero;
            else
                return (Vector2)(transform.position - ship.transform.position).normalized *
                _gravityConstant * ship.mass / (distance * distance);
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