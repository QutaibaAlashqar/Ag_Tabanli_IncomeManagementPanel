using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public string? MessageContent { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime StartDate { get; set; }

    }
}
