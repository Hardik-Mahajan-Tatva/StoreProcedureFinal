using PizzaShop.Repository.Models;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using System.Security.Claims;
using Newtonsoft.Json;
using PizzaShop.Repository.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace PizzaShop.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUsersloginRepository _usersLoginRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUsersloginRepository usersLoginRepository, IConfiguration configuration)
        {
            _usersLoginRepository = usersLoginRepository;
            _configuration = configuration;
        }
        public async Task<Userslogin?> AuthenticateUser(string email, string password)
        {
            var user = await _usersLoginRepository.GetUserLoginAsync(email, password);

            if (user == null)
                return null;

            return user;
        }
        public async Task<Userslogin?> GetUser(string email)
        {
            var user = await _usersLoginRepository.GetUserAsync(email);

            if (user == null)
                return null;

            return user;
        }
        public async Task<bool> CheckIfUserExists(string email)
        {
            if (await _usersLoginRepository.GetUserByEmailAsync(email) != null)
                return true;
            else
                return false;
        }
        public async Task<string> GeneratePasswordResetToken(string email)
        {
            var user = await _usersLoginRepository.GetUserByEmailAsync(email);
            if (user == null) return string.Empty;

            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            await _usersLoginRepository.SavePasswordResetToken(user.Userloginid,
              token,
              DateTime.SpecifyKind(DateTime.UtcNow.AddHours(24), DateTimeKind.Unspecified),
              false);

            return token;
        }
        public async Task<bool> ValidatePasswordResetToken(string token)
        {
            var tokenEntry = await _usersLoginRepository.GetUserByResetToken(token);
            if (tokenEntry == null || tokenEntry.Resettokenexpiration.GetValueOrDefault() < DateTime.UtcNow || tokenEntry.Resettokenused == true)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> UpdateUserPassword(string token, string newPassword)
        {
            var user = await _usersLoginRepository.GetUserByResetToken(token);
            if (user == null || user.Resettokenexpiration.GetValueOrDefault() < DateTime.UtcNow || user.Resettokenused == true)
            {
                return false;
            }

            bool passwordUpdated = await _usersLoginRepository.SetUserPassword(user.Userloginid, newPassword);
            if (!passwordUpdated)
            {
                return false;
            }

            user.Resettokenused = true;
            user.Isfirstlogin = false;
            await _usersLoginRepository.InvalidateResetToken(user);

            return true;
        }
        public List<Permission> GetUserPermissionsFromToken(ClaimsPrincipal user)
        {
            var permissionClaim = user.FindFirst("Permissions")?.Value;
            return string.IsNullOrEmpty(permissionClaim)
                ? new List<Permission>()
                : JsonConvert.DeserializeObject<List<Permission>>(permissionClaim) ?? new List<Permission>();
        }
        public LoginViewModel DecodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            try
            {
                var principal = handler.ValidateToken(token, validations, out var validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                var decodedEmail = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                return new LoginViewModel
                {
                    Email = decodedEmail,
                    Password = string.Empty,
                    RememberMe = jwtToken?.Claims.FirstOrDefault(c => c.Type == "RememberMe")?.Value == "True"
                };
            }
            catch (Exception)
            {
                return new LoginViewModel();
            }
        }
    }
}

