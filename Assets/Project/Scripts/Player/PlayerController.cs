using BigProject.Intercatable;
using UnityEngine;

namespace BigProject.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _stoppingDistance = 5f;
        private bool _canMoveToDestination = false;
        private Vector3 _destination = Vector3.zero;
        private IInteractable _interactable = null;
        void Update()
        {
            if (_canMoveToDestination)
            {
                if (Vector3.Distance(_destination, transform.position) <= _stoppingDistance)
                {
                    Debug.Log("Приблизились к объекту");
                    _canMoveToDestination = false;
                    if (_interactable != null)
                    {
                        // Есть объект, можно взаимодействовать с ним
                        Debug.Log("Взаимодействие");
                        _interactable.Interact();
                    }
                }
                else
                {
                    Move();
                }
            }
        }

        private void Move()
        {
            Debug.Log($"Двигаюсь к точке {_destination}");
        }

        public void SetDestination(Vector3 destination)
        {
            _canMoveToDestination = true;
            _destination = destination;
        }

        public void SetInterableObject(IInteractable interactableObject)
        {
            _interactable = interactableObject;
        }
    }
}
