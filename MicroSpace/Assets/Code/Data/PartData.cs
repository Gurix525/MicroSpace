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
        public float[] LocalPosition = new float[2];
        public float LocalRotation;
        public float MaxEndurance;
        public string Name;
        public float Resilience;
    }
}