namespace OSBBProject1.Models
{
    public class ChangeHistory
    {
        public int Id { get; set; }
        public string Action { get; set; } // Тип зміни (редагування, видалення і т.д.)
        public string ChangedBy { get; set; } // Адмін, який зробив зміну
        public string EntityType { get; set; } // Тип сутності (наприклад, Resident)
        public int EntityId { get; set; } // ID сутності (наприклад, ID користувача)
        public string OldValue { get; set; } // Старі значення
        public string NewValue { get; set; } // Нові значення
        public DateTime ChangeDate { get; set; } // Дата зміни
    }
}
