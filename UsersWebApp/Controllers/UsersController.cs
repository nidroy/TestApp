using Microsoft.AspNetCore.Mvc;
using UsersWebApp.Data;
using UsersWebApp.Models;

namespace UsersWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            ApplicationContext context,
            ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Запрос списка пользователей");
                var users = _context.Users.ToList();
                _logger.LogInformation($"Найдено {users.Count} пользователей");
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователей");
                throw;
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    _logger.LogInformation($"Создан пользователь: {user.UserName} (ID: {user.Id})");
                    return RedirectToAction("Index");
                }

                _logger.LogWarning("Некорректные данные пользователя: {@User}", user);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя");
                throw;
            }
        }
    }
}
