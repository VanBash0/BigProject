using BigProject.Systems;

namespace BigProject.Managers
{
    public class GameLogManagerTicker : ITickable
    {
        private const string CRITICAL_NOT_CORRECT_INIT = "{0} not inited correct!";

        private ManualLoop _manualLoop;

        public GameLogManagerTicker(ManualLoop manualLoop)
        {
            _manualLoop = manualLoop;

            if (_manualLoop == null)
            {
                GameLogManager.Critical(string.Format(CRITICAL_NOT_CORRECT_INIT, GetType().Name));
            }
            else
            {
                _manualLoop.AddTickable(this);
            }
        }

        public void Tick()
        {
            GameLogManager.Update();
        }
    }
}