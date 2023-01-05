using Attributes;
using ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine;
using System;

namespace Entities
{
    public class Satellite : Entity
    {
        #region Fields

        [SerializeField]
        private GameObject _obstaclePrefab;

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

        private List<GameObject> _obstacles = new();

        private bool _isSatelliteLoaded = true;

        private static readonly float _satelliteUnloadDistance = 200F;

        #endregion Fields

        #region Public Properties

        public List<Block> Blocks { get => _blocks; set => _blocks = value; }

        public List<Wall> Walls { get; set; } = new();

        public List<Floor> Floors { get; set; } = new();

        public Vector2 Position { get => _position; set => _position = value; }

        public float Rotation { get => _rotation; set => _rotation = value; }

        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        public Rigidbody2D Rigidbody2D { get; private set; }

        #endregion Public Properties

        #region Public Static Properties

        public static List<Satellite> Satellites { get; } = new();

        public static List<Satellite> EnabledSatellites { get; } = new();

        public static UnityEvent<Satellite> FirstSatelliteCreated = new();

        #endregion Public Static Properties

        #region Public Methods

        public void UpdateSatellite()
        {
            UpdateBlocks();
            UpdateObstacles();
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

        #endregion Public Methods

        #region Private Methods

        private void DestroySatellite()
        {
            SetAstronautsFree();
            SetCameraFree();
            Destroy(gameObject);
        }

        private void SetAstronautsFree()
        {
            var astronauts = GetComponentsInChildren<Astronaut>();
            foreach (var astronaut in astronauts)
            {
                astronaut.transform.parent = null;
            }
        }

        private void SetCameraFree()
        {
            if (Camera.main.transform.parent == transform)
                Camera.main.transform.parent = null;
        }

        private bool IsSatelliteEmpty()
        {
            return Blocks.Count == 0;
        }

        private void UpdateProperties()
        {
            Position = transform.position;
            Rotation = transform.eulerAngles.z;
            Velocity = GetComponent<Rigidbody2D>().velocity;
        }

        private void SetSatelliteChildrenActive(bool state)
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(state);
            if (state && !EnabledSatellites.Contains(this))
                EnabledSatellites.Add(this);
            else if (!state && EnabledSatellites.Contains(this))
                EnabledSatellites.Remove(this);
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
                if (block != null && !IsBlockTemporalDesignation(block))
                {
                    block.UpdateBlock();
                    Blocks.Add(block);
                    Blocks = Blocks.OrderByDescending(block => block.LocalPosition.y)
                        .ThenBy(block => block.LocalPosition.x)
                        .ToList();
                    Walls = Blocks
                        .Where(block => block is Wall)
                        .Select(block => block as Wall)
                        .ToList();
                    Floors = Blocks
                        .Where(block => block is Floor)
                        .Select(block => block as Floor)
                        .ToList();
                }
            }
        }

        private void UpdateFloors()
        {
            bool originalQueriesStartInColliders = Physics2D.queriesStartInColliders;
            Physics2D.queriesStartInColliders = true;
            foreach (Floor floor in Floors)
            {
                UpdateFloor(floor);
            }
            Physics2D.queriesStartInColliders = originalQueriesStartInColliders;
        }

        private static void UpdateFloor(Floor floor)
        {
            for (int i = 0; i < 8; i++)
            {
                RaycastHit2D[] hits = Physics2D.LinecastAll(
                    floor.CheckingLines[i].A, floor.CheckingLines[i].B);
                if (!TryGetNeighbouringBlock<Wall>(hits, floor, out SolidBlock detectedBlock))
                    TryGetNeighbouringBlock<Floor>(hits, floor, out detectedBlock);
                floor.NeighbouringBlocks[i] = detectedBlock;
            }
        }

        private static bool TryGetNeighbouringBlock<T>(
            RaycastHit2D[] hits,
            Floor floor,
            out SolidBlock block)
            where T : SolidBlock
        {
            hits = hits
                .Where(hit => hit.collider.TryGetComponent<T>(out _))
                .ToArray();
            if (typeof(T) == typeof(Floor))
                hits = hits
                    .Where(hit => hit.collider.gameObject != floor.gameObject)
                    .ToArray();
            block = hits.Select(hit => hit.collider.GetComponent<T>())
                .FirstOrDefault();
            return block != null;
        }

