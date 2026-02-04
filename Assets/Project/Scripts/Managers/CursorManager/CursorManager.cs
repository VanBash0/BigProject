using UnityEngine;

namespace BigProject.Managers.CursorManager
{
    public class CursorManager : MonoBehaviour
    {
        [SerializeField] private Texture2D _defaultCursorTexture;
        [SerializeField] private Vector2 _defaultCursorHotspot;
        [SerializeField] private CursorMode _cursorMode;

        private void Awake()
        {
            ResetToDefault();
        }

        /// <summary>
        /// Устанавливает новый курсор
        /// <param name="cursorTexture">Текстура нового курсора</param>
        /// <param name="hotspot">Точка нажатия курсора</param>
        /// </summary>
        public void SetCursor(Texture2D cursorTexture, Vector2 hotspot = default)
        {
            Cursor.SetCursor(cursorTexture, hotspot, _cursorMode);
        }

        /// <summary>
        /// Возвращает к курсору по умолчанию
        /// </summary>
        public void ResetToDefault()
        {
            Cursor.SetCursor(_defaultCursorTexture, _defaultCursorHotspot, _cursorMode);
        }
    }
}