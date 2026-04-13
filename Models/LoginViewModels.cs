using System.ComponentModel.DataAnnotations;
/*поля для входа, все тоже самое, что и для регистрации*/
namespace Web.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; } 

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 
    }
}