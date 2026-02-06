using BigProject.Managers;
using BigProject.Player;
using BigProject.Settings;
using BigProject.Systems.HUD;
using System;
using UnityEngine;
using UnityEngine.Assertions;


namespace BigProject.Utilities
{
    public class GameplayStatesHandler : IDisposable
    {
        private GlobalConfig _config;
        private PlayerInputHandler _input;
        private GameplayManager _gameplayManager;
        private HUD _hud;

        public GameplayStatesHandler(GlobalConfig config, GameplayManager gameplayManager, PlayerInputHandler input, HUD hud)
        {
            _gameplayManager = gameplayManager;
            _config = config;
            _input = input;
            _hud =  hud;
            Assert.IsNotNull(_gameplayManager, "Gameplay states handler get null Gameplay Manager.");
            Assert.IsNotNull(_input, "Gameplay states handler get null Player Input Handler.");
            Assert.IsNotNull(_config, "Gameplay states handler get null config.");
            Assert.IsNotNull(_hud, "Gameplay states handler get null HUD.");
            _gameplayManager.StateChanged += OnGameStateChanged;
        }

        public void Dispose()
        {
            _gameplayManager.StateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameplayState state)
        {
            switch (state)
            {
                case GameplayState.Play:
                    _hud.ShowWidget(_config.HUDJournalWidgetId, 0.1f);
                    _hud.ShowWidget(_config.HUDInventoryWidgetId, 0.1f);
                    _input.SwitchToPlayerActionMap();
                    break;
                case GameplayState.MiniGame:
                    _hud.HideWidget(_config.HUDJournalWidgetId);
                    _input.SwitchToMiniGameActionMap();
                    break;
                case GameplayState.Dialogue:
                    _hud.HideWidget(_config.HUDInventoryWidgetId);
                    _input.SwitchToMiniGameActionMap();
                    break;
                default:
                    break;
            }
        }
    }
}
