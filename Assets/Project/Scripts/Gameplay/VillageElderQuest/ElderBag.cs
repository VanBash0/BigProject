using BigProject.Intercatable;
using BigProject.Systems.Inventory;
using BigProject.Systems.QuestSystem;
using UnityEngine;

namespace BigProject.Gameplay.VillageElderQuest
{
    public class ElderBag : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private QuestActionHandlerMono _actionHandler;

        private void Start()
        {
            if (_actionHandler.CurrentState == QuestActionState.Completed)
            {
                Destroy(gameObject);
            }
        }

        public void Interact()
        {
            if (_actionHandler.CurrentState == QuestActionState.Active)
            {
                _actionHandler.MakeTransition(0);
                Destroy(gameObject);
            }
        }
    }
}

