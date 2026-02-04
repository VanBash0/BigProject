using UnityEngine;
using BigProject.Systems.HUD;
using System;
using TMPro;

namespace BigProject.UI
{
    /// <summary>
    /// Отображение журнала в интерфейсе.
    /// </summary>
    public class JournalView : MonoBehaviour, IHUDWidget
    {
        [SerializeField]
        private GameObject _journalObj;
        [SerializeField]
        TMP_Text _name;
        [SerializeField]
        TMP_Text _task;

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
    }
}