using BigProject.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gamelogs_test : MonoBehaviour
{
    void Start()
    {
        GameLogManager.Instance.Info("Info");
        GameLogManager.Instance.Warning("Warning");
        GameLogManager.Instance.Error("Error");
        GameLogManager.Instance.Critical("Critical");
        GameLogManager.Instance.Debug("Debug");
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