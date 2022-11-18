using Attributes;
using ExtensionMethods;
using ScriptableObjects;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;

namespace Main
{
    public class BuildingManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private ColorsScriptableObject _colors;

        [SerializeField]
        private BlockModelListScriptableObject _modelList;

        [SerializeField]
        private ShapeListScriptableObject _shapeList;

        [SerializeField]
        private GameObject _temporalDesignationPrefab;

        [SerializeField]
        private GameObject _cancelDesignationPrefab;

        [SerializeField]
        private GameObject _miningDesignationPrefab;

        [SerializeField]
        private GameObject _shipPrefab;

        [SerializeField]
        private GameObject _wallPrefab;

        [SerializeField]
        private GameObject _floorPrefab;

        [SerializeField]
        private GameObject _wallDesignationPrefab;

        [SerializeField]
        private GameObject _floorDesignationPrefab;

        [SerializeField]
        private GameObject _buildingUI;

        [SerializeField]
        private GameObject _shapePickerUI;

        [SerializeField]
        private GameObject _modelPickerUI;

        [SerializeField]
        private Transform _worldTransform;

        [SerializeField]
        private int _maxDesignDistance = 9;

        [SerializeField]
        [ReadonlyInspector]
        private BuildingMode _buildingMode = BuildingMode.Wall;

        [SerializeField]
        [ReadonlyInspector]
        private ShapeScriptableObject _shape;

        [SerializeField]
        [ReadonlyInspector]
        private BlockModelScriptableObject _model;

        [SerializeField]
        [Range(0F, 359F)]
        private float _prefabRotation;

        private GameObject _currentDesignation;

        private Transform _temporalParent;

        private Vector2 _originalDesignationPosition;

        private List<GameObject> _temporalDesignations = new();

        private UnityEvent _updateCalled = new();

        private bool _isPointerOverUI = false;

        private GameObject _selectedDesignationPrefab;

        #endregion Fields

        #region Properties

        public static BuildingManager Instance { get; private set; }

        public GameObject WallPrefab => _wallPrefab;

        public GameObject FloorPrefab => _floorPrefab;

        public GameObject WallDesignationPrefab => _wallDesignationPrefab;
        public GameObject FloorDesignationPrefab => _floorDesignationPrefab;

        #endregion Properties

        #region Private

        private void SubscribeToShapeChangedEvent()
        {
            UIController.OnShapeChanged.AddListener(ChangeSelectedShape);
        }

        private void SubscribeToModelChangedEvent()
        {
            UIController.OnModelChanged.AddListener(ChangeSelectedModel);
        }

        private void ChangeSelectedShape(int shapeId)
        {
            _shape = _shapeList.GetShape(shapeId);
            StartFromPreviousMode();
        }

        private void ChangeSelectedModel(int modelId)
        {
            _model = _modelList.GetModel(modelId);
            StartFromPreviousMode();
        }

        private void SetBuildingMenuActive(bool state)
        {
            if (_buildingUI != null)
                _buildingUI.SetActive(state);
        }

        private void SetShapePickerActive(bool state)
        {
            if (_shapePickerUI != null)
                _shapePickerUI.SetActive(state);
        }

        private void SetModelPickerActive(bool state)
        {
            _modelPickerUI.SetActive(state);
        }

        private bool AreDesignationsObstructed()
        {
            foreach (var item in _temporalDesignations)
                if (item.GetComponent<BlockDesignation>().IsObstructed)
                    return true;
            return false;
        }

        private bool IsDesignationObstructed()
        {
            return _currentDesignation.GetComponent<BlockDesignation>().IsObstructed;
        }

        private void InvokeUpdateEvent()
        {
            _updateCalled?.Invoke();
        }

