using System;
using System.Collections.Generic;
using UnityEngine;

namespace BigProject.Managers
{
    /// <summary>
    /// Saves and loads ISavable collection data.
    /// </summary>
    public class SavesManager
    {
        /// <summary>
        /// Stores data from an ISavable collection.
        /// </summary>
        /// <returns>True when success.</returns>
        public bool SaveGame(string saveName, IEnumerable<ISavable> data)
        {
            List<string> jsonRecs = new();

            foreach (ISavable savable in data)
            {
                string jsonData = JsonUtility.ToJson(savable.SavingData);

                if (String.IsNullOrEmpty(jsonData))
                {
                    Debug.LogWarning($"Data with key {savable.Key} in save {saveName} is empty. It will be ignored.");
                    continue;
                }

                // Collect a string with the object's id and data and add it to the list.
                jsonData = $"[{savable.Key}]{jsonData}";
                jsonRecs.Add(jsonData);
            }

            if (jsonRecs.Count == 0)
            {
                Debug.LogWarning("Try to save empty data.");
                return false;
            }

            // Collect all records to one and save with one key.
            string summaryData = String.Join('\n', jsonRecs);
            PlayerPrefs.SetString(saveName, summaryData);
            PlayerPrefs.Save();
            Debug.Log("Game progress saved.");
            return true;
        }

        /// <summary>
        /// Load data to ISavable collection.
        /// </summary>
        /// <returns>True when success.</returns>
        public bool LoadGame(string saveName, IEnumerable<ISavable> data)
        {
            string summaryData = PlayerPrefs.GetString(saveName);

            if (String.IsNullOrEmpty(summaryData))
            {
                Debug.LogWarning($"Try to load non-existent save {saveName}.");
                return false;
            }

            if (GetJsonRecords(out Dictionary<string, string> jsonRecs, summaryData, saveName))
            {
                foreach (ISavable savable in data)
                {
                    if (!jsonRecs.ContainsKey(savable.Key))
                    {
                        Debug.LogWarning($"Try to load non-existent data with key {savable.Key} in save {saveName}. It will be ignored.");
                        continue;
                    }

                    // Write object's fields with data from the corresponding row.
                    JsonUtility.FromJsonOverwrite(jsonRecs[savable.Key], savable.SavingData);
                    savable.OnLoad();
                }

                Debug.Log("Game progress loaded.");
                return true;
            }
            

            Debug.LogError($"Unable to load save {saveName}.");
            return false;
        }

        public bool HasSave(string saveName)
        {
            string summaryData = PlayerPrefs.GetString(saveName);

            if (String.IsNullOrEmpty(summaryData))
            {
                Debug.LogWarning($"Checking save data for non-existent save {saveName}.");
                return false;
            }

            Debug.Log($"Found saves for {saveName}.");
            return true;
        }

        /// <summary>
        /// Creates a json dictionary of data entries.
        /// </summary>
        /// <param name="jsonRecs">Records dictionary</param>
        /// <param name="summaryData">Initial data</param>
        /// <returns>True when success.</returns>
        private bool GetJsonRecords(out Dictionary<string, string> jsonRecs, string summaryData, string saveName)
        {
            jsonRecs = new();

            // Go through all the rows with data.
            foreach (string jsonRec in summaryData.Split('\n'))
            {
                // Find the id (key) of the record.
                int keyStart = jsonRec.IndexOf('[') + 1;
                int keyEnd = jsonRec.IndexOf(']');

                if (keyStart == -1 || keyEnd == -1 || keyEnd < keyStart)
                {
                    Debug.LogWarning($"Incorrect record format in save {saveName}: {jsonRec}.\nIt will be ignored.");
                    continue;
                }

                string key = jsonRec[keyStart..keyEnd];

                // Check for duplicate.
                if (jsonRecs.ContainsKey(key))
                {
                    Debug.LogWarning($"Duplicate key {key} in save {saveName}. It will be ignored.");
                    continue;
                }

                // Json substring.
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

        public void DeleteSave(string saveName)
        {
           PlayerPrefs.DeleteKey(saveName);
           Debug.Log($"Save {saveName} deleted.");
        }
    }
}