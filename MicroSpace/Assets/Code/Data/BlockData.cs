﻿using Assets.Code.Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data
{
    [Serializable]
    public class BlockData
    {
        public float CurrentEndurance;
        public float[] LocalPosition = new float[2];
        public float LocalRotation;
        public float MaxEndurance;
        public string Name;
        public float Resilience;

        public static implicit operator BlockData(Block block)
        {
            BlockData blockData = new();
            blockData.Name = block.Name;
            blockData.Resilience = block.Resilience;
            blockData.MaxEndurance = block.MaxEndurance;
            blockData.CurrentEndurance = block.CurrentEndurance;
            blockData.LocalPosition = new float[]
            {
                block.gameObject.transform.localPosition.x,
                block.gameObject.transform.localPosition.y
            };
            blockData.LocalRotation = block.gameObject.transform.localEulerAngles.z;

            return blockData;
        }
    }
}