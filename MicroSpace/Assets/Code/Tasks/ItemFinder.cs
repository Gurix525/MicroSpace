using System.Collections.Generic;
using System.Linq;
using Entities;
using Miscellaneous;
using UnityEngine;
using ExtensionMethods;
using System;

namespace Tasks
{
    public static class ItemFinder
    {
        public static Transform FindClosestItem(int modelId, Vector3 startPosition)
        {
            return Item.EnabledItems
                .Where(item => item.ModelId == modelId)
                .ToDictionary(item => item, item =>
                {
                    PathProvider.TryGetPath(
                        startPosition,
                        item.transform.position,
                        out List<Vector3> path,
                        item.transform);
                    return path;
                })
                .Where(item => item.Value != null)
                .OrderBy(item => item.Value.ToArray().GetPathLength())
                .FirstOrDefault()
                .Key.transform;
        }

        public static bool AreItemsAvailable(KeyValuePair<int, float>[] items)
        {
            foreach (var item in items)
            {
                if (!IsItemAvailable(item.Key, item.Value))
                    return false;
            }
            return true;
        }

        public static bool IsItemAvailable(int modelId, float mass)
        {
            if (Item.EnabledMassItems
                .Where(item => item.ModelId == modelId)
                .Aggregate(0F, (sum, item) => sum += item.Mass) >= mass)
                return true;
            return false;
        }

        public static bool AreItemsAccessible(Astronaut astronaut, KeyValuePair<int, float>[] items)
        {
            foreach (var item in items)
            {
                if (!IsItemAccessible(astronaut, item.Key, item.Value))
                    return false;
            }
            return true;
        }

        public static bool IsItemAccessible(Astronaut astronaut, int modelId, float mass)
        {
            return Item.EnabledMassItems
                .Where(item => item.ModelId == modelId)
                .Where(item => PathProvider.TryGetPath(
                    astronaut.transform.position,
                    item.transform.position,
                    out _,
                    item.transform))
                .Aggregate(0F, (sum, item) => sum += item.Mass)
                >= mass;
        }
    }
}