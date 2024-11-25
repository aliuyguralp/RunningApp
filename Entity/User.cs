using System.ComponentModel.DataAnnotations;

namespace RunningApplicationNew.Entity
{
    public class User
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required] // Zorunlu alan
        [MaxLength(100)]
        public string Name { get; set; }

        [Required] // Zorunlu alan
        [MaxLength(100)]
        public string SurName { get; set; }

        public string UserName { get; set; }

        [Required]
        [EmailAddress] // Email formatı kontrolü
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public int Age { get; set; }


        public double Height { get; set; }

        public double Weight { get; set; }

        [Required]
        [MinLength(6)]
        public string PasswordHash { get; set; } // Şifreler hashlenmiş saklanır

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Kullanıcı oluşturulma tarihi
    }
}
