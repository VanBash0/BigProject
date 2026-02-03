using BigProject.Managers;
using BigProject.Systems.DialogueSystem;
using UnityEngine;

namespace BigProject.Gameplay.VillageWatermillQuest
{
    /// <summary>
    /// Доп. зависимости (по хорошему надо будет убирать, т. к. это обход нехватки функционала в ряде модулей).
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private DialogueModeSwitch _dialogueModeSwitch;

        public void Init()
        {
            ServiceLocator.AddService(_dialogueModeSwitch);
        }
    }
}