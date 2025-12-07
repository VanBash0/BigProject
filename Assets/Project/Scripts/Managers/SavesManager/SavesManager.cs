using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject
{
    /// <summary>
    /// Сохраняет и загружает данные коллекции ISavable.
    /// </summary>
    public class SavesManager
    {
        /// <summary>
        /// Сохраняет данные из коллекции ISavable
        /// </summary>
        /// <param name="saveName">Имя сохранения</param>
        /// <param name="data">Сохраняемые данные</param>
        /// <returns>True если данные успешно сохранены.</returns>
        public bool SaveGame(string saveName, IEnumerable<ISavable> data)
        {
            List<string> jsonRecs = new();

            foreach (var savable in data)
            {
                string jsonData = JsonUtility.ToJson(savable.SavingData);

                if (String.IsNullOrEmpty(jsonData))
                {
                    Debug.LogWarning($"Data with key {savable.Key} in save {saveName} is empty. It will be ignored.");
                    continue;
                }

                // Собираем строку с id и данными объекта и добавляем в список.
                jsonData = $"[{savable.Key}]{jsonData}";
                jsonRecs.Add(jsonData);
            }

            if (jsonRecs.Count == 0)
            {
                Debug.LogWarning("Try to save empty data.");
                return false;
            }

            // Собираем все записи в одну и записываем под одним ключем.
            string summaryData = String.Join('\n', jsonRecs);
            PlayerPrefs.SetString(saveName, summaryData);
            PlayerPrefs.Save();
            Debug.Log("Game progress saved.");
            return true;
        }

        /// <summary>
        /// Загружает данные в коллекцию ISavable
        /// </summary>
        /// <param name="saveName">Имя сохранения</param>
        /// <param name="data">Загружаемые данные</param>
        /// <returns>True если данные успешно загружены.</returns>
        public bool LoadGame(string saveName, IEnumerable<ISavable> data)
        {
            string summaryData = PlayerPrefs.GetString(saveName);

            if (String.IsNullOrEmpty(summaryData))
            {
                Debug.LogWarning($"Try to load non-existent save {saveName}.");
                return false;
            }

            if (GetJsonRecords(out var jsonRecs, summaryData, saveName))
            {
                foreach (var savable in data)
                {
                    if (!jsonRecs.ContainsKey(savable.Key))
                    {
                        Debug.LogWarning($"Try to load non-existent data with key {savable.Key} in save {saveName}. It will be ignored.");
                        continue;
                    }

                    // Перезаписываем поля объекта данными из соответствующей строки.
                    JsonUtility.FromJsonOverwrite(jsonRecs[savable.Key], savable.SavingData);
                    savable.OnLoad();
                }

                Debug.Log("Game progress loaded.");
                return true;
            }
            

            Debug.LogError($"Unable to load save {saveName}.");
            return false;
        }

        /// <summary>
        /// Создает словарь json записей с данными.
        /// </summary>
        /// <param name="jsonRecs">Словарь json записей с данными</param>
        /// <param name="summaryData">Исходные данные</param>
        /// <param name="saveName">Имя сохранения</param>
        /// <returns>True если словарь успешно создан.</returns>
        private bool GetJsonRecords(out Dictionary<string, string> jsonRecs, string summaryData, string saveName)
        {
            jsonRecs = new();

            // Проходим по всем строкам с данными.
            foreach (string jsonRec in summaryData.Split('\n'))
            {
                // Находим id (key) записи.
                int keyStart = jsonRec.IndexOf('[') + 1;
                int keyEnd = jsonRec.IndexOf(']');

                if (keyStart == -1 || keyEnd == -1 || keyEnd < keyStart)
                {
                    Debug.LogWarning($"Incorrect record format in save {saveName}: {jsonRec}.\nIt will be ignored.");
                    continue;
                }

                string key = jsonRec[keyStart..keyEnd];

                // Проверка на дубликаты.
                if (jsonRecs.ContainsKey(key))
                {
                    Debug.LogWarning($"Duplicate key {key} in save {saveName}. It will be ignored.");
                    continue;
                }

                // Json подстрока.
                string jsonData = jsonRec[(keyEnd + 1)..];

                if (String.IsNullOrEmpty(jsonData))
                {
                    Debug.LogWarning($"Empty entry with key {key} in save {saveName}. It will be ignored.");
                    continue;
                }

                jsonRecs.Add(key, jsonData);
            }

            return jsonRecs.Count > 0;
        }

        /// <summary>
        /// Удаляет сохранение по имени.
        /// </summary>
        public void DeleteSave(string saveName)
        {
           PlayerPrefs.DeleteKey(saveName);
           Debug.Log($"Save {saveName} deleted.");
        }
    }
}