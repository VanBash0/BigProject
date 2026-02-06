using UnityEngine;
using BigProject.Systems;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

namespace BigProject.Managers
{
    public enum GameplayState
    {
        Play,
        Dialogue,
        MiniGame,
        Map,
        Inventory,
        Pause
    }

    /// <summary>
    /// Переводит игру в различные геймплейные состояния, переключая очереди обновления.
    /// </summary>
    public class GameplayManager
    {
        private GameplayState _state;
        private readonly ManualLoop _manualLoop;
        private readonly Dictionary<GameplayState, List<int>> _tickQueueIds = new();
        private readonly List<int> _activeQueueIds = new();

        public event Action<GameplayState> StateChanged;

        public GameplayManager(ManualLoop manualLoop)
        {
            Assert.IsNotNull(manualLoop, "Gameplay Manager: manual loop is null.");
            _state = GameplayState.Play;
            _manualLoop = manualLoop;
        }

        /// <summary>
        /// Добавляет очередь обновления с привязкой к состоянию игры.
        /// </summary>
        /// <param name="state">Состояние игры</param>
        /// <param name="id">Идентификатор очереди</param>
        public void AddQueueToState(GameplayState state, int id)
        {
            if (_tickQueueIds.TryGetValue(state, out var stateIds))
            {
                stateIds.Add(id);
            }
            else
            {
                _tickQueueIds.Add(state, new() { id });
            }

            if (_manualLoop.IsTickableQueueActive(id))
            {
                _activeQueueIds.Add(id);
            }
        }

        /// <summary>
        /// Меняет состояние игры.
        /// </summary>
        /// <param name="state">Новое состояние</param>
        public void ChangeState(GameplayState state)
        {
            if (_state == state)
            {
                return;
            }

            foreach (int id in _activeQueueIds)
            {
                _manualLoop.SetTickableQueueActive(id, false);
            }

            _activeQueueIds.Clear();
            _state = state;
            StateChanged?.Invoke(_state);

            if (_tickQueueIds.TryGetValue(_state, out var nextIds))
            {
                foreach (int id in nextIds)
                {
                    _manualLoop.SetTickableQueueActive(id, true);
                    _activeQueueIds.Add(id);
                }
            }
        }
    }
}