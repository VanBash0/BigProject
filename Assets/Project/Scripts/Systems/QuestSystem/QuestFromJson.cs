using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using BigProject.Managers;

namespace BigProject.Systems.QuestSystem
{
    /// <summary>
    /// Квест, забирающий данные с Json строки.
    /// </summary>
    internal class QuestFromJson : IQuest, ISavable
    {
        // Имена сериализируемых переменных даны с учетом того, что они в таком же виде отображаются и в json файле, поэтому нет нижних подчеркиваний и т. п.

        [SerializeField]
        private int id;
        [SerializeField]
        private string name;
        [SerializeField]
        private QuestState currentState;
        [SerializeField]
        List<Action> actions;
        [SerializeField]
        List<QuestCondition> questStates;

        private Dictionary<int, Action> _actionsDict;
        private Dictionary<int, QuestActionState> _lastChangedActions = new();
        private Dictionary<int, QuestActionHandler> _actionHandlers;

        /// <summary>
        /// Активность хранит свое состояние и условия переходов в другие состояния.
        /// </summary>
        [Serializable]
        private class Action
        {
            public int id;
            public string name = "action";
            public QuestActionType type = QuestActionType.FireproofResult;
            public QuestActionState currentState = QuestActionState.Inactive;
            public List<ActionCondition> conditions;
            public List<ManualActionTransition> manualTransitions;
        }

        /// <summary>
        /// Условие для перехода активности в заданное состояние.
        /// Хранит зависимости от других состояний, при выполнении их условий совершается переход.
        /// </summary>
        [Serializable]
        private class ActionCondition
        {
            // После выполнения условие может быть удалено.
            // Полезно для разовых условий (пр.: после активации кнопки выполнение условий активации уже не требуется)
            public bool isOneShot;

            // Из какого состояния переходим. 
            public QuestActionState fromState;

            // В какое состояние переходит активность при выполнении условий.
            public QuestActionState toState;

            // Зависимость включает в себя id влияющей активности и ее состояние, при котором ее требования считаются выполненными.
            [Serializable]
            public class Dependency
            {
                public int id;
                public QuestActionState state;
            }

            // Класс-обертка над списком зависимостей. Встроенный JsonUtility не умеет работать с вложенными списками.
            [Serializable]
            public class DependencyPack
            {
                // Условия выполняются при выполнении условий всех зависимостей из списка (по сути оператор И).
                public List<Dependency> dependencies;
            }

            // Список наборов зависимотсей. Нужен для возможности связывать условия оператором ИЛИ.
            // Условия выполняются при выполнении любого из наборов условий.
            public List<DependencyPack> dependencyPacks;
        }

        /// <summary>
        /// Переходы, которые допускаются для ручного управления (внешним кодом).
        /// </summary>
        [Serializable]
        private class ManualActionTransition
        {
            public int id;
            public QuestActionState fromState;
            public QuestActionState toState;
            public bool isOneShot;
        }

        /// <summary>
        /// Условие для перехода квеста в заданное состояние.
        /// Состояние квеста привязано к некоторой активности,
        /// сложные условия можно задавать в самой привязанной активности.
        /// </summary>
        [Serializable]
        private class QuestCondition
        {
            public QuestState state;

            // Влияющая активность.
            public int actionId;
            public QuestActionState actionState;
        }

        public int ID => id;
        public string Name => name;
        public QuestState CurrentState
        {
            get => currentState;
            private set => currentState = value;
        }

        public event Action<IQuest> Progressed;
        public event Action<IQuest> StateChanged;

        // Поля ISavable
        public string Key => $"Quest_{Name}";
        // Сохраняем все параметры квеста.
        public object SavingData => this;


        /// <param name="jsonData">Данные квеста в формате Json</param>
        public QuestFromJson(string jsonData)
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
            _actionHandlers = new();
            Init();
        }

