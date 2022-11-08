using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

namespace Main
{
    public class UIController : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private UIDocument _contextualMenu;

        [SerializeField]
        private UIDocument _speedometer;

        [SerializeField]
        private InputActionAsset _inputAsset;

        private Label _speedometerLabel;

        private GameObject _context = null;

        #endregion Fields

        #region Private

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
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = PlayerController.DefaultPoint.ReadValue<Vector2>();
                _contextualMenu.enabled = true;
                var root = _contextualMenu.rootVisualElement.Q("root");
                root.style.top = Screen.height - mousePosition.y;
                root.style.left = mousePosition.x;
                root.Clear();

                _context = findContext(); // Casting ray to find clicked object

                Button button = new(SelectFocusedShip);
                button.text = "Przejmij statek";
                button.style.fontSize = 40;
                root.Add(button);

                Button button2 = new(SelectTarget);
                button2.text = "Obierz jako cel";
                button2.style.fontSize = 40;
                root.Add(button2);

                GameObject findContext()
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                    return hit.collider?.transform.parent.gameObject;// zwraca statek, nie collider (w przyszłości prawdopodobnie do zmiany);
                }
            }
        }

        private void CloseContextualMenu()
        {
            _contextualMenu.enabled = false;
        }

        private void UpdateSpeedometer()
        {
            _speedometerLabel.text = $"{GameManager.Speedometer:0.000} m/s";
        }

        private void SubscribeToInputEvents()
        {
            PlayerController.DefaultRightClick.performed += OpenContextualMenu;
        }

        //private void UnsubscribeFromInputEvents()
        //{
        //    PlayerController.DefaultRightClick.performed += OpenContextualMenu;
        //}

        #endregion Private

        #region Unity

        private void Awake()
        {
        }

        private void OnEnable()
        {
            //SubscribeToInputEvents();
        }

        private void Start()
        {
            SubscribeToInputEvents();
            _speedometerLabel = _speedometer.rootVisualElement
                .Q("background")
                .Q("speed") as Label;
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