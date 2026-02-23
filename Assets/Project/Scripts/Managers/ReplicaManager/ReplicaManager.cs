
using BigProject.UI.Replica;
using System.Collections;
using UnityEngine;

namespace BigProject.Managers
{
    public class ReplicaManager
    {
        private const float REPLICA_LIFE_TIME = 3f;

        private static ReplicaView _replicaView;

        private static Coroutine _currentCoroutine;

        public ReplicaManager(ReplicaView replicaView)
        {
            _replicaView = replicaView;
            _replicaView.HideReplicaWindow();
        }

        public static void ShowReplica(string text)
        {
            _replicaView.SetReplicaText(text);
            _replicaView.ShowReplicaWindow();

            if (_currentCoroutine != null)
            {
                _replicaView.StopCoroutine(_currentCoroutine);
            }

            _currentCoroutine = _replicaView.StartCoroutine(WaitAndCloseReplicaWindow());
        }
        
        public static void HideReplica()
        {
            if (_currentCoroutine != null)
            {
                _replicaView.StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
            _replicaView.HideReplicaWindow();
        }

        private static IEnumerator WaitAndCloseReplicaWindow()
        {
            yield return new WaitForSeconds(REPLICA_LIFE_TIME);
            _replicaView.HideReplicaWindow();
        }
    }
}
