using BigProject.Managers;
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
                SceneLoaderManager.Instance.LoadScene(Scenes.MainMenu);
            }
        }
    }
}