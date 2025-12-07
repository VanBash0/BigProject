using System;
using System.Collections.Generic;

namespace BigProject
{
    /// <summary>
    /// —осто€ние квеста (веро€тно будет 2 в итоге - активен/пройден)
    /// </summary>
    public enum QuestState
    {
        Inactive,
        Active,
        Completed,
        Failed
    }

    /// <summary>
    /// —осто€ние любых активностей в квесте по типу: вз€ть шестерню, поговорить с нпс и т. п.
    /// –асположены в пор€дке жизненного цикла активности, что учитываетс€ при разрешении конфликтов состо€ний
    /// (когда по логике квеста объект может быть как Active, так и Completed например).
    /// </summary>
    public enum QuestActionState
    {
        Undefined, // дл€ указани€ любого состо€ни€ в услови€х квеста
        Inactive,
        Active,
        Completed,
        Failed
    }

    /// <summary>
    ///  вест представл€ет собой набор активностей (id активности + сост€ние) и св€зывающих их условий.
    /// ¬нешний код соверщает допустимые переходы (не науршающие логику квеста), что автоматически мен€ет состо€ни€ св€занных услови€ми активностей.
    /// </summary>
    public interface IQuest
    {
        int ID { get; }
        string Name { get; }
        QuestState CurrentState { get; }

        /// <summary>
        /// ƒл€ отслеживани€ прогресса люой активности в квесте.
        /// </summary>
        event Action<IQuest> Progressed;

        /// <summary>
        /// ƒл€ отслеживани€ смены состо€ни€ всего квеста (завершен, провален и т. п.)
        /// </summary>
        event Action<IQuest> StateChanged;
 
        /// <summary>
        /// —овершает ручной переход активности в новое состо€ние.
        /// </summary>
        /// <param name="actionId">ID активности</param>
        /// <param name="newState">Ќовое состо€ние активности</param>
        /// <param name="forced">≈сли true, то переход будет совершен, игнориру€ логику квеста. Ќе рекомендуетс€.</param>
        /// <returns>True если переход был успешно совершен.</returns>
        bool ManualTransition(int actionId, QuestActionState newState, bool forced = false);

        /// <summary>
        /// ¬озвращает состо€ние активности.
        /// </summary>
        /// <param name="id">ID активности</param>
        /// <param name="state">ѕолучаемое значение состо€ни€</param>
        /// <returns>True если состо€ние успешно найдено.</returns>
        bool TryGetActionState(int id, out QuestActionState state);

        /// <summary>
        /// ¬озвращает последние изменившиес€ активности. 
        /// ћожно использовать после вызова MakeTransition дл€ отслеживани€ прогресса квеста.
        /// </summary>
        /// <returns>—ловарь с последними изменившимис€ активност€ми.</returns>
        IReadOnlyDictionary<int, QuestActionState> GetLastChangedActions();

        /// <summary>
        /// ¬озвращает все активности. 
        /// <returns>—ловарь со всеми активност€ми.</returns>
        IReadOnlyDictionary<int, QuestActionState> GetAllActions();

        /// <summary>
        /// ¬озвращает обработчика активности.
        /// </summary>
        /// <param name="actionId">ID активности</param>
        /// <param name="actionHandler">ќобработчик активности</param>
        /// <returns>True обработчик успешно создан.</returns>
        bool TryGetActionHandler(int actionId, out IQuestActionHandler actionHandler);
    }
}