        // См. IQuest
        public bool ManualTransition(int actionId, QuestActionState newState, bool forced = false)
        {
            // Действия возможны только в активном незавершенном квесте.
            if (CurrentState != QuestState.Active)
            {
                Debug.LogWarning($"Quest [{Name}] in state [{CurrentState}], but you try to access it.");
                return false;
            }
            
            if (!_actionsDict.TryGetValue(actionId, out var targetAction))
            {
                Debug.LogError($"Action [{actionId}] not found in quest [{name}].");
                return false;
            }

            if (targetAction.currentState == newState)
            {
                Debug.LogWarning($"Action [{actionId}] in quest [{name}] already in state [{newState}].");
                return false;
            }

            if (newState == QuestActionState.Undefined)
            {
                Debug.LogWarning($"Action [{actionId}] in quest [{name}] can't be in undefined state. Transition will be ignored.");
                return false;
            }

            if (forced)
            {
                Debug.LogWarning($"Quest [{name}], make forced transition of Action [{actionId}], new state [{newState}].");
                MakeTransition(targetAction, new() { toState = newState });
                return true;
            }

            foreach (var transition in targetAction.manualTransitions)
            {
                // Если ручной переход допустим.
                // Учитывается возможный переход из Undefined (в этом случае любое текущее состояние активности считается подходящим).
                if (IsEqualStates(transition.fromState, targetAction.currentState) && transition.toState ==  newState)
                {
                    MakeTransition(targetAction, transition);
                    return true;
                }
            }

            Debug.LogError($"Quest [{Name}] has no manual transitions to Action [{actionId}] state [{newState}].");
            return false;
        }

        // См. IQuest
        public bool TryGetActionState(int id, out QuestActionState state)
        {
            if (_actionsDict.TryGetValue(id, out Action action))
            {
                state = currentState == QuestState.Inactive ? QuestActionState.Inactive : action.currentState;
                return true;
            }

            state = QuestActionState.Undefined;
            return false;
        }

        // См. IQuest
        public IReadOnlyDictionary<int, QuestActionState> GetLastChangedActions() => _lastChangedActions;

        // См. IQuest
        public IReadOnlyDictionary<int, QuestActionState> GetAllActions() => _actionsDict.ToDictionary(x => x.Key, x => x.Value.currentState);

        // См. IQuest
        public bool TryGetActionHandler(int actionId, out IQuestActionHandler actionHandler)
        {
            if (_actionHandlers.ContainsKey(actionId))
            {
                actionHandler = _actionHandlers[actionId];
                return true;
            }

            if (!_actionsDict.ContainsKey(actionId))
            {
                Debug.LogError($"Quest [{Name}] unable to set action [{actionId}] handler: id not found.");
                actionHandler = null;
                return false;
            }

            var targetAction = _actionsDict[actionId];
            // Получаем все ручные транзакции.
            var transitions = targetAction.manualTransitions.ToDictionary(x => x.id, x => (x.fromState, x.toState));
            actionHandler = new QuestActionHandler(this, actionId, targetAction.name, targetAction.currentState, transitions);
            _actionHandlers.Add(actionId, actionHandler as QuestActionHandler);
            return true;
        }

        // ISavable
        public void OnLoad()
        {
            Init();
            ProgressNotify();
        }

        private void Init()
        {
            ActionsToDictionary();
            InitialActionsCheck();

            // Доступные состояния квеста сортируем по убыванию на случай конфликтов
            // (если текущее положение квеста удовелтворяет сразу нескольким состояниям).
            questStates.Sort((a, b) => b.state.CompareTo(a.state));

            // Обновляем все состояния (возможно сразу есть выполняемые условия).
            ResetActions();
            ResetQuestState();

            // После загрузки все активности представляют собой поcледние изменения.
            _lastChangedActions = _actionsDict.ToDictionary(x => x.Key, x => x.Value.currentState);
            //// Проверяем наличие неопределенных состояний.
            //CheckForUndefinedActions();
        }

        /// <summary>
        /// Совершает переход актвиности согласно транзакции.
        /// </summary>
        private void MakeTransition(Action action, ManualActionTransition transition)
        {
            _lastChangedActions.Clear(); // Сброс последних изменений перед новыми.
            action.currentState = transition.toState;

            // Ручные переходы могут быть единоразовыми.
            if (transition.isOneShot)
            {
                action.manualTransitions.Remove(transition);

                if (_actionHandlers.ContainsKey(action.id))
                {
                    _actionHandlers[action.id].RemoveTransition(transition.id);
                }
            }

            CommitActionChange(action);
            ResetActions();
            ResetQuestState();
            ProgressNotify();
        }

