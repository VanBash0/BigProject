using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;

namespace BigProject.Gameplay
{
    /// <summary>
    /// Door to new scene.
    /// </summary>
    public class DoorHandler : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Scenes _sceneToLoad;

        public void Interact()
        {
            if (ServiceLocator.TryGetService(out SceneLoadManager sceneLoader))
            {
                sceneLoader.LoadScene(_sceneToLoad);
            }
            else
            {
                string msg = string.Format(LogStr.CRITICAL_UNABLE_GET_SERVICE, gameObject.name, typeof(SceneLoadManager));
                Debug.LogError(msg);
            }
        }
    }
}