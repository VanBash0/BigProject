using BigProject.Common.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BigProject.UI
{
    public class MenuPanelUI : MainMenuPanel
    {
        [SerializeField] Button _newGameButton;
        [SerializeField] Button _continueButton;
        [SerializeField] Button _savesButton;
        [SerializeField] Button _settingsButton;
        [SerializeField] Button _quitButton;

        private void OnEnable()
        {
            _newGameButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked New Game Button");
                SceneLoader.Instance.LoadScene(Scenes.MainScene);
            });

            _continueButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked Continue Button");
                SceneLoader.Instance.LoadScene(Scenes.MainScene);
            });

            _savesButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked Saves Button");
                _mainMenuPanelManager.GetSavesPanel().gameObject.SetActive(true);
                gameObject.SetActive(false);
            });

            _settingsButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked Settings Button");
                _mainMenuPanelManager.GetSettingsPanel().gameObject.SetActive(true);
                gameObject.SetActive(false);
            });

            _quitButton.onClick.AddListener(() =>
            {
                Debug.Log("Clicked Quit Button");
                Application.Quit();
            });
        }

        private void OnDisable()
        {
            _newGameButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();
            _savesButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
        }
    }
}