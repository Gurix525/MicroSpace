using System.Collections.Generic;
using System.Linq;
using Miscellaneous;

namespace Tasks
{
    public class Task : IIdentifiable
    {
        public int Id { get; }
        public Dictionary<int, float> Items { get; } = new();
        public int ToolType { get; }
        public float ToolPower { get; }
        public int SkillModel { get; }
        public float SkillPower { get; }

        public static List<Task> Tasks { get; } = new();

        public Task(int[] itemModels, float[] itemAmounts, int toolModel, float toolPower, int skillModel, float skillPower)
        {
            Id = IdManager.NextId;
            Items = itemModels.Zip(itemAmounts, (model, amount) => new { model, amount })
                .ToDictionary(item => item.model, item => item.amount);
            ToolType = toolModel;
            ToolPower = toolPower;
            SkillModel = skillModel;
            SkillPower = skillPower;
        }

        public static void AddTask(Task task)
        {
            if (!Tasks.Contains(task))
                Tasks.Add(task);
        }

        public static void RemoveTask(Task task)
        {
            if (Tasks.Contains(task))
                Tasks.Remove(task);
        }
    }
}