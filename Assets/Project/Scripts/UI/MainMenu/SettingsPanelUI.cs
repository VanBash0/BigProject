using UnityEngine;
using UnityEngine.UI;

namespace BigProject.UI
{
    public class SettingsPanelUI : MainMenuPanel
    {
        [SerializeField] Button _backButton;


        private void OnEnable()
        {
            _backButton.onClick.AddListener(() =>
            {
                _mainMenuPanelManager.GetMenuPanel().gameObject.SetActive(true);
                gameObject.SetActive(false);
            });
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveAllListeners();
        }
    }
}
