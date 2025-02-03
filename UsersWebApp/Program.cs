using Microsoft.EntityFrameworkCore;
using UsersWebApp.Data;
using UsersWebApp.Logging;

namespace UsersWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем проверку на выполнение миграций
            var isMigration = args.Any(arg =>
                arg.Contains("ef", StringComparison.OrdinalIgnoreCase) ||
                arg.Contains("migration", StringComparison.OrdinalIgnoreCase));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Добавляем IHostEnvironment в провайдер
            builder.Services.AddSingleton<IHostEnvironment>(builder.Environment);

            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Настройка логирования только для приложения (не для миграций)
            if (!isMigration)
            {
                // Фильтрация логов при добавлении провайдеров
                builder.Logging.AddFilter("Microsoft", LogLevel.None);
                builder.Logging.AddFilter("System", LogLevel.None);

                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();
                // Регистрируем FileLoggerProvider с передачей IHostEnvironment
                builder.Logging.AddProvider(new FileLoggerProvider(
                    builder.Configuration,
                    builder.Environment));
            }
            else
            {
                // Минимальное логирование для миграций
                builder.Logging.AddConsole();
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Логирование пути к файлам при запуске
            if (!isMigration)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                var logPath = Path.Combine(app.Environment.ContentRootPath, "logs", "app.log");
                logger.LogInformation("Logs directory: {Path}", logPath);
            }

            app.Run();
        }
    }
}
