using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OSBBProject1.Models
{
    public class Bill
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int LightBill { get; set; }

        public int WaterBill { get; set; }

        public bool Status { get; set; } 
    }
}
