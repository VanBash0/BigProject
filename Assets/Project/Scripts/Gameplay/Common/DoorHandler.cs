using BigProject.Intercatable;
using BigProject.Managers;
using BigProject.Systems;
using UnityEngine;

namespace BigProject.Gameplay.Common
{
    /// <summary>
    /// Door to new scene.
    /// </summary>
    public class DoorHandler : MovingNextSceneHandler, IInteractable
    {
        public void Interact() => Move();
    }
}