using System.ComponentModel.DataAnnotations;
/*здесь поля, к которым обращаются при создании кнопок, также здесь есть возможные разные ошибки при вводе */
namespace Web.Models
{
    public class RegisterViewModels

    {
        [Display(Name = "Введите логин")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [StringLength(20,MinimumLength = 3, ErrorMessage = "Ваш логин должен быть от 3 до 20 символов")]
        public String Login { get; set; }
        
        [Display(Name = "Введите пароль")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Ваш пароль должен быть от 3 до 20 символов")]
        public String Password { get; set; }

        [Display(Name = "Подтвердите ваш пароль")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Пароли не совпадают")]
        public String ConfirmPassword { get; set; }
        
    }


}
