using BigProject.Systems.HUD;
using UnityEngine;

namespace BigProject.UI.Common
{
    public class ResetUI : MonoBehaviour, IHUDWidget
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