        /// <summary>
        /// Все уведомления о прогрессе.
        /// </summary>
        private void ProgressNotify()
        {
            SendToActionHandlers();
            Progressed?.Invoke(this);
        }

        /// <summary>
        /// Рассылает уведомления об изменившемся состоянии.
        /// </summary>
        private void SendToActionHandlers()
        {
            foreach (var actionHandler in _actionHandlers)
            {
                // Уведомляем только изменившиеся активности.
                if (_lastChangedActions.TryGetValue(actionHandler.Key, out var newState))
                {
                    actionHandler.Value.OnStateChanged(newState);
                }
            }
        }


        /// <summary>
        /// Для оптимизации поиска активности переводим в словарь.
        /// </summary>
        private void ActionsToDictionary()
        {
            _actionsDict = actions.ToDictionary(x => x.id, x => x);
            //actions.Clear(); - можно стереть список, если не надо будет сохранять все активности в квесте.
        }

        /// <summary>
        /// Проверяет начальные состояния у активностей.
        /// </summary>
        private void InitialActionsCheck()
        {
            foreach (var action in _actionsDict.Values)
            {
                if (action.currentState == QuestActionState.Undefined)
                {
                    Debug.LogError($"Quest [{Name}] has action [{action.name}] in undefined state.");
                    return;
                }
                else if (action.type == QuestActionType.FireproofResult)
                {
                    TryCleanUpForRelease(action);
                }
            }
        }

        /// <summary>
        /// Перепроверяет активности и меняет их состояния при выполнении условий.
        /// </summary>
        private void ResetActions()
        {
            // Для перекрестных зависимостей - будем обходить состояния по кругу,
            // пока не убедимся, что все состояния приняли конечные значения.
            bool actionsChanged = false;

            do
            {
                foreach (var action in _actionsDict.Values)
                {
                    var startState = action.currentState;
                    ResetAction(action);
                    actionsChanged = (startState != action.currentState);

                    if (actionsChanged)
                    {
                        CommitActionChange(action);
                        break;
                    }
                }
            } while (actionsChanged);
        }

        /// <summary>
        /// Фиксирует активность в списке изменений.
        /// </summary>
        private void CommitActionChange(Action action)
        {
            if (_lastChangedActions.ContainsKey(action.id))
            {
                _lastChangedActions[action.id] = action.currentState;
            }
            else
            {
                _lastChangedActions.Add(action.id, action.currentState);
            }
        }

        /// <summary>
        /// Проверяет состояние активности и меняет при выполнении условий.
        /// </summary>
        private void ResetAction(Action action)
        {
            // Для конфликтов, когда активность удовлетворяет нескольким состояниям - берем наибольшее.
            QuestActionState maxMetState = QuestActionState.Inactive;

            // Список для выполненных условий с флагом isOneShot (см. ActionCondition)
            List<ActionCondition> conditionsToRemove = new();

            foreach (ActionCondition condition in action.conditions)
            {
                // Если условия перехода не удовлетворяют текущему состоянию или переход не актуален с точки зрения наибольшего выполнимого состояния.
                if (!IsEqualStates(condition.fromState, action.currentState) || condition.toState <= maxMetState)
                {
                    continue;
                }

                if (IsConditionMet(condition))
                {
                    // При выполнении условий переходим в новое состояние, только если текущее ниже.
                    if (action.currentState < condition.toState)
                    {
                        action.currentState = condition.toState;

                        if (action.currentState == QuestActionState.Released)
                        {
                            action.conditions.Clear();
                            action.manualTransitions.Clear();
                            return;
                        }
                        else if (TryCleanUpForRelease(action))
                        {
                            return;
                        }
                        else if (condition.isOneShot)
                        {
                            conditionsToRemove.Add(condition);
                        }
                    }

                    maxMetState = action.currentState;
                }
                // Если мы перепрыгнули недопустимое состояние - опускаемся на ближайшее выполнимое.
                // Такой сценарий работает при переходах Undefined->Конкретный, при несоблюдении будет авто откат на допустимое состояние.
                else if (action.currentState >= condition.toState)
                {
                    action.currentState = maxMetState;
                }
            }

            // Удаляем выполненные OneShot условия.
            action.conditions.RemoveAll(x => conditionsToRemove.Contains(x));
        }

