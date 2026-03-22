using UnityEngine;

namespace BigProject.Editor
{
    public class Waypoint : MonoBehaviour
    {
#if UNITY_EDITOR
        private const float DRAW_RADIUS = 1.0f;

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DRAW_RADIUS);
        }
#endif
    }
}