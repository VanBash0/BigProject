using BigProject.Managers;
using BigProject.Systems;
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