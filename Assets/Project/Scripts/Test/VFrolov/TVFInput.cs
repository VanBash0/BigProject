using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zenject;

namespace BigProject.Test.VFrolov
{
    /// <summary>
    /// Ввод для тестовой сцены.
    /// </summary>
    public class TVFInput : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset _actions;
        private InputAction _clickAction, _cursorMoveAction, _exitAction, _saveAction, _loadAction;
        private Vector2 _cursorPosition = new();
        public UnityAction<GameObject> GOClicked;
        public UnityAction<Vector2> CursorMoved;

        [Inject]
        private ProgressManager _progressManager;

        private void Awake()
        {
            _clickAction = _actions.FindAction("Click");
            _cursorMoveAction = _actions.FindAction("CursorMove");
            _exitAction = _actions.FindAction("Exit");
            _saveAction = _actions.FindAction("Save");
            _loadAction = _actions.FindAction("Load");
        }

        private void OnEnable()
        {
            _clickAction.Enable();
            _cursorMoveAction.Enable();
            _exitAction.Enable();
            _saveAction.Enable();
            _loadAction.Enable();
            _clickAction.performed += OnClick;
            _cursorMoveAction.performed += OnCursorMove;
            _exitAction.performed += OnExit;
            _saveAction.performed += OnSave;
            _loadAction.performed += OnLoad;
        }

        private void OnDisable()
        {
            _clickAction.performed -= OnClick;
            _cursorMoveAction.performed -= OnCursorMove;
            _exitAction.performed -= OnExit;
            _saveAction.performed -= OnSave;
            _loadAction.performed -= OnLoad;
            _clickAction.Disable();
            _cursorMoveAction.Disable();
            _exitAction.Disable();
            _saveAction.Disable();
            _loadAction.Disable();
        }

        private void OnClick(InputAction.CallbackContext _)
        {
            Ray ray = Camera.main.ScreenPointToRay(_cursorPosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
                GOClicked?.Invoke(hit.collider.gameObject);
        }

        private void OnCursorMove(InputAction.CallbackContext context)
        {
            _cursorPosition = context.ReadValue<Vector2>();
            CursorMoved?.Invoke(_cursorPosition);
        }

        private void OnExit(InputAction.CallbackContext _)
        {
            Application.Quit();
        }

        private void OnSave(InputAction.CallbackContext _)
        {
            _progressManager.SaveProgress();
        }

        private void OnLoad(InputAction.CallbackContext _)
        {
            _progressManager.LoadProgress();
        }
    }
}