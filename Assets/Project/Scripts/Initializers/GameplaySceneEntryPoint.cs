using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using BigProject.Player;

namespace BigProject.Initializers
{
    /// <summary>
    /// Точка входа с execute order до MonoBehaviour.
    /// </summary>
    public class GameplaySceneEntryPoint : MonoBehaviour
    {
        [SerializeField, Tooltip("Actions to execute for early initialize.")]
        private UnityEvent _initActions;

        private void Awake()
        {
            ServiceLocator.ReleaseAllEmpty();
            ServiceLocator.AddServiceResolver(() => DialogueManager.Instance);
            ServiceLocator.AddServiceResolverLazy(() => GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInputHandler>());
            ProgressManager pm = ServiceLocator.GetService<ProgressManager>();
            Assert.IsTrue(pm != null, "Unable to get progress manager at scene entry");
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

            _initActions?.Invoke();
        }
    }
}