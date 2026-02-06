using BigProject.Intercatable.HighlightedObjects;
using BigProject.NPC;
using UnityEngine;

namespace BigProject.Gameplay
{
    public class CharacterCursorSwitch : HighlightEffect
    {
        [SerializeField]
        private CursorChangingEffect _cursorEffect;
        [SerializeField]
        private DialogNPC _dialog;

        public override void DisableEffect()
        {
            _cursorEffect.DisableEffect();
        }

        public override void EnableEffect()
        {
            if (_dialog.StartDialogLine != null)
            {
                _cursorEffect.EnableEffect();
            }
        }
    }
}