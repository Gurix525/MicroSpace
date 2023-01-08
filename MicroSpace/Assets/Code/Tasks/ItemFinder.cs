using System.Collections.Generic;
using System.Linq;
using Entities;
using UnityEngine;

namespace Tasks
{
    public static class ItemFinder
    {
        public static Transform FindClosestItem(int modelId, Vector3 startPosition)
        {
            return Item.EnabledItems
                .Where(item => item.ModelId == modelId)
                .OrderBy(item => Vector2.Distance(startPosition, item.transform.position))
                .FirstOrDefault()
                .transform;
        }

        public static bool AreRequiredItemsAvailable(KeyValuePair<int, float>[] items)
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
            if (Item.EnabledMassItems.Where(item => item.ModelId == modelId)
                .Aggregate(0F, (sum, item) => sum += item.Mass) >= mass)
                return true;
            return false;
        }
    }
}