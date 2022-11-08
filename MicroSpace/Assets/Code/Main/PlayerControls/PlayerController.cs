using System;
using System.Collections;
using System.Collections.Generic;
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

        public static InputAction DefaultPause { get; set; }
        public static InputAction DefaultZoom { get; set; }
        public static InputAction DefaultQuickSave { get; set; }
        public static InputAction DefaultQuickLoad { get; set; }
        public static InputAction DefaultEnableSteering { get; set; }
        public static InputAction DefaultEnableBuilding { get; set; }
        public static InputAction DefaultPoint { get; set; }
        public static InputAction DefaultClick { get; set; }
        public static InputAction DefaultRightClick { get; set; }

        public static InputAction SteeringPause { get; set; }
        public static InputAction SteeringZoom { get; set; }
        public static InputAction SteeringQuickSave { get; set; }
        public static InputAction SteeringQuickLoad { get; set; }
        public static InputAction SteeringDirection { get; set; }
        public static InputAction SteeringDisableSteering { get; set; }
        public static InputAction SteeringRotation { get; set; }
        public static InputAction SteeringAdjustSpeed { get; set; }
        public static InputAction SteeringPoint { get; set; }
        public static InputAction SteeringClick { get; set; }
        public static InputAction SteeringRightClick { get; set; }

        public static InputAction BuildingDisableBuilding { get; set; }
        public static InputAction BuildingPoint { get; set; }
        public static InputAction BuildingClick { get; set; }
        public static InputAction BuildingRightClick { get; set; }

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

            DefaultPause = ActionMapDefault.FindAction("Pause");
            DefaultZoom = ActionMapDefault.FindAction("Zoom");
            DefaultQuickSave = ActionMapDefault.FindAction("QuickSave");
            DefaultQuickLoad = ActionMapDefault.FindAction("QuickLoad");
            DefaultEnableSteering = ActionMapDefault.FindAction("EnableSteering");
            DefaultEnableBuilding = ActionMapDefault.FindAction("EnableBuilding");
            DefaultPoint = ActionMapDefault.FindAction("Point");
            DefaultClick = ActionMapDefault.FindAction("Click");
            DefaultRightClick = ActionMapDefault.FindAction("RightClick");

            SteeringPause = ActionMapSteering.FindAction("Pause");
            SteeringZoom = ActionMapSteering.FindAction("Zoom");
            SteeringQuickSave = ActionMapSteering.FindAction("QuickSave");
            SteeringQuickLoad = ActionMapSteering.FindAction("QuickLoad");
            SteeringDirection = ActionMapSteering.FindAction("Direction");
            SteeringDisableSteering = ActionMapSteering.FindAction("DisableSteering");
            SteeringRotation = ActionMapSteering.FindAction("Rotation");
            SteeringAdjustSpeed = ActionMapSteering.FindAction("AdjustSpeed");
            SteeringPoint = ActionMapSteering.FindAction("Point");
            SteeringClick = ActionMapSteering.FindAction("Click");
            SteeringRightClick = ActionMapSteering.FindAction("RightClick");

            BuildingDisableBuilding = ActionMapBuilding.FindAction("DisableBuilding");
            BuildingPoint = ActionMapBuilding.FindAction("Point");
            BuildingClick = ActionMapBuilding.FindAction("Click");
            BuildingRightClick = ActionMapBuilding.FindAction("RightClick");
        }

        #endregion Unity
    }
}