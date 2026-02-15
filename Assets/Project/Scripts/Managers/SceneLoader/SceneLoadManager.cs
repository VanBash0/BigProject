using BigProject.Systems;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BigProject.Managers
{
    public enum Scenes
    {
        MainScene,
        SceneLoaderManager_test_1,
        SceneLoaderManager_test_2,
        Village,
        Watermill,
        WatermillScene,
        VillageMainScene,
        MainMenu
    }

    public class SceneLoadManager : IDisposable
    {
        public event Action<Scenes> SceneLoaded;

        private const string FADER_PREFAB_PATH = "Prefabs/Fader";

        private readonly MonoBehaviour _coroutineStarter;
        private readonly Fader _fader;

        private bool _isLoading;

        public SceneLoadManager(MonoBehaviour coroutineStarter)
        {
            _coroutineStarter = coroutineStarter;

            if (_fader == null)
            {
                Fader faderPrefab = Resources.Load<Fader>(FADER_PREFAB_PATH);
                _fader = UnityEngine.Object.Instantiate(faderPrefab);

                UnityEngine.Object.DontDestroyOnLoad(_fader.gameObject);
            }
        }

        public void LoadScene(Scenes scene)
        {
            if (_isLoading)
            {
                return;
            }

            string currentSceneName = SceneManager.GetActiveScene().name;
            string newSceneName = scene.ToString();

            if (currentSceneName == newSceneName)
            {
                GameLogManager.Warning(LogStr.WARNING_SAME_SCENE);
            }

            _coroutineStarter.StartCoroutine(LoadSceneRoutine(scene, newSceneName));
        }

        private IEnumerator LoadSceneRoutine(Scenes scene, string sceneName)
        {
            _isLoading = true;

            // 1. Затемнение
            bool waitFading = true;
            _fader.FadeIn(() => waitFading = false);

            while (waitFading)
            {
                yield return null;
            }

            // 2. Загрузка сцены
            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            while (async.progress < 0.9f)
            {
                yield return null;
            }

            async.allowSceneActivation = true;

            // 3. Оповещение
            Debug.Log(string.Format(LogStr.INFO_SCENE_LOADING, scene));
            SceneLoaded?.Invoke(scene);

            // 4. Появление
            waitFading = true;
            _fader.FadeOut(() => waitFading = false);

            while (waitFading)
            {
                yield return null;
            }

            _isLoading = false;
        }
        
        public void Dispose()
        {
            UnityEngine.Object.Destroy(_fader.gameObject);
        }
    }
}