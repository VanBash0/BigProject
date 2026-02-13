using BigProject.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gamelogs_test : MonoBehaviour
{
    void Start()
    {
        GameLogManager.Info("Info");
        GameLogManager.Warning("Warning");
        GameLogManager.Error("Error");
        GameLogManager.Critical("Critical");
        GameLogManager.Debug("Debug");
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