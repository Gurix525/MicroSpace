using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "BlockList",
        menuName = "ScriptableObjects/BlockList")]
    public class BlockListScriptableObject : ScriptableObject
    {
        [SerializeField]
        private List<BlockScriptableObject> _blocks = new();

        public IReadOnlyList<BlockScriptableObject> Blocks => _blocks;

        private static List<BlockScriptableObject> GetAllBlocks()
        {
            string[] guids = AssetDatabase
                .FindAssets("t:" + typeof(BlockScriptableObject).Name);
            var blocks = new BlockScriptableObject[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                blocks[i] = AssetDatabase.LoadAssetAtPath<BlockScriptableObject>(path);
            }
            return blocks.OrderBy(block => block.Id).ToList();
        }

        private void OnValidate()
        {
            _blocks = GetAllBlocks();
            CheckForUnfinishedBlocks();
            CheckForIdDuplicates();
        }

        private void CheckForIdDuplicates()
        {
            bool areDuplicates = false;
            var duplicates = _blocks.GroupBy(block => block.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                areDuplicates = true;
                _blocks.FindAll(block => block.Id == duplicate.Id)
                    .ForEach(block => Debug.LogError($"Zduplikowane ID w bloku {block}"));
            });
            if (areDuplicates)
                EditorApplication.isPlaying = false;
        }

        private void CheckForUnfinishedBlocks()
        {
            _blocks.ForEach(block =>
            {
                if (block.IsNotFullyCreated())
                    Debug.LogWarning($"Blok {block} wymaga dodatkowych informacji");
            });
        }
    }
}