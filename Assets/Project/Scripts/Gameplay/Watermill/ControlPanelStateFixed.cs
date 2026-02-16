using BigProject.Managers;
using BigProject.Player;
using BigProject.Systems;
using BigProject.Systems.QuestSystem;
using BigProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace BigProject.Gameplay.Watermill
{
    public class ControlPanelStateFixed : IControlPanelState
    {
        private ControlPanel _controlPanel;
        private PlayerInputHandler _input;
        private IQuestActionHandler _activateMechAction;
        private CancellationTokenSource _ctSource;
        private float _leverMoveTime;
        private float _leverStaggerTime;
        private float _staggerDistance;
        private int _noteItemId;
        private bool _isMoving;
        private GearsHandler _gearsHandler;
        private Lever _chosenLever;
        private List<Lever> _levers;
        private List<LeverPoint> _leversPoints;

        private class LeverPoint
        {
            public Transform transform;
            public int id;
            public LeverPoint next, prev;
            public Vector2 nextRail, prevRail;
            public bool isFree;
        }

        public ControlPanelStateFixed(ControlPanel controlPanel, PlayerInputHandler input, List<Transform> pointsTransforms,
            List<Lever> levers, float leverMoveTime, float leverStaggerTime, float staggerDistance, int noteItemId, 
            GearsHandler gearsHandler, IQuestActionHandler activateMechAction)
        {
            _controlPanel = controlPanel;
            _input = input;
            _activateMechAction = activateMechAction;
            _activateMechAction.StateChanged += OnStateChanged;
            _levers = levers;
            _leverMoveTime = leverMoveTime;
            _leverStaggerTime = leverStaggerTime;
            _staggerDistance = staggerDistance;
            _noteItemId = noteItemId;
            _isMoving = false;
            _chosenLever = null;
            _gearsHandler = gearsHandler;
            SetLeversPoints(pointsTransforms);
        }

        public bool IsReady => _activateMechAction.CurrentState == QuestActionState.Active;

        public void Start()
        {
            // реплика о починке
            // появление записки
            OnStateChanged();
        }
        public void Stop()
        {
            _chosenLever = null;
        }

        public void OnClicked()
        {
            if (GameplayUtilities.TryGetClickedObject(_input.GetMousePosition(), out GameObject go))
            {
                Lever lever = _levers.FirstOrDefault(x => x.Transform == go.transform);

                if (lever != null)
                {
                    _chosenLever = lever;
                }
            }
        }

        public void OnSwiped(Vector2 delta)
        {
            if (_isMoving || _chosenLever == null)
            {
                return;
            }

            LeverPoint currentPoint = _leversPoints[_chosenLever.PointId];

            (float, LeverPoint)[] routes =
            {
                (Vector2.Angle(delta, currentPoint.nextRail), currentPoint.next),
                (Vector2.Angle(delta, currentPoint.prevRail), currentPoint.prev)
            };

            int i = routes[0].Item1 < routes[1].Item1 ? 0 : 1;

            if (routes[i].Item1 < 30f)
            {
                _ctSource?.Dispose();
                _ctSource = new();
                _ = MoveLever(_chosenLever, currentPoint, routes[i].Item2, _ctSource.Token);
            }
        }

        public void OnUnclicked()
        {
            _chosenLever = null;
        }

        public void Dispose()
        {
            if (_gearsHandler != null)
            {
                _gearsHandler.enabled = true;
            }

            _activateMechAction.StateChanged -= OnStateChanged;
            _ctSource?.Cancel();
            _ctSource?.Dispose();
        }

        private void SetLeversPoints(List<Transform> pointsTransforms)
        {
            int pointsNumber = pointsTransforms.Count;
            int i = 0;
            _leversPoints = pointsTransforms.ConvertAll(x => new LeverPoint() { transform = x, id = i++ });

            for (i = 0; i < pointsNumber; i++)
            {
                LeverPoint point = _leversPoints[i];
                point.next = _leversPoints[(i + 1) % pointsNumber];
                point.prev = _leversPoints[(i - 1 + pointsNumber) % pointsNumber];
                point.nextRail = point.next.transform.localPosition - point.transform.localPosition;
                point.prevRail = point.prev.transform.localPosition - point.transform.localPosition;
                point.isFree = true;
            }

            foreach (Lever lever in _levers)
            {
                _leversPoints.ElementAt(lever.PointId).isFree = false;
            }    
        }

        private bool IsLeversInTargetPosition()
        {
            foreach (Lever lever in _levers)
            {
                if (!lever.InTargetPoint())
                {
                    return false;
                }
            }

            return true;
        }

        private async Awaitable MoveLever(Lever lever, LeverPoint currentPoint, LeverPoint target, CancellationToken ct)
        {
            _isMoving = true;
            Vector3 newLeverPosition = lever.Transform.localPosition;
            newLeverPosition.x = target.transform.localPosition.x;
            newLeverPosition.y = target.transform.localPosition.y;

            if (target.isFree)
            {
                currentPoint.isFree = true;
                lever.PointId = target.id;
                target.isFree = false;
                await _controlPanel.MoveLever(lever.Transform, newLeverPosition, _leverMoveTime, ct);
                _chosenLever = null;

                if (IsLeversInTargetPosition())
                {
                    MakeTransition();
                }
            }
            else
            {
                Vector3 startPosition = lever.Transform.localPosition;
                Vector3 endPosition = startPosition + (newLeverPosition - startPosition).normalized * _staggerDistance;
                await _controlPanel.MoveLever(lever.Transform, endPosition, _leverStaggerTime + 0.1f, ct);
                await _controlPanel.MoveLever(lever.Transform, startPosition, _leverStaggerTime + 0.1f, ct);
            }

            _isMoving = false;
        }

        private void MakeTransition()
        {
            try
            {
                ServiceLocator.GetService<InventorySystem>().RemoveItemById(_noteItemId);
            }
            catch (Exception ex)
            {
                string msg = $"Some error ocurred while releasing the item from inventory: {ex.Message}";
                GameLogManager.Critical(msg);
                Debug.Log(msg);
            }

            try
            {
                _activateMechAction.MakeTransition(0);
            }
            catch (Exception ex)
            {
                string msg = $"Unable to make action transition from fixed control panel: {ex.Message}";
                GameLogManager.Critical(msg);
                Debug.Log(msg);
            }
        }

        private void OnStateChanged()
        {
            if (_activateMechAction.CurrentState >= QuestActionState.Completed)
            {
                _controlPanel.ChangeState(ControlPanelState.Completed);
                _controlPanel.DeactivateMiniGame();
            }
        }
    }
}