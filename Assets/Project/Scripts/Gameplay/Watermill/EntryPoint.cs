using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.UI;
using UnityEngine;

namespace BigProject.Gameplay.Watermill
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private ControlPanel _controlPanel;
        [SerializeField]
        private AudioClip _music;

        public void Init()
        {
            if (ServiceLocator.TryGetService(out MusicManager musicManager))
            {
                musicManager.PlayMusic(_music, 0.1f, 0.1f);
            }

            _controlPanel.Init(ServiceLocator.GetService<GameplayManager>(),
                ServiceLocator.GetService<PlayerInputHandler>(), ServiceLocator.GetService<InventorySystem>(),
                ServiceLocator.GetService<InventoryUI>());
        }
    }
}