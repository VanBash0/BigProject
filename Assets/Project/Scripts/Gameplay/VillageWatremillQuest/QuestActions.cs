using BigProject.Gameplay.Watermill;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.VillageWatermillQuest
{
    public class QuestActions : MonoBehaviour
    {
        [SerializeField]
        private int _noteItemId;
        [SerializeField]
        private int _brokenLeverItemId;
        [SerializeField]
        private int _repairedLeverItemId;
        [SerializeField]
        private GameObject _miller;
        [SerializeField]
        private GameObject _chests;
        [SerializeField]
        private Vector3 _millerFinalPosition;
        [SerializeField]
        private GearsHandler _millWheelHandler;
        [SerializeField]
        private GameObject _runeBar;

        private InventorySystem _inventory;
        private GameLogManager _logger;
       // private 

        private void Start()
        {
            _inventory = ServiceLocator.GetService<InventorySystem>();
            _logger = ServiceLocator.GetService<GameLogManager>();
            Assert.IsNotNull(_inventory, $"{gameObject.name} unable to get inventory system.");
            Assert.IsNotNull(_logger, $"{gameObject.name} unable to get log manager.");
        }

        public void GetWatermillNote()
        {
            _logger.Info("Add mill sketch to inventory.");
            _inventory.AddItemByItemID(_noteItemId);
            //ServiceLocator.GetService<Journal> add note
            _logger.Info("Add note about mill to journal.");
        }

        public void GetRepairedLever()
        {
            _logger.Info("Remove broken lever from inventory.");
            _inventory.RemoveItemById(_brokenLeverItemId);
            _logger.Info("Add repaired lever to inventory.");
            _inventory.AddItemByItemID(_repairedLeverItemId);
        }

        public void DespawnMiller()
        {
            _logger?.Info("Despawn miller from scene.");
            _miller.SetActive(false);
        }

        public void SpawnMiller()
        {
            _logger?.Info("Move miller to quest final position and spawn chests.");
            _chests.SetActive(true);
            _miller.transform.position = _millerFinalPosition;
        }

        public void RotateMillWheelOn()
        {
            _logger?.Info("Switch rotation of mill wheel on.");
            _millWheelHandler.enabled = true;
        }

        public void GetRune()
        {
            _runeBar.SetActive(true);
            RunesSystem.Instance.AddRune();
        }
    }
}