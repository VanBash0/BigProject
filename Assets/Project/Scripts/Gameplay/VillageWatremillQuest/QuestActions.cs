using BigProject.Gameplay.Watermill;
using BigProject.Managers;
using BigProject.Settings;
using BigProject.Systems;
using BigProject.Systems.HUD;
using BigProject.Utilities;
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
        private HUDConfig _hudConfig;

        private InventorySystem _inventory;
        private RunesSystem _runes;
        private HUD _hud;

        public void Init(InventorySystem inventory, RunesSystem runes, HUD hud)
        {
            _inventory = inventory;
            _runes = runes;
            _hud = hud;
            ExceptionUtilities.ThrowIfNull(_inventory, gameObject.name, "Inventory System is null.");
            ExceptionUtilities.ThrowIfNull(_runes, gameObject.name, "Rune System is null.");
            ExceptionUtilities.ThrowIfNull(_hud, gameObject.name, "HUD is null.");
        }

        private void Start()
        {
            Assert.IsNotNull(_miller, $"{gameObject.name}: unable to get Miller.");
            Assert.IsNotNull(_chests, $"{gameObject.name}: unable to get Chests.");
            Assert.IsNotNull(_millWheelHandler, $"{gameObject.name}: unable to get Mill Wheel.");
            Assert.IsNotNull(_hudConfig, $"{gameObject.name}: unable to get HUD config.");
        }

        public void GetWatermillNote()
        {
            GameLogManager.Info("Add mill sketch to inventory.");
            _inventory.AddItemByItemID(_noteItemId);
            GameLogManager.Info("Add note about mill to journal.");
        }

        public void GetRepairedLever()
        {
            GameLogManager.Info("Remove broken lever from inventory.");
            _inventory.RemoveItemById(_brokenLeverItemId);
            GameLogManager.Info("Add repaired lever to inventory.");
            _inventory.AddItemByItemID(_repairedLeverItemId);
        }

        public void DespawnMiller()
        {
            GameLogManager.Info("Despawn miller from scene.");
            _miller.SetActive(false);
        }

        public void SpawnMiller()
        {
            GameLogManager.Info("Move miller to quest final position and spawn chests.");
            _chests.SetActive(true);
            _miller.transform.position = _millerFinalPosition;
        }

        public void RotateMillWheelOn()
        {
            GameLogManager.Info("Switch rotation of mill wheel on.");
            _millWheelHandler.enabled = true;
        }

        public void GetRune()
        {
            _hud.ShowWidget(_hudConfig.HUDRunesWidgetId);
            _runes.AddRune();
        }
    }
}