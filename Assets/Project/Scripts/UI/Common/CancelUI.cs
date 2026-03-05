using BigProject.Systems;
using BigProject.Systems.HUD;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BigProject.UI.Common
{
    public class CancelUI : MonoBehaviour, IHUDWidget
    {
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}