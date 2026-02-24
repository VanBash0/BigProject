using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.UI;
using BigProject.Utilities;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace BigProject.Gameplay.TownHall
{
    public enum ChestPuzzleState
    {
        Closed,
        
        Open
    }

    public class ChestPuzzle : MonoBehaviour, IInteractable
    {
        [Header("Base settings")]
        [SerializeField]
        private List<Transform> _keysHolders;
        [SerializeField]
        private List<GameObject> _keysPrefabs;
        [SerializeField]
        private List<int> _correctKeysIds;
        [SerializeField]
        private CinemachineCamera _chestCamera;
        [SerializeField]
        private GameObject _exitButton;
        [SerializeField]
        private float _autoExitTime = 0.5f;
        [SerializeField]
        private Collider _collider;
        [SerializeField]
        private Transform _chestCup;
        [SerializeField]
        private Image _note; 

        [Header("Player settings")]
        //[SerializeField]
        //private QuestActionHandlersContainer _actions;
        [SerializeField]
        private SkinnedMeshRenderer _playerRenderer;
        [SerializeField]
        private Collider _playerCollider;
        [SerializeField]
        private int _noteItemId;

        private GameplayManager _gameplayManager;
        private InventorySystem _inventory;
        private InventoryUI _inventoryUI;
        private bool _isActive = false;
        private List<int> _keysIds;
        private List<GameObject> _keys = new();
        private List<string> _keysNames = new();

        public void Init(GameplayManager gameplayManager, PlayerInputHandler inputHandler, InventorySystem inventory, InventoryUI inventoryUI)
        {
            _gameplayManager = gameplayManager;
            _inventory = inventory;
            _inventoryUI = inventoryUI;
        }

        private void Awake()
        {
            Assert.IsNotNull(_chestCamera, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Cinemachine Camera"));
            Assert.IsNotNull(_exitButton, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Exit Button"));
            Assert.IsNotNull(_collider, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Collider"));
            //Assert.IsTrue(_keysHolders.Count == _keysPrefabs.Count && _keysPrefabs.Count == _correctKeysIds.Count, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name,
            //    "Keys holder and prefabs and ids containers must have same size"));
            _keysIds = Enumerable.Repeat(-1, _keysHolders.Count).ToList();
        }

        public void Interact()
        {
            ActivateMiniGame();
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
            _chestCamera.enabled = true;
            _gameplayManager.ChangeState(GameplayState.MiniGame);
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime * 0.85f);
            _playerRenderer.enabled = false;
            _playerCollider.enabled = false;
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime);
            _collider.enabled = false;
            _isActive = true;
            _exitButton.SetActive(true);

            // test only
            _inventoryUI.SetNoteVisibility(false);

            if (_inventory.HasItemByName("townhall_note"))
            {
                Item note = _inventory.GetItemByName("townhall_note");
                _note.sprite = note._noteSprite;
                _note.enabled = true;
            }
        }

        private IEnumerator DeactivateRoutine()
        {
            _exitButton.SetActive(false);
            _inventoryUI.SetNoteVisibility(false);
            _isActive = false;
            yield return new WaitForSeconds(_autoExitTime);
            _chestCamera.enabled = false;
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime * 0.15f);
            _playerRenderer.enabled = true;
            _playerCollider.enabled = true;
            yield return new WaitForSeconds(GameplayUtilities.CurrentCameraTransitionTime);
            _collider.enabled = true;
            _gameplayManager.ChangeState(GameplayState.Play);

            // test only
            _note.enabled = false;
        }

        public void InstallKey(string itemName, int keyHolderId, int keyPrefabId)
        {
            Transform keyHolder = _keysHolders.ElementAtOrDefault(keyHolderId);

            if (keyHolder == null)
            {
                Debug.LogError(string.Format(LogStr.ERROR_QUEST, $"chest puzzle unable to use key holder {keyHolderId}"));
                return;
            }

            if (_keysIds[keyHolderId] >= 0)
            {
                GameLogManager.Info(string.Format(LogStr.INFO_QUEST, $"chest puzzle try to fill busy key holder {keyHolderId}"));
                return;
            }

            GameObject keyPrefab = _keysPrefabs.ElementAtOrDefault(keyPrefabId);

            if (keyPrefab == null)
            {
                Debug.LogError(string.Format(LogStr.ERROR_QUEST, $"chest puzzle unable to instantiate key prefab {keyPrefabId}"));
                return;
            }

            GameObject key = Instantiate(keyPrefab, keyHolder);
            key.transform.localPosition = new(0f, 0f, -0.5f);
            key.transform.eulerAngles = new(0f, 90f, -90f);
            key.transform.localScale = new(2f, 2f, 2f);
            _keys.Add(key);
            _inventory.RemoveItemByName(itemName);
            _inventoryUI.SetNoteVisibility(true);
            _keysIds[keyHolderId] = keyPrefabId;
            _keysNames.Add(itemName);

            if (IsAllKeysInside())
            {
                ApplyKeys();
            }
        }

        private void ApplyKeys()
        {
            if (IsCorrectKeys())
            {
                Vector3 targetAngles = _chestCup.transform.localEulerAngles;
                targetAngles.x -= 90f;
                _chestCup.DOLocalRotate(targetAngles, 2f);

                foreach (GameObject key in _keys)
                {
                    targetAngles = key.transform.localEulerAngles;
                    targetAngles.x += 180f;
                    key.transform.DOLocalRotate(targetAngles, 2f);
                }

                DeactivateMiniGame();
            }
            else
            {
                for (int i = 0; i < _keysIds.Count; i++)
                {
                    _keysIds[i] = -1;
                }

                foreach (GameObject key in _keys)
                {
                    Destroy(key);
                }

                _keys.Clear();

                foreach (string itemName in _keysNames)
                {
                    _inventory.AddItemByName(itemName);
                }

                _keysNames.Clear();
            }
        }

        private bool IsCorrectKeys()
        {
            for (int i = 0; i < _keys.Count; i++)
            {
                if (_keysIds[i] != _correctKeysIds[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAllKeysInside() => !_keysIds.Contains(-1);
    }
}