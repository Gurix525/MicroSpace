using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "BlockList",
        menuName = "ScriptableObjects/BlockList")]
    public class BlockModelList : ScriptableObject
    {
        [SerializeField]
        private List<BlockModel> _models = new();

        public IReadOnlyList<BlockModel> Models => _models;

        public BlockModel GetModel(int modelId)
        {
            return _models.Find(block => block.Id == modelId);
        }

#if UNITY_EDITOR

        private static List<BlockModel> GetAllModels()
        {
            string[] guids = AssetDatabase
                .FindAssets("t:" + typeof(BlockModel).Name);
            var blocks = new BlockModel[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                blocks[i] = AssetDatabase.LoadAssetAtPath<BlockModel>(path);
            }
            return blocks.OrderBy(block => block.Id).ToList();
        }

        private void OnValidate()
        {
            _models = GetAllModels();
            CheckForUnfinishedModels();
            CheckForIdDuplicates();
        }

        private void CheckForIdDuplicates()
        {
            bool areDuplicates = false;
            var duplicates = _models.GroupBy(model => model.Id)
                            .SelectMany(g => g.Skip(1))
                            .ToList();
            duplicates.ForEach(duplicate =>
            {
                areDuplicates = true;
                _models.FindAll(model => model.Id == duplicate.Id)
                    .ForEach(model => Debug.LogError($"Zduplikowane ID w bloku {model}"));
            });
            if (areDuplicates)
                EditorApplication.isPlaying = false;
        }

        private void CheckForUnfinishedModels()
        {
            _models.ForEach(model =>
            {
                if (model.IsNotFullyCreated())
                    Debug.LogWarning($"Blok {model} wymaga dodatkowych informacji");
            });
        }

#endif
    }
}