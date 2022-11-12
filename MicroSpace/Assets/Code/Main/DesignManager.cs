using Attributes;
using ExtensionMethods;
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
    public class DesignManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private ColorsScriptableObject _colors;

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
        private Transform _worldTransform;

        [SerializeField]
        private int _maxDesignDistance = 9;

        [SerializeField]
        [ReadonlyInspector]
        private BuildingMode _buildingMode = BuildingMode.Wall;

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

        public static DesignManager Instance { get; private set; }

        public GameObject WallPrefab => _wallPrefab;

        public GameObject FloorPrefab => _floorPrefab;

        public GameObject WallDesignationPrefab => _wallDesignationPrefab;
        public GameObject FloorDesignationPrefab => _floorDesignationPrefab;

        #endregion Properties

        #region Public

        //public void StartDesignateBlock(BlockType blockType)
        //{
        //    StartCoroutine(DesignateBlock(blockType));
        //}

        //public void StartCancelDesignation()
        //{
        //    StartCoroutine(CancelDesignation());
        //}

        //public void StartDesignateMining()
        //{
        //    StartCoroutine(DesignateMining());
        //}

        #endregion Public

        #region Private

        //private static void UpdateShip(GameObject ship)
        //{
        //    ship.GetComponent<Ship>().UpdateShip();
        //}

        //private IEnumerator DesignateBlock(BlockType blockType)
        //{
        //    var prefab = blockType switch
        //    {
        //        BlockType.Floor => _floorDesignationPrefab,
        //        _ => _wallDesignationPrefab
        //    };
        //    GameManager.SwitchSetup();
        //    GameObject designation = Instantiate(_temporalDesignationPrefab, _worldTransform);
        //    // FindClosestBlock jest potrzebne żeby nie krzyczało że pusty obiekt
        //    IBlock closestBlock = FindClosestBlock(designation, Vector3.zero);
        //    while (!PlayerController.BuildingClick.IsPressed() || IsDesignationObstructed(designation))
        //    {
        //        if (PlayerController.BuildingRightClick.IsPressed() ||
        //                PlayerController.BuildingDisableBuilding.IsPressed())
        //        {
        //            Destroy(designation);
        //            GameManager.SwitchSetup();
        //            yield break;
        //        }
        //        var mousePos = GetMouseWorldPosition();
        //        closestBlock = FindClosestBlock(designation, mousePos);
        //        MoveBlockDesignation(designation, closestBlock, mousePos);
        //        if (designation.transform.parent == null)
        //            designation.GetComponent<SpriteRenderer>().color =
        //                _colors.TemporalDesignationObstructed;
        //        yield return null;
        //    }
        //    if (designation.transform.parent == null)
        //    {
        //        Destroy(designation);
        //        GameManager.SwitchSetup();
        //        yield break;
        //    }
        //    Vector3 originalPos = designation.transform.localPosition;
        //    originalPos = originalPos.Round();
        //    Destroy(designation);
        //    yield return null;
        //    Vector3 localMousePos = new();
        //    Vector3 oldLocalMousePos = Vector3.positiveInfinity;
        //    List<GameObject> designations = new();
        //    while (!PlayerController.BuildingClick.IsPressed() || AreDesignationsObstructed(designations))
        //    {
        //        if (PlayerController.BuildingRightClick.IsPressed() ||
        //            PlayerController.BuildingDisableBuilding.IsPressed())
        //        {
        //            DestroyDesignations(designations);
        //            GameManager.SwitchSetup();
        //            yield break;
        //        }
        //        localMousePos = closestBlock.Parent
        //                .InverseTransformPoint(GetMouseWorldPosition());
        //        localMousePos = localMousePos.Round();
        //        if (localMousePos != oldLocalMousePos)
        //        {
        //            DestroyDesignations(designations);
        //            CreateDesignations(
        //                closestBlock, originalPos, designations,
        //                _temporalDesignationPrefab, ref localMousePos);
        //        }
        //        oldLocalMousePos = localMousePos;
        //        yield return null;
        //    }
        //    for (int i = 0; i < designations.Count; i++)
        //    {
        //        var block = Instantiate(prefab, closestBlock.Parent);
        //        block.transform.localPosition = designations[i].transform.localPosition;
        //    }
        //    DestroyDesignations(designations);
        //    UpdateShip(closestBlock.Parent.gameObject);
        //    GameManager.SwitchSetup();
        //}

        //private IEnumerator CancelDesignation()
        //{
        //    GameManager.SwitchSetup();
        //    RaycastHit2D hit = new();
        //    GameObject designation = Instantiate(_cancelDesignationPrefab);
        //    SpriteRenderer designationSpriteRenderer = designation
        //        .GetComponent<SpriteRenderer>();
        //    designationSpriteRenderer.color = _colors.Invisible;
        //    while (!PlayerController.BuildingClick.IsPressed() || hit.collider == null)
        //    {
        //        if (PlayerController.BuildingRightClick.IsPressed() ||
        //            PlayerController.BuildingDisableBuilding.IsPressed())
        //        {
        //            Destroy(designation);
        //            GameManager.SwitchSetup();
        //            yield break;
        //        }
        //        hit = Physics2D.GetRayIntersection(
        //            Camera.main.ScreenPointToRay(
        //                PlayerController.BuildingPoint.ReadValue<Vector2>()));
        //        if (hit.collider != null)
        //        {
        //            designation.transform.parent = hit.collider.transform.parent;
        //            designation.transform.localPosition =
        //                hit.collider.transform.localPosition;
        //            designation.transform.localEulerAngles =
        //                hit.collider.transform.localEulerAngles;
        //            designationSpriteRenderer.color = _colors.CancelDesignationInactive;
        //        }
        //        else
        //        {
        //            designation.transform.parent = null;
        //            designationSpriteRenderer.color = _colors.Invisible;
        //        }
        //        yield return null;
        //    }
        //    Vector2 originalPos = new();
        //    originalPos = hit.collider.transform.localPosition.Round();
        //    Destroy(designation);
        //    yield return null;
        //    Vector3 localMousePos = new();
        //    Vector3 oldLocalMousePos = Vector3.positiveInfinity;
        //    List<GameObject> designations = new();
        //    List<Block> blocks = new();
        //    while (!PlayerController.BuildingClick.IsPressed() || hit.collider == null)
        //    {
        //        if (PlayerController.BuildingRightClick.IsPressed() ||
        //            PlayerController.BuildingDisableBuilding.IsPressed())
        //        {
        //            DestroyDesignations(designations);
        //            GameManager.SwitchSetup();
        //            yield break;
        //        }
        //        localMousePos = hit.collider.transform.parent
        //                .InverseTransformPoint(GetMouseWorldPosition());
        //        localMousePos = localMousePos.Round();
        //        if (localMousePos != oldLocalMousePos)
        //        {
        //            DestroyDesignations(designations);
        //            CreateDesignations(
        //                hit.collider.GetComponent<Block>(), originalPos,
        //                designations, _cancelDesignationPrefab, ref localMousePos);
        //            foreach (Transform item in hit.transform)
        //            {
        //                var block = item.GetComponent<Block>();
        //                if (block != null)
        //                    blocks.Add(block);
        //            }
        //            foreach (var item in designations)
        //            {
        //                var block = blocks
        //                    .Where(x => x.GetComponent<CancelDesignation>() == null)
        //                    .ToList()
        //                    .Find(x => x.transform.localPosition.Round() ==
        //                    item.transform.localPosition.Round());
        //                if (block != null)
        //                {
        //                    if (block is BlockDesignation || block.IsMarkedForMining)
        //                        item.GetComponent<CancelDesignation>().IsActive = true;
        //                    else
        //                        item.GetComponent<CancelDesignation>().IsActive = false;
        //                }
        //            }
        //            blocks.Clear();
        //        }
        //        oldLocalMousePos = localMousePos;
        //        yield return null;
        //    }
        //    foreach (Transform item in hit.transform)
        //    {
        //        var block = item.GetComponent<Block>();
        //        if (block != null)
        //            blocks.Add(block);
        //    }
        //    foreach (var item in designations)
        //    {
        //        var block = blocks.Find(x => x.transform.localPosition.Round() ==
        //            item.transform.localPosition.Round());
        //        if (block != null)
        //        {
        //            if (block is BlockDesignation)
        //                Destroy(block.gameObject);
        //            else block.IsMarkedForMining = false;
        //        }
        //    }
        //    DestroyDesignations(designations);
        //    UpdateShip(hit.transform.gameObject);
        //    GameManager.SwitchSetup();
        //}

        //private IEnumerator DesignateMining()
        //{
        //    GameManager.SwitchSetup();
        //    RaycastHit2D hit = new();
        //    GameObject designation = Instantiate(_miningDesignationPrefab);
        //    SpriteRenderer designationSpriteRenderer = designation
        //        .GetComponent<SpriteRenderer>();
        //    designationSpriteRenderer.color = _colors.Invisible;
        //    while (!PlayerController.BuildingClick.IsPressed() || hit.collider == null)
        //    {
        //        if (PlayerController.BuildingRightClick.IsPressed() ||
        //            PlayerController.BuildingDisableBuilding.IsPressed())
        //        {
        //            Destroy(designation);
        //            GameManager.SwitchSetup();
        //            yield break;
        //        }
        //        hit = Physics2D.GetRayIntersection(
        //            Camera.main.ScreenPointToRay(
        //                PlayerController.BuildingPoint.ReadValue<Vector2>()));
        //        if (hit.collider != null)
        //        {
        //            if (hit.collider.GetComponent<Block>() is not BlockDesignation)
        //            {
        //                designation.transform.parent = hit.collider.transform.parent;
        //                designation.transform.localPosition =
        //                    hit.collider.transform.localPosition;
        //                designation.transform.localEulerAngles =
        //                    hit.collider.transform.localEulerAngles;
        //                designationSpriteRenderer.color = _colors.MiningDesignationInactive;
        //            }
        //            else
        //            {
        //                designation.transform.parent = null;
        //                designationSpriteRenderer.color = _colors.Invisible;
        //            }
        //        }
        //        else
        //        {
        //            designation.transform.parent = null;
        //            designationSpriteRenderer.color = _colors.Invisible;
        //        }
        //        yield return null;
        //    }
        //    Vector2 originalPos = new();
        //    originalPos = hit.collider.transform.localPosition.Round();
        //    Destroy(designation);
        //    yield return null;
        //    Vector3 localMousePos = new();
        //    Vector3 oldLocalMousePos = Vector3.positiveInfinity;
        //    List<GameObject> designations = new();
        //    List<Block> blocks = new();
        //    while (!PlayerController.BuildingClick.IsPressed() || hit.collider == null)
        //    {
        //        if (PlayerController.BuildingClick.IsPressed() ||
        //            PlayerController.BuildingDisableBuilding.IsPressed())
        //        {
        //            DestroyDesignations(designations);
        //            GameManager.SwitchSetup();
        //            yield break;
        //        }
        //        localMousePos = hit.collider.transform.parent
        //                .InverseTransformPoint(GetMouseWorldPosition());
        //        localMousePos = localMousePos.Round();
        //        if (localMousePos != oldLocalMousePos)
        //        {
        //            DestroyDesignations(designations);
        //            CreateDesignations(
        //                hit.collider.GetComponent<Block>(), originalPos,
        //                designations, _miningDesignationPrefab, ref localMousePos);
        //            foreach (Transform item in hit.transform)
        //            {
        //                var block = item.GetComponent<Block>();
        //                if (block != null)
        //                    blocks.Add(block);
        //            }
        ////////////////            foreach (var item in designations)
        ////////////////            {
        ////////////////                var block = blocks
        ////////////////                    .Where(x => x.GetComponent<MiningDesignation>() == null)
        ////////////////                    .ToList()
        ////////////////                    .Find(x => x.transform.localPosition.Round() ==
        ////////////////                    item.transform.localPosition.Round());
        ////////////////                if (block != null)
        ////////////////                {
        ////////////////                    if (block is not BlockDesignation)
        ////////////////                        item.GetComponent<MiningDesignation>().IsActive = true;
        ////////////////                    else
        ////////////////                        item.GetComponent<MiningDesignation>().IsActive = false;
        ////////////////                }
        ////////////////            }
        //            blocks.Clear();
        //        }
        //        oldLocalMousePos = localMousePos;
        //        yield return null;
        //    }
        //    foreach (Transform item in hit.transform)
        //    {
        //        var block = item.GetComponent<Block>();
        //        if (block != null)
        //            blocks.Add(block);
        //    }
        //    foreach (var item in designations)
        //    {
        //        var block = blocks.Find(x => x.transform.localPosition.Round() ==
        //            item.transform.localPosition.Round());
        //        if (block != null)
        //        {
        //            if (block is not BlockDesignation)
        //                block.IsMarkedForMining = true;
        //        }
        //    }
        //    DestroyDesignations(designations);
        //    UpdateShip(hit.transform.gameObject);
        //    GameManager.SwitchSetup();
        //}

        /// <summary>
        ///
        /// </summary>
        // tu sie zaczynaja nowe

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

        private void MoveCurrentDesignationToMouse()
        {
            if (_isPointerOverUI)
                return;
            MoveDesignation(_currentDesignation, GetMouseWorldPosition());
        }

        private void MoveDesignation(GameObject designation, Vector3 targetPosition)
        {
            Block closestBlock = FindClosestBlock(_currentDesignation, targetPosition);
            if (closestBlock != null ?
                    Vector2.Distance(closestBlock.Transform.position, targetPosition) < 1.5F :
                    false)
            {
                var v3relative = closestBlock.Transform.InverseTransformPoint(targetPosition);
                designation.transform.parent = closestBlock.Parent;
                designation.transform.localPosition =
                    closestBlock.Transform.localPosition +
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
            _updateCalled.AddListener(MoveCurrentDesignationToMouse);
            PlayerController.BuildingClick.AddListener(ActionType.Performed, PlaceDesignation);
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
        }

        private void ClearCurentDesignation()
        {
            Destroy(_currentDesignation);
            _currentDesignation = null;
        }

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
                    _temporalDesignations[^1].transform.localPosition = new Vector3(x, y, _prefabRotation);
                }
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
            PlayerController.BuildingRightClick.AddListener(
                ActionType.Performed, BreakDesignationGrid);
            PlayerController.BuildingPoint.AddListener(
                ActionType.Performed, CreateDesignationGrid);
            PlayerController.BuildingClick.AddListener(
                ActionType.Performed, FinalizeDesignationGrid);
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
                _temporalParent.rotation = _currentDesignation.transform.rotation;
                _temporalParent.parent = Camera.main.transform;
                _currentDesignation.transform.parent = _temporalParent;
            }
            else
                _temporalParent = _currentDesignation.transform.parent;
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

        private void UpdateShip()
        {
            _temporalParent?.GetComponent<Ship>()?.UpdateShip();
        }

        private void CreateFinalDesignations()
        {
            foreach (GameObject temporalDesignation in _temporalDesignations)
            {
                GameObject newDesignation = Instantiate(
                    _wallDesignationPrefab,
                    temporalDesignation.transform.position,
                    temporalDesignation.transform.rotation,
                    _temporalParent);
            }
        }

        private void CreateShipIfTemporalParentIsNotAShip()
        {
            if (!_temporalParent?.GetComponent<Ship>())
            {
                GameObject ship = Instantiate(
                    _shipPrefab,
                    _temporalParent.position,
                    _temporalParent.rotation,
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
                if (gameObject.name == "TemporalParent")
                {
                    Destroy(_temporalParent.gameObject);
                    _temporalParent = null;
                }
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

        private void StartMiningDesignation()
        {
            PlayerController.BuildingPoint
                .AddListener(ActionType.Performed, CreateMiningDesignation);
            PlayerController.BuildingClick
                .AddListener(ActionType.Performed, PlaceMiningDesignation);
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
            _updateCalled.AddListener(SetDesignationsActive);
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

        private void SetDesignationsActive()
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
            Debug.Log(hit.collider);
            return hit.collider?.GetComponent<Block>();
        }

        private void SetBuildingModeCancel(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Cancel);
            _selectedDesignationPrefab = _cancelDesignationPrefab;
        }

        private void ChangePrefabRotation(CallbackContext context)
        {
            _prefabRotation += 90F;
            if (_prefabRotation >= 360F)
                _prefabRotation = 0F;
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

        #endregion Private

        #region Unity

        private void Update()
        {
            InvokeUpdateEvent();
            CheckIfCursorOverUI();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            SubscribeToInputEvents();
            StartFromPreviousMode();
        }

        private void OnDisable()
        {
            ClearUpdateEventListeners();
            UnsubscribeFromInputEvents();
            ClearCurentDesignation();
            ClearTemporalParent();
            DestroyDesignations();
        }

        #endregion Unity
    }
}