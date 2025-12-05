using BigProject.Intercatable;
using UnityEngine;
using UnityEngine.AI;

namespace BigProject.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler _inputHandler;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        [SerializeField] private float _navMeshHitPointDistance = 5f;
        [SerializeField] private float _stoppingDistance = 5f;
        [SerializeField] private float _characterSpeed;

        private IInteractable _interactable = null;
        private Vector3 _destination;
        private bool _isMoving;

        private Camera _camera;

        private void Awake()
        {
            _navMeshAgent.speed = _characterSpeed;
            _navMeshAgent.stoppingDistance = _stoppingDistance;
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void OnEnable()
        {
            _inputHandler.Click += OnClick;
        }
        private void OnDisable()
        {
            _inputHandler.Click -= OnClick;
        }

        private void OnClick()
        {
            Debug.Log("Clicked");
            Vector2 mousePosition = _inputHandler.GetMousePosition();
            Ray ray = _camera.ScreenPointToRay(mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Попал в: {hit.collider.name}");
                
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, _navMeshHitPointDistance, NavMesh.AllAreas))
                {
                    SetDestination(navMeshHit.position);
                    Move();

                    // Передаем интерактивный объект игроку
                    IInteractable interactableObject = hit.collider.GetComponent<IInteractable>();
                    SetInterableObject(interactableObject);
                }
            }
        }

        private void Update()
        {
            if (_isMoving)
            {
                if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                {
                    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _isMoving = false;
                        Interact();
                    }
                }
            }
        }

        private void Move()
        {
            _isMoving = true;
            _navMeshAgent.SetDestination(_destination);
            Debug.Log($"Двигаюсь к точке {_destination}");
        }

        private void Interact()
        {
            Debug.Log("Взаимодействие");
            if (_interactable != null)
                _interactable.Interact();
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
        }

        public void SetInterableObject(IInteractable interactableObject)
        {
            _interactable = interactableObject;
        }
    }
}
