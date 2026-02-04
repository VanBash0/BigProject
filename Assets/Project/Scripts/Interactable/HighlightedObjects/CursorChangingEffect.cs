using BigProject.Managers.CursorManager;
using UnityEngine;

namespace BigProject.Intercatable.HighlightedObjects
{
    public class CursorChangingEffect : HighlightEffect
    {
        [SerializeField] private CursorManager _cursorManager; //Будет изменено после появления системы внедрения зависимостей
        [SerializeField] private Texture2D _highlightCursorTexture;
        [SerializeField] private Vector2 _highlightCursorHotspot = Vector2.zero;

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