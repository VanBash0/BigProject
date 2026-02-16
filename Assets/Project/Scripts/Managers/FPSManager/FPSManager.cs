using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigProject.Managers.FPSManager
{
    public class FPSManager : MonoBehaviour, ISavable
    {
        [SerializeField] private Slider _fpsSlider;
        [SerializeField] private TMP_Text _fpsText;

        private FPSData _fpsData = new FPSData();

        private const int MIN_FPS = 30;
        private const int MAX_FPS = 500;
        private const string UNRECTRICTED_FPS_TEXT = "Без ограничения FPS";
        private const string RECTRICTED_FPS_TEXT = "Ограничение FPS: ";

        public string Key => "FPS_Settings";
        public object SavingData => _fpsData;

        [Serializable]
        private class FPSData
        {
            public int TargetFPS;
        }

        private void Awake()
        {
            if (_fpsSlider == null || _fpsText == null)
            {
                Debug.LogError("Fps slider or text is not assigned");
                return;
            }

            _fpsSlider.minValue = MIN_FPS;
            _fpsSlider.maxValue = MAX_FPS;
            LoadFPSSettings();
            _fpsSlider.onValueChanged.AddListener(OnFPSChanged);
        }

        public void OnLoad()
        {
            ApplyFPSSettings();
        }

        private void LoadFPSSettings()
        {
            SavesManager savesManager = ServiceLocator.GetService<SavesManager>();
            List<ISavable> dataList = new List<ISavable> { this };

            if (savesManager.LoadGame("FPS_Settings", dataList))
            {
                return;
            }

            int monitorRefreshRate = Mathf.Clamp(GetMonitorRefreshRate(), MIN_FPS, MAX_FPS);
            _fpsData.TargetFPS = monitorRefreshRate;
            ApplyFPSSettings();
        }

        private void ApplyFPSSettings()
        {
            if (_fpsData.TargetFPS == MAX_FPS)
            {
                Application.targetFrameRate = -1;
                _fpsSlider.value = MAX_FPS;
            }
            else
            {
                Application.targetFrameRate = _fpsData.TargetFPS;
                _fpsSlider.value = _fpsData.TargetFPS;
            }

            UpdateFPSText();
        }

        private void OnFPSChanged(float value)
        {
            int intValue = (int)value;

            if (intValue >= MAX_FPS)
            {
                _fpsData.TargetFPS = MAX_FPS;
                Application.targetFrameRate = -1;
            }
            else
            {
                _fpsData.TargetFPS = intValue;
                Application.targetFrameRate = intValue;
            }

            UpdateFPSText();
            SaveFPSSettings();
        }

        private void SaveFPSSettings()
        {
            SavesManager savesManager = ServiceLocator.GetService<SavesManager>();
            List<ISavable> dataList = new List<ISavable> { this };
            savesManager.SaveGame("FPS_Settings", dataList);
        }

        private void UpdateFPSText()
        {
            if (Application.targetFrameRate < 0)
            {
                _fpsText.text = UNRECTRICTED_FPS_TEXT;
            }
            else
            {
                _fpsText.text = RECTRICTED_FPS_TEXT + Application.targetFrameRate;
            }
        }

        private int GetMonitorRefreshRate()
        {
            double refreshRate = Screen.currentResolution.refreshRateRatio.value;
            return Mathf.RoundToInt((float)refreshRate);
        }
    }
}