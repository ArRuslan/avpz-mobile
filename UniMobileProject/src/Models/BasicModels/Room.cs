using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniMobileProject.src.Models.BasicModels
{
    public class Room
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
