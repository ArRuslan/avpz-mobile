using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.BasicModels
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string PaymentDate { get; set; } = string.Empty;
        public float Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
