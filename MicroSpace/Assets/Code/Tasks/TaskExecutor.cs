using Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace Tasks
{
    public class TaskExecutor : MonoBehaviour
    {
        #region Fields

        private Task _assignedTask;
        private Dictionary<int, float> _requiredItems;
        private Astronaut _astronaut;

        #endregion Fields

        #region Properties

        private Astronaut Astronaut =>
            _astronaut ??= GetComponent<Astronaut>();

        public Task AssignedTask => _assignedTask;

        public Transform CurrentTarget { get; private set; }

        #endregion Properties

        #region Public

        public void AssignTask(Task task)
        {
            _requiredItems = task.Items;
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

        #endregion Public

        #region Unity

        private void Start()
        {
            Task.Tasks.CollectionChanged += OnTasksCollectionChanged;
        }

        private void FixedUpdate()
        {
            if (_assignedTask != null)
            {
                AssignCurrentTarget();
                //if (IsTargetInRange())
                //    ExecuteTask();
            }
        }

        #endregion Unity

        #region Private

        private void AssignCurrentTarget()
        {
            if (HasRequiredItems(out (int, float) nextMissingItem))
                CurrentTarget = _assignedTask.Target;
            else
                CurrentTarget = ItemFinder.FindClosestItem(
                    nextMissingItem.Item1,
                    transform.position);
        }

        private bool HasRequiredItems(out (int, float) nextMissingItem)
        {
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

        #endregion Private
    }
}