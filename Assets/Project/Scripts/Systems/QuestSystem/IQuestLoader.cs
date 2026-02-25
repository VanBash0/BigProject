using System.Collections.Generic;

namespace BigProject.Systems.QuestSystem
{
    public interface IQuestLoader
    {
        /// <summary>
        /// Get quest by name.
        /// </summary>
        /// <returns>True when success.</returns>
        public bool GetQuest(string name, out IQuest quest);

        /// <exception cref="ArgumentException">Thrown when loaded data has incorrect values.</exception>
        public List<IQuest> GetAllQuests();
    }
}