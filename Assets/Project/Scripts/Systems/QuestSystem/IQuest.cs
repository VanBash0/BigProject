using System;
using System.Collections.Generic;

namespace BigProject.Systems.QuestSystem
{
    /// <summary>
    /// Состояние квеста (вероятно будет 2 в итоге - активен/пройден)
    /// </summary>
    public enum QuestState
    {
        Inactive,
        Active,
        Completed,
        Failed
    }

    /// <summary>
    /// Состояние любых активностей в квесте по типу: взять шестерню, поговорить с нпс и т. п.
    /// Расположены в порядке жизненного цикла активности, что учитывается при разрешении конфликтов состояний
    /// (когда по логике квеста объект может быть как Active, так и Completed например).
    /// </summary>
    public enum QuestActionState
    {
        Undefined, // для указания любого состояния в условиях квеста
        Inactive,
        Active,
        Completed,
        Failed,
        Released
    }

    /// <summary>
    /// Тип активности.
    /// </summary>
    public enum QuestActionType
    {
        /// <summary>
        /// "Несгораемый" результат. При достижении состояния Completed/Failed возможен только переход в Released.
        /// </summary>
        FireproofResult,

        /// <summary>
        /// Возможен переход из Completed/Failed обратно в Active/Inactive.
        /// </summary>
        MaxMet
    }

    /// <summary>
    /// Квест представляет собой набор активностей (id активности + состяние) и связывающих их условий.
    /// Внешний код соверщает допустимые переходы (не науршающие логику квеста), что автоматически меняет состояния связанных условиями активностей.
    /// </summary>
    public interface IQuest
    {
        int ID { get; }
        string Name { get; }
        QuestState CurrentState { get; }

        /// <summary>
        /// Для отслеживания прогресса люой активности в квесте.
        /// </summary>
        event Action<IQuest> Progressed;

        /// <summary>
        /// Для отслеживания смены состояния всего квеста (завершен, провален и т. п.)
        /// </summary>
        event Action<IQuest> StateChanged;
 
        /// <summary>
        /// Совершает ручной переход активности в новое состояние.
        /// </summary>
        /// <param name="actionId">ID активности</param>
        /// <param name="newState">Новое состояние активности</param>
        /// <param name="forced">Если true, то переход будет совершен, игнорируя логику квеста. Не рекомендуется.</param>
        /// <returns>True если переход был успешно совершен.</returns>
        bool ManualTransition(int actionId, QuestActionState newState, bool forced = false);

        /// <summary>
        /// Возвращает состояние активности.
        /// </summary>
        /// <param name="id">ID активности</param>
        /// <param name="state">Получаемое значение состояния</param>
        /// <returns>True если состояние успешно найдено.</returns>
        bool TryGetActionState(int id, out QuestActionState state);

        /// <summary>
        /// Возвращает последние изменившиеся активности. 
        /// Можно использовать после вызова MakeTransition для отслеживания прогресса квеста.
        /// </summary>
        /// <returns>Словарь с последними изменившимися активностями.</returns>
        IReadOnlyDictionary<int, QuestActionState> GetLastChangedActions();

        /// <summary>
        /// Возвращает все активности. 
        /// <returns>Словарь со всеми активностями.</returns>
        IReadOnlyDictionary<int, QuestActionState> GetAllActions();

        /// <summary>
        /// Возвращает обработчика активности.
        /// </summary>
        /// <param name="actionId">ID активности</param>
        /// <param name="actionHandler">Ообработчик активности</param>
        /// <returns>True обработчик успешно создан.</returns>
        bool TryGetActionHandler(int actionId, out IQuestActionHandler actionHandler);
    }
}