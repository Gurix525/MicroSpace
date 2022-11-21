using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Zwraca true, jeśli rodzic, głęboki rodzic albo ten obiekt
        /// posiada zadany komponent.
        /// </summary>
        public static bool TryGetComponentUpInHierarchy<T>(
            this Component component,
            out T outComponent)
            where T : Component
        {
            outComponent = GetComponentUpInHierarchy<T>(component);
            return outComponent != null;
        }

        /// <summary>
        /// Zwraca zadany komponent w obiekcie, rodzicu albo głębokim rodzicu.
        /// </summary>
        public static T GetComponentUpInHierarchy<T>(this Component component)
            where T : Component
        {
            if (component.TryGetComponent(out T newComponent))
                return newComponent;
            if (component.transform.parent != null)
                return GetComponentUpInHierarchy<T>(component.transform.parent);
            return null;
        }
    }
}