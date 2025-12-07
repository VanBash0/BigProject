using UnityEngine;

namespace BigProject.Test.VFrolov
{
    /// <summary>
    /// Для вращения шестерни и мельницы.
    /// </summary>
    public class TVFRotate : MonoBehaviour
    {
        [SerializeField]
        private float _rotateSpeed = 15f;

        private void Update()
        {
            transform.Rotate(transform.right, _rotateSpeed * Time.deltaTime);
        }
    }
}