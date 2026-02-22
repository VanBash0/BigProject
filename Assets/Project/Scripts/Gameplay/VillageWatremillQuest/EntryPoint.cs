using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.DialogueSystem;
using BigProject.Systems.HUD;
using UnityEngine;

namespace BigProject.Gameplay.VillageWatermillQuest
{
    /// <summary>
    /// Доп. зависимости (по хорошему надо будет убирать, т. к. это обход нехватки функционала в ряде модулей).
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        QuestActions _questActions;

        [SerializeField]
        private AudioClip _music;

        public void Init()
        {
            _questActions.Init(ServiceLocator.GetService<InventorySystem>(), ServiceLocator.GetService<RunesSystem>(),
                ServiceLocator.GetService<HUD>());

            if (ServiceLocator.TryGetService(out MusicManager musicManager))
            {
                musicManager.PlayMusic(_music, 0.1f, 0.1f);
            }
        }
    }
}