        private void UpdateObstacles()
        {
            ResetObstacles();
            foreach (Wall wall in Walls)
            {
                if (!wall.IsIncludedInObstacle)
                {
                    CreateObstacle(wall);
                }
            }
        }

        private void ResetObstacles()
        {
            while (_obstacles.Count > 0)
            {
                Destroy(_obstacles[0]);
                _obstacles.RemoveAt(0);
            }
            foreach (Wall wall in Walls)
                wall.IsIncludedInObstacle = false;
        }

        private void CreateObstacle(Wall originalWall)
        {
            float minX = originalWall.LocalPosition.x;
            float maxY = originalWall.LocalPosition.y;
            float maxX = FindObstacleMaxX(originalWall);
            float minY = FindObstacleMinY(minX, maxX, maxY);

            foreach (Wall wall in Walls)
            {
                if (wall.LocalPosition.x >= minX
                    && wall.LocalPosition.x <= maxX
                    && wall.LocalPosition.y >= minY
                    && wall.LocalPosition.y <= maxY)
                    wall.IsIncludedInObstacle = true;
            }
            Vector2 obstacleCenter = new(
                (maxX + minX) / 2F,
                (maxY + minY) / 2F);
            Vector3 obstacleSize = new(
                maxX - minX + 1F,
                maxY - minY + 1F,
                0.2F);
            GameObject obstacle = Instantiate(_obstaclePrefab, transform);
            _obstacles.Add(obstacle);
            obstacle.transform.localPosition = Vector2.zero;
            var obstacleComponent = obstacle.GetComponent<NavMeshObstacle>();
            obstacleComponent.center = obstacleCenter;
            obstacleComponent.size = obstacleSize;
        }

        private float FindObstacleMinY(float minX, float maxX, float maxY)
        {
            if (IsThereAnotherYLevel(minX, maxX, maxY))
                return FindObstacleMinY(minX, maxX, maxY - 1F);
            return maxY;
        }

        private bool IsThereAnotherYLevel(float minX, float maxX, float maxY)
        {
            List<Wall> foundWalls = new();
            foreach (Wall wall in Walls)
            {
                Vector2 roudedWallPosition = wall.LocalPosition.Round();
                if (roudedWallPosition.x >= minX
                    && roudedWallPosition.x <= maxX
                    && roudedWallPosition.y == maxY - 1F)
                    if (!wall.IsIncludedInObstacle)
                        foundWalls.Add(wall);
            }
            if (foundWalls.Count == maxX - minX + 1F)
                return true;
            return false;
        }

        private float FindObstacleMaxX(Wall wall)
        {
            if (IsThereNotIncludedWallToTheRight(wall, out Wall rightWall))
                return FindObstacleMaxX(rightWall);
            return wall.LocalPosition.x;
        }

        private bool IsThereNotIncludedWallToTheRight(Wall wall, out Wall rightWall)
        {
            foreach (Wall otherWall in Walls)
                if (wall != otherWall)
                    if (otherWall.LocalPosition.Round()
                        == new Vector2(wall.LocalPosition.x + 1F, wall.LocalPosition.y).Round())
                        if (!otherWall.IsIncludedInObstacle)
                        {
                            rightWall = otherWall;
                            return true;
                        }
            rightWall = null;
            return false;
        }

        private bool IsBlockTemporalDesignation(Block block)
        {
            return block is TemporalDesignation;
        }

        private void GetRigidbody2D()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void AddSatelliteToList()
        {
            Satellites.Add(this);
            EnabledSatellites.Add(this);
            if (Satellites.Count == 1)
                FirstSatelliteCreated.Invoke(this);
        }

        private void RemoveSatelliteFromList()
        {
            Satellites.Remove(this);
        }

        #endregion Private Methods

        #region Unity

        private new void Awake()
        {
            base.Awake();
            GetRigidbody2D();
            AddSatelliteToList();
        }

        private void OnDestroy()
        {
            RemoveSatelliteFromList();
        }

        #endregion Unity
    }
}