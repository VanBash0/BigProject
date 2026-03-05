using BigProject.Initializers;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigProject.Utilities
{
    public class EscExit : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (ServiceLocator.TryGetService(out SceneLoadManager sceneLoader) && ServiceLocator.TryGetService(out GameplayManager gameplayManager))
                {
                    if (gameplayManager.State == GameplayState.Play)
                    {
                        sceneLoader.LoadScene(Scenes.MainMenu);
                        Bootstrapper.SetStage(GameExecutionStage.Launch);
                    }
                }
                else
                {
                    string msg = string.Format(LogStr.CRITICAL_UNABLE_GET_SERVICE, gameObject.name, typeof(SceneLoadManager));
                    Debug.LogError(msg);
                }
            }
        }
    }
}