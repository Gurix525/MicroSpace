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
    }
}