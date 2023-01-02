using System.Collections.Specialized;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Tasks
{
    public class TaskSource : MonoBehaviour
    {
        #region Fields

        private ObservableCollection<Task> _tasks = new();

        #endregion Fields

        #region Public

        public void AddTask(Task task)
        {
            task.TaskExecuted.AddListener(OnTaskExecuted);
            _tasks.Add(task);
        }

        #endregion Public

        #region Unity

        private void OnEnable()
        {
            EnableTasks();
            _tasks.CollectionChanged += OnTasksCollectionChanged;
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

        private void OnTasksCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (gameObject.activeInHierarchy)
                EnableTasks();
        }

        private void OnTaskExecuted(Task task)
        {
            Task.RemoveTask(task);
            _tasks.Remove(task);
        }

        #endregion Private
    }
}