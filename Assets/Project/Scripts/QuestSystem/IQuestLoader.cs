using System.Collections.Generic;

namespace BigProject
{
    /// <summary>
    /// Загрузчик квестов.
    /// </summary>
    public interface IQuestLoader
    {
        /// <summary>
        /// Возвращает квест по имени.
        /// </summary>
        /// <param name="name">Имя квеста</param>
        /// <param name="quest">Возвращаемый квест.</param>
        /// <returns>True если квест найден.</returns>
        bool GetQuest(string name, out IQuest quest);

        /// <returns>Все найденные квесты.</returns>
        List<IQuest> GetAllQuests();
    }
}