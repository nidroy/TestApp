namespace UsersWebApp.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;
        private readonly long _fileSizeLimit;
        private readonly int _retainedFileCount;

        public FileLoggerProvider(IConfiguration config, IHostEnvironment env)
        {
            // Получаем путь к корневой папке приложения
            var basePath = env.ContentRootPath;

            _filePath = config.GetValue("Logging:File:Path",
                Path.Combine(basePath, "Logs", "app.log"));

            _fileSizeLimit = config.GetValue<long>(
                "Logging:File:FileSizeLimitBytes", 10485760);

            _retainedFileCount = config.GetValue<int>(
                "Logging:File:RetainedFileCountLimit", 5);
        }

        public ILogger CreateLogger(string categoryName) =>
            new FileLogger(categoryName, _filePath, _fileSizeLimit, _retainedFileCount);

        public void Dispose() { }
    }
}
