using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using ScriptableObjects;

namespace Main
{
    public class UIController : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private InputActionAsset _inputAsset;

        [SerializeField]
        private TextMeshProUGUI _velocityText;

        [SerializeField]
        private GameObject _buttonPrefab;

        [SerializeField]
        private GameObject _shapePicker;

        [SerializeField]
        private ShapeListScriptableObject _shapeList;

        [SerializeField]
        private GameObject _shapeButtonPrefab;

        [SerializeField]
        private GameObject _contextualMenu;

        private GameObject _context = null;

        private bool _isPointerOverUI = false;

        #endregion Fields

        #region Private

        private void InvokeShapeChangedEvent(int shapeId)
        {
            OnShapeChanged.Invoke(shapeId);
        }

        private void CheckIfPointerIsOverUI()
        {
            _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        private void CreateShapeButtons()
        {
            foreach (ShapeScriptableObject shape in _shapeList.Shapes)
            {
                GameObject button = Instantiate(
                    _shapeButtonPrefab, _shapePicker.transform);
                button.transform.GetChild(0).GetComponent<Image>().sprite = shape.Sprite;
                button.GetComponent<Button>().onClick
                    .AddListener(() => InvokeShapeChangedEvent(shape.Id));
            }
        }

        private void SelectFocusedShip()
        {
            GameManager.SelectFocusedShip(_context);
            CloseContextualMenu();
        }

        private void SelectTarget()
        {
            GameManager.SelectTarget(_context);
            CloseContextualMenu();
        }

        private void OpenContextualMenu(CallbackContext context)
        {
            if (_isPointerOverUI)
                return;
            ClearContextualMenu();
            Vector2 mousePosition = PlayerController.DefaultPoint
                .ReadValue<Vector2>();
            _contextualMenu.SetActive(true);
            _contextualMenu.transform.position = mousePosition;
            _context = FindContext(mousePosition);
            CreateButton(SelectFocusedShip, "Steruj statkiem");
            CreateButton(SelectTarget, "Obierz jako cel");
        }

        private void ClearContextualMenu()
        {
            for (int i = 0; i < _contextualMenu.transform.childCount; i++)
                Destroy(_contextualMenu.transform.GetChild(i).gameObject);
        }

        // Zwraca statek, nie collider (w przyszłości prawdopodobnie do zmiany);
        private GameObject FindContext(Vector3 mousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            return hit.collider?.transform.parent.gameObject;
        }

        private void CreateButton(UnityAction action, string displayText)
        {
            GameObject button = Instantiate(_buttonPrefab, _contextualMenu.transform);
            button.GetComponent<Button>().onClick.AddListener(action);
            button.transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>().text = displayText;
        }

        private void CloseContextualMenu()
        {
            _contextualMenu.SetActive(false);
        }

        private void UpdateSpeedometer()
        {
            _velocityText.text = $"{GameManager.Speedometer:0.000} m/s";
        }

        private void SubscribeToInputEvents()
        {
            PlayerController.DefaultRightClick
                .AddListener(ActionType.Performed, OpenContextualMenu);
        }

        //private void UnsubscribeFromInputEvents()
        //{
        //    PlayerController.DefaultRightClick.performed += OpenContextualMenu;
        //}

        #endregion Private

        #region Public

        public static UnityEvent<int> OnShapeChanged = new();

        #endregion Public

        #region Unity

        private void OnEnable()
        {
            //SubscribeToInputEvents();
        }

        private void Start()
        {
            SubscribeToInputEvents();
            CreateShapeButtons();
        }

        private void Update()
        {
            CheckIfPointerIsOverUI();
        }

        private void FixedUpdate()
        {
            UpdateSpeedometer();
        }

        private void OnDisable()
        {
            //UnsubscribeFromInputEvents();
        }

        #endregion Unity
    }
}