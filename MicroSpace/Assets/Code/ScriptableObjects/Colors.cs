using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "Colors",
        menuName = "ScriptableObjects/Colors")]
    public class Colors : ScriptableObject
    {
        #region Fields

        [Header("General")]
        [SerializeField]
        private Color _invisible;

        [SerializeField]
        private Color _highlighted;

        [Header("Designations")]
        [SerializeField]
        private Color _temporalDesignationNormal;

        [SerializeField]
        private Color _temporalDesignationObstructed;

        [SerializeField]
        private Color _wallDesignationNormal;

        [SerializeField]
        private Color _wallDesignationObstructed;

        [SerializeField]
        private Color _floorDesignationNormal;

        [SerializeField]
        private Color _floorDesignationObstructed;

        [SerializeField]
        private Color _cancelDesignationActive;

        [SerializeField]
        private Color _cancelDesignationInactive;

        [SerializeField]
        private Color _miningDesignationActive;

        [SerializeField]
        private Color _miningDesignationInactive;

        #endregion Fields

        #region Properties

        public Color Invisible => _invisible;
        public Color Highlighted => _highlighted;

        public Color TemporalDesignationNormal => _temporalDesignationNormal;
        public Color TemporalDesignationObstructed => _temporalDesignationObstructed;
        public Color WallDesignationNormal => _wallDesignationNormal;
        public Color WallDesignationObstructed => _wallDesignationObstructed;
        public Color FloorDesignationNormal => _floorDesignationNormal;
        public Color FloorDesignationObstructed => _floorDesignationObstructed;
        public Color CancelDesignationActive => _cancelDesignationActive;
        public Color CancelDesignationInactive => _cancelDesignationInactive;
        public Color MiningDesignationActive => _miningDesignationActive;
        public Color MiningDesignationInactive => _miningDesignationInactive;

        #endregion Properties

        #region Public

        public Color MixColors(Color a, Color b)
        {
            return new Color((a.r + b.r) / 2, (a.g + b.g) / 2, (a.b + b.b) / 2);
        }

        #endregion Public
    }
}