using BigProject.Gameplay.Common;
using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.Systems.Inventory;
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
        private ChestPuzzle _chestPuzzle;
        [SerializeField]
        private MiniGameActivator _miniGameActivator;
        [SerializeField]
        private GameObject _townhallQuestObject;
        [SerializeField]
        private int _townhallQuestId;

        private void Awake()
        {
            Assert.IsNotNull(_questActions, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest Actions"));
            Assert.IsNotNull(_chestPuzzle, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Chest Puzzle"));
            Assert.IsNotNull(_miniGameActivator, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Mini Game Activator"));
            Assert.IsNotNull(_townhallQuestObject, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest objects"));
        }

        public void Init()
        {
            if (ServiceLocator.GetService<ProgressManager>().GetQuestState(_townhallQuestId) == Systems.QuestSystem.QuestState.Active)
            {
                _townhallQuestObject.SetActive(true);
            }

            _questActions.Init(ServiceLocator.GetService<InventorySystem>(), ServiceLocator.GetService<InventoryUI>(),
                ServiceLocator.GetService<GameplayManager>(), ServiceLocator.GetService<RunesSystem>());
            _chestPuzzle.Init(ServiceLocator.GetService<InventorySystem>(), ServiceLocator.GetService<InventoryUI>(),
                ServiceLocator.GetService<ProgressManager>());
            _miniGameActivator.Init(ServiceLocator.GetService<GameplayManager>(), ServiceLocator.GetService<PlayerInputHandler>(),
                ServiceLocator.GetService<InventoryUI>());
        }
    }
}