using BigProject.Managers;
using BigProject.Systems.Inventory;
using BigProject.Systems.QuestSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BigProject.Utilities
{
    public class TestSceneInput : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    Application.Quit();
                else if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    ServiceLocator.ReleaseService<ProgressManager>();
                    ServiceLocator.AddService(new ProgressManager("Player", new QuestJsonLoader("Data\\Quests"), new()));
                    var inv = ServiceLocator.GetService<InventorySystem>();
                    for (int i = 0; i < 15; i++)
                        if (inv.HasItemByID(i))
                            inv.RemoveItemById(i);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }
}