        private bool TryCleanUpForRelease(Action action)
        {
            if (action.currentState < QuestActionState.Completed || action.type == QuestActionType.MaxMet)
                return false;

            ActionCondition targetCondition = null;

            foreach (ActionCondition condition in action.conditions)
            {
                if (IsEqualStates(condition.fromState, action.currentState) && condition.toState == QuestActionState.Released)
                {
                    targetCondition = condition;
                    break;
                }
            }

            action.conditions.Clear();

            if (targetCondition != null)
            {
                action.conditions.Add(targetCondition);
            }

            ManualActionTransition targetTransition = null;

            foreach (ManualActionTransition transition in action.manualTransitions)
            {
                if (IsEqualStates(transition.fromState, action.currentState) && transition.toState == QuestActionState.Released)
                {
                    targetTransition = transition;
                    break;
                }
            }

            action.manualTransitions.Clear();

            if (targetTransition != null)
            {
                action.manualTransitions.Add(targetTransition);
            }

            return true;
        }

        /// <returns>True если условие выполняется</returns>
        private bool IsConditionMet(ActionCondition condition)
        {
            // Проходим по всем наборам зависимостей - между ними фактически связка ИЛИ
            foreach (var dependencyPack in condition.dependencyPacks)
            {
                // Если выполнен хотя бы один из наборов - условия выполнены.
                if (IsDependenciesSatisfied(dependencyPack.dependencies))
                {
                    return true;
                }
            }

            // Если ни один из наборов не выполнен - условия не выполнены.
            return false;
        }

        /// <returns>True если условия всех зависимостей соблюдены.</returns>
        private bool IsDependenciesSatisfied(List<ActionCondition.Dependency> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                // Находим влияющую активнсоть.
                if (!_actionsDict.TryGetValue(dependency.id, out var influenceAction))
                {
                    Debug.LogWarning($"Quest [{name}]. Unable to find influence Action with id [{dependency.id}]. Skip condition.");
                    continue;
                }

                // Если она в неправильном состоянии - все условие не выполнено. Далее можно не проверять.
                // Учитывается Undefined состояние в условии - тогда активность может быть в любом состоянии, что равносильно ее отсутствию в условиях.
                if (!IsEqualStates(influenceAction.currentState, dependency.state))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Проверяет глобальное состояние квеста и меняет его при выполнении условий.
        /// </summary>
        private void ResetQuestState()
        {
            foreach (var questState in questStates)
            {
                if (CurrentState == questState.state)
                {
                    continue;
                }

                if (!_actionsDict.TryGetValue(questState.actionId, out var influenceAction))
                {
                    Debug.LogWarning($"Quest [{name}] unable to find Action [{questState.actionId}] while changing quest global state.");
                    continue;
                }

                if (influenceAction.currentState == questState.actionState)
                {
                    CurrentState = questState.state;
                    Debug.Log($"Quest change global state to [(questState.state)]");
                    StateChanged?.Invoke(this);

                    // Состояния осортированы по убыванию, при выполнении наибольшего дальнейшие условия можно не проверять.
                    break;
                }
            }
        }

        /// <summary>
        /// Сравнивает состояния активнсотей с учетом неопределенности.
        /// </summary>
        private bool IsEqualStates(QuestActionState state1, QuestActionState state2) =>
            state1 == state2 || state1 == QuestActionState.Undefined || state2 == QuestActionState.Undefined;

        #region FOR_TEST_PURPOSES

        /// <summary>
        /// Конструктор для тестов.
        /// </summary>
        public QuestFromJson()
        {
            //TestInit();
            //TestWrite();
            TestRead();
            Init();
        }

        /// <summary>
        /// Тестовая запись в json.
        /// </summary>
        private void TestWrite()
        {
            string questData = JsonUtility.ToJson(this);
            string filePath = Application.persistentDataPath + "/quest.json";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(questData);
            }
        }

