using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

namespace BigProject.Test.VFrolov
{
    /// <summary>
    /// Отображает статус квеста.
    /// </summary>
    public class TVFQuestProgressView : MonoBehaviour
    {
        [Inject]
        private ProgressManager _progressManager;
        [SerializeField]
        private TMP_Text _questLog;

        private void OnEnable()
        {
            // Подписываемся на квест с id 10 (questId).
            _progressManager.AddQuestListener(TVFConfig.questId, OnQuestProgress);
        }

        private void OnDisable()
        {
            _progressManager.RemoveQuestListener(TVFConfig.questId, OnQuestProgress);
        }

        private void Start()
        {
            PrintAllActivities();
        }

        private void OnQuestProgress(IQuest _)
        {
            PrintAllActivities();
        }

        private void PrintAllActivities()
        {
            StringBuilder summary = new("Статус:\n");

            // Получаем все активности квеста.
            foreach (var entry in _progressManager.GetAllActions(TVFConfig.questId))
            {
                // В entry есть id активнсоти и ее текущий статус.
                // Форматируем для удобоваримого вида.
                string color = GetColorByState(entry.Value);
                string ActivityName = TVFConfig.GetNameByID(entry.Key);
                string ActivityState = TVFConfig.GetNameByState(entry.Value);
                summary.Append($"{ActivityName}: <color={color}>{ActivityState}</color>\n");
            }

            _questLog.text = summary.ToString();
        }

        private string GetColorByState(QuestActionState state) =>
            state switch
            {
                QuestActionState.Inactive => "grey",
                QuestActionState.Active => "white",
                QuestActionState.Completed => "green",
                _ => "red"
            };
    }
}