using BigProject.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace BigProject.UI
{
    public class MenuPanelUI : MainMenuPanel
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        private void OnEnable()
        {
            _newGameButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked New Game Button");
                SceneLoaderManager.Instance.LoadScene(Scenes.MainScene);
            });

            _continueButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked Continue Button");
                SceneLoaderManager.Instance.LoadScene(Scenes.MainScene);
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
            _settingsButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
        }
    }
}