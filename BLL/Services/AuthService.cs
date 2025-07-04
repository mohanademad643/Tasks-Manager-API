
using BLL.DTOs;
using BLL.DTOs.Authentication;
using BLL.Iservices;
using DAL.Identity;
using DAL.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BLL.Services
{

        public class AuthService : IAuthService
        {
            private readonly UserManager<User> _userManager;
            private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;
        public AuthService(UserManager<User> userManager,IConfiguration configuration)
            {
                _userManager = userManager;
                _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JWTSettings");
        }


        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
            {

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                    throw new UnauthorizedAccessException("Invalid credentials");

                if (user.Status != "active")
                    throw new UnauthorizedAccessException("User account is not active");
            var signinCredentials = GetSigningCredentials();
            var Claims = GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signinCredentials, Claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddDays(7),
                    User = MapUserToDto(user)
                };
            }

            public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
            {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new ApplicationException("A user with this email already exists.");

            var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    Status = "active",
                    Role = registerDto.Role
                };
              var UserEmail = await _userManager.FindByEmailAsync(registerDto.Email);

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                    throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            var signinCredentials = GetSigningCredentials();
            var Claims = GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signinCredentials, Claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddDays(7),
                    User = MapUserToDto(user)
                };
            }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private List<Claim> GetClaims(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Email)
    };

            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        private UserDto MapUserToDto(User user) => new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Status = user.Status,
                Role = user.Role,
                TotalTasks = user.TotalTasks
            };
        }
    }

