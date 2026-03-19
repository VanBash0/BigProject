using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BigProject.UI
{
    public class MainMenuPanelManager : MonoBehaviour
    {
        [SerializeField] private MainMenuPanel _menuPanel;
        [SerializeField] private MainMenuPanel _settingsPanel;
        [SerializeField] private GameObject _studioLogo;
        [SerializeField] private GameObject _blurScreen;
        [SerializeField] private Volume _blurVolume;
        private DepthOfField _blurDepthOfField;

        private void Awake()
        {
            _menuPanel.SetMainMenuPanelManager(this);
            _settingsPanel.SetMainMenuPanelManager(this);
            if (!_blurVolume.profile.TryGet(out _blurDepthOfField))
            {
                Debug.LogWarning("Global Volume for blur wasn't set in MainMenuPanelManager");
            }
        }

        public MainMenuPanel GetMenuPanel() { return _menuPanel; }
        public MainMenuPanel GetSettingsPanel() { return _settingsPanel; }
        public GameObject GetStudioLogo() { return _studioLogo; }
        public GameObject GetBlurScreen() { return _blurScreen; }

        public void ToggleBlur(bool isBlurOn)
        {
            _blurDepthOfField.focusDistance.value = isBlurOn ? 0.8f : 10f;
            _blurScreen.SetActive(isBlurOn);
        }
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