using UnityEngine;
using Attributes;

namespace ScriptableObjects
{
    public static class IdManager
    {
        private static int _nextId = 1;

        public static int NextId
        {
            get => _nextId++;
            set => _nextId = value;
        }
    }
}