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
            _tasks.Add(task);
        }

        #endregion Public

        #region Unity

        private void OnEnable()
        {
            EnableTasks();
            _tasks.CollectionChanged += TasksCollectionChanged;
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

        private void TasksCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (gameObject.activeInHierarchy)
                EnableTasks();
        }

        #endregion Private
    }
}