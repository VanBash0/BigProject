using BigProject.Managers;
using BigProject.Player;
using BigProject.Settings;
using BigProject.Systems;
using BigProject.Systems.HUD;
using BigProject.UI;
using BigProject.Utilities;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Initializers
{
    /// <summary>
    /// Services that persist between game scenes.
    /// </summary>
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private HUDConfig _hudConfig;
        [SerializeField]
        private ItemsDatabaseSO _itemsDatabase;
        [SerializeField]
        private QuestJournalConfig _journalConfig;
        [SerializeField]
        private GameObject _hudPrefab;

        [field: SerializeField]
        public Scenes _sceneToLoad; // For feature load progress

        private HUD _hud;
        private GameObject _hudObj;
        private QuestJournal _questJournal;
        private InventorySystem _inventory;
        private RunesSystem _runesSystem;
        private JournalView _journalView;
        private InventoryUI _inventoryUI;
        private RunePanelUI _runeUI;
        private PlayerInputHandler _playerInput;
        private GameplayStatesHandler _statesHandler;

        private static bool _isInstantiated;

        public static void Init()
        {
            _isInstantiated = false;
        }

        private void Awake()
        {
            if (_isInstantiated)
            {
                Debug.LogWarning(String.Format(LogStr.WARNING_DUPLICATE_UNIQUE_ENTITY, "Gameplay Entry Point"));
                Destroy(gameObject);
                return;
            }

            _isInstantiated = true;

            Assert.IsNotNull(_hudConfig, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, "Gameplay Entry Point",  "HUD config"));
            Assert.IsNotNull(_itemsDatabase, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, "Gameplay Entry Point", "Items Database"));
            Assert.IsNotNull(_hudPrefab, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, "Gameplay Entry Point", "HUD Prefab"));
            Assert.IsNotNull(_journalConfig, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, "Gameplay Entry Point", "Journal Config"));

            GameObject gameplayServices = new GameObject("GameplayServices");
            transform.parent = gameplayServices.transform; // For dispose after gameplay exit
            DontDestroyOnLoad(gameplayServices);
            InitServices();
        }

        public void InitServices()
        {
            GameLogManager.Info(LogStr.INFO_INITIALIZING_GAMEPLAY_SERVICES);
            _inventory = new InventorySystem(_itemsDatabase);
            _hud = new();
            _playerInput = new();
            _questJournal = new QuestJournal(ServiceLocator.GetService<ProgressManager>(), _journalConfig);
            _questJournal.Init();
            _runesSystem = new();
            GameplayManager gameplayManager = new(ServiceLocator.GetService<ManualLoop>());
            _statesHandler = new(_hudConfig, gameplayManager, _playerInput, _hud);

            ServiceLocator.AddService(_questJournal);
            ServiceLocator.AddService(_runesSystem);
            ServiceLocator.AddService(_inventory);
            ServiceLocator.AddService(_hud);
            ServiceLocator.AddService(_playerInput);
            ServiceLocator.AddService(gameplayManager);

            InitHUD();
            GameLogManager.Info(LogStr.INFO_INITIALIZING_GAMEPLAY_SERVICES_COMPLETED);
        }

        private void InitHUD()
        {
            GameLogManager.Info(LogStr.INFO_INITIALIZING_HUD);
            _hudObj = Instantiate(_hudPrefab);
            _journalView = _hudObj.GetComponentInChildren<JournalView>();
            _runeUI = _hudObj.GetComponentInChildren<RunePanelUI>();
            _inventoryUI = _hudObj.GetComponentInChildren<InventoryUI>();
            ServiceLocator.AddService(_inventoryUI);

            DontDestroyOnLoad(_hudObj);

            _journalView.Init(_questJournal);
            _inventoryUI.Init(_inventory);
            _runeUI.Init(_runesSystem);
            _hud = ServiceLocator.GetService<HUD>();
            _hud.AddWidget(_hudConfig.HUDInventoryWidgetId, _inventoryUI);
            _hud.AddWidget(_hudConfig.HUDJournalWidgetId, _journalView);
            _hud.AddWidget(_hudConfig.HUDRunesWidgetId, _runeUI);
            _hud.HideWidget(_hudConfig.HUDInventoryWidgetId);
            _hud.HideWidget(_hudConfig.HUDJournalWidgetId);
            _hud.HideWidget(_hudConfig.HUDRunesWidgetId);
            _hud.ShowWidget(_hudConfig.HUDInventoryWidgetId, 2f);
            _hud.ShowWidget(_hudConfig.HUDJournalWidgetId, 2f);
            GameLogManager.Info(LogStr.INFO_INITIALIZING_HUD_COMPLETED);
        }

        public void OnDestroy()
        {
            Remover.SafeDispose(_inventory);
            Remover.SafeDispose(_journalView);
            Remover.SafeDispose(_questJournal);
            Remover.SafeDispose(_hud);
            Remover.SafeDispose(_playerInput);
            Remover.SafeDispose(_statesHandler);

            Destroy(_hudObj);

            ServiceLocator.ReleaseService<QuestJournal>();
            ServiceLocator.ReleaseService<RunesSystem>();
            ServiceLocator.ReleaseService<InventorySystem>();
            ServiceLocator.ReleaseService<HUD>();
            ServiceLocator.ReleaseService<InventoryUI>();
            ServiceLocator.ReleaseService<PlayerInputHandler>();
            ServiceLocator.ReleaseService<GameplayManager>();
        }
    }
}