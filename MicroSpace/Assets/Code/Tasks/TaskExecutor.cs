using Entities;
using Inventory;
using Miscellaneous;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using ExtensionMethods;
using System.Linq;

namespace Tasks
{
    public class TaskExecutor : MonoBehaviour
    {
        #region Fields

        private Task _assignedTask;
        private Dictionary<int, float> _requiredItems;
        private Astronaut _astronaut;
        private Container _container;
        private TargetType _targetType = TargetType.TaskExecution;

        #endregion Fields

        #region Properties

        private Astronaut Astronaut =>
            _astronaut ??= GetComponent<Astronaut>();

        private Container Container =>
            _container ??= GetComponent<Container>();

        public Task AssignedTask => _assignedTask;

        public Transform CurrentTarget { get; private set; }

        #endregion Properties

        #region Public

        public void AssignTask(Task task)
        {
            _requiredItems = task.Items;
            _assignedTask = task;
            task.AssignAstronaut(Astronaut.Id);
            task.Executing.AddListener(OnTaskExecuting);
            Astronaut.AstronautState = AstronautState.Working;
        }

        public void UnassignTask()
        {
            Astronaut.AstronautState = AstronautState.Idle;
            if (_assignedTask != null)
            {
                _assignedTask.UnassignAstronaut();
                DropAllItems();
                _assignedTask.Executing.RemoveListener(OnTaskExecuting);
                CurrentTarget = null;
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
                if (CurrentTarget == null)
                    AssignCurrentTarget();
                if (IsTargetInRange())
                    PerformActionOnTarget();
            }
        }

        #endregion Unity

        #region Private

        private void PerformActionOnTarget()
        {
            switch (_targetType)
            {
                case TargetType.ItemPickUp:
                    PickUpItem();
                    break;

                default:
                    ExecuteTask();
                    break;
            }
        }

        private void PickUpItem()
        {
            var targetItem = CurrentTarget.GetComponent<Entities.Item>();
            if (targetItem is MassItem)
            {
                var massItem = (MassItem)targetItem;
                Container.AddItem(massItem.ModelId, massItem.Mass);
                targetItem.DestroyRigidEntity();
            }
            else
                throw new NotImplementedException(
                    "Single item pick up not implemented yet");
        }

        private void DropAllItems()
        {
            Vector3 velocity = transform.parent != null ?
                transform.parent.GetComponentUpInHierarchy<Rigidbody2D>().velocity
                : Satellite.EnabledSatellites
                .OrderBy(satellite => Vector2.Distance(
                    satellite.Position, transform.position))
                .First().GetComponent<Rigidbody2D>().velocity;
            foreach (var item in Container.ContainerItems)
            {
                DropItem(item, velocity);
            }
            Container.Clear();
        }

        private void DropItem(ContainerItem item, Vector3 velocity)
        {
            var newItem = Instantiate(
                    Prefabs.MassItem,
                    transform.position,
                    Quaternion.identity,
                    References.WorldTransform)
                    .GetComponent<MassItem>();
            newItem.ModelId = item.ModelId;
            newItem.Mass = item.Mass;
            newItem.GetComponent<Rigidbody2D>().velocity = velocity;
        }

        private void AssignCurrentTarget()
        {
            if (HasRequiredItems(out (int, float) nextMissingItem))
            {
                _targetType = TargetType.TaskExecution;
                CurrentTarget = _assignedTask.Target;
            }
            else
            {
                _targetType = TargetType.ItemPickUp;
                CurrentTarget = ItemFinder.FindClosestItem(
                    nextMissingItem.Item1,
                    transform.position);
                if (CurrentTarget == null)
                    UnassignTask();
            }
        }

        private bool HasRequiredItems(out (int, float) nextMissingItem)
        {
            foreach (var item in _requiredItems)
            {
                if (!Container.HasItem(item.Key, item.Value))
                {
                    nextMissingItem = (item.Key, item.Value);
                    return false;
                }
            }
            nextMissingItem = (0, 0);
            return true;
        }

        private bool IsTargetInRange()
        {
            if (CurrentTarget != null)
                return Vector2.Distance(
                    transform.position,
                    CurrentTarget.position)
                    <= 1.5F;
            return false;
        }

        private void ExecuteTask()
        {
            if (_assignedTask.ElapsedTime < _assignedTask.RequiredTime)
                _assignedTask.ElapsedTime += Time.fixedDeltaTime;
            else
            {
                Task currentTask = _assignedTask;
                currentTask.Executing.Invoke(currentTask);
                currentTask.Action();
                currentTask.Executed.Invoke(currentTask);
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

        private void OnTaskExecuting(Task task)
        {
            foreach (var item in _requiredItems)
            {
                Container.RemoveItem(item.Key, item.Value);
            }
        }

        #endregion Private
    }
}