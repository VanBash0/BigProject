using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;

namespace BigProject.Gameplay.VillageWatermillQuest
{
    public class ChestsHandler : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private QuestActionHandlerMono _action;

        public void Interact()
        {
            if (_action.CurrentState == QuestActionState.Active)
            {
               // ServiceLocator.GetService<InventorySystem>().AddItemById();
                _action.MakeTransition(0);
            }
        }
    }
}
