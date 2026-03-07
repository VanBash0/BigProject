using BigProject.Gameplay.Common;
using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.Inventory;
using BigProject.Systems.QuestSystem;
using BigProject.UI;
using BigProject.Utilities;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.TownHall
{
    public class ChestPuzzle : MonoBehaviour, IInteractable, ISavable
    {
        [SerializeField]
        private MiniGameActivator _activator;
        [SerializeField]
        private List<Transform> _keysHolders;
        [SerializeField]
        private List<GameObject> _keysPrefabs;
        [SerializeField]
        private List<int> _correctKeysIds;
        [SerializeField]
        private Transform _chestCup;
        [SerializeField]
        private string _brokenKeyItemName;
        [SerializeField]
        private float _brokenKeyMovingTime;
        [SerializeField]
        private QuestActionHandlersContainer _actionHandlers;
        [SerializeField]
        private string _actionTryBrokenKeyName;
        [SerializeField]
        private string _actionOpenName;
        [SerializeField]
        private string _noteItemName;

        private InventorySystem _inventory;
        private InventoryUI _inventoryUI;
        private List<int> _keysIds;
        private List<GameObject> _keys = new();
        private List<string> _keysNames = new();
        private DataToSave _dataToSave = new();
        private ProgressManager _progressManager;

        private readonly Vector3 BROKEN_KEY_MOVING_OFFSET = new(0f, -1f, -0.5f);
        private readonly Vector3 BROKEN_KEY_ROTATION_OFFSET = new(-40f, 0f, 0f);

        [Serializable]
        private class DataToSave
        {
            [Serializable]
            public class HolderKeyPair
            {
                public string keyName;
                public int holderId;
                public int keyId;
            }

            public List<HolderKeyPair> keysData = new();
        }

        public string Key => "Townhall_data";

        public object SavingData => _dataToSave;

        public void Init(InventorySystem inventory, InventoryUI inventoryUI, ProgressManager progressManager)
        {
            _inventory = inventory;
            _inventoryUI = inventoryUI;
            _progressManager = progressManager;
            ExceptionUtilities.ThrowIfNull(_inventory, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Gameplay Manager"));
            ExceptionUtilities.ThrowIfNull(_inventoryUI, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Player Input Handler"));
            ExceptionUtilities.ThrowIfNull(_progressManager, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Progress Manager"));
        }

        private void Awake()
        {
            Assert.IsNotNull(_activator, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Activator"));
            Assert.IsNotNull(_chestCup, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Chest cup"));
            Assert.IsNotNull(_actionHandlers, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest action handler"));
            _keysIds = Enumerable.Repeat(-1, _keysHolders.Count).ToList();
        }

        private void Start()
        {
            _progressManager.LoadAdditionalData(this);
        }

        public void OnLoad()
        {
            foreach (DataToSave.HolderKeyPair holderKeyPair in _dataToSave.keysData)
            {
                SetKeyToHolder(holderKeyPair.keyName, holderKeyPair.holderId, holderKeyPair.keyId);
            }
        }

        public void Interact()
        {
            _activator.ActivateMiniGame();
        }

        public void InstallKey(string itemName, int keyHolderId, int keyPrefabId)
        {
            if (!SetKeyToHolder(itemName, keyHolderId, keyPrefabId))
            {
                Debug.LogWarning(String.Format(LogStr.WARNING_QUEST, "chest puzzle unable to set key to holder"));
                return;
            }

            if (_inventory.HasItemByName(itemName))
            {
                _inventory.RemoveItemByName(itemName);
            }

            if (itemName.Equals(_brokenKeyItemName))
            {
                RemoveBrokenKey();
                return;
            }

            _dataToSave.keysData.Add(new DataToSave.HolderKeyPair() 
            {
                keyName = itemName,
                holderId = keyHolderId, 
                keyId = keyPrefabId 
            });

            if (IsAllKeysInside())
            {
                ApplyKeys();
            }
        }

        private void RemoveBrokenKey()
        {
            _keysNames.Clear();
            GameObject key = _keys.ElementAtOrDefault(0);
            _keys.Clear();
            ResetKeysIds();
            
            if (key != null)
            {
                StartCoroutine(RemoveBrokenKeyRoutine(key.transform));
            }
        }

        private IEnumerator RemoveBrokenKeyRoutine(Transform key)
        {
            Vector3 newPosition = key.localPosition;
            newPosition += BROKEN_KEY_MOVING_OFFSET;
            Vector3 newAngles = key.localEulerAngles;
            newAngles += BROKEN_KEY_ROTATION_OFFSET;
            key.DOLocalRotate(newAngles, _brokenKeyMovingTime);
            key.DOLocalMove(newPosition, _brokenKeyMovingTime);
            _actionHandlers[_actionTryBrokenKeyName].MakeTransition(0);
            yield return new WaitForSeconds(_brokenKeyMovingTime + 0.1f);
            Destroy(key.gameObject);
            _activator.DeactivateMiniGame();
        }

        private bool SetKeyToHolder(string itemName, int keyHolderId, int keyPrefabId)
        {
            Transform keyHolder = _keysHolders.ElementAtOrDefault(keyHolderId);

            if (keyHolder == null)
            {
                Debug.LogError(string.Format(LogStr.ERROR_QUEST, $"chest puzzle unable to use key holder {keyHolderId}"));
                return false;
            }

            if (_keysIds[keyHolderId] >= 0)
            {
                GameLogManager.Info(string.Format(LogStr.INFO_QUEST, $"chest puzzle try to fill busy key holder {keyHolderId}"));
                return false;
            }

            GameObject keyPrefab = _keysPrefabs.ElementAtOrDefault(keyPrefabId);

            if (keyPrefab == null)
            {
                Debug.LogError(string.Format(LogStr.ERROR_QUEST, $"chest puzzle unable to instantiate key prefab {keyPrefabId}"));
                return false;
            }

            GameObject key = Instantiate(keyPrefab, keyHolder);
            key.transform.localPosition = new(0f, 0f, -0.5f);
            key.transform.eulerAngles = new(0f, 90f, -90f);
            key.transform.localScale = new(2f, 2f, 2f);
            _keys.Add(key);
            _keysIds[keyHolderId] = keyPrefabId;
            _keysNames.Add(itemName);
            return true;
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

                _inventory.RemoveItemByName(_noteItemName);
                _actionHandlers[_actionOpenName].MakeTransition(0);
                _activator.DeactivateMiniGame();
            }
            else
            {
                ResetKeys();
            }
        }

        private void ResetKeysIds()
        {
            for (int i = 0; i < _keysIds.Count; i++)
            {
                _keysIds[i] = -1;
            }
        }

        private void ResetKeys()
        {
            ResetKeysIds();

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
            _dataToSave.keysData.Clear();
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

        private void OnActivated(bool isActivated)
        {
            if (!isActivated && _actionHandlers[_actionOpenName].CurrentState != QuestActionState.Completed)
            {
                ResetKeys();
            }
        }

        private void OnEnable()
        {
            _activator.Activated += OnActivated;
            _actionHandlers[_actionOpenName].StateChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            _activator.Activated -= OnActivated;
            _actionHandlers[_actionOpenName].StateChanged -= OnStateChanged;
        }

        private void OnStateChanged()
        {
            if (_actionHandlers[_actionOpenName].CurrentState == QuestActionState.Completed)
            {
                ReplicaManager.ShowReplica("Óðà!!!");
                _progressManager.SaveAdditionalData(this);
            }
        }
    }
}