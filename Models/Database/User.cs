using System.ComponentModel.DataAnnotations;

namespace Web.Models.Database

{
//здесь поля, которые содержатся в бд
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // Хеш, не сам пароль

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // Статистика пока простые поля, потом расширим
        public int TotalTasksSolved { get; set; } = 0;
        public int CorrectAnswers { get; set; } = 0;
        public int ExperiencePoints { get; set; } = 0;

    }

}
