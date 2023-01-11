﻿using UnityEngine;

namespace Miscellaneous
{
    public class Prefabs : MonoBehaviour
    {
        #region Fields

        [Header("Blocks")]
        [SerializeField]
        private GameObject _cancelDesignation;

        [SerializeField]
        private GameObject _floor;

        [SerializeField]
        private GameObject _floorDesignation;

        [SerializeField]
        private GameObject _miningDesignation;

        [SerializeField]
        private GameObject _temporalDesignation;

        [SerializeField]
        private GameObject _wall;

        [SerializeField]
        private GameObject _wallDesignation;

        [Header("Main")]
        [SerializeField]
        private GameObject _astronaut;

        [SerializeField]
        private GameObject _navMesh;

        [SerializeField]
        private GameObject _obstacle;

        [SerializeField]
        private GameObject _satellite;

        [Header("RigidEntities")]
        [SerializeField]
        private GameObject _massItem;

        [SerializeField]
        private GameObject _singleItem;

        [Header("UI")]
        [SerializeField]
        private GameObject _contextualMenuButton;

        [SerializeField]
        private GameObject _modelButton;

        [SerializeField]
        private GameObject _shapeButton;

        #endregion Fields

        #region Properties

        public static Prefabs Instance { get; private set; }

        public static GameObject CancelDesignation => Instance._cancelDesignation;

        public static GameObject Floor => Instance._floor;

        public static GameObject FloorDesignation => Instance._floorDesignation;

        public static GameObject MiningDesignation => Instance._miningDesignation;

        public static GameObject TemporalDesignation => Instance._temporalDesignation;

        public static GameObject Wall => Instance._wall;

        public static GameObject WallDesignation => Instance._wallDesignation;

        public static GameObject Astronaut => Instance._astronaut;

        public static GameObject NavMesh => Instance._navMesh;

        public static GameObject Obstacle => Instance._obstacle;

        public static GameObject Satellite => Instance._satellite;

        public static GameObject SingleItem => Instance._singleItem;

        public static GameObject MassItem => Instance._massItem;

        public static GameObject ContextualMenuButton => Instance._contextualMenuButton;

        public static GameObject ModelButton => Instance._modelButton;

        public static GameObject ShapeButton => Instance._shapeButton;

        #endregion Properties

        #region Unity

        private void Awake()
        {
            Instance = this;
        }

        #endregion Unity
    }
}