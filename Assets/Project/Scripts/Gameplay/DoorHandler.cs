using BigProject.Intercatable;
using BigProject.Managers;
using UnityEngine;

namespace BigProject.Gameplay
{
    public class DoorHandler : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Scenes _sceneToLoad;

        public void Interact()
        {
            if (ServiceLocator.TryGetService(out SceneLoaderManager sceneLoader))
            {
                sceneLoader.LoadScene(_sceneToLoad);
            }
            else
            {
                string msg = $"{gameObject.name} door unable to get scene loader.";
                Debug.LogError(msg);

                if (ServiceLocator.TryGetService(out GameLogManager logger))
                {
                    logger.Error(msg);
                }
            }
        }
    }
}
