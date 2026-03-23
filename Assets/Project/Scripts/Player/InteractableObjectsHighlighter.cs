using BigProject.Managers;
using BigProject.Managers.CursorManager;
using BigProject.Systems;
using BigProject.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BigProject.Intercatable.HighlightedObjects
{
    public class InteractableObjectsHighlighter : MonoBehaviour
    {
        [SerializeField] 
        private float _objectCheckDelay = 0.1f;

        private Camera _camera;
        private SceneLoadManager _sceneLoader;
        private WaitForSeconds _objectCheckWait;
        private HighlightedObject _currentObject;
        private CursorManager _cursorManager;

        public void Init(SceneLoadManager sceneLoader, CursorManager cursorManager)
        {
            _sceneLoader = sceneLoader;
            _cursorManager = cursorManager;
            ExceptionUtilities.ThrowIfNull(_sceneLoader, string.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "SceneLoadManager"));
            ExceptionUtilities.ThrowIfNull(_cursorManager, string.Format(LogStr.CRITICAL_NULL_REFERENCE, "InteractableObjectsHighlighter", "CursorManager"));
        }

        private void Awake()
        {
            _objectCheckWait = new(_objectCheckDelay);
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

        public void RestartChecking()
        {
            _camera = Camera.main;
            StopAllCoroutines();
            _cursorManager.ResetToDefault();
            StartCoroutine(ObjectCheckRoutine());
        }

        private void OnEnable()
        {
            _sceneLoader.SceneLoadingStarted += StopAllCoroutines;
            _sceneLoader.SceneLoadingCompleted += RestartChecking;
        }

        private void OnDisable()
        {
            _sceneLoader.SceneLoadingStarted -= StopAllCoroutines;
            _sceneLoader.SceneLoadingCompleted -= RestartChecking;
        }
    }
}