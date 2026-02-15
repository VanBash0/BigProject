namespace BigProject.Systems
{
    public static class LogStr
    {
        public const string INFO_SESSION_STARTED = "=== Session started ===";
        public const string INFO_APPLICATION_QUITTING = "=== Application quitting ===";
        public const string INFO_DELETE_OLD_LOG_FILE = "Delete old log file: {0}.";
        public const string INFO_SCENE_LOADING = "Load scene: {0}.";

        public const string WARNING_UNHANDLED_SYSTEM_MESSAGE_TYPE = "Unhandled sysytem message type!\nMessage:\n{0}.";
        public const string WARNING_SAME_SCENE = "You are trying to load already loaded scene.";

        public const string ERROR_WRITE_FAILED = "Logger write failed: {0}.";
        public const string ERROR_FILE_DELETE_FAILED = "Failed to delete {0}: {1}.";
        public const string ERROR_CREATE_DIRECTORY = "Cannot create log dir: {0}." +
            "\n\n Will be created in Persistent Data Path: " +
            "\n\t %userprofile%\\AppData\\LocalLow\\{1}\\{2}\\";

        public const string CRITICAL_UNABLE_GET_SERVICE = "{0}: can't get {1} service.";
    }
}