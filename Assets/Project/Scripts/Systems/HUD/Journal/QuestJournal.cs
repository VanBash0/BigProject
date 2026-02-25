using UnityEngine;
using BigProject.Managers;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using UnityEngine.Localization.Settings;
using BigProject.Systems.QuestSystem;
using BigProject.Utilities;


namespace BigProject.Systems.HUD
{
    /// <summary>
    /// Journal logic.
    /// </summary>
    public class QuestJournal : IDisposable
    {
        private QuestJournalConfig _config;
        private ProgressManager _pm;
        private List<(IQuestActionHandler, Action)> _journalWriters = new();


        private string _taskNote = "";
        private int _currentQuestId;
        private bool _hasActiveQuest;

        public event Action<string> TaskChanged;
        public event Action<string> QuestChanged;

        public QuestJournal(ProgressManager pm, QuestJournalConfig config)
        {
            _config = config;
            _pm= pm;
            ExceptionUtilities.ThrowIfNull(_config, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "Quest journal", "Config"));
            ExceptionUtilities.ThrowIfNull(_pm, String.Format(LogStr.CRITICAL_NULL_REFERENCE, "Quest journal", "Progress Manager"));
            //Init();
        }

        public void Dispose()
        {
            FinishCurrentQuest();

            foreach (QuestJournalTriggers questTriggers in _config)
            {
                _pm.RemoveQuestListener(questTriggers.QuestId, OnQuestStateChanged);
            }
        }

        public void Init()
        {
            ReleaseWriters();
            _hasActiveQuest = false;

            foreach (QuestJournalTriggers questTriggers in _config)
            {
                if (!_pm.AddQuestListener(questTriggers.QuestId, OnQuestStateChanged))
                {
                    Debug.LogWarning(String.Format(LogStr.WARNING_QUEST, $"Journal unable to subscribe on quest {questTriggers.QuestId}"));
                    continue;
                }

                // When first active quest met - start recording.
                if (!_hasActiveQuest && _pm.GetQuestState(questTriggers.QuestId) == QuestState.Active)
                {
                    StartQuestRecord(questTriggers);
                }
            }
        }

        private void StartQuestRecord(QuestJournalTriggers questTriggers)
        {
            if (questTriggers == null)
            {
                Debug.LogError(String.Format(LogStr.ERROR_QUEST, "journal unable to get triggers"));
                return;
            }

            _hasActiveQuest = true;
            _currentQuestId = questTriggers.QuestId;
            GameLogManager.Info(String.Format(LogStr.INFO_QUEST, $"start record quest {_currentQuestId} tasks to journal"));

            foreach (QuestJournalTriggers.JournalTrigger questTrigger in questTriggers.Triggers)
            {
                if (_pm.TryGetQuestActionHandler(questTriggers.QuestId, questTrigger.ActionId, out IQuestActionHandler actionHandler))
                {
                    // Add writer for this action handler.
                    Action writer = () =>
                    {
                        if (actionHandler.CurrentState == questTrigger.StateWhenWrite)
                        {
                            WriteToJournal(questTrigger.TableEntryKey);
                        }
                    };

                    // For case when condition already completed (ex. loading progress).
                    writer.Invoke();
                    actionHandler.StateChanged += writer;
                    _journalWriters.Add((actionHandler, writer));
                }
                else
                {
                    Debug.LogWarning(String.Format(LogStr.WARNING_QUEST, $"journal unable to get action {questTrigger.ActionId} of quest {_currentQuestId}"));
                }
            }

            string questName = LocalizationSettings.StringDatabase.GetLocalizedString(_config.LocalizationTableName, questTriggers.NameTableEntryKey);
            QuestChanged?.Invoke(questName);
            TaskChanged?.Invoke(_taskNote);
        }

        private void ReleaseWriters()
        {
            if (_journalWriters.Count > 0)
            {
                foreach ((IQuestActionHandler, Action) writerRecord in _journalWriters)
                {
                    writerRecord.Item1.StateChanged -= writerRecord.Item2;
                }

                _journalWriters.Clear();
            }
        }

        private void WriteToJournal(string tableEntry)
        {
            _taskNote = LocalizationSettings.StringDatabase.GetLocalizedString(_config.LocalizationTableName, tableEntry);
            TaskChanged?.Invoke(_taskNote);
        }

        private void OnQuestStateChanged(IQuest quest)
        {
            if (_hasActiveQuest)
            {
                if (_currentQuestId == quest.ID && quest.CurrentState > QuestState.Active)
                {
                    FinishCurrentQuest();
                }
            }
            else if (quest.CurrentState == QuestState.Active)
            {
                StartQuestRecord(_config.GetQuestJournalTriggers(_currentQuestId));
            }    
        }

        private void FinishCurrentQuest()
        {
            if (_hasActiveQuest)
            {
                _taskNote = "";
                QuestChanged?.Invoke("");
                TaskChanged?.Invoke(_taskNote);
                ReleaseWriters();
                _hasActiveQuest = false;
            }
        }
    }
}