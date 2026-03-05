using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BigProject.UI.Dialogue
{
    public class AnswerOptionTextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private GameObject _hoverObj;
        public void OnPointerEnter(PointerEventData eventData)
        {
            _hoverObj.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hoverObj.SetActive(false);
        }
    }

}

