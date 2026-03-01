using BigProject.Managers;
using BigProject.NPC;
using BigProject.Player;
using System.Collections;
using UnityEngine;

namespace Managers.Gameplay
{
    public class AmbassadorDialogueManager : MonoBehaviour
    {
        [SerializeField] private string _playerTag = "Player";
        [SerializeField] private DialogNPC _ambassador;
        [SerializeField] private Transform _villageCentre;
        [SerializeField] private DialogNPC _nextDialogueNPC;

        private PlayerController _player;
        private DialogueManager _dialogueManager;

        public void Init(PlayerController player, DialogueManager dialogueManager)
        {
            _player = player;
            _dialogueManager = dialogueManager;
        }
        
        /// <summary>
        /// Forces player to get to ambassador, then teleports to a specific location and forces to get to the next dialogue
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_playerTag))
            {
                return;
            }

            if (ServiceLocator.TryGetService(out GameplayManager gameplayManager))
            {
                gameplayManager.ChangeState(GameplayState.Dialogue);
            }
            else
            {
                Debug.LogError("Can't get GameplayManager");
                return;
            }

            if (_player == null)
            {
                Debug.LogError("Player is null");
                return;
            }

            StartCoroutine(AmbassadorDialogueRoutine());
        }

        private IEnumerator AmbassadorDialogueRoutine()
        {
            _player.SetInterableObject(_ambassador);
            _player.SetDestination(_ambassador.transform.position);
            _player.Move();

            while (_dialogueManager.IsDialogue || _player.IsMoving)
            {
                yield return null;
            }

            if (ServiceLocator.TryGetService(out GameplayManager gameplayManager))
            {
                gameplayManager.ChangeState(GameplayState.Dialogue);
            }
            else
            {
                Debug.LogError("Can't get GameplayManager");
                yield break;
            }

            _player.transform.position = _villageCentre.position;
            _player.SetInterableObject(_nextDialogueNPC);
            _player.SetDestination(_nextDialogueNPC.transform.position);
            _player.Move();
            Destroy(gameObject);
        }
    }
}