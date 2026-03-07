using BigProject.Managers;
using BigProject.Systems;
using BigProject.Systems.Inventory;
using BigProject.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.VillageElderQuest
{
    public class QuestActions : MonoBehaviour
    {
        [SerializeField]
        private string _bagItemName;
        [SerializeField]
        private GameObject _bag;
        [SerializeField]
        private GameObject _ambassador;
        [SerializeField]
        private List<string> _keysItemsNames;

        private InventorySystem _inventory;

        public void Init(InventorySystem inventory)
        {
            _inventory = inventory;
            ExceptionUtilities.ThrowIfNull(_inventory, String.Format(gameObject.name, "Inventory System"));
        }

        private void Start()
        {
            Assert.IsNotNull(_bag, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Bag"));
            Assert.IsNotNull(_ambassador, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Ambassador"));
        }

        public void ShowBag()
        {
            _bag.SetActive(true);
        }

        public void GetBag()
        {
            GameLogManager.Info(String.Format(LogStr.INFO_QUEST, "add bag to inventory."));
            _inventory.AddItemByName(_bagItemName);
            ReplicaManager.ShowReplica("Bag");
        }

        public void RemoveBag()
        {
            GameLogManager.Info(String.Format(LogStr.INFO_QUEST, "remove bag from inventory."));
            _inventory.AddItemByName(_bagItemName);
        }

        public void AmbassadorAppearance()
        {
            _ambassador.SetActive(true);
        }

        public void RemoveAmbassador()
        {
            if (_ambassador != null)
            {
                Destroy(_ambassador);
            }
        }

        public void GetKeys()
        {
            GameLogManager.Info(String.Format(LogStr.INFO_QUEST, "add keys to inventory."));

            foreach (string key in _keysItemsNames)
            {
                _inventory.AddItemByName(key);
            }
        }
    }
}
