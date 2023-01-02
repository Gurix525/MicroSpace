using System;
using Entities;
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

        private bool IsTargetInRange =>
            Vector2.Distance(transform.position, _assignedTask.Target.position) <= 1.6F;

        public void AssignTask(Task task)
        {
            _assignedTask = task;
        }

        private void FixedUpdate()
        {
            if (_assignedTask != null)
                if (IsTargetInRange)
                    ExecuteTask();
        }

        private void ExecuteTask()
        {
            if (_assignedTask.ElapsedTime < _assignedTask.RequiredTime)
                _assignedTask.ElapsedTime += Time.fixedDeltaTime;
            else
            {
                _assignedTask.Action();
                Astronaut.AstronautState = AstronautState.Idle;
                _assignedTask = null;
            }
        }
    }
}