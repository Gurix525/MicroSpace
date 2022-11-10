using Attributes;
using ExtensionMethods;
using Ships;
using System;
using System.Collections;
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

        private GameObject _temporalParent;

        private List<GameObject> _temporalDesignations = new();

        private UnityEvent _updateCalled = new();

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

        private Vector2 GetMouseWorldPosition()
        {
            Vector2 position = PlayerController.BuildingPoint.ReadValue<Vector2>();
            position = Camera.main.ScreenToWorldPoint(position);
            return position;
        }

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
        //            foreach (var item in designations)
        //            {
        //                var block = blocks
        //                    .Where(x => x.GetComponent<MiningDesignation>() == null)
        //                    .ToList()
        //                    .Find(x => x.transform.localPosition.Round() ==
        //                    item.transform.localPosition.Round());
        //                if (block != null)
        //                {
        //                    if (block is not BlockDesignation)
        //                        item.GetComponent<MiningDesignation>().IsActive = true;
        //                    else
        //                        item.GetComponent<MiningDesignation>().IsActive = false;
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
        //            if (block is not BlockDesignation)
        //                block.IsMarkedForMining = true;
        //        }
        //    }
        //    DestroyDesignations(designations);
        //    UpdateShip(hit.transform.gameObject);
        //    GameManager.SwitchSetup();
        //}

        //private static bool AreDesignationsObstructed(List<GameObject> designations)
        //{
        //    foreach (var item in designations)
        //        if (item.GetComponent<BlockDesignation>().IsObstructed)
        //            return true;
        //    return false;
        //}

        //private static bool IsDesignationObstructed(GameObject designation)
        //{
        //    return designation.GetComponent<BlockDesignation>().IsObstructed;
        //}

        /// <summary>
        ///
        /// </summary>
        // tu sie zaczynaja nowe

        private void DestroyDesignations()
        {
            for (int i = 0; i < _temporalDesignations.Count; i++)
            {
                Destroy(_temporalDesignations[0]);
                _temporalDesignations.RemoveAt(0);
                i--;
            }
        }

        private void CreateDesignations(Vector3 originalPos,
            List<GameObject> designations)
        {
            DestroyDesignations();
            Vector3 localMousePos = _temporalParent.transform
                .InverseTransformPoint(GetMouseWorldPosition());
            localMousePos = localMousePos.Round();
            ClampDistance(originalPos, ref localMousePos, _maxDesignDistance);
            int lesserX = 0;
            int lesserY = 0;
            int greaterX = 0;
            int greaterY = 0;
            if (localMousePos.x < originalPos.x)
            {
                lesserX = (int)localMousePos.x;
                greaterX = (int)originalPos.x;
            }
            else
            {
                greaterX = (int)localMousePos.x;
                lesserX = (int)originalPos.x;
            }
            if (localMousePos.y < originalPos.y)
            {
                lesserY = (int)localMousePos.y;
                greaterY = (int)originalPos.y;
            }
            else
            {
                greaterY = (int)localMousePos.y;
                lesserY = (int)originalPos.y;
            }
            for (int x = lesserX; x <= greaterX; x++)
                for (int y = lesserY; y <= greaterY; y++)
                {
                    designations.Add(Instantiate(

                        _temporalDesignationPrefab,
                        _temporalParent.transform));
                    designations[^1].transform.localPosition = new Vector3(x, y, _prefabRotation);
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
                .Where(x => x != designation.GetComponent<BlockDesignation>())
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
                designation.transform.parent = null;
                designation.transform.position = targetPosition;
                designation.transform.rotation = Quaternion.identity;
            }
        }

        private void StartBlockDesignation()
        {
            _currentDesignation = Instantiate(
                _temporalDesignationPrefab,
                GetMouseWorldPosition(),
                Quaternion.Euler(0, 0, _prefabRotation),
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
            _buildingMode = mode;
        }

        private void ClearCurentDesignation()
        {
            Destroy(_currentDesignation);
            _currentDesignation = null;
        }

        private void CreateDesignationGrid(CallbackContext context)
        {
            CreateDesignations(_currentDesignation.transform.localPosition, _temporalDesignations);
        }

        private void StartFromPreviousMode(CallbackContext context)
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
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SetTemporalParent();
                ClearInputActionsListeners();
                ClearUpdateEventListeners();
                PlayerController.BuildingRightClick.AddListener(
                    ActionType.Performed, StartFromPreviousMode);
                PlayerController.BuildingPoint.AddListener(
                    ActionType.Performed, CreateDesignationGrid);
                PlayerController.BuildingClick.AddListener(
                    ActionType.Performed, FinalizeDesignationGrid);
            }
        }

        private void SetTemporalParent()
        {
            if (_currentDesignation.transform.parent == null)
            {
                _temporalParent = new();
                _temporalParent.transform.position = _currentDesignation.transform.position;
                _currentDesignation.transform.parent = _temporalParent.transform;
            }
            else
                _temporalParent = _currentDesignation.transform.parent.gameObject;
        }

        private void FinalizeDesignationGrid(CallbackContext obj)
        {
            DestroyDesignations();
            StartFromPreviousMode(new());
        }

        private void SetBuildingModeWall(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Wall);
            StartBlockDesignation();
        }

        private void SetBuildingModeFloor(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Floor);
            StartBlockDesignation();
        }

        private void SetBuildingModeEquipment(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Equipment);
        }

        private void SetBuildingModeMining(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Mining);
        }

        private void SetBuildingModeCancel(CallbackContext context)
        {
            SetBuildingMode(BuildingMode.Cancel);
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
            _updateCalled?.Invoke();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            SubscribeToInputEvents();
            StartFromPreviousMode(new());
        }

        private void OnDisable()
        {
            ClearUpdateEventListeners();
            UnsubscribeFromInputEvents();
            ClearCurentDesignation();
        }

        #endregion Unity
    }
}