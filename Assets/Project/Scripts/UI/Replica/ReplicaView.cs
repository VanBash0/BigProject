using UnityEngine;
using TMPro;

namespace BigProject.UI.Replica
{
    public class ReplicaView : MonoBehaviour
    {
        [SerializeField]
        private GameObject _replicaWindow;
        [SerializeField]
        private TextMeshProUGUI _replicaText;

        public void ShowReplicaWindow()
        {
            _replicaWindow.SetActive(true);
        }

        public void HideReplicaWindow()
        {
            _replicaWindow.SetActive(false);
        }

        public void SetReplicaText(string text)
        {
            _replicaText.text = text;
        }
    }
}

