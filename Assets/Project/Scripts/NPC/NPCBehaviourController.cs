using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace BigProject.NPC
{
    public class NPCBehaviourController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _character;
        [SerializeField] private Animator _animatorController;
        [SerializeField] private NPCTargetInfo[] _targetInfos;
        [SerializeField] private bool _isRandomBehaviour;
        [Tooltip("Amount of targets the character will visit. If equals to -1, they will move infinitely.")]
        [SerializeField] private int _targetsAmount = -1;

        [Serializable]
        private struct NPCTargetInfo
        {
            public Transform TargetPosition;
            [Tooltip("Min time the character will stay at their target before moving to next one.")]
            public float MinDelayTime;
            [Tooltip("Max time the character will stay at their target before moving to next one.")]
            public float MaxDelayTime;
            public string TargetAnimationTrigger;
            public string MovingAnimationTrigger;
            public float MoveTime;
        }

        private int _currentTargetIndex;
        private int _visitedTargetsCount;
        private string _lastTriggerName;

        private void Awake()
        {
            if (_character == null || _animatorController == null)
            {
                Debug.LogError("Animator or NavMesh components are not set");
            }

            StartCoroutine(NPCBehaviourRoutine());
        }

        private void SetAnimationTrigger(string triggerName)
        {
            if (string.IsNullOrEmpty(triggerName) || _lastTriggerName == triggerName)
            {
                return;
            }

            _lastTriggerName = triggerName;
            _animatorController.SetTrigger(triggerName);
        }

        //Sends character to a target point with animation and sets an animation on arrival
        private IEnumerator NPCBehaviourRoutine()
        {
            int nextTargetIndex = _isRandomBehaviour ? Random.Range(0, _targetInfos.Length) : 0;

            while (_targetsAmount == -1 || _visitedTargetsCount < _targetsAmount)
            {
                int currentIterationIndex = nextTargetIndex;
                NPCTargetInfo currentTarget = _targetInfos[currentIterationIndex];
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.TargetPosition.position);
                bool alreadyAtTarget = distanceToTarget <= _character.stoppingDistance;

                if (alreadyAtTarget)
                {
                    SetAnimationTrigger(currentTarget.TargetAnimationTrigger);
                    _visitedTargetsCount++;

                    if (currentTarget.MaxDelayTime > 0)
                    {
                        float delayTime = Random.Range(currentTarget.MinDelayTime, currentTarget.MaxDelayTime);
                        yield return new WaitForSeconds(delayTime);
                    }
                }
                else
                {
                    if (currentTarget.MoveTime > 0 && currentTarget.TargetPosition != null)
                    {
                        _character.SetDestination(currentTarget.TargetPosition.position);
                        float pathDistance = _character.remainingDistance;

                        if (pathDistance > 0)
                        {
                            _character.speed = pathDistance / currentTarget.MoveTime;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("MoveTime is less or equals to 0");
                        _character.SetDestination(currentTarget.TargetPosition.position);
                    }

                    SetAnimationTrigger(currentTarget.MovingAnimationTrigger);
                    yield return new WaitUntil(() =>
                        !_character.pathPending && _character.remainingDistance <= _character.stoppingDistance);
                    _character.ResetPath();
                    SetAnimationTrigger(currentTarget.TargetAnimationTrigger);
                    _visitedTargetsCount++;

                    if (currentTarget.MaxDelayTime > 0)
                    {
                        float delayTime = Random.Range(currentTarget.MinDelayTime, currentTarget.MaxDelayTime);
                        yield return new WaitForSeconds(delayTime);
                    }
                }

                _currentTargetIndex = currentIterationIndex;
                nextTargetIndex = GetNextTargetIndex();
            }
        }

        private int GetNextTargetIndex()
        {
            if (!_isRandomBehaviour)
            {
                return (_currentTargetIndex + 1) % _targetInfos.Length;
            }
            else
            {
                if (_targetInfos.Length == 1)
                {
                    return 0;
                }

                //Gives random index excluding _currentTargetIndex
                int nextIndex = Random.Range(0, _targetInfos.Length - 1);
                return nextIndex < _currentTargetIndex ? nextIndex : nextIndex + 1;
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}