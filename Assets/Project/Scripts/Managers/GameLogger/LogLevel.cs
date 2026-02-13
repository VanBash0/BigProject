namespace BigProject.Managers
{
    public enum LogLevel
    {
        None, // in Editor development
        Debug, // to activate on build: BuildProfiles > PlatformSettings > WindowsSettings > DevelopmentBuild = on
        Release, // to activate on build: BuildProfiles > PlatformSettings > WindowsSettings > DevelopmentBuild = off
    }
}