using Attributes;
using Entities;
using Miscellaneous;
using System.Collections.Generic;
using System.Linq;
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
            Task.Tasks.CollectionChanged += OnTasksCollectionChanged;
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
                    AssignAstronautToTask(astronaut);
                }
        }

        private void AssignAstronautToTask(Astronaut astronaut)
        {
            for (int i = 0; i < FreeTasks.Count; i++)
            {
                if (IsPathToTaskValid(astronaut, i))
                {
                    astronaut.GetComponent<TaskExecutor>().AssignTask(FreeTasks[i]);
                    FreeTasks.RemoveAt(i);
                    return;
                }
            }
        }

        private bool IsPathToTaskValid(Astronaut astronaut, int i)
        {
            return PathProvider.TryGetPath(
                                astronaut.transform.position,
                                FreeTasks[i].Target.position,
                                out _,
                                FreeTasks[i].Target);
        }

        private void UpdateFreeTasksList()
        {
            FreeTasks = _tasks
                .Where(task => task.AssignedAstronautId == 0
                    && ItemFinder.AreRequiredItemsAvailable(task.Items.ToArray()))
                .ToList();
        }

        private void UpdateIdleAstronautsList()
        {
            IdleAstronauts = Astronaut.Astronauts
                .Where(astronaut => astronaut.AstronautState == AstronautState.Idle)
                .ToArray();
        }

        private void OnTasksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _tasks = Task.Tasks.ToArray();
        }

        #endregion Private
    }
}