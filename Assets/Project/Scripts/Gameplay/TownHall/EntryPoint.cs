using BigProject.Gameplay.Common;
using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.TownHall
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private QuestActions _questActions;
        [SerializeField]
        private ItemsDatabaseSO _itemsDB;
        [SerializeField]
        private ChestPuzzle _chestPuzzle;
        [SerializeField]
        private MiniGameActivator _miniGameActivator;

        private void Awake()
        {
            Assert.IsNotNull(_questActions, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest Actions"));
            Assert.IsNotNull(_itemsDB, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Items Database"));
            Assert.IsNotNull(_chestPuzzle, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Chest Puzzle"));
            Assert.IsNotNull(_miniGameActivator, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Mini Game Activator"));
        }

        public void Init()
        {
            _questActions.Init(ServiceLocator.GetService<InventorySystem>(), _itemsDB);
            _chestPuzzle.Init(ServiceLocator.GetService<InventorySystem>(), ServiceLocator.GetService<InventoryUI>());
            _miniGameActivator.Init(ServiceLocator.GetService<GameplayManager>(), ServiceLocator.GetService<PlayerInputHandler>(), ServiceLocator.GetService<InventoryUI>());
        }
    }
}