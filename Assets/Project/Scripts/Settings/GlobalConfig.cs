using UnityEngine;

namespace BigProject.Settings
{
    /// <summary>
    /// Базовые настройки билда.
    /// </summary>
    [CreateAssetMenu(fileName = "GlobalConfig", menuName = "Scriptable Objects/Configs/GlobalConfig")]
    public class GlobalConfig : ScriptableObject
    {
        [field:SerializeField]
        public string PlayerProfileName { get; private set; }
        [field: SerializeField]
        public string QuestsFolder { get; private set; }
        [field: SerializeField]
        public int HUDInventoryWidgetId { get; private set; }
        [field: SerializeField]
        public int HUDJournalWidgetId { get; private set; }
    }
}