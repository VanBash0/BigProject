namespace BigProject.Interactive
{
    public interface IInteractive
    {
        // Вызывается при взаимодействии с объектом
        void OnInteract();
        // Возвращает true, если для взаимодействия нужно подойти близко
        bool RequiresProximity();
    }
}
