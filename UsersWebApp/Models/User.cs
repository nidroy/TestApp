using System.ComponentModel.DataAnnotations;

namespace UsersWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Возраст обязателен")]
        [Range(1, 150, ErrorMessage = "Возраст должен быть от 1 до 150")]
        public int Age { get; set; }
    }
}
