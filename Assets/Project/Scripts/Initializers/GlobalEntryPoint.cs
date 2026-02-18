using BigProject.Managers;
using BigProject.Settings;
using BigProject.Systems;
using BigProject.Systems.QuestSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Initializers
{
    /// <summary>
    /// Global services and settings.
    /// </summary>
    public class GlobalEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private GlobalConfig _config;
        [SerializeField]
        private MusicManager _musicManagerPrefab;
        [SerializeField]
        private LogLevel _currentLogLevel = LogLevel.None;

        private static bool _isInstantiated;

        public static void Init()
        {
            _isInstantiated = false;
        }

        private void Awake()
        {
            if (_isInstantiated)
            {
                Debug.LogWarning("Global point should exist in one copy.");
                Destroy(gameObject);
                return;
            }
            
            Assert.IsNotNull(_config, "Global entry has no point config.");
            Assert.IsNotNull(_musicManagerPrefab, "Global entry has no music manager prefab.");
            _isInstantiated = true;

            GameObject globalServices = new GameObject("GlobalServices");
            DontDestroyOnLoad(globalServices);

            ManualLoop manualLoop = new GameObject("ManualLoop").AddComponent<ManualLoop>();
            manualLoop.transform.parent = globalServices.transform;
            ServiceLocator.AddService(manualLoop);

            ServiceLocator.AddService(new GameLogManagerTicker(manualLoop));
            GameLogManager.Init(_currentLogLevel);

            ServiceLocator.AddService(new SceneLoadManager(manualLoop));

            SavesManager savesManager = new();
            ServiceLocator.AddService(savesManager);
            ServiceLocator.AddService(new ProgressManager(_config.PlayerProfileName, new QuestJsonLoader(_config.QuestsFolder), savesManager));

            MusicManager musicManager = Instantiate(_musicManagerPrefab);
            musicManager.transform.parent = globalServices.transform;
            ServiceLocator.AddService(musicManager);
        }
    }
}