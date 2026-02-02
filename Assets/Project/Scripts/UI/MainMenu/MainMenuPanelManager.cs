using UnityEngine;

namespace BigProject.UI
{
    public class MainMenuPanelManager : MonoBehaviour
    {
        [SerializeField] MainMenuPanel _menuPanel;
        [SerializeField] MainMenuPanel _savesPanel;
        [SerializeField] MainMenuPanel _settingsPanel;

        private void Awake()
        {
            _menuPanel.SetMainMenuPanelManager(this);
            _savesPanel.SetMainMenuPanelManager(this);
            _settingsPanel.SetMainMenuPanelManager(this);
        }

        public MainMenuPanel GetMenuPanel() { return _menuPanel; }

        public MainMenuPanel GetSavesPanel() { return _savesPanel; }

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