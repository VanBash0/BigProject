using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BigProject.Common.Managers
{
    public class TestSceneLoader : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (SceneManager.GetActiveScene().name == Scenes.TestScene1.ToString())
                    SceneLoader.Instance.LoadScene(Scenes.TestScene2);
                else
                    SceneLoader.Instance.LoadScene(Scenes.TestScene1);
            }
        }
    }
}