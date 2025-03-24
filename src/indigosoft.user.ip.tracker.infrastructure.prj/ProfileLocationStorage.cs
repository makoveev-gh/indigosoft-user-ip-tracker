namespace Indigosoft.User.Ip.Tracker.Infrastructure;

public static class ProfileLocationStorage
{
    static ProfileLocationStorage()
    {
        ProfileRootDir = PathConstants.AppProgramDataPath;
    }

    /// <summary>
    /// Gets or sets the root directory of the application profile.
    /// </summary>
    public static string ProfileRootDir { get; set; }

    /// <summary>
    /// Gets the path where application configuration files will be stored.
    /// </summary>
    public static string ConfigDirPath => Path.Combine(ProfileRootDir, "");

    /// <summary>
    /// Gets the path where the application's database files will be stored.
    /// </summary>
    public static string DatabaseDirPath => Path.Combine(ProfileRootDir, "Database");

    /// <summary>
    /// Gets the name of the file that contains the application's configuration.
    /// </summary>
    public static string ConfigFileName => "app.json";

    /// <summary>
    /// Gets the full path to the application's configuration file.
    /// </summary>
    public static string ConfigPath => Path.Combine(ConfigDirPath, ConfigFileName);
}
