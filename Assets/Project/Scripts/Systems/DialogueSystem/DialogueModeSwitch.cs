using BigProject.Managers;
using BigProject.Player;
using System;
using UnityEngine;

namespace BigProject.Systems.DialogueSystem
{
    /// <summary>
    /// Потенциально поменяется, пока это нужно для перехвата статуса диалога из DialogueSystem.
    /// </summary>
    public class DialogueModeSwitch : MonoBehaviour
    {
        public Action DialogueСompleted;       

        private void OnEnable()
        {
            if (ServiceLocator.TryGetService(out PlayerInputHandler input))
            {
                input.SwitchToMiniGameActionMap();
            }
        }

        private void OnDisable()
        {
            if (ServiceLocator.TryGetService(out PlayerInputHandler input))
            {
                input.SwitchToPlayerActionMap();
                DialogueСompleted?.Invoke();
            }
        }
    }
}