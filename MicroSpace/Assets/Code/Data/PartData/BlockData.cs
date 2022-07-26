using Assets.Code.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data.PartDataImplementations
{
    [Serializable]
    public class BlockData : PartData
    {
        public static implicit operator BlockData(Block block)
        {
            var blockData = block.BlockData ?? new();
            //blockData.Name = block.Name;
            //blockData.Resilience = block.Resilience;
            //blockData.MaxEndurance = block.MaxEndurance;
            //blockData.CurrentEndurance = block.CurrentEndurance;
            blockData.LocalPosition = new int[]
            {
                (int)Math.Round( block.gameObject.transform.localPosition.x),
                (int)Math.Round( block.gameObject.transform.localPosition.y)
            };
            blockData.LocalRotation = block.gameObject.transform.localEulerAngles.z;
            block.BlockData = blockData;
            return blockData;
        }
    }
}