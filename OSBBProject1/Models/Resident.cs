using System.ComponentModel.DataAnnotations;

namespace OSBBProject1.Models
{
    public class Resident
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим.")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁЇїІіЄєҐґ]+$", ErrorMessage = "В імені можуть бути лише букви.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Логін є обов'язковим.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Номер квартири є обов'язковим.")]
        public int FlatNumber { get; set; }

        [Required(ErrorMessage = "Кількість жителів є обов'язковою.")]
        [Range(1, int.MaxValue, ErrorMessage = "Кількість жителів має бути цілим числом і більше або дорівнює 1.")]
        public int AmountOfResidents { get; set; }

        public string Description { get; set; }
    }
}