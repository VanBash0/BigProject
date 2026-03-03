using BigProject.Gameplay.Common;
using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems.Inventory;
using BigProject.UI;
using UnityEngine;

namespace BigProject.Gameplay.Watermill
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private ControlPanel _controlPanel;
        [SerializeField]
        private MiniGameActivator _miniGameActivator;
        [SerializeField]
        private AudioClip _music;

        public void Init()
        {
            if (ServiceLocator.TryGetService(out MusicManager musicManager))
            {
                musicManager.PlayMusic(_music, 0.1f, 0.1f);
            }

            GameplayManager gameplayManager = ServiceLocator.GetService<GameplayManager>();
            PlayerInputHandler inputHandler = ServiceLocator.GetService<PlayerInputHandler>();
            _controlPanel.Init(gameplayManager, inputHandler, ServiceLocator.GetService<InventorySystem>());
            _miniGameActivator.Init(gameplayManager, inputHandler, ServiceLocator.GetService<InventoryUI>());
        }
    }
}