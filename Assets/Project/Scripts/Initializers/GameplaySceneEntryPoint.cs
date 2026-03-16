using BigProject.Managers;
using BigProject.Systems.QuestSystem;
using UnityEngine;
using UnityEngine.Events;
using BigProject.Systems;
using BigProject.NPC;

namespace BigProject.Initializers
{
    /// <summary>
    /// Scene dependencies.
    /// </summary>
    public class GameplaySceneEntryPoint : MonoBehaviour
    {
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
            GameLogManager.Info(LogStr.INFO_INITIALIZING_SCENE_SERVICES);
            ProgressManager pm = ServiceLocator.GetService<ProgressManager>();

            QuestActionHandlerMono[] actionsHandlers = FindObjectsByType<QuestActionHandlerMono>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestActionHandlerMono actionHandler in actionsHandlers)
            {
                actionHandler.Init(pm);
            }

            QuestActionHandlersContainer[] actionHandlersContainers = FindObjectsByType<QuestActionHandlersContainer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestActionHandlersContainer container in actionHandlersContainers)
            {
                container.Init(pm);
            }

            QuestInteractableHandler[] interactableHandlers = FindObjectsByType<QuestInteractableHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestInteractableHandler interactableHandler in interactableHandlers)
            {
                interactableHandler.Init(pm);
            }

            QuestTriggerHandler[] triggersHandlers = FindObjectsByType<QuestTriggerHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (QuestTriggerHandler triggerHandler in triggersHandlers)
            {
                triggerHandler.Init(pm);
            }

            InitDialogueNPCs();

            GameLogManager.Info(LogStr.INFO_INITIALIZING_SCENE_SERVICES_COMPLETED);
            _initActions?.Invoke();
        }

        private void InitDialogueNPCs()
        {
            DialogNPC[] dialogueNPCs = FindObjectsByType<DialogNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            DialogueManager dialogueManager = ServiceLocator.GetService<DialogueManager>();

            foreach (DialogNPC dialogueNPC in dialogueNPCs)
            {
                dialogueNPC.Init(dialogueManager);
            }
        }
    }
}