using BigProject.Initializers;
using BigProject.Managers;
using BigProject.Systems;
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

        private void Start()
        {
            if (ServiceLocator.TryGetService(out ProgressManager progressManager))
            {
                _continueButton.interactable = progressManager.HasSavedProgress();
            }
        }

        private void OnEnable()
        {
            _newGameButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked New Game Button");
                if (ServiceLocator.TryGetService(out SceneLoadManager sceneLoader))
                {
                    Bootstrapper.SetStage(GameExecutionStage.Gameplay);
                    sceneLoader.LoadScene(Scenes.Village);
                }
                else
                {
                    string msg = string.Format(LogStr.CRITICAL_UNABLE_GET_SERVICE, gameObject.name, typeof(SceneLoadManager));
                    Debug.LogError(msg);
                }
            });

            _continueButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked Continue Button");
                if (ServiceLocator.TryGetService(out SceneLoadManager sceneLoader))
                {
                    sceneLoader.LoadScene(Scenes.Village);
                }
                else
                {
                    string msg = string.Format(LogStr.CRITICAL_UNABLE_GET_SERVICE, gameObject.name, typeof(SceneLoadManager));
                    Debug.LogError(msg);
                }
            });

            _settingsButton.onClick.AddListener(() =>
            {
                //Debug.Log("Clicked Settings Button");
                _mainMenuPanelManager.GetSettingsPanel().gameObject.SetActive(true);
                _mainMenuPanelManager.GetStudioLogo().SetActive(false);
                _mainMenuPanelManager.ToggleBlur(true);
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