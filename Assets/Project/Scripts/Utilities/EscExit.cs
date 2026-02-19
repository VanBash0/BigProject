using BigProject.Initializers;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BigProject.Utilities
{
    public class EscExit : MonoBehaviour
    {
        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (ServiceLocator.TryGetService(out SceneLoadManager sceneLoader))
                {
                    sceneLoader.LoadScene(Scenes.MainMenu);
                    Bootstrapper.SetStage(GameExecutionStage.Launch);
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