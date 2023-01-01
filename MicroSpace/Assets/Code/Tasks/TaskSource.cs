using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Tasks
{
    public class TaskSource : MonoBehaviour
    {
        private ObservableCollection<Task> _tasks = new();

        public ObservableCollection<Task> Tasks => _tasks;

        #region Unity

        private void OnEnable()
        {
            EnableTasks();
            Tasks.CollectionChanged += TasksCollectionChanged;
        }

        private void OnDisable()
        {
            DisableTasks();
        }

        #endregion Unity

        #region Private

        private void EnableTasks()
        {
            foreach (var task in _tasks)
                Task.AddTask(task);
        }

        private void DisableTasks()
        {
            foreach (var task in _tasks)
                Task.RemoveTask(task);
        }

        private void TasksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (gameObject.activeInHierarchy)
                EnableTasks();
        }

        #endregion Private
    }
}