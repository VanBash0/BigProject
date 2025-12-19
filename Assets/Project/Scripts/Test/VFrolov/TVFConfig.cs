using Zenject;
using BigProject.Managers;
using BigProject.Systems;

namespace BigProject.Test.VFrolov
{
    /// <summary>
    /// Все нужные квесту ID и имена для удобного вывода статистики.
    /// В реальных сценах возможно ID будут браться из БД, собираемой дизайнерами?
    /// </summary>
    public static class TVFConfig
    {
        public static int questId = 10;
        public static int getGearId = 0, installGearId = 1, leverId = 2, buttonId = 3, millId = 4;

        [Inject]
        private static void Construct(ProgressManager pm)
        {
            // Автосохранения можно включать/выключать.
            // В тестовом квесте нет условий завершения квеста, поэтому автосохранения не будут производиться в любом случае.
            pm.AutoSave = false;
        }

        public static string GetNameByID(int id) =>
            id switch
            {
                0 => "Установка шестерни",
                1 => "Перемещение рычага",
                2 => "Нажатие кнопки",
                3 => "Колесо мельницы",
                _ => "Активность"
            };

        public static string GetNameByState(QuestActionState state) =>
            state switch
            {
                QuestActionState.Inactive => "неактивно",
                QuestActionState.Active => "активно",
                QuestActionState.Completed => "выполнено",
                QuestActionState.Failed => "провалено",
                _ => "неизвестно"
            };
    }
}
