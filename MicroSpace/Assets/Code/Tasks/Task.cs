using System.Collections.Generic;
using System.Linq;
using Miscellaneous;

namespace Tasks
{
    public class Task : IIdentifiable
    {
        #region Properties

        public int Id { get; }
        public Dictionary<int, float> Items { get; } = new();
        public int ToolType { get; }
        public float ToolPower { get; }
        public int SkillModel { get; }
        public float SkillPower { get; }

        public static List<Task> Tasks { get; } = new();

        #endregion Properties

        #region Constructors

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

        #endregion Constructors

        #region Public

        public override string ToString()
        {
            return $"Task nr {Id}";
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

        #endregion Public
    }
}