using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Gameplay.TownHall
{
    public class KeysConfig : MonoBehaviour
    {
        [SerializeField]
        private List<KeySet> _keys;

        [Serializable]
        private class KeySet
        {
            public string itemName;
            public int prefabId;
        }

        public bool ContainsKey(string name) => _keys.Find(x => x.itemName == name) != null;

        public bool TryGetPrefabId(string name, out int prefabId)
        {
            KeySet keySet = _keys.Find(x => x.itemName == name);

            if (keySet == null)
            {
                prefabId = -1;
                return false;
            }

            prefabId = keySet.prefabId;
            return true;
        }
    }
}