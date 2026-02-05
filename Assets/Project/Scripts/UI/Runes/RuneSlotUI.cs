using UnityEngine;

namespace BigProject.UI
{
    public class RuneSlotUI : MonoBehaviour
    {
        [SerializeField] GameObject _runeImage;
        public void ShowRune()
        {
            _runeImage.SetActive(true);
        }
    }
}