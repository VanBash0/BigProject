using BigProject.Intercatable;
using BigProject.Managers;
using UnityEngine;

namespace BigProject.Gameplay
{
    public class TeleportHandler : MonoBehaviour
    {
        [SerializeField]
        private Scenes _sceneToLoad;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != "Player")
            {
                return;
            }

            if (ServiceLocator.TryGetService(out SceneLoaderManager sceneLoader))
            {
                sceneLoader.LoadScene(_sceneToLoad);
            }
            else
            {
                string msg = $"{gameObject.name} teleport unable to get scene loader.";
                Debug.LogError(msg);

                if (ServiceLocator.TryGetService(out GameLogManager logger))
                {
                    logger.Error(msg);
                }
            }
        }
    }
}