        /// <summary>
        /// Тестовое чтение из json.
        /// </summary>
        private void TestRead()
        {
            string filePath = Application.persistentDataPath + "/quest.json";
            string questData = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(questData, this);
        }

        /// <summary>
        /// Тестовая инициализация, только для тестов записи в json/проверки работы.
        /// </summary>
        private void TestInit()
        {
            id = 10;
            name = "Tutorial";
            CurrentState = QuestState.Active;

            #region ACTIONS

            Action action1 = new()
            {
                id = 0,
                name = "Raise gear",
                currentState = QuestActionState.Active,
                manualTransitions = new()
                {
                    new()
                    {
                        id = 0,
                        fromState = QuestActionState.Active,
                        toState = QuestActionState.Completed
                    },
                    new()
                    {
                        id = 1,
                        fromState = QuestActionState.Completed,
                        toState = QuestActionState.Active
                    }
                }
            };

            Action action2 = new()
            {

                id = 1,
                name = "Install gear",
                currentState = QuestActionState.Inactive,
                conditions = new()
                {
                    new ActionCondition()
                    {
                        fromState = QuestActionState.Inactive,
                        toState = QuestActionState.Active,
                        dependencyPacks = new()
                        {
                            new() { dependencies = new () { new() { id = 0, state = QuestActionState.Completed } } }
                        }
                    }
                },
                manualTransitions = new()
                {
                    new()
                    {
                        id = 0,
                        fromState = QuestActionState.Active,
                        toState = QuestActionState.Completed
                    }
                }
            };

            Action action3 = new()
            {
                id = 2,
                name = "Down the lever",
                currentState = QuestActionState.Inactive,
                conditions = new()
                {
                    new ActionCondition()
                    {
                        fromState = QuestActionState.Inactive,
                        toState = QuestActionState.Active,
                        dependencyPacks = new()
                        {
                            new() { dependencies = new () { new () { id = 1, state = QuestActionState.Completed } } }
                        }
                    }
                },
                manualTransitions = new()
                {
                    new()
                    {
                        id = 0,
                        fromState = QuestActionState.Active,
                        toState = QuestActionState.Completed,
                    },
                    new()
                    {
                        id = 1,
                        fromState = QuestActionState.Completed,
                        toState = QuestActionState.Active,
                    }
                }
            };

            Action action4 = new()
            {
                id = 3,
                name = "Push button",
                currentState = QuestActionState.Inactive,
                conditions = new()
                {
                    new ActionCondition()
                    {
                        fromState = QuestActionState.Inactive,
                        toState = QuestActionState.Active,
                        dependencyPacks = new()
                        {
                            new() { dependencies = new () { new () { id = 1, state = QuestActionState.Completed } } }
                        }
                    }
                },
                manualTransitions = new()
                {
                    new()
                    {
                        id = 0,
                        fromState = QuestActionState.Active,
                        toState = QuestActionState.Completed,
                    }
                }
            };

            Action action5 = new()
            {
                id = 4,
                name = "Mill rotation",
                currentState = QuestActionState.Inactive,
                conditions = new()
                {
                    new ActionCondition()
                    {
                        fromState = QuestActionState.Inactive,
                        toState = QuestActionState.Active,
                        dependencyPacks = new()
                        {
                             new() { dependencies = new () { new () { id = 1, state = QuestActionState.Completed } } }
                        }
                    },

                    new ActionCondition()
                    {
                        fromState = QuestActionState.Active,
                        toState = QuestActionState.Completed,
                        dependencyPacks = new()
                        {
                            new()
                            {
                                dependencies = new()
                                {
                                    new () { id = 2, state = QuestActionState.Completed },
                                    new () { id = 3, state = QuestActionState.Completed }
                                }
                            }
                        }
                    }
                }
            };

            #endregion

            actions = new() { action1, action2, action3, action4, action5 };

            #region QUEST_STATES

            QuestCondition condition1 = new()
            {
                state = QuestState.Completed,
                actionId = 4,
                actionState = QuestActionState.Completed
            };

            QuestCondition condition2 = new()
            {
                state = QuestState.Failed,
                actionId = 4,
                actionState = QuestActionState.Failed
            };

            #endregion

            questStates = new() { condition1, condition2 };
        }

        #endregion
    }
}