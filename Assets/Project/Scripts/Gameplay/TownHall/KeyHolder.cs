using BigProject.Systems;
using BigProject.Systems.Inventory;
using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Gameplay.TownHall
{
    public class KeyHolder : MonoBehaviour, IUsesItem
    {
        [SerializeField]
        private KeysConfig _keysConfig;
        [SerializeField]
        private ChestPuzzle _chestPuzzle;
        [SerializeField]
        private int _keyHolderId;

        private void Awake()
        {
            Assert.IsNotNull(_keysConfig, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Keys Config"));
            Assert.IsNotNull(_keysConfig, String.Format(LogStr.CRITICAL_NOT_SERIALIZED_FIELD, gameObject.name, "Chest Puzzle"));
        }

        public bool DoesUseItem(Item item) => _keysConfig.ContainsKey(item._name);

        public void UseItem(Item item)
        {
            if (_keysConfig.TryGetPrefabId(item._name, out int prefabId))
            {
                _chestPuzzle?.InstallKey(item._name, _keyHolderId, prefabId);
                return;
            }

            Debug.LogWarning(String.Format(LogStr.WARNING_QUEST, "Try to apply unsupported item"));
        }
    }
}