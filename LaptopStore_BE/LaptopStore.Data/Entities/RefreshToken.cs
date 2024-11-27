using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaptopStore.Data.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string JwtId { get; set; }

        public bool IsRevoked { get; set; } = false;

        [Required]
        public DateTime DateAdded { get; set; }

        [Required]
        public DateTime DateExpire { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