        private void CheckIfCursorOverUI()
        {
            _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        private Vector2 GetMouseWorldPosition()
        {
            Vector2 position = PlayerController.BuildingPoint.ReadValue<Vector2>();
            position = Camera.main.ScreenToWorldPoint(position);
            return position;
        }

        private void DestroyDesignations()
        {
            for (int i = 0; i < _temporalDesignations.Count; i++)
            {
                Destroy(_temporalDesignations[0]);
                _temporalDesignations.RemoveAt(0);
                i--;
            }
        }

        private void ClampDistance(Vector3 originalPos,
            ref Vector3 localMousePos, float maxDistance)
        {
            float x = localMousePos.x - originalPos.x >= maxDistance ?
                originalPos.x + maxDistance :
                originalPos.x - localMousePos.x >= maxDistance ?
                originalPos.x - maxDistance :
                localMousePos.x;
            float y = localMousePos.y - originalPos.y >= maxDistance ?
                originalPos.y + maxDistance :
                originalPos.y - localMousePos.y >= maxDistance ?
                originalPos.y - maxDistance :
                localMousePos.y;
            localMousePos = new(x, y);
        }

        private Block FindClosestBlock(
            GameObject designation, Vector3 searchStartPosition)
        {
            var num = _worldTransform.GetComponentsInChildren<Block>(false)
                .Where(x => x != designation?.GetComponent<BlockDesignation>())
                .Where(x => x.Parent.GetComponent<Ship>() != null);
            var closestWall = num.Count() > 0 ?
                num.Aggregate((closest, next) =>
                Vector2.Distance(closest.Transform.position, searchStartPosition) <
                Vector2.Distance(next.Transform.position, searchStartPosition) ?
                closest : next) :
                null;
            return closestWall;
        }

        private void MoveDesignation(GameObject designation, Vector3 targetPosition)
        {
            Block closestBlock = FindClosestBlock(_currentDesignation, targetPosition);
            if (closestBlock != null ?
                    Vector2.Distance(closestBlock.Transform.position, targetPosition) < 1.5F :
                    false)
            {
                var v3relative = closestBlock.Transform
                    .InverseTransformPoint(targetPosition)
                    .RotateAroundPivot(Vector3.zero,
                    closestBlock.transform.localEulerAngles);
                designation.transform.parent = closestBlock.Parent;
                designation.transform.localPosition =
                    closestBlock.transform.localPosition +
                    v3relative
                    .Round();
                designation.transform.localEulerAngles = new Vector3(0, 0, _prefabRotation);
            }
            else
            {
                designation.transform.parent = _worldTransform;
                designation.transform.position = targetPosition;
                designation.transform.localEulerAngles =
                    GameManager.FocusedShipRotation.eulerAngles +
                    new Vector3(0, 0, _prefabRotation);
            }
        }

        private void StartBlockDesignation()
        {
            _currentDesignation = Instantiate(
                _temporalDesignationPrefab,
                GetMouseWorldPosition(),
                GameManager.FocusedShipRotation,
                _worldTransform);
            _currentDesignation.GetComponent<TemporalDesignation>()
                .TemporalBlockType = _buildingMode switch
                {
                    BuildingMode.Floor => BlockType.FloorDesignation,
                    _ => BlockType.WallDesignation
                };
            SetBlockModel(_currentDesignation);
            SetBlockShape(_currentDesignation);
            _updateCalled.AddListener(MoveCurrentDesignationToMouse);
            PlayerController.BuildingClick.AddListener(ActionType.Performed, PlaceDesignation);
        }

        private void SetBlockShape(GameObject block)
        {
            if (_buildingMode == BuildingMode.Wall)
            {
                block.GetComponent<Block>().ShapeId = _shape.Id;
                if (block.TryGetComponent(out SpriteMask mask))
                {
                    mask.sprite = _shape.Sprite;
                }
                if (block.TryGetComponent(out PolygonCollider2D collider))
                {
                    PolygonCollider2D shapeCollider =
                        _shape.Prefab.GetComponent<PolygonCollider2D>();
                    for (int i = 0; i < collider.pathCount; i++)
                        collider.SetPath(i, shapeCollider.GetPath(i));
                }
            }
            if (_buildingMode == BuildingMode.Floor)
            {
                block.GetComponent<Block>().ShapeId = 0;
                if (block.TryGetComponent(out SpriteMask mask))
                {
                    mask.sprite = _shapeList.GetShape(0).Sprite;
                }
            }
        }

        private void SetBlockModel(GameObject block)
        {
            block.GetComponent<Block>().ModelId = _model.Id;
            if (block.TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.sprite = _model.Sprite;
            }
        }

        private void ClearInputActionsListeners()
        {
            PlayerController.BuildingClick.ClearAllEvents();
            PlayerController.BuildingPoint.ClearAllEvents();
        }

        private void ClearUpdateEventListeners()
        {
            _updateCalled.RemoveAllListeners();
        }

        private void SetBuildingMode(BuildingMode mode)
        {
            ClearUpdateEventListeners();
            ClearCurentDesignation();
            ClearInputActionsListeners();
            DestroyDesignations();
            ClearTemporalParent();
            _buildingMode = mode;
            if (mode == BuildingMode.Wall)
                SetShapePickerActive(true);
            else
                SetShapePickerActive(false);
            if (mode == BuildingMode.Wall ||
                mode == BuildingMode.Floor)
                SetModelPickerActive(true);
            else
                SetModelPickerActive(false);
        }

        private void ClearCurentDesignation()
        {
            Destroy(_currentDesignation);
            _currentDesignation = null;
        }

        private void StartFromPreviousMode()
        {
            switch (_buildingMode)
            {
                case BuildingMode.Wall:
                    SetBuildingModeWall(new());
                    break;

                case BuildingMode.Floor:
                    SetBuildingModeFloor(new());
                    break;

                case BuildingMode.Equipment:
                    SetBuildingModeEquipment(new());
                    break;

                case BuildingMode.Mining:
                    SetBuildingModeMining(new());
                    break;

                case BuildingMode.Cancel:
                    SetBuildingModeCancel(new());
                    break;
            }
        }

        private void SetOriginalDesignationPosition()
        {
            _originalDesignationPosition =
                (Vector2)_currentDesignation?.transform.localPosition;
        }

        private void SetTemporalParent()
        {
            if (_currentDesignation.transform.parent?.GetComponent<Ship>() == null)
            {
                _temporalParent = new GameObject("TemporalParent").transform;
                _temporalParent.position = _currentDesignation.transform.position;
                _temporalParent.rotation = Quaternion.identity;
                _temporalParent.parent = Camera.main.transform;
                _currentDesignation.transform.parent = _temporalParent;
            }
            else
                _temporalParent = _currentDesignation.transform.parent;
        }

        private void MarkBlocksDesignedToMining()
        {
            List<Wall> walls = _temporalParent.GetComponentsInChildren<Wall>().ToList();
            List<Floor> floors = _temporalParent.GetComponentsInChildren<Floor>().ToList();
            foreach (MiningDesignation designation in _temporalDesignations
                .Select(designation => designation.GetComponent<MiningDesignation>()))
            {
                bool isSet = false;
                foreach (Wall wall in walls)
                    if (wall.transform.localPosition.Round() ==
                        designation.transform.localPosition.Round())
                    {
                        wall.IsMarkedForMining = true;
                        isSet = true;
                        break;
                    }
                if (!isSet)
                    foreach (Floor floor in floors)
                        if (floor.transform.localPosition.Round() ==
                            designation.transform.localPosition.Round())
                        {
                            floor.IsMarkedForMining = true;
                            break;
                        }
            }
        }

        private void SetMiningDesignationsActive()
        {
            List<SolidBlock> blocks =
                _temporalParent != null ?
                _temporalParent.GetComponentsInChildren<SolidBlock>().ToList() :
                new();
            foreach (GameObject designation in _temporalDesignations)
            {
                foreach (SolidBlock block in blocks)
                    if (block.LocalPosition ==
                        (Vector2)designation.transform.localPosition.Round())
                    {
                        designation.GetComponent<MiningDesignation>().IsActive = true;
                        break;
                    }
            }
        }

        private Block GetBlockUnderPointer()
        {
            Ray ray = Camera.main.ScreenPointToRay(
                PlayerController.BuildingPoint.ReadValue<Vector2>());
            var hit = Physics2D.GetRayIntersection(ray);
            return hit.collider?.GetComponent<Block>();
        }

        private void UpdateShip()
        {
            _temporalParent?.GetComponent<Ship>()?.StartUpdateShip();
        }

        private void CreateFinalDesignations()
        {
            GameObject prefab = _buildingMode switch
            {
                BuildingMode.Wall => _wallDesignationPrefab,
                _ => _floorDesignationPrefab
            };
            foreach (GameObject temporalDesignation in _temporalDesignations)
            {
                GameObject newDesignation = Instantiate(
                    prefab,
                    _temporalParent);
                newDesignation.transform.localPosition =
                    temporalDesignation.transform.localPosition.Round();
                newDesignation.transform.localEulerAngles =
                    new Vector3(0, 0, _prefabRotation);
                SetBlockModel(newDesignation);
                SetBlockShape(newDesignation);
            }
        }

        private void CreateShipIfTemporalParentIsNotAShip()
        {
            if (!_temporalParent?.GetComponent<Ship>())
            {
                GameObject ship = Instantiate(
                    _shipPrefab,
                    _temporalParent.position,
                    Quaternion.identity,
                    _worldTransform);
                ship.GetComponent<Rigidbody2D>().velocity =
                    GameManager.FocusedShipVelocity;
                TransferChildren(_temporalParent, ship.transform);
                Destroy(_temporalParent.gameObject);
                _temporalParent = ship.transform;
            }
        }

        private void TransferChildren(Transform originalParent, Transform targetParent)
        {
            foreach (Transform child in originalParent)
                child.parent = targetParent;
        }

        private void ClearTemporalParent()
        {
            if (_temporalParent != null)
                if (_temporalParent.name == "TemporalParent")
                {
                    Destroy(_temporalParent.gameObject);
                    _temporalParent = null;
                }
        }

        private void StartCancelDesignation()
        {
            CreateCancelDesignation(new());
            PlayerController.BuildingPoint
                .AddListener(ActionType.Performed, CreateCancelDesignation);
            PlayerController.BuildingClick
                .AddListener(ActionType.Performed, PlaceCancelDesignation);
        }

        private void CancelDesignations()
        {
            List<Block> blocks = _temporalParent.GetComponentsInChildren<Block>().ToList();
            List<Block> blocksToDestroy = new();
            foreach (CancelDesignation designation in _temporalDesignations
                .Select(designation => designation.GetComponent<CancelDesignation>()))
            {
                bool isSet = false;
                foreach (Block block in blocks)
                {
                    if (block is WallDesignation &&
                        block.transform.localPosition.Round() ==
                        designation.transform.localPosition.Round())
                    {
                        blocksToDestroy.Add(block);
                        isSet = true;
                        break;
                    }
                }
                if (isSet)
                    continue;
                foreach (Block block in blocks)
                {
                    if (block is FloorDesignation &&
                        block.transform.localPosition.Round() ==
                        designation.transform.localPosition.Round())
                    {
                        blocksToDestroy.Add(block);
                        isSet = true;
                        break;
                    }
                }
                if (isSet)
                    continue;
                foreach (Block block in blocks)
                {
                    if (block.transform.localPosition.Round() ==
                        designation.transform.localPosition.Round())
                    {
                        block.IsMarkedForMining = false;
                        break;
                    }
                }
            }
            for (int i = 0; i < blocksToDestroy.Count; i++)
            {
                var block = blocksToDestroy[i];

                block.gameObject.SetActive(false);

                Destroy(block.gameObject);
            }
        }

        private void SetCancelDesignationsActive()
        {
            List<Block> blocks =
                _temporalParent != null ?
                _temporalParent.GetComponentsInChildren<Block>().ToList() :
                new();
            foreach (GameObject designation in _temporalDesignations)
            {
                foreach (Block block in blocks)
                {
                    if (block is SolidBlock)
                        if (!block.IsMarkedForMining)
                            continue;
                    if (block.LocalPosition ==
                        (Vector2)designation.transform.localPosition.Round())
                    {
                        designation.GetComponent<CancelDesignation>().IsActive = true;
                        break;
                    }
                }
            }
        }

        private void StartMiningDesignation()
        {
            CreateMiningDesignation(new());
            PlayerController.BuildingPoint
                .AddListener(ActionType.Performed, CreateMiningDesignation);
            PlayerController.BuildingClick
                .AddListener(ActionType.Performed, PlaceMiningDesignation);
        }

        private void SubscribeToInputEvents()
        {
            PlayerController.BuildingWall
                .AddListener(ActionType.Performed, SetBuildingModeWall);
            PlayerController.BuildingFloor
                .AddListener(ActionType.Performed, SetBuildingModeFloor);
            PlayerController.BuildingEquipment
                .AddListener(ActionType.Performed, SetBuildingModeEquipment);
            PlayerController.BuildingMining
                .AddListener(ActionType.Performed, SetBuildingModeMining);
            PlayerController.BuildingCancel
                .AddListener(ActionType.Performed, SetBuildingModeCancel);
            PlayerController.BuildingChangeRotation
                .AddListener(ActionType.Performed, ChangePrefabRotation);
        }

        private void UnsubscribeFromInputEvents()
        {
            PlayerController.BuildingWall.ClearAllEvents();
            PlayerController.BuildingFloor.ClearAllEvents();
            PlayerController.BuildingEquipment.ClearAllEvents();
            PlayerController.BuildingMining.ClearAllEvents();
            PlayerController.BuildingCancel.ClearAllEvents();
            PlayerController.BuildingChangeRotation.ClearAllEvents();
        }

        private void MoveCurrentDesignationToMouse()
        {
            if (_isPointerOverUI)
                return;
            MoveDesignation(_currentDesignation, GetMouseWorldPosition());
        }

        private void SetDefaultShape()
        {
            _shape = _shapeList.GetShape(0);
        }

        private void SetDefaultModel()
        {
            _model = _modelList.GetModel(0);
        }

        #endregion Private

        #region Callbacks

        private void CreateDesignationGrid(CallbackContext context)
        {
            if (_isPointerOverUI)
                return;
            DestroyDesignations();
            Vector3 localMousePos = _temporalParent.transform
                .InverseTransformPoint(GetMouseWorldPosition());
            localMousePos = localMousePos.Round();
            ClampDistance(_originalDesignationPosition, ref localMousePos, _maxDesignDistance);
            int lesserX = 0;
            int lesserY = 0;
            int greaterX = 0;
            int greaterY = 0;
            if (localMousePos.x < _originalDesignationPosition.x)
            {
                lesserX = (int)Math.Round(localMousePos.x);
                greaterX = (int)Math.Round(_originalDesignationPosition.x);
            }
            else
            {
                greaterX = (int)Math.Round(localMousePos.x);
                lesserX = (int)Math.Round(_originalDesignationPosition.x);
            }
            if (localMousePos.y < _originalDesignationPosition.y)
            {
                lesserY = (int)Math.Round(localMousePos.y);
                greaterY = (int)Math.Round(_originalDesignationPosition.y);
            }
            else
            {
                greaterY = (int)Math.Round(localMousePos.y);
                lesserY = (int)Math.Round(_originalDesignationPosition.y);
            }
            for (int x = lesserX; x <= greaterX; x++)
                for (int y = lesserY; y <= greaterY; y++)
                {
                    _temporalDesignations.Add(Instantiate(
                        _selectedDesignationPrefab,
                        _temporalParent));
                    _temporalDesignations[^1].transform.localPosition =
                        new Vector3(x, y).Round();
                    _temporalDesignations[^1].transform.localEulerAngles =
                        new Vector3(0, 0, _prefabRotation);
                    _temporalDesignations[^1].GetComponent<TemporalDesignation>()
                        .TemporalBlockType = _buildingMode switch
                        {
                            BuildingMode.Floor => BlockType.FloorDesignation,
                            _ => BlockType.WallDesignation
                        };
                    SetBlockModel(_temporalDesignations[^1]);
                    SetBlockShape(_temporalDesignations[^1]);
                }
        }

        private void PlaceDesignation(CallbackContext context)
        {
            if (_isPointerOverUI)
                return;
            if (IsDesignationObstructed())
                return;
            SetTemporalParent();
            SetOriginalDesignationPosition();
            ClearCurentDesignation();
            ClearInputActionsListeners();
            ClearUpdateEventListeners();
            CreateDesignationGrid(new());
            PlayerController.BuildingRightClick.AddListener(
                ActionType.Performed, BreakDesignationGrid);
            PlayerController.BuildingPoint.AddListener(
                ActionType.Performed, CreateDesignationGrid);
            PlayerController.BuildingClick.AddListener(
                ActionType.Performed, FinalizeDesignationGrid);
        }

        private void CreateMiningDesignation(CallbackContext context)
        {
            ClearCurentDesignation();
            Block block = GetBlockUnderPointer();
            if (_isPointerOverUI)
                return;
            if (block == null)
                return;
            if (block is not SolidBlock)
                return;
            _temporalParent = block.transform.parent;
            _currentDesignation = Instantiate(_miningDesignationPrefab, _temporalParent);
            _currentDesignation.transform.localPosition = block.transform.localPosition;
            _currentDesignation.transform.localRotation = block.transform.localRotation;
            _currentDesignation.GetComponent<MiningDesignation>().IsActive = true;
        }

        private void PlaceMiningDesignation(CallbackContext context)
        {
            if (_isPointerOverUI)
                return;
            if (_currentDesignation == null)
                return;
            ClearInputActionsListeners();
            SetOriginalDesignationPosition();
            ClearCurentDesignation();
            CreateDesignationGrid(new());
            _updateCalled.AddListener(SetMiningDesignationsActive);
            PlayerController.BuildingPoint
                .AddListener(ActionType.Performed, CreateDesignationGrid);
            PlayerController.BuildingRightClick.AddListener(
                ActionType.Performed, BreakDesignationGrid);
            PlayerController.BuildingClick
                .AddListener(ActionType.Performed, FinalizeMiningGrid);
        }

        private void FinalizeMiningGrid(CallbackContext context)
        {
            if (_isPointerOverUI)
                return;
            MarkBlocksDesignedToMining();
            UpdateShip();
            StartFromPreviousMode();
        }

        private void ChangePrefabRotation(CallbackContext context)
        {
            _prefabRotation -= 90F;
            if (_prefabRotation <= -360F)
                _prefabRotation = 0F;
            StartFromPreviousMode();
        }

        private void CreateCancelDesignation(CallbackContext obj)
        {
            ClearCurentDesignation();
            Block block = GetBlockUnderPointer();
            if (_isPointerOverUI)
                return;
            if (block == null)
                return;
            if (block is SolidBlock)
                if (!block.IsMarkedForMining)
                    return;
            _temporalParent = block.transform.parent;
            _currentDesignation = Instantiate(_cancelDesignationPrefab, _temporalParent);
            _currentDesignation.transform.localPosition = block.transform.localPosition;
            _currentDesignation.transform.localRotation = block.transform.localRotation;
            _currentDesignation.GetComponent<CancelDesignation>().IsActive = true;
        }

        private void PlaceCancelDesignation(CallbackContext obj)
        {
            if (_isPointerOverUI)
                return;
            if (_currentDesignation == null)
                return;
            ClearInputActionsListeners();
            SetOriginalDesignationPosition();
            ClearCurentDesignation();
            CreateDesignationGrid(new());
            _updateCalled.AddListener(SetCancelDesignationsActive);
            PlayerController.BuildingPoint
                .AddListener(ActionType.Performed, CreateDesignationGrid);
            PlayerController.BuildingRightClick.AddListener(
                ActionType.Performed, BreakDesignationGrid);
            PlayerController.BuildingClick
                .AddListener(ActionType.Performed, FinalizeCancelGrid);
        }

        private void FinalizeCancelGrid(CallbackContext obj)
        {
            if (_isPointerOverUI)
                return;
            CancelDesignations();
            UpdateShip();
            StartFromPreviousMode();
        }

        private void SetBuildingModeWall(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Wall);
            _selectedDesignationPrefab = _temporalDesignationPrefab;
            StartBlockDesignation();
        }

