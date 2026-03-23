using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;

namespace BigProject.Gameplay.Common
{
    /// <summary>
    /// Trigger area to new scene.
    /// </summary>
    public class TeleportHandler : MovingNextSceneHandler
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Move();
            }
        }
    }
}