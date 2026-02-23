using BigProject.Managers;
using BigProject.Systems.QuestSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using BigProject.Player;
using BigProject.Systems;
using System;
using BigProject.NPC;
using System.Linq;

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

            Assert.IsNotNull(_playerController, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, "Scene Entry Point", "Player Controller"));
            GameLogManager.Info(LogStr.INFO_INITIALIZING_SCENE_SERVICES);
            _playerController.Init(ServiceLocator.GetService<PlayerInputHandler>());
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

            InitDialogueNPCs();

            GameLogManager.Info(LogStr.INFO_INITIALIZING_SCENE_SERVICES_COMPLETED);
            _initActions?.Invoke();
        }

        private void InitDialogueNPCs()
        {
            var dialogueNPCs = FindObjectsByType<DialogNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            DialogueManager dialogueManager = ServiceLocator.GetService<DialogueManager>();

            foreach (DialogNPC dialogueNPC in dialogueNPCs)
            {
                dialogueNPC.Init(dialogueManager);
            }
        }
    }
}