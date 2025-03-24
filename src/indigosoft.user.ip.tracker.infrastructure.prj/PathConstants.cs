using System.Reflection;

namespace Indigosoft.User.Ip.Tracker.Infrastructure;

public static class PathConstants
{
    #region Data
    /// <summary>
    /// Path to the common application data folder for all users on the system.
    /// </summary>
    private static readonly string ProgramDataPath = AppDomain.CurrentDomain.BaseDirectory;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the path where application data will be stored.
    /// </summary>
    public static string AppProgramDataPath => Path.Combine(
        ProgramDataPath);

    /// <summary>
    /// Gets the path where application logs will be stored.
    /// </summary>
    public static string LogsRootPath => Path.Combine(
        AppProgramDataPath,
        "Logs");

    /// <summary>
    /// Gets the path where temporary application data will be stored.
    /// </summary>
    public static string TempDataPath => Path.Combine(
        AppProgramDataPath,
        "Temp");

    #endregion
}
