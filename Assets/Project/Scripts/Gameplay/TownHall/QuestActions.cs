using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.Inventory;
using BigProject.Utilities;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.TownHall
{
    public class QuestActions : MonoBehaviour
    {
        [SerializeField]
        private string _noteItemName;

        private InventorySystem _inventory;
        private ItemsDatabaseSO _itemsDB;
        private int pillarId = 6;

        [Serializable]
        private struct PillarNote
        {
            public Texture2D image;
            public Vector2 uv;
        }

        public void Init(InventorySystem inventory, ItemsDatabaseSO itemsDB)
        {
            _inventory = inventory;
            _itemsDB = itemsDB;
            ExceptionUtilities.ThrowIfNull(_inventory, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Inventory System"));
            ExceptionUtilities.ThrowIfNull(_itemsDB, String.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "Items Database"));
        }

        // Draw clue to note texture.
        public void AddRecordToNote(string recordName)
        {
            if (!_inventory.HasItemByName(_noteItemName))
            {
                _inventory.AddItemByName(_noteItemName);
            }    

            _inventory.AddItemModifier(recordName);

            // For test only
            //_inventory.AddItemByItemID(pillarId);
            //pillarId++;
        }
    }
}
