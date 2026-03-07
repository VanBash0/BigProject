using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.Inventory;
using BigProject.UI;
using BigProject.Utilities;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
namespace BigProject.Gameplay.TownHall
{
    public class QuestActions : MonoBehaviour
    {
        [SerializeField]
        private string _noteItemName;
        [SerializeField]
        private float _firstInteractionClueTime;
        [SerializeField]
        private string _brokenKeyItemName;
        [SerializeField]
        private GameObject _firstTouchChestTrigger;
        [SerializeField]
        private Collider _chestCollider;
        [SerializeField]
        private float _checkPillarsClueTime;
        [SerializeField]
        private float _checkTownhallClueTime;
        [SerializeField]
        private GameObject _rune;

        private InventorySystem _inventory;
        private InventoryUI _inventoryUI;
        private Coroutine _firstInteractionClue;
        private Coroutine _interactPillarsClue;
        private GameplayManager _gameplayManager;
        RunesSystem _runesSystem;

        public void Init(InventorySystem inventory, InventoryUI inventoryUI, GameplayManager gameplayManager, RunesSystem runesSystem)
        {
            _inventory = inventory;
            _inventoryUI = inventoryUI;
            _gameplayManager = gameplayManager;
            _runesSystem = runesSystem;
            ExceptionUtilities.ThrowIfNull(_inventory, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Inventory System"));
            ExceptionUtilities.ThrowIfNull(_inventoryUI, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Inventory UI"));
            ExceptionUtilities.ThrowIfNull(_gameplayManager, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Gameplay Manager"));
            ExceptionUtilities.ThrowIfNull(_runesSystem, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Runes System"));

            // test
            _inventory.AddItemByName(_noteItemName);
            _inventory.AddItemByName("key_1");
            _inventory.AddItemByName("key_2");
            _inventory.AddItemByName("key_3");
            _inventory.AddItemByName("key_4");
        }

        private void Awake()
        {
            Assert.IsNotNull(_firstTouchChestTrigger, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "First touch trigger"));
            Assert.IsNotNull(_chestCollider, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Chest collider"));
            Assert.IsNotNull(_rune, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Rune"));
        }

        // Draw clue to note texture.
        public void AddRecordToNote(string recordName)
        {
            if (!_inventory.HasItemByName(_noteItemName))
            {
                GameLogManager.Info(String.Format(LogStr.INFO_QUEST, "add townhall empty note"));
                _inventory.AddItemByName(_noteItemName);
                _inventoryUI.SetNoteVisibility(true);
            }

            GameLogManager.Info(String.Format(LogStr.INFO_QUEST, $"add townhall note record {recordName}"));
            _inventory.AddItemModifier(recordName);
        }

        public void AllRecordsCollected()
        {
            ReplicaManager.ShowReplica("Все собрану яхуу");
        }

        public void FirstTownHallMeeting()
        {
            ReplicaManager.ShowReplica("Так, тут есть опоры");
            _firstInteractionClue = StartCoroutine(InteractionClueRoutune());
        }

        public void FirstInteraction()
        {
            if (_firstInteractionClue != null)
            {
                StopCoroutine(_firstInteractionClue);
            }

            if (_firstTouchChestTrigger != null)
            {
                ReplicaManager.ShowReplica("Теперь сундук");
                Destroy(_firstTouchChestTrigger);
            }
        }

        public void GetBrokenKey()
        {
            GameLogManager.Info(String.Format(LogStr.INFO_QUEST, "add broken key"));
            _inventory.AddItemByName(_brokenKeyItemName);
        }

        public void FirstTouchChest()
        {
            ReplicaManager.ShowReplica("Надо ключи чекать");
        }

        public void ReadyToInsertKey()
        {
            if (_firstTouchChestTrigger != null)
            {
                _firstTouchChestTrigger.SetActive(false);
            }

            _chestCollider.enabled = true;
        }

        public void BrokenKeyInserted()
        {
            _interactPillarsClue = StartCoroutine(CheckPillarsRoutine());
            //_gameplayManager.StateChanged += DisableChestColliderRoutine;
            StartCoroutine(DisableChestColliderRoutine());
        }

        public void PillarHasTouched()
        {
            if (_interactPillarsClue != null)
            {
                StopCoroutine(_interactPillarsClue);
            }
        }

        public void PuzzleWasPlayed()
        {
            //if (_gameplayManager.State == GameplayState.Play)
            //{
            //    _chestCollider.enabled = false;
            //}
            //else
            //{
                StartCoroutine(CheckPillarsRoutine());
                StartCoroutine(DisableChestColliderRoutine());
                //_gameplayManager.StateChanged += DisableChestColliderRoutine;
            //}
        }

        public void ShowRune()
        {
            _rune.SetActive(true);
        }

        public void GetRune()
        {
            _runesSystem.AddRune();
        }


        private IEnumerator InteractionClueRoutune()
        {
            yield return new WaitForSeconds(_firstInteractionClueTime);
            ReplicaManager.ShowReplica("Надо подергать что-то");
        }

        private IEnumerator CheckPillarsRoutine()
        {
            yield return new WaitForSeconds(_checkTownhallClueTime);
            ReplicaManager.ShowReplica("Посмотрим что еще тут есть");
            yield return new WaitForSeconds(_checkPillarsClueTime);
            ReplicaManager.ShowReplica("Колонны!");
        }

        private IEnumerator DisableChestColliderRoutine()
        {
            yield return new WaitUntil(() => _gameplayManager.State == GameplayState.Play);
            _chestCollider.enabled = false;
            //_chestCollider.enabled = false;
            //_gameplayManager.StateChanged -= DisableChestColliderRoutine;
        }
    }
}
