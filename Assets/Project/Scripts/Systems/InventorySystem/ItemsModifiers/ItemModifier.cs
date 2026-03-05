using System;
using UnityEngine;

namespace BigProject.Systems.Inventory.ItemsModifiers
{ 
    [Serializable]
    public class ItemModifier
    {
        [field: SerializeField]
        public string ModifierName { get; private set; }
        [field: SerializeField]
        public string ItemName { get; private set; }
        [field: SerializeField]
        public Sprite ItemSprite { get; private set; }
        [field: SerializeField]
        public Sprite NoteSprite { get; private set; }
        [field: SerializeField]
        public Vector2 ItemUV {  get; private set; }
        [field: SerializeField]
        public Vector2 NoteUV { get; private set; }
    }
}