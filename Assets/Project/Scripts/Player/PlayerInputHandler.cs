using BigProject.Interactive;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigProject.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Настройки луча")]
        [SerializeField] private float _maxRayDistance = 1000f;

        private Camera _camera;
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }
        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Получаем позицию мыши из новой Input System
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Ray ray = _camera.ScreenPointToRay(mousePosition);
                CastRay(ray);
            }
        }

        private void CastRay(Ray ray)
        {
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxRayDistance))
            {
                Debug.Log($"Попал в: {hit.collider.name}");
                Debug.Log($"Координаты в пространстве: {hit.point}");
                IInteractive interactiveObject = hit.collider.GetComponent<IInteractive>();
                if (interactiveObject != null)
                {
                    // Попали в объект, с которым можно взаимодействовать
                    if (interactiveObject.RequiresProximity())
                    {
                        // Нужно подойти к объекту прежде чем взаимодействовать
                        Debug.Log("Пока что так");
                    }
                    // Позже, взаимодействие должно осуществляться в другом месте - скрипте игрока. Пока что здесь
                    interactiveObject.OnInteract();
                }
            }
        }
    }
}

