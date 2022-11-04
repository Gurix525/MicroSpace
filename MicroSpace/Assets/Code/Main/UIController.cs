using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Main
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private UIDocument _contextualMenu;
        [SerializeField] private UIDocument _speedometer;

        private Label _speedometerLabel;

        private GameObject _context = null;

        private void Start()
        {
            _speedometerLabel = _speedometer.rootVisualElement
                .Q("background")
                .Q("speed") as Label;
        }

        public void OpenContextualMenu()
        {
            _contextualMenu.enabled = true;
            var root = _contextualMenu.rootVisualElement.Q("root");
            root.style.top = Screen.height - Input.mousePosition.y;
            root.style.left = Input.mousePosition.x;
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
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                return hit.collider?.transform.parent.gameObject;// zwraca statek, nie collider (w przyszłości prawdopodobnie do zmiany);
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

        private void CloseContextualMenu()
        {
            _contextualMenu.enabled = false;
        }

        private void UpdateSpeedometer()
        {
            _speedometerLabel.text = $"{GameManager.Speedometer:0.000} m/s";
        }

        private void FixedUpdate()
        {
            UpdateSpeedometer();
        }
    }
}