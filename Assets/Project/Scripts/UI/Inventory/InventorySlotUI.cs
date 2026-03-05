using BigProject.Systems.Inventory;
using BigProject.Systems.Inventory.ItemsModifiers;
using System.Collections.Generic;
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
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _hoverSprite;
        
        private InventoryItemUI _inventoryItemUI;
        private bool _isEmpty = true;
        private bool _isSelected = false;

        public void SetSlot(Item item, Camera camera, Image noteImage = null, IReadOnlyList<ItemModifier> modifiers = null)
        {
            if (_inventoryItemUI != null)
                ClearSlot();

            _inventoryItemUI = Instantiate(_inventoryItemPrefab, this.transform).GetComponent<InventoryItemUI>();
            _inventoryItemUI.SetItem(item, camera, noteImage, modifiers);
            _inventoryItemUI.OnStartDrag += SlotSelected;
            _isEmpty = false;
            _isSelected = false;
        }

        public void ClearSlot()
        {
            if (_inventoryItemUI == null)
                return;

            Destroy(_inventoryItemUI.gameObject);
            _inventoryItemUI = null;
            _isEmpty = true;
            _isSelected = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isSelected)
                return;

            _slotImage.sprite = _hoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isSelected)
                return;
            
            _slotImage.sprite = _defaultSprite;
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
                _slotImage.sprite = _defaultSprite;
            }
        }
    }
}