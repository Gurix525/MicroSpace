using Attributes;
using ExtensionMethods;
using Miscellaneous;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Tilemaps;
using ScriptableObjects;
using System.Collections.ObjectModel;

namespace Entities
{
    public class Satellite : Entity
    {
        #region Fields

        [SerializeField]
        [ReadonlyInspector]
        private ObservableCollection<Block> _blocks = new();

        [SerializeField]
        [ReadonlyInspector]
        private Vector2 _position;

        [SerializeField]
        [ReadonlyInspector]
        private float _rotation;

        [SerializeField]
        [ReadonlyInspector]
        private Vector2 _velocity;

        [SerializeField]
        private Tilemap _floorDesignationsTilemap;

        [SerializeField]
        private Tilemap _floorsTilemap;

        [SerializeField]
        private Tilemap _wallDesignationsTilemap;

        [SerializeField]
        private Tilemap _wallsTilemap;

        private TilemapRenderer _wallsRenderer;

        private TilemapRenderer _wallDesignationsRenderer;

        private TilemapRenderer _floorsRenderer;

        private TilemapRenderer _floorDesignationsRenderer;

        private List<GameObject> _obstacles = new();

        private Rigidbody2D _rigidbody;

        private bool _hasWallsChangedLastFrame = false;

        #endregion Fields

        #region Public Properties

        public ObservableCollection<Block> Blocks { get => _blocks; set => _blocks = value; }

        public List<Wall> Walls { get; set; } = new();

        public List<WallDesignation> WallDesignations { get; set; } = new();

        public List<FloorDesignation> FloorDesignations { get; set; } = new();

        public List<Floor> Floors { get; set; } = new();

        public Vector2 Position { get => _position; set => _position = value; }

        public float Rotation { get => _rotation; set => _rotation = value; }

        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        public Tilemap FloorsTilemap => _floorsTilemap;
        public Tilemap WallsTilemap => _wallsTilemap;
        public Tilemap FloorDesignationsTilemap => _floorDesignationsTilemap;
        public Tilemap WallDesignationsTilemap => _wallDesignationsTilemap;

        public Rigidbody2D Rigidbody =>
            _rigidbody ??= GetComponent<Rigidbody2D>();

        #endregion Public Properties

        #region Public Static Properties

        public static List<Satellite> Satellites { get; } = new();

        public static List<Satellite> EnabledSatellites { get; } = new();

        public static UnityEvent<Satellite> FirstSatelliteCreated = new();

        #endregion Public Static Properties

        #region Public Methods

        private void UpdateSatellite()
        {
            if (!_hasWallsChangedLastFrame)
                return;

            _hasWallsChangedLastFrame = false;

            Debug.ClearDeveloperConsole();
            System.Diagnostics.Stopwatch watch = new();
            watch.Start();

            UpdateObstacles();

            watch.Stop();
            Debug.Log("Obstacles: " + watch.Elapsed.TotalMilliseconds);
            watch.Restart();

            UpdateFloors();

            Debug.Log("Floors: " + watch.Elapsed.TotalMilliseconds);
            watch.Restart();

            UpdateProperties();

            Debug.Log("Properties: " + watch.Elapsed.TotalMilliseconds);

            if (IsSatelliteEmpty())
                DestroySelf();
        }

        public static void ForEach(Action<Satellite> action)
        {
            foreach (Satellite satellite in Satellites)
                action(satellite);
        }

        #endregion Public Methods

        #region Unity

        private new void Awake()
        {
            base.Awake();
            AssignReferences();
            AddSelfToList();
            Blocks.CollectionChanged += OnBlocksCollectionChanged;
        }

        private void Start()
        {
            UpdateTilemaps();
        }

        private void Update()
        {
            UpdateSatellite();
        }

        private void OnEnable()
        {
            if (!EnabledSatellites.Contains(this))
                EnabledSatellites.Add(this);
        }

        private void OnDisable()
        {
            if (EnabledSatellites.Contains(this))
                EnabledSatellites.Remove(this);
        }

        private void OnDestroy()
        {
            RemoveSelfFromList();
        }

        #endregion Unity

        #region Private Methods

        private void AssignReferences()
        {
            _wallsRenderer = _wallsTilemap.GetComponent<TilemapRenderer>();
            _floorsRenderer = _floorsTilemap.GetComponent<TilemapRenderer>();
            _floorDesignationsRenderer = _floorDesignationsTilemap.GetComponent<TilemapRenderer>();
            _wallDesignationsRenderer = _wallDesignationsTilemap.GetComponent<TilemapRenderer>();
        }

        private void UpdateTilemaps()
        {
            _wallDesignationsRenderer.sortingOrder = Id;
            _wallsRenderer.sortingOrder = Id;
            _floorsRenderer.sortingOrder = Id;
            _floorDesignationsRenderer.sortingOrder = Id;
        }

        private void DestroySelf()
        {
            RemoveSelfFromList();
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
            GameObject obstacle = Instantiate(Prefabs.Obstacle, transform);
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

        private void AddSelfToList()
        {
            if (!Satellites.Contains(this))
                Satellites.Add(this);
            if (!EnabledSatellites.Contains(this))
                EnabledSatellites.Add(this);
            if (Satellites.Count == 1)
                FirstSatelliteCreated.Invoke(this);
        }

        private void RemoveSelfFromList()
        {
            if (Satellites.Contains(this))
                Satellites.Remove(this);
            if (EnabledSatellites.Contains(this))
                EnabledSatellites.Remove(this);
        }

        #endregion Private Methods

        #region Callbacks

        private void OnBlocksCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _hasWallsChangedLastFrame = true;
        }

        #endregion Callbacks
    }
}