using BigProject.Intercatable;
using BigProject.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace BigProject.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Animator _animatorController;

        [SerializeField] private float _navMeshHitPointDistance = 5f;
        [SerializeField] private float _rotationSpeed = 10f;

        private PlayerInputHandler _inputHandler;
        private IInteractable _interactable = null;
        private Vector3 _destination;
        private bool _isMoving;

        private Camera _camera;

        private const string MOVING_ANIM_BOOL = "IsMoving";

        public bool IsMoving => _isMoving;

        public void Init(PlayerInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
            ExceptionUtilities.ThrowIfNull(_inputHandler, gameObject.name, "player input handler is null!");
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            _navMeshAgent.updateRotation = false;
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
            Vector2 mousePosition = _inputHandler.GetMousePosition();
            Ray ray = _camera.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                
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
                RotateTowardsMovement();

                if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                {
                    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        _isMoving = false;
                        _animatorController.SetBool(MOVING_ANIM_BOOL, false);
                        Interact();
                    }
                }
            }
        }

        private void RotateTowardsMovement()
        {
            if (_navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon)
            {
                // Получаем направление движения
                Vector3 moveDirection = _navMeshAgent.velocity.normalized;
                moveDirection.y = 0; // Игнорируем вертикальную составляющую

                if (moveDirection != Vector3.zero)
                {
                    // Создаем поворот к направлению движения
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                    // Плавно поворачиваем игрока
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        _rotationSpeed * Time.deltaTime
                    );
                }
            }
        }

        public void Move()
        {
            _isMoving = true;
            _animatorController.SetBool(MOVING_ANIM_BOOL, true);
            _navMeshAgent.SetDestination(_destination);
        }

        private void Interact()
        {
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
