using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Attributes;
using ScriptableObjects;
using System.Collections;
using Maths;

namespace Entities
{
    public class Satellite : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [ReadonlyInspector]
        private int _id;

        [SerializeField]
        [ReadonlyInspector]
        private List<Block> _blocks = new();

        [SerializeField]
        [ReadonlyInspector]
        private Vector2 _position;

        [SerializeField]
        [ReadonlyInspector]
        private float _rotation;

        [SerializeField]
        [ReadonlyInspector]
        private Vector2 _velocity;

        private bool _isSatelliteLoaded = true;

        private static readonly float _satelliteUnloadDistance = 200F;

        #endregion Fields

        #region Properties

        public List<Block> Blocks { get => _blocks; set => _blocks = value; }

        public List<Wall> Walls => _blocks
            .Where(block => block is Wall)
            .Select(block => block as Wall)
            .ToList();

        public List<Floor> Floors => _blocks
            .Where(block => block is Floor)
            .Select(block => block as Floor)
            .ToList();

        public int ElementsCount { get => Blocks.Count; }

        public int Id { get => _id; set => _id = value; }

        public Vector2 Position { get => _position; set => _position = value; }

        public float Rotation { get => _rotation; set => _rotation = value; }

        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        public Rigidbody2D Rigidbody2D { get; private set; }

        #endregion Properties

        #region Public

        public void UpdateSatellite()
        {
            SimulatePhysics();
            UpdateBlocks();
            UpdateFloors();
            UpdateProperties();
            if (IsSatelliteEmpty())
                DestroySatellite();
        }

        public void ActivateOrDeactivateChildren(Transform focusedSatellite)
        {
            if (IsSatelliteDistantFromGivenSatellite(focusedSatellite) && _isSatelliteLoaded)
                SetSatelliteChildrenActive(false);
            else if (!IsSatelliteDistantFromGivenSatellite(focusedSatellite) && !_isSatelliteLoaded)
                SetSatelliteChildrenActive(true);
        }

        #endregion Public

        #region Private

        private void DestroySatellite()
        {
            if (Camera.main.transform.parent == transform)
                Camera.main.transform.parent = null;
            Destroy(gameObject);
        }

        private bool IsSatelliteEmpty()
        {
            return Blocks.Count <= 0;
        }

        private void UpdateProperties()
        {
            Position = transform.position;
            Rotation = transform.eulerAngles.z;
            Velocity = GetComponent<Rigidbody2D>().velocity;
        }

        private void SetId()
        {
            if (Id == 0)
                Id = IdManager.NextId;
        }

        private void SetSatelliteChildrenActive(bool state)
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(state);
            _isSatelliteLoaded = state;
        }

        private bool IsSatelliteDistantFromGivenSatellite(Transform satellite)
        {
            return Vector2.Distance(transform.position, satellite.position) >
                _satelliteUnloadDistance;
        }

        private void UpdateBlocks()
        {
            Blocks.Clear();
            foreach (Transform child in transform)
            {
                var block = child.gameObject.GetComponent<Block>();
                if (block != null && !IsTemporalDesignation(block))
                {
                    block.UpdateBlock();
                    Blocks.Add(block);
                }
            }
        }

        private void UpdateFloors()
        {
            Physics2D.queriesStartInColliders ^= true;
            foreach (Floor floor in Floors)
            {
                for (int i = 0; i < 8; i++)
                {
                    Debug.DrawLine(
                        floor.CheckingLines[i].A,
                        floor.CheckingLines[i].B,
                        Color.red,
                        float.PositiveInfinity);
                    var hits = Physics2D.LinecastAll(
                        floor.CheckingLines[i].A, floor.CheckingLines[i].B);
                    SolidBlock detectedBlock = hits
                        .Where(hit => hit.collider.TryGetComponent<Wall>(out _))
                        .Select(hit => hit.collider.GetComponent<Wall>())
                        .FirstOrDefault();
                    if (detectedBlock == null)
                        detectedBlock = hits
                            .Where(hit => hit.collider.TryGetComponent<Floor>(out _))
                            .Where(hit => hit.collider.gameObject != floor.gameObject)
                            .Select(hit => hit.collider.GetComponent<Floor>())
                            .FirstOrDefault();
                    floor.NeighbouringBlocks[i] = detectedBlock;
                    //Debug.Log(detectedBlock);
                }
            }
            Physics2D.queriesStartInColliders ^= true;
        }

        private bool IsTemporalDesignation(Block block)
        {
            return block is TemporalDesignation;
        }

        private void GetRigidbody2D()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private static void SimulatePhysics()
        {
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.Simulate(0.000001F);
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        }

        #endregion Private

        #region Unity

        private void Awake()
        {
            SetId();
            GetRigidbody2D();
        }

        #endregion Unity
    }
}