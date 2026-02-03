using UnityEngine;

namespace BigProject.UI
{
    public class MainMenuPanelManager : MonoBehaviour
    {
        [SerializeField] private MainMenuPanel _menuPanel;
        [SerializeField] private MainMenuPanel _settingsPanel;

        private void Awake()
        {
            _menuPanel.SetMainMenuPanelManager(this);
            _settingsPanel.SetMainMenuPanelManager(this);
        }

        public MainMenuPanel GetMenuPanel() { return _menuPanel; }
        public MainMenuPanel GetSettingsPanel() { return _settingsPanel; }
    }

    public class MainMenuPanel : MonoBehaviour
    {
        protected MainMenuPanelManager _mainMenuPanelManager;
        public void SetMainMenuPanelManager(MainMenuPanelManager mainMenuPanelManager)
        {
            _mainMenuPanelManager = mainMenuPanelManager;
        }
    }
}