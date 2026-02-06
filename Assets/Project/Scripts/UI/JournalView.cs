using UnityEngine;
using BigProject.Systems.HUD;
using System;
using TMPro;
using UnityEngine.Assertions;

namespace BigProject.UI
{
    /// <summary>
    /// Отображение журнала в интерфейсе.
    /// </summary>
    public class JournalView : MonoBehaviour, IHUDWidget, IDisposable
    {
        [SerializeField]
        private GameObject _journalObj;
        [SerializeField]
        private TMP_Text _name;
        [SerializeField]
        private TMP_Text _task;

        private QuestJournal _journal;

        public void Init(QuestJournal journal)
        {
            Assert.IsNotNull(journal, "Journal view unable to work with null journal.");
            _journal = journal;
            journal.QuestChanged += OnQuestStateChanged;
            journal.TaskChanged += OnTaskChanged;
        }

        public void Hide()
        {
            _journalObj.SetActive(false);
        }

        public void Show()
        {
            _journalObj.SetActive(true);
        }

        public void OnQuestStateChanged(string name)
        {
            _name.text = name;
        }

        public void OnTaskChanged(string task)
        {
            _task.text = task;
        }

        public void Dispose()
        {
            _journal.QuestChanged -= OnQuestStateChanged;
            _journal.TaskChanged -= OnTaskChanged;
        }
    }
}