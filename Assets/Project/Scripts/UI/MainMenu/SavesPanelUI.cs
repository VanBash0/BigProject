using UnityEngine;
using UnityEngine.UI;

public class SavesPanelUI : MainMenuPanel
{
    [SerializeField] Button _backButton;


    private void OnEnable()
    {
        _backButton.onClick.AddListener(() => {
            _mainMenuPanelManager.GetMenuPanel().gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveAllListeners();
    }
}
