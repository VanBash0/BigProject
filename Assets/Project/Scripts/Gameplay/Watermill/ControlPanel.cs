using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.Systems.QuestSystem;
using BigProject.UI;
using BigProject.Utilities;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.Watermill
{
    public enum ControlPanelState
    {
        Broken,
        Incompleted,
        Fixed,
        Completed
    }


    /// <summary>
    /// Levers Puzzle Panel
    /// </summary>
    public class ControlPanel : MonoBehaviour, IInteractable
    {
        [Header("Base settings")]
        [SerializeField]
        private GameObject _exitButton;
        [SerializeField]
        private CinemachineCamera _mechCamera;
        [SerializeField]
        private float _autoExitTime = 0.5f;
        [SerializeField]
        private Collider _collider;
        [SerializeField]
        private GearsHandler _gearsHandler;

        [Header("Player settings")]
        [SerializeField]
        private QuestActionHandlersContainer _actions;
        [SerializeField]
        private SkinnedMeshRenderer _playerRenderer;
        [SerializeField]
        private Collider _playerCollider;
        [SerializeField]
        private int _noteItemId;

        [Header("Levers settings")]
        [SerializeField]
        private GameObject _brokenLever;
        [SerializeField]
        private GameObject _repairedLeverHolder;
        [SerializeField]
        private GameObject _repairedLever;
        [SerializeField]
        private float _brokenLeverOffset = 1f;
        [SerializeField]
        private float _brokenLeverRemoveTime = 2f;
        [SerializeField]
        private float _repairedLeverInstallTime = 1f;
        [SerializeField]
        private int _brokenLeverItemId;
        [SerializeField]
        private float _leverMoveTime = 1f;
        [SerializeField]
        private float _leverStaggerTime = 0.2f;
        [SerializeField]
        private float _staggerDistance = 0.1f;
        [SerializeField]
        private List<Lever> _levers;
        [SerializeField]
        private List<Transform> _leversPoints;

        private PlayerInputHandler _inputHandler;
        private InventorySystem _inventory;
        private InventoryUI _inventoryUI;
        private IControlPanelState _state;
        private bool _isActive = false;
        private bool _isLeverMoving;
        private Vector2 _deltaInversion = new(-1f, 1f);
        private GameplayManager _gameplayManager;

        public void Init(GameplayManager gameplayManager, PlayerInputHandler inputHandler, InventorySystem inventory, InventoryUI inventoryUI)
        {
            _gameplayManager = gameplayManager;
            _inputHandler = inputHandler;
            _inventory = inventory;
            _inventoryUI = inventoryUI;
            ExceptionUtilities.ThrowIfNull(_gameplayManager, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Gameplay Manager"));
            ExceptionUtilities.ThrowIfNull(_inputHandler, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Player Input Handler"));
            ExceptionUtilities.ThrowIfNull(_inventory, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Inventory System"));
            ExceptionUtilities.ThrowIfNull(_inventoryUI, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Inventory UI"));
            ChangeState(ControlPanelState.Broken);
        }

        public void Awake()
        {
            Assert.IsNotNull(_exitButton, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Exit button"));
            Assert.IsNotNull(_actions, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Actions Container"));
            Assert.IsNotNull(_playerRenderer, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Player Renderer"));
            Assert.IsNotNull(_playerCollider, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Player Collider"));
            Assert.IsNotNull(_mechCamera, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Mech Camera"));
            Assert.IsNotNull(_brokenLever, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Broken Lever"));
            Assert.IsNotNull(_repairedLeverHolder, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Repaired Lever Holder"));
            Assert.IsNotNull(_repairedLever, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Repaired Lever"));
            Assert.IsNotNull(_gearsHandler, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Gears Handler"));
            Assert.IsNotNull(_collider, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Collider"));
        }

        private void OnDestroy()
        {
            _state?.Dispose();
        }

        public void Interact()
        {
            if (_state != null && _state.IsReady)
            {
                ActivateMiniGame();
            }
        }

        public async Awaitable MoveLever(Transform lever, Vector3 targetPosition, float time, CancellationToken ct)
        {
            if (_isLeverMoving)
            {
                return;
            }

            _isLeverMoving = true;
            lever.DOLocalMove(targetPosition, time);
            await Awaitable.WaitForSecondsAsync(time + 0.1f, cancellationToken: ct);
            _isLeverMoving = false;
        }

        public void ActivateMiniGame()
        {
            if (!_isActive)
            {
                StartCoroutine(ActivateRoutine());
            }
        }

        public void DeactivateMiniGame()
        {
            if (_isActive)
            {
                StartCoroutine(DeactivateRoutine());
            }
        }

        private IEnumerator ActivateRoutine()
        {
            _mechCamera.enabled = true;
            _gameplayManager.ChangeState(GameplayState.MiniGame);
            yield return new WaitForFixedUpdate(); // Wait one frame for calculate camera transition.
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime * 0.85f);
            _playerRenderer.enabled = false;
            _playerCollider.enabled = false;
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime);
            _collider.enabled = false;
            _isActive = true;
            _exitButton.SetActive(true);
            _inventoryUI.SetNoteVisibility(true);
            _state?.Start();
        }

        private IEnumerator DeactivateRoutine()
        {
            _exitButton.SetActive(false);
            _inventoryUI.SetNoteVisibility(false);
            _state?.Stop();
            _isActive = false;
            yield return new WaitForSeconds(_autoExitTime);
            _mechCamera.enabled = false;
            yield return new WaitForFixedUpdate(); // Wait one frame for calculate camera transition.
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime * 0.15f);
            _playerRenderer.enabled = true;
            _playerCollider.enabled = true;
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime);
            _collider.enabled = true;
            _gameplayManager.ChangeState(GameplayState.Play);
        }

        private void OnClicked()
        {
            if (!_isActive)
            {
                return;
            }

            _state?.OnClicked();
        }

        private void OnSwiped(Vector2 delta)
        {
            if (!_isActive)
            {
                return;
            }

            _state?.OnSwiped(delta * _deltaInversion);
        }

        private void OnUnclicked()
        {
            if (!_isActive)
            {
                return;
            }

            _state?.OnUnclicked();
        }

        public void ChangeState(ControlPanelState newState)
        {
            _state?.Dispose();
            GameLogManager.Info($"Control panel change state to: {newState}");

            switch (newState)
            {
                case ControlPanelState.Broken:
                    _state = new ControlPanelStateBroken(this, _inputHandler, _brokenLever, 
                        _brokenLeverOffset, _brokenLeverRemoveTime, _brokenLeverItemId, _actions["GetBrokenLever"], _inventory);
                    break;
                case ControlPanelState.Incompleted:
                    _state = new ControlPanelStateIncompleted(this, _inputHandler, _repairedLeverHolder, 
                        _repairedLever, _repairedLeverInstallTime, _actions["InstallLever"], _inventory);
                    break;
                case ControlPanelState.Fixed:
                    _state = new ControlPanelStateFixed(this, _inputHandler, _leversPoints, _levers, _leverMoveTime,
                        _leverStaggerTime, _staggerDistance, _noteItemId, _gearsHandler, _actions["ActivateMech"], _inventory);

                    if (_isActive)
                    {
                        _inventoryUI.SetNoteVisibility(true);
                    }

                    break;
                default:
                    _state = null;
                    break;
            }

            _state?.Start();
        }

        public void ApplyItem(Item item)
        {
            if (_isActive)
            {
                _state?.ApplyItem(item);
            }
        }

        private void OnEnable()
        {
            _inputHandler.MiniGameClick += OnClicked;
            _inputHandler.MiniGameSwipe += OnSwiped;
            _inputHandler.MiniGameUnclick += OnUnclicked;
        }

        private void OnDisable()
        {
            _inputHandler.MiniGameClick -= OnClicked;
            _inputHandler.MiniGameSwipe -= OnSwiped;
            _inputHandler.MiniGameUnclick -= OnUnclicked;
        }
    }
}