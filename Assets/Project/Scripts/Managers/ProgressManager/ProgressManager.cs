using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BigProject.Systems.QuestSystem;

namespace BigProject.Managers
{
    /// <summary>
    /// Progress manager, manages game quests and progress recording.
    /// </summary>
    public class ProgressManager : ISavable, IDisposable
    {
        // Objects whose state needs to be fixed when saving progress.
        [SerializeField]
        private List<ISavable> _savable;

        private string _profileName; // To separate player profiles.
        private SavesManager _savesManager;
        private Dictionary<int, IQuest> _quests;

        // ISavable
        public string Key => "GeneralProgress";
        public object SavingData => this;

        /// <summary>
        /// When True, saves progress when any of the tracked quests changes status.
        /// </summary>
        public bool AutoSave { get; set; } = true;


        /// <param name="profileName">Player profile name</param>
        /// <param name="questLoader">Quest loader to use</param>
        public ProgressManager(string profileName, IQuestLoader questLoader, SavesManager savesManager)
        {
            _profileName = profileName;

            // Record general data from Progress Manager.
            _savable = new() { this }; 

            _savesManager = savesManager;

            try
            {
                _quests = questLoader.GetAllQuests().ToDictionary(x => x.ID, x => x);
            }
            catch (ArgumentException ex)
            {
                Debug.LogError($"ProgressManager can't add quest.\n{ex.Message}");
                _quests = new();
            }
            catch (Exception ex)
            {
                Debug.LogError($"ProgressManager try to add quests with the same key!\n{ex.Message}");
                _quests = new();
            }

            AddQuestsToSavable();

            foreach (var quest in _quests.Values)
            {
                quest.StateChanged += OnQuestProgressed;
            }
        }

        /// <summary>
        /// Add quests to save data.
        /// </summary>
        private void AddQuestsToSavable()
        {
            foreach (IQuest quest in _quests.Values)
            {
                if (quest is ISavable savable)
                {
                    AddSavable(savable);
                }
            }
        }

        /// <summary>
        /// Subscribe to quest.
        /// </summary>
        public bool AddQuestListener(int quiestId, Action<IQuest> callback)
        {
            if (!_quests.TryGetValue(quiestId, out var quest))
            {
                Debug.LogError($"Progress manager unable to add listener. Has no Quest [{quiestId}].");
                return false;
            }


            quest.Progressed += callback;
            return true;
        }

        /// <summary>
        /// Unsubscribe from quest.
        /// </summary>
        public void RemoveQuestListener(int quiestId, Action<IQuest> callback)
        {
            if (_quests.TryGetValue(quiestId, out var quest))
            {
                quest.Progressed -= callback;
            }
            else
            {
                Debug.LogWarning($"Progress manager unable to remove listener. Has no Quest [{quiestId}].");
            }
        }

        /// <summary>
        /// Adds a saved object (e.g. inventory, characters, etc.).
        /// </summary>
        public void AddSavable(ISavable savable)
        {
            if (_savable.Contains(savable))
            {
                Debug.LogWarning($"Progress manager already tracking savable data [{savable.Key}]");
                return;
            }

            _savable.Add(savable);
        }

        /// <summary>
        /// Remove object from saving data.
        /// </summary>
        public void RemoveSavable(ISavable savable)
        {
            if (!_savable.Remove(savable))
            {
                Debug.LogWarning($"Progress manager try to remove not tracking savable data [{savable.Key}].");
            }
        }

        public void SaveProgress()
        {
            _savesManager.SaveGame(_profileName, _savable);
        }

        public void LoadProgress()
        {
            _savesManager.LoadGame(_profileName, _savable);
        }

        public bool HasSavedProgress()
        {
            return _savesManager.HasSave(_profileName);
        }

        /// <summary>
        /// Manual quest transition.
        /// </summary>
        /// <param name="newState">New action state</param>
        /// <returns></returns>
        public bool ManualProgress(int questId, int actionId, QuestActionState newState)
        {
            if (!_quests.TryGetValue(questId, out var quest))
            {
                Debug.LogError($"Progress manager has no quest [{questId}], but trigger try to access it.");
                return false;
            }

            QuestState prevState = quest.CurrentState;
            return quest.ManualTransition(actionId, newState);
        }

        /// <returns>Quest's action state.</returns>
        public QuestActionState GetActionState(int questId, int actionId)
        {
            if (!_quests.TryGetValue(questId, out var quest))
            {
                Debug.LogError($"Progress manager has no quest [{questId}], but you try to get Action state from it.");
                return QuestActionState.Undefined;
            }

            if (!quest.TryGetActionState(actionId, out var actionState))
            {
                Debug.LogError($"Quest [{questId}] has no Action [{actionId}], but you try to get it.");
                return QuestActionState.Undefined;
            }

            return actionState;
        }

        /// <returns>All quest's actions.</returns>
        public IReadOnlyDictionary<int, QuestActionState> GetAllActions(int questId)
        {
            if (!_quests.TryGetValue(questId, out var quest))
            {
                Debug.LogError($"Progress manager has no quest [{questId}], but you try to get Action state from it.");
                return new Dictionary<int, QuestActionState>();
            }

            return quest.GetAllActions();
        }

        /// <param name="actionHandler">Quest action handler</param>
        /// <returns>True when success.</returns>
        public bool TryGetQuestActionHandler(int questId, int actionId, out IQuestActionHandler actionHandler)
        {
            if (!_quests.TryGetValue(questId, out var quest))
            {
                Debug.LogError($"Progress manager has no quest [{questId}], but you try to get action handler from it.");
                actionHandler = null;
                return false;
            }

            return quest.TryGetActionHandler(actionId, out actionHandler);
        }

        /// <returns>Actual state of quest.</returns>
        public QuestState GetQuestState(int questId)
        {
            if (!_quests.TryGetValue(questId, out var quest))
            {
                Debug.LogWarning($"Progress manager has no quest [{questId}], but you try to get quest state.");
                return QuestState.Inactive;
            }

            return quest.CurrentState;            
        }

        private void OnQuestProgressed(IQuest quest)
        {
            if (!_quests.ContainsKey(quest.ID))
            {
                Debug.LogWarning($"Progress manager get callback from untracked quest [{quest.Name}].");
                return;
            }

            if (AutoSave)
            {
                Debug.Log("Autosaving...");
                SaveProgress();
            }
        }

        public void Dispose()
        {
            foreach (var quest in _quests.Values)
            {
                quest.Progressed -= OnQuestProgressed;
            }
        }
    }
}