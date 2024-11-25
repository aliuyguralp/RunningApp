using System.ComponentModel.DataAnnotations;

namespace RunningApplicationNew.Entity.Dtos
{
    public class CreateUserDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string SurName { get; set; }

        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; } // Kullanıcıdan alınacak şifre (hashlenmemiş)

        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }
}
