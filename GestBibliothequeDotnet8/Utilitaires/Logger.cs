namespace GestBibliothequeDotnet8.Utilitaires
{
    public static class Logger
    {
        private static readonly string _filePath;

        static Logger()
        {
            _filePath = ConfigurationManager.FilePath;  
            if (string.IsNullOrEmpty(_filePath))
            {
                _filePath = "C:\\logs\\application.log"; 
            }
        }
        public static void Log(string message, string type = "INFO")
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

        public static void LogError(string message) => Log(message, "ERROR");

        public static void LogWarning(string message) => Log(message, "WARN");

        public static void LogInfo(string message) => Log(message, "INFO");

        public static void LogDebug(string message) => Log(message, "DEBUG");

        // Classe pour gérer la configuration de façon globale
        public static class ConfigurationManager
        {
            public static string FilePath { get; private set; }

            public static void Configure(IConfiguration configuration)
            {
                FilePath = configuration["Logging:File:Path"];
            }
        }
    }
}