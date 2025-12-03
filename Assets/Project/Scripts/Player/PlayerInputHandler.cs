using BigProject.Intercatable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigProject.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Настройки луча")]
        [SerializeField] private float _maxRayDistance = 1000f;

        private Camera _camera;
        private PlayerController _playerController;
        private void Start()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            _playerController = GetComponent<PlayerController>();
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
                _playerController.SetDestination(hit.point);

                // Передаем интерактивный объект игроку
                IInteractable interactableObject = hit.collider.GetComponent<IInteractable>();
                _playerController.SetInterableObject(interactableObject);
            }
        }
    }
}

