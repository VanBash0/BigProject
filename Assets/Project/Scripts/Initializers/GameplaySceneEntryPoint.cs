using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using BigProject.Player;
using BigProject.UI;
using BigProject.Systems.HUD;
using BigProject.Settings;
using System.Linq;
using BigProject.Utilities;

namespace BigProject.Initializers
{
    /// <summary>
    /// Точка входа с execute order до MonoBehaviour.
    /// </summary>
    public class GameplaySceneEntryPoint : MonoBehaviour
    {
        [SerializeField]
        GlobalConfig _config;
        [SerializeField]
        private JournalView _journalView;
        [SerializeField]
        private QuestJournalConfig _journalConfig;
        [SerializeField]
        private InventoryUI _inventoryUI;
        [SerializeField, Tooltip("Actions to execute for early initialize.")]
        private UnityEvent _initActions;

        private HUD _hud;
        private GameplayStatesHandler _statesHandler;
        private QuestJournal _questJournal;

        private void Awake()
        {
            ServiceLocator.ReleaseAllEmpty();
            ServiceLocator.AddServiceResolver(() => DialogueManager.Instance);
            ServiceLocator.AddServiceResolverLazy(() => GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputHandler>());
            ProgressManager pm = ServiceLocator.GetService<ProgressManager>();
            Assert.IsNotNull(pm, "Unable to get progress manager at scene entry");
            InitActionsHandlers(pm);
            InitHUD(pm);
            InitGameplayHandlers();
            _initActions?.Invoke();
        }

        private void InitActionsHandlers(ProgressManager pm)
        {
            var actionsHandlers = FindObjectsByType<QuestActionHandlerMono>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestActionHandlerMono actionHandler in actionsHandlers)
            {
                actionHandler.Construct(pm);
            }

            var actionHandlersContainers = FindObjectsByType<QuestActionHandlersContainer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestActionHandlersContainer container in actionHandlersContainers)
            {
                container.Construct(pm);
            }
        }

        private void InitHUD(ProgressManager pm)
        {
            _questJournal = new QuestJournal(pm, _journalConfig);
            ServiceLocator.AddService(_questJournal);
            _journalView.Init(_questJournal);
            _questJournal.Init();
            _hud = ServiceLocator.GetService<HUD>();
            _hud.AddWidget(_config.HUDInventoryWidgetId, _inventoryUI);
            _hud.AddWidget(_config.HUDJournalWidgetId, _journalView);
        }

        private void InitGameplayHandlers()
        {
            _statesHandler = new(_config, ServiceLocator.GetService<GameplayManager>(), ServiceLocator.GetService<PlayerInputHandler>(), _hud);
        }

        private void OnDestroy()
        {
            DisposeHUD();
            DisposeGameplayHandlers();
        }

        private void DisposeHUD()
        {
            _journalView.Dispose();
            _questJournal?.Dispose();
            _hud.RemoveAllWidgets();
            ServiceLocator.ReleaseService<QuestJournal>();
        }

        private void DisposeGameplayHandlers()
        {
            _statesHandler.Dispose();
        }
    }
}