using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigProject.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private InputSystemActions _inputActions;
        
        //Player Actions
        public Action Click;
        public Action OpenMap;
        public Action OpenMenu;

        //UIActions
        //...

        //Mini-game Actions
        public Action MiniGameClick;
        public Action MiniGameRightClick;
        public Action<Vector2> MiniGameSwipe;
        public Action MiniGameUnclick;

        private void Awake()
        {
            _inputActions = new InputSystemActions();
        }

        private void Start()
        {
            _inputActions.Player.Enable();
            _inputActions.UI.Enable();
            _inputActions.MiniGame.Disable();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
            _inputActions.Player.Click.performed += OnClick;
            _inputActions.Player.OpenMap.performed += OnOpenedMap;
            _inputActions.Player.OpenMenu.performed += OnOpenedMenu;

            _inputActions.MiniGame.Click.performed += OnMiniGameClick;
            _inputActions.MiniGame.RightClick.performed += OnMiniGameRightClick;
            _inputActions.MiniGame.Swipe.performed += OnMiniGameSwipe;
            _inputActions.MiniGame.Click.canceled += OnMiniGameUnclick;
        }
        private void OnDisable()
        {
            _inputActions.Disable();
            _inputActions.Player.Click.performed -= OnClick;
            _inputActions.Player.OpenMap.performed -= OnOpenedMap;
            _inputActions.Player.OpenMenu.performed -= OnOpenedMenu;

            _inputActions.MiniGame.Click.performed -= OnMiniGameClick;
            _inputActions.MiniGame.RightClick.performed -= OnMiniGameRightClick;
            _inputActions.MiniGame.Swipe.performed -= OnMiniGameSwipe;
            _inputActions.MiniGame.Click.canceled -= OnMiniGameUnclick;
        } 

        private void OnOpenedMenu(InputAction.CallbackContext obj)
        {
            OpenMenu?.Invoke();
        }

        private void OnClick(InputAction.CallbackContext obj)
        {
            Click?.Invoke();
        }

        private void OnOpenedMap(InputAction.CallbackContext obj)
        { 
            OpenMap?.Invoke();
        }
        private void OnMiniGameClick(InputAction.CallbackContext obj)
        {
            MiniGameClick?.Invoke();
        }

        private void OnMiniGameRightClick(InputAction.CallbackContext obj)
        {
            MiniGameRightClick?.Invoke();
        }

        private void OnMiniGameSwipe(InputAction.CallbackContext obj)
        {
            MiniGameSwipe?.Invoke(obj.ReadValue<Vector2>());
        }

        private void OnMiniGameUnclick(InputAction.CallbackContext _)
        {
            MiniGameUnclick?.Invoke();
        }

        public void SwitchToPlayerActionMap()
        {
            _inputActions.Player.Enable();
            _inputActions.MiniGame.Disable();
        }

        public void SwitchToMiniGameActionMap()
        {
            _inputActions.MiniGame.Enable();
            _inputActions.Player.Disable();
        }

        public Vector2 GetMousePosition()
        {
            return _inputActions.UI.Point.ReadValue<Vector2>();
        }        
    }
}

