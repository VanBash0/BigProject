using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.HUD;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace BigProject.UI
{
    public class InventoryUI : MonoBehaviour, IHUDWidget
    {
        [SerializeField] private List<InventorySlotUI> _inventorySlots;
        [SerializeField] private Image _noteImage;
        private InventorySystem _inventorySystem;

        public void Init(InventorySystem inventorySystem)
        {
            if (inventorySystem == null)
            {
                GameLogManager.Error("InventorySystem in InventoryUI was set to null");
                throw new System.ArgumentNullException(nameof(inventorySystem), "InventorySystem cannot be null");
            }

            _inventorySystem = inventorySystem;
            _inventorySystem.OnInventoryUpdated += UpdateInventory;
        }

        private void Start()
        {
            Assert.AreEqual(5, _inventorySlots.Count, "Less than 5 inventory slots were added in InventoryUI");
            Assert.IsNotNull(_noteImage, "Note image was not initialised for InventoryUI");
            Assert.IsNotNull(_inventorySystem, "Inventory System was not initialised for InventoryUI");
        }

        private void OnDestroy()
        {
            _inventorySystem.OnInventoryUpdated -= UpdateInventory;
        }

        private void UpdateInventory()
        {
            bool hasNote = false;
            List<Item> heldItems = _inventorySystem.GetAllHeldItems();

            if (heldItems.Count == 0)
            {
                for (int i = 0; i < _inventorySlots.Count; i++)
                    _inventorySlots[i].ClearSlot();
                
                _noteImage.gameObject.SetActive(false);
                return;
            }

            for (int i = 0; i < heldItems.Count; i++)
            {
                _inventorySlots[i].SetSlot(heldItems[i], Camera.main, _noteImage);
                if (heldItems[i]._noteSprite != null)
                    hasNote = true;
            }

            for (int i = heldItems.Count; i < _inventorySlots.Count; i++)
            {
                _inventorySlots[i].ClearSlot();
            }

            // hides note image if there is no corresponding item
            if (!hasNote)
            {
                _noteImage.gameObject.SetActive(false);
            }
        }

        public void SetNoteVisibility(bool isVisible)
        {
            _noteImage.gameObject.SetActive(isVisible);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}