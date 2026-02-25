using BigProject.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BigProject.UI
{
    public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject _inventoryItemPrefab;
        [SerializeField] private Image _slotImage;
        
        [Header("Спрайты для слота")]
        [SerializeField] private Sprite _emptySprite;
        [SerializeField] private Sprite _fullSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _hoverSprite;
        
        private InventoryItemUI _inventoryItemUI;
        private bool _isEmpty = true;
        private bool _isSelected = false;
        public void SetSlot(Item item, Camera camera, Image noteImage = null)
        {
            if (_inventoryItemUI != null)
                ClearSlot();

            _inventoryItemUI = Instantiate(_inventoryItemPrefab, this.transform).GetComponent<InventoryItemUI>();
            _inventoryItemUI.SetItem(item, camera, noteImage);
            _inventoryItemUI.OnStartDrag += SlotSelected;
            _isEmpty = false;
            _isSelected = false;
            _slotImage.sprite = _fullSprite;
        }

        public void ClearSlot()
        {
            if (_inventoryItemUI == null)
                return;

            Destroy(_inventoryItemUI.gameObject);
            _inventoryItemUI = null;
            _isEmpty = true;
            _isSelected = false;
            _slotImage.sprite = _emptySprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isEmpty || _isSelected)
                return;

            _slotImage.sprite = _hoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isEmpty || _isSelected)
                return;
            
            _slotImage.sprite = _fullSprite;
        }

        private void SlotSelected(bool slotSelected)
        {
            if (slotSelected)
            {
                if (_isEmpty)
                    return;

                _isSelected = true;
                _slotImage.sprite = _selectedSprite;
            }
            else
            {
                _isSelected = false;
                _slotImage.sprite = _isEmpty ? _emptySprite : _fullSprite;
            }
        }
    }
}