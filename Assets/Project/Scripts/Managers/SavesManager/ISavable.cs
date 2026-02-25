namespace BigProject.Managers
{
    /// <summary>
    /// Interface for saving/loading objects.
    /// For successful saving/loading, saved parameters must be public or serializable.
    /// Important: The built-in JsonUtility only works with simple data, lists, and one-dimensional arrays.
    /// If you need to write complex structures, you can wrap them in subclasses.
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// A unique key by which the object will be recorded within the save (the save itself has one single key).
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Data to be saved, packed into an object.
        /// If all public and serializable fields of the class need to be written, return 'this' pointer;
        /// </summary>
        public object SavingData { get; }

        /// <summary>
        /// Выхывается после загрузки данных в объект.
        /// </summary>
        public void OnLoad() { }
    }
}