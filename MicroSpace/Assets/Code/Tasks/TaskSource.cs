using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public void RemoveTask(Task task)
        {
            task.TaskExecuted.RemoveListener(OnTaskExecuted);
            _tasks.Remove(task);
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
            if (e.NewItems != null)
                foreach (Task task in e.NewItems)
                    Task.AddTask(task);
            if (e.OldItems != null)
                foreach (Task task in e.OldItems)
                    Task.RemoveTask(task);
        }

        private void OnTaskExecuted(Task task)
        {
            _tasks.Remove(task);
        }

        #endregion Private
    }
}