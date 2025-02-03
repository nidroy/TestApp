namespace UsersWebApp.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;
        private readonly long _fileSizeLimit;
        private readonly int _retainedFileCount;

        public FileLoggerProvider(IConfiguration config)
        {
            // Добавляем проверки и значения по умолчанию
            _filePath = config.GetValue("Logging:File:Path", "Logs/app.log");
            _fileSizeLimit = config.GetValue("Logging:File:FileSizeLimitBytes", 10485760);
            _retainedFileCount = config.GetValue("Logging:File:RetainedFileCountLimit", 5);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _filePath, _fileSizeLimit, _retainedFileCount);
        }

        public void Dispose() { }
    }
}
