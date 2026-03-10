using BigProject.Gameplay.TownHall;
using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.Systems.Inventory;
using BigProject.UI;
using Managers.Gameplay;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.VillageElderQuest
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private QuestActions _questActions;
        [SerializeField]
        private GameObject _miller;
        [SerializeField]
        private Collider _watermillDoor;
        [SerializeField]
        private AmbassadorDialogueManager _ambassadorDialogueManager;
        [SerializeField]
        private PlayerController _player;
        [SerializeField]
        private GameObject _questTownhallObjects;

        private void Awake()
        {
            Assert.IsNotNull(_questActions, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest Actions"));
            Assert.IsNotNull(_miller, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Miller"));
            Assert.IsNotNull(_watermillDoor, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Watermill door"));
            Assert.IsNotNull(_ambassadorDialogueManager, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Ambassador dialogue manager"));
            Assert.IsNotNull(_questTownhallObjects, string.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Quest Townhall Objects"));
        }

        public void Init()
        {
            _questActions.Init(ServiceLocator.GetService<InventorySystem>(), ServiceLocator.GetService<InventoryUI>(), ServiceLocator.GetService<GameplayManager>());
            _questTownhallObjects.SetActive(true);
            _miller.SetActive(false);
            _ambassadorDialogueManager.Init(_player, ServiceLocator.GetService<DialogueManager>());
            _watermillDoor.enabled = false;
        }
    }
}