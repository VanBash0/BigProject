using BigProject.Managers;
using BigProject.Utilities;
using UnityEngine;

namespace BigProject.Initializers
{
    /// <summary>
    /// Stages for loading dependencies.
    /// </summary>
    public enum GameExecutionStage
    {
        Launch,
        Gameplay,
    }

    /// <summary>
    /// Game dependencies loading stages controller.
    /// </summary>
    public static class Bootstrapper
    {
        private const string INITIALIZERS_DIR = "Prefabs/Initializers/";
        private const string GLOBAL_EP_PREFAB_NAME = "GlobalEntryPoint";
        private const string GAMEPLAY_EP_PREFAB_NAME = "GameplayEntryPoint";

        // Hashing gameplay dependencies for removal.
        private static GameplayEntryPoint _gameplayEntryPoint;

        public static GameExecutionStage Stage { get; private set; }

        [RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            // Reset in case of uncleaned domain.
            ServiceLocator.Init();
            GlobalEntryPoint.Init();

            Stage = GameExecutionStage.Launch;
            MoveToStage(GameExecutionStage.Launch);
        }

        public static void SetStage(GameExecutionStage stage)
        {
            if (Stage == stage)
            {
                Debug.LogWarning($"Bootstrapper already at stage {Stage}.");
                return;
            }

            MoveToStage(stage);
        }

        private static void MoveToStage(GameExecutionStage stage)
        {
            switch (stage)
            {
                case GameExecutionStage.Launch:
                    if (Stage == GameExecutionStage.Gameplay)
                    {
                        Debug.Log($"Removing gameplay services...");
                        Remover.SafeRelease(_gameplayEntryPoint);
                    }
                    else
                    {
                        GameObject.Instantiate(Resources.Load<GlobalEntryPoint>($"{INITIALIZERS_DIR}{GLOBAL_EP_PREFAB_NAME}"));
                    }

                    break;
                case GameExecutionStage.Gameplay:
                    GameplayEntryPoint.Init();
                    _gameplayEntryPoint =  GameObject.Instantiate(Resources.Load<GameplayEntryPoint>($"{INITIALIZERS_DIR}{GAMEPLAY_EP_PREFAB_NAME}"));
                    break;
                default:
                    Debug.LogWarning($"Bootstrapper try move to incorrect stage {stage}.");
                    return;
            }

            Stage = stage;
            Debug.Log($"Game execution move to stage: {Stage}");
        }
    }
}