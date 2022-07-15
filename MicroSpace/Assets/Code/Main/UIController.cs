using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Main
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private UIDocument _contextualMenu;

        public void OpenContextualMenu()
        {
            _contextualMenu.enabled = true;
            var root = _contextualMenu.rootVisualElement.Q("root");
            root.style.top = Screen.height - Input.mousePosition.y;
            root.style.left = Input.mousePosition.x;
            root.Clear();
            Button button = new(() => CloseContextualMenu());
            button.text = "Disa jebac elo";
            root.Add(button);
            Debug.Log(root);
        }

        private void CloseContextualMenu()
        {
            _contextualMenu.enabled = false;
        }
    }
}