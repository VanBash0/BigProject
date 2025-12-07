using UnityEngine;
using Zenject;

namespace BigProject.Test.VFrolov
{
    /// <summary>
    /// Все зависимости для тестов.
    /// </summary>
    public class TVFInstaller : MonoInstaller
    {
        [SerializeField]
        private TVFInput _input;

        public override void InstallBindings()
        {
            // Профиль игрока "Player0", квесты грузим из "Data\\Quests".
            Container.Bind<ProgressManager>().FromInstance(new ProgressManager("Player0", new QuestJsonLoader("Data\\Quests"), new SavesManager())).AsSingle();
            Container.BindInstance(_input).AsSingle();
        }
    }
}