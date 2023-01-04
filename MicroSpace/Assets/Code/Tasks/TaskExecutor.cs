using Entities;
using System.Collections.Specialized;
using UnityEngine;

namespace Tasks
{
    public class TaskExecutor : MonoBehaviour
    {
        private Task _assignedTask;

        private Astronaut _astronaut;

        private Astronaut Astronaut =>
            _astronaut ??= GetComponent<Astronaut>();

        public Task AssignedTask => _assignedTask;

        public void AssignTask(Task task)
        {
            _assignedTask = task;
            task.AssignAstronaut(Astronaut.Id);
            Astronaut.AstronautState = AstronautState.Working;
        }

        public void UnassignTask()
        {
            Astronaut.AstronautState = AstronautState.Idle;
            if (_assignedTask != null)
            {
                _assignedTask.UnassignAstronaut();
                _assignedTask = null;
            }
        }

        private void Start()
        {
            Task.Tasks.CollectionChanged += OnTasksCollectionChanged;
        }

        private void FixedUpdate()
        {
            if (_assignedTask != null)
                if (IsTargetInRange())
                    ExecuteTask();
        }

        private bool IsTargetInRange()
        {
            if (_assignedTask != null)
                if (_assignedTask.Target != null)
                    return Vector2.Distance(
                        transform.position,
                        _assignedTask.Target.position)
                        <= 1.6F;
            return false;
        }

        private void ExecuteTask()
        {
            if (_assignedTask.ElapsedTime < _assignedTask.RequiredTime)
                _assignedTask.ElapsedTime += Time.fixedDeltaTime;
            else
            {
                Task executedTask = _assignedTask;
                executedTask.Action();
                executedTask.TaskExecuted.Invoke(executedTask);
                UnassignTask();
            }
        }

        private void OnTasksCollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && _assignedTask != null)
                if (e.OldItems.Contains(_assignedTask))
                    UnassignTask();
        }
    }
}