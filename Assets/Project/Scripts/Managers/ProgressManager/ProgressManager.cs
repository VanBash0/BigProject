using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BigProject.Systems;

namespace BigProject.Managers
{
    /// <summary>
    /// Менеджер прогресса, управляет игровыми квестами и фиксацией прогресса.
    /// </summary>
    public class ProgressManager : ISavable, IDisposable
    {
        // Объекты, чье состояние необходимо фиксировать при сохранении прогресса.
        [SerializeField]
        private List<ISavable> _savable;

        private string _profileName; // Ддля разделения профилей игроков.
        private SavesManager _savesManager;
        private Dictionary<int, IQuest> _quests;

        // ISavable
        public string Key => "GeneralProgress";
        public object SavingData => this;

        /// <summary>
        /// При True сохраняет прогресс, когда какой-либо из отслеживаемых квестов меняет статус.
        /// </summary>
        public bool AutoSave { get; set; } = true;


        /// <param name="profileName">Имя профился игрока</param>
        /// <param name="questLoader">Используемый загрузчик квестов</param>
        /// <param name="savesManager">Менеджер сохранения</param>
        public ProgressManager(string profileName, IQuestLoader questLoader, SavesManager savesManager)
        {
            _profileName = profileName;

            // Как минимум фиксируем общие данные из Progress Manager.
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
        /// Добавляет квесты в сохранения.
        /// </summary>
        private void AddQuestsToSavable()
        {
            foreach (var quest in _quests.Values)
            {
                if (quest is ISavable savable)
                {
                    AddSavable(savable);
                }
            }
        }

        /// <summary>
        /// Подписка на квест.
        /// </summary>
        /// <param name="quiestId">ID квеста</param>
        /// <returns></returns>
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
        /// Отписка от квеста.
        /// </summary>
        /// <param name="quiestId">ID квеста</param>
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
        /// Добавляет сохраняемый объект (пр. инвентарь, персонажей и т п).
        /// </summary>
        /// <param name="savable"></param>
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
        /// Удаляет объект из сохраняемых.
        /// </summary>
        /// <param name="savable"></param>
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

        /// <summary>
        /// Ручное продвижение квеста.
        /// </summary>
        /// <param name="questId">ID квеста</param>
        /// <param name="actionId">ID активности в квесте</param>
        /// <param name="newState">Новое состояние активности</param>
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

        /// <param name="questId">ID квеста</param>
        /// <param name="actionId">ID активности в квесте</param>
        /// <returns>Состояние активнсоти квеста.</returns>
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

        /// <param name="questId">ID квеста</param>
        /// <returns>Все активности квеста.</returns>
        public IReadOnlyDictionary<int, QuestActionState> GetAllActions(int questId)
        {
            if (!_quests.TryGetValue(questId, out var quest))
            {
                Debug.LogError($"Progress manager has no quest [{questId}], but you try to get Action state from it.");
                return new Dictionary<int, QuestActionState>();
            }

            return quest.GetAllActions();
        }

        /// <summary>
        /// Возвращает обработчика активности квеста.
        /// </summary>
        /// <param name="questId">ID квеста</param>
        /// <param name="actionId">ID активности</param>
        /// <param name="actionHandler">обработчик</param>
        /// <returns>True если обработчик успешно получен.</returns>
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