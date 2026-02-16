using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSManager : MonoBehaviour
{
    [SerializeField] private Slider _fpsSlider;
    [SerializeField] private TMP_Text _fpsText;

    private const int MIN_FPS = 30;
    private const int MAX_FPS = 500;
    private const string UNRECTRICTED_FPS_TEXT = "Без ограничения FPS";
    private const string RECTRICTED_FPS_TEXT = "Ограничение FPS: ";

    private void Awake()
    {
        if (_fpsSlider == null || _fpsText == null)
        {
            Debug.LogError("Fps slider or text is not assigned");
            return;
        }

        int monitorRefreshRate = Mathf.Clamp(GetMonitorRefreshRate(), MIN_FPS, MAX_FPS);
        Application.targetFrameRate = monitorRefreshRate;
        _fpsSlider.value = monitorRefreshRate;
        _fpsSlider.onValueChanged.AddListener(OnFPSChanged);
        UpdateFPSText();
    }

    private void OnFPSChanged(float value)
    {
        int intValue = (int)value;

        if (intValue >= MAX_FPS)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = intValue;
        }

        UpdateFPSText();
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