using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miscellaneous;

namespace Tasks
{
    public class Task : IIdentifiable
    {
        public int Id { get; }
        public Dictionary<int, int> Items { get; } = new();
        public int ToolModel { get; }
        public float ToolPower { get; }
        public int SkillModel { get; }
        public float SkillPower { get; }
    }
}
