using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Systems
{
    /// <summary>
    /// Загрузчик json квестов.
    /// Квесты хранятся в директории в Resources (все квесты при сборке упаковываются в билд и не видны игроку).
    /// </summary>
    public class QuestJsonLoader : IQuestLoader
    {
        private string _questsFolder; // Название директории в папке Resources.

        /// <param name="questsFolder">Название директории внутри Resources со всеми квестами</param>
        public QuestJsonLoader(string questsFolder)
        {
            _questsFolder = questsFolder;
        }

        // см. IQuestLoader
        public bool GetQuest(string name, out IQuest quest)
        {
            TextAsset jsonQuestAsset = Resources.Load<TextAsset>($"{_questsFolder}/{name}.json");

            if (jsonQuestAsset == null)
            {
                Debug.LogError($"No Quest file \"{name}.json\" found in \"Resources/{_questsFolder}\"");
                quest = null;
                return false;
            }

            Debug.Log($"Load quest: {jsonQuestAsset.name}");
            quest = new QuestFromJson(jsonQuestAsset.text);
            return true;
        }

        // см. IQuestLoader
        public List<IQuest> GetAllQuests()
        {
            TextAsset[] jsonQuestAssets = Resources.LoadAll<TextAsset>(_questsFolder);
            List<IQuest> quests = new();

            if (jsonQuestAssets.Length == 0)
            {
                Debug.LogWarning($"No Quest files found in Resources/{_questsFolder}.");
                return quests;
            }

            foreach (TextAsset jsonQuestAsset in jsonQuestAssets)
            {
                Debug.Log($"Load quest: {jsonQuestAsset.name}");
                quests.Add(new QuestFromJson(jsonQuestAsset.text));
            }

            return quests;
        }
    }
}