using Attributes;
using Entities;
using ExtensionMethods;
using Miscellaneous;
using System;
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
            if (IdleAstronauts.Length > 0)
                UpdateFreeTasksList();
            if (FreeTasks.Count > 0)
            {
                for (int i = 0; i < FreeTasks.Count; i++)
                {
                    var task = FreeTasks[i];
                    if (IdleAstronauts.Length == 0)
                        return;
                    var astronaut = IdleAstronauts.ToDictionary(
                        astronaut => astronaut,
                        astronaut =>
                    {
                        PathProvider.TryGetPath(
                            astronaut.transform.position,
                            task.Target.position,
                            out List<Vector3> path,
                            task.Target);
                        return path.ToArray().GetPathLength();
                    })
                        .Where(astronaut => astronaut.Value > 0F
                        && AreRequiredItemsAccessible(astronaut.Key, task))
                        .OrderBy(astronaut => astronaut.Value)
                        .FirstOrDefault()
                        .Key;
                    if (astronaut != null)
                    {
                        AssignAstronautToTask(astronaut, task);
                        i--;
                    }
                }
            }
        }

        private void AssignAstronautToTask(Astronaut astronaut, Task task)
        {
            astronaut.GetComponent<TaskExecutor>().AssignTask(task);
            FreeTasks.Remove(task);
            var idleAstronauts = IdleAstronauts.ToList();
            idleAstronauts.Remove(astronaut);
            IdleAstronauts = idleAstronauts.ToArray();
        }

        private bool AreRequiredItemsAccessible(Astronaut astronaut, Task task)
        {
            if (ItemFinder.AreItemsAvailable(task.Items.ToArray()))
                if (ItemFinder.AreItemsAccessible(astronaut, task.Items.ToArray()))
                    return true;
            return false;
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
                    && ItemFinder.AreItemsAvailable(task.Items.ToArray()))
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