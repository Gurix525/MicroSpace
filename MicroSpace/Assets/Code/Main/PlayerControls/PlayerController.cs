using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput _playerInput;

        private InputAction _pauseAction;

        private void Awake()
        {
            _pauseAction = _playerInput.actions["Pause"];
        }

        private void OnEnable()
        {
            _pauseAction.performed += SwitchPause;
        }

        private void OnDisable()
        {
            _pauseAction.performed -= SwitchPause;
        }

        private void SwitchPause(InputAction.CallbackContext context)
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            Debug.Log("KURWA");
        }
    }
}