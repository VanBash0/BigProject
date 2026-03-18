using BigProject.Gameplay.TownHall;
using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.Systems.Inventory;
using BigProject.Systems.QuestSystem;
using BigProject.UI;
using Managers.Gameplay;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.VillageElderQuest
{
    public class QuestBoundariesController : MonoBehaviour, IQuestBoundariesController
    {
        [SerializeField]
        private QuestActions _questActions;
        [SerializeField]
        private Collider _watermillDoor;
        [SerializeField]
        private AmbassadorDialogueManager _ambassadorDialogueManager;
        [SerializeField]
        private GameObject _questTownhallObjects;

        [field: SerializeField]
        public int QuestId { get; private set; }

        private void Awake()
        {
            Assert.IsNotNull(_questActions, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest Actions"));
            Assert.IsNotNull(_watermillDoor, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Watermill door"));
            Assert.IsNotNull(_ambassadorDialogueManager, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Ambassador dialogue manager"));
            Assert.IsNotNull(_questTownhallObjects, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest Townhall Objects"));
        }

        public void InitOnSceneEntry()
        {
            GameplayManager gameplayManager = ServiceLocator.GetService<GameplayManager>();
            _questActions.Init(ServiceLocator.GetService<InventorySystem>(), ServiceLocator.GetService<InventoryUI>(), gameplayManager);
            _questTownhallObjects.SetActive(true);
            _ambassadorDialogueManager.Init(ServiceLocator.GetService<PlayerController>(), ServiceLocator.GetService<DialogueManager>(), gameplayManager);
            _watermillDoor.enabled = false;
        }

        public void End()
        {
            _questTownhallObjects.SetActive(false);
            _watermillDoor.enabled = true;
        }
    }
}