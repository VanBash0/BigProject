using BigProject.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gamelogs_test : MonoBehaviour
{
    void Start()
    {
        GameLogger.Instance.Info("Info");
        GameLogger.Instance.Warning("Warning");
        GameLogger.Instance.Error("Error");
        GameLogger.Instance.Critical("Critical");
        GameLogger.Instance.Debug("Debug");
    }

    void Update()
    {
        //GameLogger.Instance.Info("Info in Update");

        DoException();
    }

    private void DoException()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            int a = 0;
            int b = 1 / a;
        }
    }
}