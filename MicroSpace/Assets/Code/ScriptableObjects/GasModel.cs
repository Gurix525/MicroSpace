using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
            fileName = "GasModel",
            menuName = "ScriptableObjects/GasModel")]
    public class GasModel : Model
    {
        public override string ToString()
        {
            return $"{_id} : {name}";
        }

        public static GasModel GetModel(int modelId)
        {
            return (GasModel)GetModelById(modelId);
        }

        protected override bool IsNotFullyCreated()
        {
            return name == string.Empty ||
                name == null;
        }
    }
}