using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigProject.Intercatable.HighlightedObjects
{
    public class InteractableObjectsHighlighter : MonoBehaviour
    {
        [SerializeField] private float _objectCheckDelay;
        [SerializeField] private Camera _camera;

        private WaitForSeconds _objectCheckWait;
        private HighlightedObject _currentObject;

        private void Awake()
        {
            _objectCheckWait = new(_objectCheckDelay);
            StartCoroutine(ObjectCheckRoutine());
        }

        // Корутина проверяет, наведён ли курсор на выделяемый предмет, если да - вызывает его эффекты
        private IEnumerator ObjectCheckRoutine()
        {
            while (true)
            {
                Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // Debug.Log(hit.transform.gameObject.name);
                    HighlightedObject newObject = hit.transform.GetComponent<HighlightedObject>();

                    if(newObject == null)
                    {
                        if (_currentObject != null)
                        {
                            _currentObject.Unhighlight();
                            _currentObject = null;
                        }

                        yield return _objectCheckWait;
                        continue;
                    }

                    if (newObject != _currentObject)
                    {
                        if (_currentObject != null)
                        {
                            _currentObject.Unhighlight();
                        }

                        newObject.Highlight();
                        _currentObject = newObject;
                    }
                }
                else
                {
                    if (_currentObject != null)
                    {
                        _currentObject.Unhighlight();
                        _currentObject = null;
                    }
                }

                yield return _objectCheckWait;
            }
        }
    }
}