using BigProject.Managers;
using BigProject.Systems.Inventory.ItemsModifiers;
using BigProject.UI;
using BigProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BigProject.Systems.Inventory
{
    public class InventorySystem : IDisposable
    {
        private ItemsDatabaseSO _itemsDatabase;
        private ModifiersDatabaseSO _modifiersDatabase;
        private List<int> _heldItems = new List<int>();
        private Dictionary<string, List<ItemModifier>> _itemsModifiers = new();
        public event Action OnInventoryUpdated;

        public InventorySystem(ItemsDatabaseSO itemsDatabase, ModifiersDatabaseSO modifiersDatabase)
        {
            for (int i = 0; i < 5; i++)
            {
                _heldItems.Add(-1);
            }

            _itemsDatabase = itemsDatabase;
            _modifiersDatabase = modifiersDatabase;
            ExceptionUtilities.ThrowIfNull(_itemsDatabase, "InventorySystem", "itemsDatabase is null");
            ExceptionUtilities.ThrowIfNull(_modifiersDatabase, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "InventorySystem", "Modifiers Database"));
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        public void Dispose()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene _, Scene __)
        {
            OnInventoryUpdated?.Invoke();
        }

        private void AddToInventory(int value)
        {
            for (int i = 0; i < _heldItems.Count; i++)
            {
                if (_heldItems[i] == -1)
                {
                    _heldItems[i] = value;
                    break;
                }
            }

            GameLogManager.Info("Added item to inventory");
            OnInventoryUpdated?.Invoke();
        }
        
        //here, id is not a database id but an inventory id
        private void RemoveFromInventory(int id)
        {
            _itemsModifiers.Remove(_itemsDatabase._items.ElementAtOrDefault(_heldItems[id])._name);

            for (int i = id; i < _heldItems.Count - 1; i++)
            {
                _heldItems[i] = _heldItems[i + 1];
            }

            _heldItems[_heldItems.Count - 1] = -1;

            GameLogManager.Info("Removed item from inventory");
            OnInventoryUpdated?.Invoke();
        }

        /// <summary>
        /// Adds item by its id in database
        /// </summary>
        public void AddItemByItemID(int itemID)
        {
            if (itemID >= _itemsDatabase._items.Count)
            {
                Debug.LogError($"itemID out of itemsDB bounds");
                return;
            }

            AddToInventory(itemID);
        }

        public void AddItemByName(string itemName)
        {
            if (_itemsDatabase._items.Where(x => x._name.Equals(itemName)).Count() == 0)
            {
                Debug.LogError($"Item {itemName} does not exist in itemsDB");
                return;
            }
            
            int itemID = _itemsDatabase._items.IndexOf(_itemsDatabase._items.Where(x => x._name.Equals(itemName)).First());
            AddToInventory(itemID);
        }

        public void AddItemModifier(string itemModifierName)
        {
            if (!_modifiersDatabase.TryGetModifier(itemModifierName, out ItemModifier itemModifier))
            {
                Debug.LogError(String.Format(LogStr.ERROR_QUEST, $"has no modifier {itemModifierName}"));
                return;
            }

            string itemName = itemModifier.ItemName;

            if (!HasItemByName(itemName))
            {
                Debug.LogWarning(String.Format(LogStr.WARNING_QUEST, $"has no item {itemName} to add modifier {itemModifierName}"));
                return;
            }

            int key = _heldItems.FindIndex(x => _itemsDatabase._items[x]._name.Equals(itemName));

            if (!_itemsModifiers.ContainsKey(itemName))
            {
                _itemsModifiers.Add(itemName, new());
            }
            else if (_itemsModifiers[itemName].Contains(itemModifier))
            {
                Debug.LogWarning(String.Format(LogStr.WARNING_QUEST, $"already has modifier {itemModifierName} on item {itemName}"));
                return;
            }

            _itemsModifiers[itemName].Add(itemModifier);
            OnInventoryUpdated?.Invoke();
        }

        /// <summary>
        /// Removes item by its id in database
        /// </summary>
        public void RemoveItemById(int itemID)
        {
            if (_heldItems.Count == 0)
            {
                Debug.LogError("Can't remove an item from an empty inventory");
                return;
            }

            if (itemID >= _itemsDatabase._items.Count)
            {
                Debug.LogError($"itemID out of itemsDB bounds");
                return;
            }

            int itemInventoryID = _heldItems.IndexOf(itemID);
            if (itemInventoryID == -1)
            {
                Debug.LogError($"Item with id {itemID} does not exist in inventory");
                return;
            }

            RemoveFromInventory(itemInventoryID);
        }

        public void RemoveItemByName(string itemName)
        {
            if (_heldItems.Count == 0)
            {
                Debug.LogError("Can't remove an item from an empty inventory");
                return;
            }

            if (_itemsDatabase._items.Where(x => x._name == itemName).Count() == 0)
            {
                Debug.LogError($"Item {itemName} does not exist in itemsDB");
                return;
            }

            int itemID = _itemsDatabase._items.IndexOf(_itemsDatabase._items.Where(x => x._name == itemName).First());
            int itemInventoryID = _heldItems.IndexOf(itemID);
            if (itemInventoryID == -1)
            {
                Debug.LogError($"Item {itemName} does not exist in inventory");
                return;
            }

            RemoveFromInventory(itemInventoryID);
        }

        /// <summary>
        /// Returns an item by its name. Use HasItemByName() beforehand
        /// </summary>
        public Item GetItemByName(string itemName)
        {
            return _itemsDatabase._items.Where(x => x._name == itemName).First();
        }

        /// <summary>
        /// Returns an item by its id in database. Use HasItemById() beforehand 
        /// </summary>
        public Item GetItemById(int itemID)
        {
            return _itemsDatabase._items[itemID];
        }

        public bool HasItemByName(string itemName)
        {
            if (_heldItems.Where((x) => x != -1 && _itemsDatabase._items[x]._name.Equals(itemName)).Count() == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if item exists by its database id
        /// </summary>
        public bool HasItemByID(int itemID)
        {
            if (_heldItems.Where((x) => x == itemID).Count() == 0)
                return false;

            return true;
        }

        /// <returns>All modifiers that item has.</returns>
        public IReadOnlyList<ItemModifier> GetHeldItemModifiers(string name) => _itemsModifiers.ContainsKey(name) ? _itemsModifiers[name] : null;

        /// <summary>
        /// Returns list of all held items
        /// </summary>
        public List<Item> GetAllHeldItems()
        {
            List<Item> items = new List<Item>();
            foreach (int id in _heldItems)
            {
                if (id == -1)
                    break;
                items.Add(_itemsDatabase._items[id]);
            }
            return items;
        }
    }
}