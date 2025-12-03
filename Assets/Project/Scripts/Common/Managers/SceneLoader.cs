using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BigProject.Common.Managers
{
    public enum Scenes
    {
        MainScene,
        TestScene1,
        TestScene2
    }

    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        private bool _isLoading;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(Scenes sceneName)
        {
            Debug.Log(sceneName);

            if (_isLoading)
                return;

            var currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == sceneName.ToString())
                throw new System.Exception("You are trying to load already loaded scene.");

            StartCoroutine(LoadSceneRoutine(sceneName.ToString()));
        }

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            _isLoading = true;

            var waitFading = true;
            Fader.Instance.FadeIn(() => waitFading = false);

            while (waitFading)
                yield return null;

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
                yield return null;

            async.allowSceneActivation = true;

            waitFading = true;
            Fader.Instance.FadeOut(() => waitFading = false);

            while (waitFading)
                yield return null;

            _isLoading = false;
        }
    }
}