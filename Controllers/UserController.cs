using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using RunningApplicationNew.Entity.Dtos;
using RunningApplicationNew.Entity;
using RunningApplicationNew.Helpers;
using System;
using RunningApplicationNew.DataLayer;
using RunningApplicationNew.IRepository;


namespace RunningApplicationNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;

        public UserController(IUserRepository userRepository, IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO createUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kullanıcıyı e-posta ile kontrol et
            var existingUser = await _userRepository.GetByEmailAsync(createUserDTO.Email);
            if (existingUser != null)
                return Conflict("Bu email zaten kayıtlı.");

            // Şifreyi hash'le
            var hashedPassword = HashPassword(createUserDTO.Password);

            // Yeni kullanıcıyı oluştur
            var newUser = new User
            {
                Name = createUserDTO.Name,
                SurName = createUserDTO.SurName,
                UserName = createUserDTO.UserName,
                Email = createUserDTO.Email,
                PhoneNumber = createUserDTO.PhoneNumber,
                Address = createUserDTO.Address,
                Age = createUserDTO.Age,
                Height = createUserDTO.Height,
                Weight = createUserDTO.Weight,
                PasswordHash = hashedPassword
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kullanıcıyı e-posta ile al
            var user = await _userRepository.GetByEmailAsync(loginUserDTO.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Şifre doğrulaması
            if (!VerifyPassword(loginUserDTO.Password, user.PasswordHash))
                return Unauthorized("Geçersiz şifre.");

            // JWT token oluştur
            var token = _jwtHelper.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kullanıcıyı e-posta ile al
            var user = await _userRepository.GetByEmailAsync(changePasswordDTO.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Mevcut şifre doğrulaması
            if (!VerifyPassword(changePasswordDTO.CurrentPassword, user.PasswordHash))
                return Unauthorized("Geçersiz mevcut şifre.");

            // Yeni şifreyi hash'le
            var hashedNewPassword = HashPassword(changePasswordDTO.NewPassword);

            // Yeni şifreyi kullanıcıya kaydet
            user.PasswordHash = hashedNewPassword;

            // Veritabanında güncelleme yap
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Şifre başarıyla değiştirildi.");
        }
        [HttpPost("change-passwordLoggedIn")]
        public async Task<IActionResult> ChangePasswordLoggedIn([FromBody] ChangePasswordLoggedDto changePasswordDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // JWT token'ını almak için Authorization header'ı kontrol et
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Token'dan alınan email ile kullanıcıyı al
            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Mevcut şifre doğrulaması
            if (!VerifyPassword(changePasswordDTO.CurrentPassword, user.PasswordHash))
                return Unauthorized("Geçersiz mevcut şifre.");

            // Yeni şifreyi hash'le
            var hashedNewPassword = HashPassword(changePasswordDTO.NewPassword);

            // Yeni şifreyi kullanıcıya kaydet
            user.PasswordHash = hashedNewPassword;

            // Veritabanında güncelleme yap
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Şifre başarıyla değiştirildi.");
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // JWT token'ını almak için Authorization header'ı kontrol et
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token bulunamadı.");

            // Token'ı doğrula ve email bilgisi al
            var emailFromToken = _jwtHelper.ValidateTokenAndGetEmail(token);
            if (emailFromToken == null)
                return Unauthorized("Geçersiz token.");

            // Token'dan alınan email ile kullanıcıyı al
            var user = await _userRepository.GetByEmailAsync(emailFromToken);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            // Kullanıcının sadece güncellenebilir alanlarını değiştir
            if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
                user.PhoneNumber = updateUserDto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Address))
                user.Address = updateUserDto.Address;

            if (updateUserDto.Age.HasValue)
                user.Age = updateUserDto.Age.Value;

            if (updateUserDto.Height.HasValue)
                user.Height = updateUserDto.Height.Value;

            if (updateUserDto.Weight.HasValue)
                user.Weight = updateUserDto.Weight.Value;

            // Veritabanında güncelleme yap
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return Ok("Profil bilgileri başarıyla güncellendi.");
        }

    }


}