        private void SetBuildingModeFloor(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Floor);
            _selectedDesignationPrefab = _temporalDesignationPrefab;
            StartBlockDesignation();
        }

        private void SetBuildingModeEquipment(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Equipment);
        }

        private void SetBuildingModeMining(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Mining);
            _selectedDesignationPrefab = _miningDesignationPrefab;
            StartMiningDesignation();
        }

        private void SetBuildingModeCancel(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Cancel);
            _selectedDesignationPrefab = _cancelDesignationPrefab;
            StartCancelDesignation();
        }

        private void BreakDesignationGrid(CallbackContext context)
        {
            if (_isPointerOverUI)
                return;
            ClearTemporalParent();
            DestroyDesignations();
            StartFromPreviousMode();
        }

        private void FinalizeDesignationGrid(CallbackContext context)
        {
            if (_isPointerOverUI)
                return;
            if (AreDesignationsObstructed())
                return;
            CreateShipIfTemporalParentIsNotAShip();
            CreateFinalDesignations();
            UpdateShip();
            StartFromPreviousMode();
        }

        #endregion Callbacks

        #region Unity

        private void Update()
        {
            InvokeUpdateEvent();
            CheckIfCursorOverUI();
        }

        private void Awake()
        {
            Instance = this;
            SetDefaultShape();
            SetDefaultModel();
        }

        private void Start()
        {
            SubscribeToShapeChangedEvent();
            SubscribeToModelChangedEvent();
        }

        private void OnEnable()
        {
            SubscribeToInputEvents();
            StartFromPreviousMode();
            SetBuildingMenuActive(true);
        }

        private void OnDisable()
        {
            ClearUpdateEventListeners();
            UnsubscribeFromInputEvents();
            ClearCurentDesignation();
            ClearTemporalParent();
            DestroyDesignations();
            SetBuildingMenuActive(false);
        }

        #endregion Unity
    }
}