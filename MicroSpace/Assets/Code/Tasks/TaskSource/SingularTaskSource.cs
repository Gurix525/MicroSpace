using System;

namespace Tasks
{
    public class SingularTaskSource : TaskSource
    {
        private TaskProvider _taskProvider;
        private Task _task;

        public Task Task => _task;

        #region Unity

        private void Awake()
        {
            _taskProvider = GetComponent<TaskProvider>();
            _task = _taskProvider.GetTask();
        }

        private void OnEnable()
        {
            EnableTask();
        }

        private void OnDisable()
        {
            DisableTask();
        }

        #endregion Unity

        #region Private

        private void EnableTask()
        {
            Task.AddTask(_task);
        }

        private void DisableTask()
        {
            Task.RemoveTask(_task);
        }

        #endregion Private
    }
}