public class LoggerSingleton
{
    private static LoggerSingleton _instance;
    private static readonly object _lock = new object();
    private string _filePath;

    // Le constructeur privé empêche la création d'instances depuis l'extérieur
    private LoggerSingleton()
    {
        // Utilisation de la configuration globale
        _filePath = ConfigurationManager.GetFilePath();  // Récupérer le chemin de fichier via une classe de gestion de configuration
        if (string.IsNullOrEmpty(_filePath))
        {
            _filePath = "C:\\logs\\application3.log";  // Valeur par défaut en cas d'absence de configuration
        }
    }

    // La méthode publique qui garantit qu'il n'y a qu'une seule instance de Logger
    public static LoggerSingleton Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new LoggerSingleton();
                }
                return _instance;
            }
        }
    }

    // Méthode pour enregistrer les messages dans un fichier log
    public void Log(string message, string type = "INFO")
    {
        try
        {
            // message avec un type (INFO, ERROR, WARN, DEBUG)
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{type}] {message}";

            using (StreamWriter writer = new StreamWriter(_filePath, true))
            {
                writer.WriteLine(logMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la journalisation : {ex.Message}");
        }
    }

    public void LogError(string message) => Log(message, "ERROR");
    public void LogWarning(string message) => Log(message, "WARN");
    public void LogInfo(string message) => Log(message, "INFO");
    public void LogDebug(string message) => Log(message, "DEBUG");
    public static class ConfigurationManager
    {
        public static string FilePath { get; private set; }

        public static void Configure(IConfiguration configuration)
        {
            FilePath = configuration["Logging:File:Path"];
        }
        public static string GetFilePath()
        {
            return FilePath;
        }
    }
}
