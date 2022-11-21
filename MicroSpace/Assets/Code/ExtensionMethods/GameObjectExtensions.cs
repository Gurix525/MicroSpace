using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtensionMethods
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Zwraca true, jeśli rodzic, głęboki rodzic albo ten obiekt
        /// posiada zadany komponent.
        /// </summary>
        public static bool TryGetComponentUpInHierarchy<T>(
            this GameObject gameObject,
            out T outComponent)
            where T : Component
        {
            outComponent = GetComponentUpInHierarchy<T>(gameObject);
            return outComponent != null;
        }

        /// <summary>
        /// Zwraca zadany komponent w obiekcie, rodzicu albo głębokim rodzicu.
        /// </summary>
        public static T GetComponentUpInHierarchy<T>(this GameObject gameObject)
            where T : Component
        {
            if (gameObject.TryGetComponent(out T newComponent))
                return newComponent;
            if (gameObject.transform.parent != null)
                return GetComponentUpInHierarchy<T>(
                    gameObject.transform.parent.gameObject);
            return null;
        }
    }
}