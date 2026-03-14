using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace BigProject.Utilities
{
    public static class GameplayUtilities
    {
        public static bool TryGetClickedObject(Vector2 mousePosition, out GameObject gameObject)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                gameObject = hit.collider.gameObject;
                return true;
            }

            gameObject = null;
            return false;
        }

        public static float CurrentCameraTransitionTime
        {
            get
            {
                CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

                if (brain == null)
                {
                    return 0f;
                }

                CinemachineBlend blend = brain.ActiveBlend;
                return blend != null ? blend.Duration - blend.TimeInBlend : 0f;
            }
        }

        public static IEnumerator DoAfterConditionRoutine(Func<bool> conditionFunc, Action actionFunc)
        {
            yield return new WaitUntil(conditionFunc);
            actionFunc();
        }

        public static bool IsPointerOverUI()
        {
            if (EventSystem.current == null || Pointer.current == null)
            {
                return false;
            }

            PointerEventData eventData = new(EventSystem.current)
            {
                position = Pointer.current.position.ReadValue()
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
    }
}