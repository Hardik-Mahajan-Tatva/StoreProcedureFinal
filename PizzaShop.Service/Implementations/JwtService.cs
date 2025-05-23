using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PizzaShop.Service.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IUsersloginRepository _userLoginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRolesService _rolesService;
        public JwtService(IConfiguration configuration, IUsersloginRepository userLoginRepository, IUserRepository userRepository, IRolesService rolesService)
        {
            _key = configuration["Jwt:Key"]
                ?? throw new ArgumentNullException(nameof(configuration), "JWT key is missing in configuration.");

            _issuer = configuration["Jwt:Issuer"]
                ?? throw new ArgumentNullException(nameof(configuration), "JWT issuer is missing in configuration.");

            _audience = configuration["Jwt:Audience"]
                ?? throw new ArgumentNullException(nameof(configuration), "JWT audience is missing in configuration.");

            _userLoginRepository = userLoginRepository;
            _userRepository = userRepository;
            _rolesService = rolesService;
        }
        #region  Generate Token
        public async Task<string> GenerateJwtToken(string email, bool rememberMe = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);

            var userDetail = await _userLoginRepository.GetUserByEmailAsync(email)
                ?? throw new Exception("User not found while generating JWT token.");

            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var roleName = await _rolesService.GetRoleNameById(userDetail.Roleid);
            if (roleName == null)
            {
                throw new Exception("RoleName not found.");
            }
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim("username", userDetail.Username),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, roleName),
        new Claim("Roleid", userDetail.Roleid.ToString()),
        new Claim("ProfileImage", user.Profileimg ?? "")
    };

            if (rememberMe)
            {
                claims.Add(new Claim("RememberMe", "True"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion
        #region  ValidatToken
        public ClaimsPrincipal? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}