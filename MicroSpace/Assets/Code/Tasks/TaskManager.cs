using System;
using System.Collections.Generic;
using System.Linq;
using Attributes;
using Entities;
using UnityEngine;

namespace Tasks
{
    public class TaskManager : MonoBehaviour
    {
        [SerializeField]
        [ReadonlyInspector]
        private Task[] _tasks;

        private Astronaut[] IdleAstronauts = new Astronaut[0];
        private List<Task> FreeTasks = new();

        #region Unity

        private void Start()
        {
            Task.Tasks.CollectionChanged += TasksCollectionChanged;
        }

        private void FixedUpdate()
        {
            UpdateIdleAstronautsList();
            AssignAstronautsToTasks();
        }

        #endregion Unity

        #region Private

        private void AssignAstronautsToTasks()
        {
            if (IdleAstronauts.Count() > 0)
                UpdateFreeTasksList();
            foreach (var astronaut in IdleAstronauts)
                if (FreeTasks.Count > 0)
                {
                    FreeTasks[0].AssignedAstronautId = astronaut.Id;
                    astronaut.AstronautState = AstronautState.Working;
                    astronaut.GetComponent<TaskExecutor>().AssignTask(FreeTasks[0]);
                    FreeTasks.RemoveAt(0);
                }
        }

        private void UpdateFreeTasksList()
        {
            FreeTasks = _tasks
                .Where(task => task.AssignedAstronautId == 0)
                .ToList();
        }

        private void UpdateIdleAstronautsList()
        {
            IdleAstronauts = Astronaut.Astronauts
                .Where(astronaut => astronaut.AstronautState == AstronautState.Idle)
                .ToArray();
        }

        private void TasksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _tasks = Task.Tasks.ToArray();
        }

        #endregion Private
    }
}