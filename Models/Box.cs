using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Box
    {
        public int BoxID { get; set; }
        
        public int UserID { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int BoxType { get; set; }
    }
}
