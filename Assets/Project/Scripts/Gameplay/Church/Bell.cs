using UnityEngine;

namespace BigProject.Gameplay.Church
{
    public class Bell : MonoBehaviour
    {
        public int Id;

        public void Ring()
        {
            // todo - add music, animation and another
            Debug.Log("Колокльчик #" + Id + " звенит!!!");
        }
    }
}

