using BigProject.Managers;
using BigProject.Systems.QuestSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using BigProject.Player;

namespace BigProject.Initializers
{
    /// <summary>
    /// Scene dependencies.
    /// </summary>
    public class GameplaySceneEntryPoint : MonoBehaviour
    {
        [SerializeField]
        PlayerController _playerController;
        [SerializeField, Tooltip("Actions to execute for early initialize.")]
        private UnityEvent _initActions;

        private void Awake()
        {
#if UNITY_EDITOR
            if (Bootstrapper.Stage != GameExecutionStage.Gameplay)
            {
                Bootstrapper.SetStage(GameExecutionStage.Gameplay);
            }
#endif

            Assert.IsNotNull(_playerController, "Scene entry point unable to get player controller.");
            GameLogManager.Info("Start initializing scene services...");
            _playerController.Init(ServiceLocator.GetService<PlayerInputHandler>());
            ServiceLocator.AddServiceResolver(() => DialogueManager.Instance);
            ProgressManager pm = ServiceLocator.GetService<ProgressManager>();

            var actionsHandlers = FindObjectsByType<QuestActionHandlerMono>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestActionHandlerMono actionHandler in actionsHandlers)
            {
                actionHandler.Init(pm);
            }

            var actionHandlersContainers = FindObjectsByType<QuestActionHandlersContainer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestActionHandlersContainer container in actionHandlersContainers)
            {
                container.Init(pm);
            }

            GameLogManager.Info("Finish initializing scene services.");
            _initActions?.Invoke();
        }
    }
}