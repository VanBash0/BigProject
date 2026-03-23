using BigProject.Managers.CursorManager;
using BigProject.Systems;
using BigProject.Utilities;
using UnityEngine;

namespace BigProject.Intercatable.HighlightedObjects
{
    public class CursorChangingEffect : HighlightEffect
    {
        [SerializeField] private Texture2D _highlightCursorTexture;
        [SerializeField] private Vector2 _highlightCursorHotspot = Vector2.zero;

        private CursorManager _cursorManager;

        public void Init(CursorManager cursorManager)
        {
            _cursorManager = cursorManager;
            ExceptionUtilities.ThrowIfNull(_cursorManager, string.Format(LogStr.CRITICAL_NULL_REFERENCE, gameObject.name, "CursorManager"));
        }

        public override void EnableEffect()
        {
            _cursorManager.SetCursor(_highlightCursorTexture, _highlightCursorHotspot);
        }

        public override void DisableEffect()
        {
            _cursorManager.ResetToDefault();
        }
    }
}