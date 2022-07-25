using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Data
{
    [Serializable]
    public abstract class PartData
    {
        public float CurrentEndurance;
        public int[] LocalPosition = new int[2];
        public float LocalRotation;
        public float MaxEndurance;
        public string Name;
        public float Resilience;
    }
}