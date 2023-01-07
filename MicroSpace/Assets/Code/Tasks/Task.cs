using Attributes;
using Miscellaneous;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using UnityEngine.Events;
using UnityEngine;

namespace Tasks
{
    [Serializable]
    public class Task : IIdentifiable
    {
        #region Properties

        [SerializeField]
        [ReadonlyInspector]
        private string _name;

        public int Id { get; }
        public int AssignedAstronautId { get; private set; }
        public Transform Target { get; }
        public int TargetId { get; }
        public Dictionary<int, float> Items { get; } = new();
        public int ToolType { get; }
        public float ToolPower { get; }
        public int SkillModel { get; }
        public float SkillPower { get; }
        public float RequiredTime { get; }
        public float ElapsedTime { get; set; } = 0f;
        public Action Action { get; }
        public UnityEvent<Task> Executing { get; } = new();
        public UnityEvent<Task> Executed { get; } = new();

        public static ObservableCollection<Task> Tasks { get; } = new();

        #endregion Properties

        #region Constructors

        public Task(Transform target, int targetId, int[] itemModels, float[] itemAmounts, int toolModel, float toolPower, int skillModel, float skillPower, float time, Action action)
        {
            Id = IdManager.NextId;
            Target = target;
            TargetId = targetId;
            Items = itemModels.Zip(itemAmounts, (model, amount) => new { model, amount })
                .ToDictionary(item => item.model, item => item.amount);
            ToolType = toolModel;
            ToolPower = toolPower;
            SkillModel = skillModel;
            SkillPower = skillPower;
            _name = ToString();
            RequiredTime = time;
            Action = action;
        }

        #endregion Constructors

        #region Public

        public override string ToString()
        {
            return $"Task nr {Id}";
        }

        public void AssignAstronaut(int id)
        {
            AssignedAstronautId = id;
        }

        public void UnassignAstronaut()
        {
            AssignedAstronautId = 0;
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