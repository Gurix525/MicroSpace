using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Main
{
    public class PlayerController : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private PlayerInput _playerInput;

        #endregion Fields

        #region Properties

        public static PlayerController Instance { get; private set; }
        public static PlayerInput PlayerInput { get; private set; }

        public static InputActionMap ActionMapDefault { get; private set; }
        public static InputActionMap ActionMapSteering { get; private set; }
        public static InputActionMap ActionMapBuilding { get; private set; }

        public static ModifiableInputAction DefaultPause { get; set; } = new();
        public static ModifiableInputAction DefaultZoom { get; set; } = new();
        public static ModifiableInputAction DefaultQuickSave { get; set; } = new();
        public static ModifiableInputAction DefaultQuickLoad { get; set; } = new();
        public static ModifiableInputAction DefaultEnableSteering { get; set; } = new();
        public static ModifiableInputAction DefaultEnableBuilding { get; set; } = new();
        public static ModifiableInputAction DefaultPoint { get; set; } = new();
        public static ModifiableInputAction DefaultClick { get; set; } = new();
        public static ModifiableInputAction DefaultRightClick { get; set; } = new();
        public static ModifiableInputAction DefaultDirection { get; set; } = new();
        public static ModifiableInputAction DefaultRotation { get; set; } = new();
        public static ModifiableInputAction DefaultAlignCamera { get; set; } = new();

        public static ModifiableInputAction SteeringPause { get; set; } = new();
        public static ModifiableInputAction SteeringZoom { get; set; } = new();
        public static ModifiableInputAction SteeringQuickSave { get; set; } = new();
        public static ModifiableInputAction SteeringQuickLoad { get; set; } = new();
        public static ModifiableInputAction SteeringDirection { get; set; } = new();
        public static ModifiableInputAction SteeringDisableSteering { get; set; } = new();
        public static ModifiableInputAction SteeringRotation { get; set; } = new();
        public static ModifiableInputAction SteeringAdjustSpeed { get; set; } = new();
        public static ModifiableInputAction SteeringPoint { get; set; } = new();
        public static ModifiableInputAction SteeringClick { get; set; } = new();
        public static ModifiableInputAction SteeringRightClick { get; set; } = new();
        public static ModifiableInputAction SteeringSwitchToBuilding { get; set; } = new();
        public static ModifiableInputAction SteeringAlignCamera { get; set; } = new();

        public static ModifiableInputAction BuildingDisableBuilding { get; set; } = new();
        public static ModifiableInputAction BuildingZoom { get; set; } = new();
        public static ModifiableInputAction BuildingPause { get; set; } = new();
        public static ModifiableInputAction BuildingPoint { get; set; } = new();
        public static ModifiableInputAction BuildingClick { get; set; } = new();
        public static ModifiableInputAction BuildingRightClick { get; set; } = new();
        public static ModifiableInputAction BuildingWall { get; set; } = new();
        public static ModifiableInputAction BuildingFloor { get; set; } = new();
        public static ModifiableInputAction BuildingEquipment { get; set; } = new();
        public static ModifiableInputAction BuildingMining { get; set; } = new();
        public static ModifiableInputAction BuildingCancel { get; set; } = new();
        public static ModifiableInputAction BuildingChangeRotation { get; set; } = new();
        public static ModifiableInputAction BuildingDirection { get; set; } = new();
        public static ModifiableInputAction BuildingRotation { get; set; } = new();
        public static ModifiableInputAction BuildingAlignCamera { get; set; } = new();

        #endregion Properties

        #region Unity

        private void Awake()
        {
            Instance = this;
            PlayerInput = _playerInput;

            PlayerInput.SwitchCurrentActionMap("Steering");
            ActionMapSteering = PlayerInput.currentActionMap;
            PlayerInput.SwitchCurrentActionMap("Building");
            ActionMapBuilding = PlayerInput.currentActionMap;
            PlayerInput.SwitchCurrentActionMap("Default");
            ActionMapDefault = PlayerInput.currentActionMap;

            DefaultPause.Action = ActionMapDefault.FindAction("Pause");
            DefaultZoom.Action = ActionMapDefault.FindAction("Zoom");
            DefaultQuickSave.Action = ActionMapDefault.FindAction("QuickSave");
            DefaultQuickLoad.Action = ActionMapDefault.FindAction("QuickLoad");
            DefaultEnableSteering.Action = ActionMapDefault.FindAction("EnableSteering");
            DefaultEnableBuilding.Action = ActionMapDefault.FindAction("EnableBuilding");
            DefaultPoint.Action = ActionMapDefault.FindAction("Point");
            DefaultClick.Action = ActionMapDefault.FindAction("Click");
            DefaultRightClick.Action = ActionMapDefault.FindAction("RightClick");
            DefaultDirection.Action = ActionMapDefault.FindAction("Direction");
            DefaultRotation.Action = ActionMapDefault.FindAction("Rotation");
            DefaultAlignCamera.Action = ActionMapDefault.FindAction("AlignCamera");

            SteeringPause.Action = ActionMapSteering.FindAction("Pause");
            SteeringZoom.Action = ActionMapSteering.FindAction("Zoom");
            SteeringQuickSave.Action = ActionMapSteering.FindAction("QuickSave");
            SteeringQuickLoad.Action = ActionMapSteering.FindAction("QuickLoad");
            SteeringDirection.Action = ActionMapSteering.FindAction("Direction");
            SteeringDisableSteering.Action = ActionMapSteering.FindAction("DisableSteering");
            SteeringRotation.Action = ActionMapSteering.FindAction("Rotation");
            SteeringAdjustSpeed.Action = ActionMapSteering.FindAction("AdjustSpeed");
            SteeringPoint.Action = ActionMapSteering.FindAction("Point");
            SteeringClick.Action = ActionMapSteering.FindAction("Click");
            SteeringRightClick.Action = ActionMapSteering.FindAction("RightClick");
            SteeringSwitchToBuilding.Action = ActionMapSteering.FindAction("SwitchToBuilding");
            SteeringAlignCamera.Action = ActionMapSteering.FindAction("AlignCamera");

            BuildingDisableBuilding.Action = ActionMapBuilding.FindAction("DisableBuilding");
            BuildingZoom.Action = ActionMapBuilding.FindAction("Zoom");
            BuildingPause.Action = ActionMapBuilding.FindAction("Pause");
            BuildingPoint.Action = ActionMapBuilding.FindAction("Point");
            BuildingClick.Action = ActionMapBuilding.FindAction("Click");
            BuildingRightClick.Action = ActionMapBuilding.FindAction("RightClick");
            BuildingWall.Action = ActionMapBuilding.FindAction("Wall");
            BuildingFloor.Action = ActionMapBuilding.FindAction("Floor");
            BuildingEquipment.Action = ActionMapBuilding.FindAction("Equipment");
            BuildingMining.Action = ActionMapBuilding.FindAction("Mining");
            BuildingCancel.Action = ActionMapBuilding.FindAction("Cancel");
            BuildingChangeRotation.Action = ActionMapBuilding.FindAction("ChangeRotation");
            BuildingDirection.Action = ActionMapBuilding.FindAction("Direction");
            BuildingRotation.Action = ActionMapBuilding.FindAction("Rotation");
            BuildingAlignCamera.Action = ActionMapBuilding.FindAction("AlignCamera");
        }

        #endregion Unity
    }
}