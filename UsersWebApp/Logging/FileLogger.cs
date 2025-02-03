using System.Text;

namespace UsersWebApp.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _name;
        private readonly string _filePath;
        private readonly long _fileSizeLimit;
        private readonly int _retainedFileCount;

        public FileLogger(string name, string filePath, long fileSizeLimit, int retainedFileCount)
        {
            _name = name;
            _filePath = filePath;
            _fileSizeLimit = fileSizeLimit;
            _retainedFileCount = retainedFileCount;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Фильтрация по уровню логирования
            if (!IsEnabled(logLevel)) return;

            // Фильтрация системных сообщений
            if (eventId.Name?.Contains("Microsoft.EntityFrameworkCore") == true) return;

            var message = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_name}: {formatter(state, exception)}{(exception != null ? $"\n{exception}" : "")}";

            RotateFiles();
            WriteToFile(message);
        }

        private void WriteToFile(string message)
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                lock (this)
                {
                    File.AppendAllText(_filePath, message + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        private void RotateFiles()
        {
            try
            {
                var fileInfo = new FileInfo(_filePath);
                if (fileInfo.Exists && fileInfo.Length > _fileSizeLimit)
                {
                    var directory = fileInfo.DirectoryName;
                    var pattern = $"{Path.GetFileNameWithoutExtension(_filePath)}*{Path.GetExtension(_filePath)}";

                    var files = Directory.GetFiles(directory, pattern)
                        .OrderByDescending(f => f)
                        .ToList();

                    while (files.Count >= _retainedFileCount)
                    {
                        File.Delete(files.Last());
                        files.RemoveAt(files.Count - 1);
                    }

                    File.Move(
                        _filePath,
                        Path.Combine(
                            directory,
                            $"{Path.GetFileNameWithoutExtension(_filePath)}-{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(_filePath)}"
                        )
                    );
                }
            }
            catch { /* Обработка ошибок ротации */ }
        }
    }
}
