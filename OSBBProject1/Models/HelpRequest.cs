namespace OSBBProject1.Models
{
    public class HelpRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }  // Ім'я користувача, який подав скаргу
        public string Text { get; set; }      // Текст скарги
        public DateTime SendDate { get; set; } // Дата надходження скарги
        public bool Status { get; set; }      // Статус скарги (читається чи ні)
    }
}