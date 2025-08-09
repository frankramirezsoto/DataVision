using DataVision.Data;
using DataVision.DTOs;
using DataVision.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataVision.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(LoginRequest request);
        Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request);
        Task<UserDto?> GetUserByIdAsync(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Credenciales inválidas"
                    };
                }

                // Read JWT config
                var jwtSettings = _configuration.GetSection("Jwt");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
                var expiresAt = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"]));

                // Create claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID in standard claim
                    new Claim(ClaimTypes.Name, user.Username), // Username in standard claim
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username), // Subject (standard JWT claim)
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var credentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                );

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return new LoginResponse
                {
                    Success = true,
                    Token = tokenString,
                    Message = "Autenticación exitosa",
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username
                    },
                    ExpiresAt = expiresAt
                };
            }
            catch
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                };
            }
        }


        public async Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                if (existingUser != null)
                {
                    return new ApiResponse<UserDto>
                    {
                        Success = false,
                        Message = "El usuario ya existe",
                        Errors = new List<string> { "Username already taken" }
                    };
                }

                // Create new user
                var user = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username
                };

                return new ApiResponse<UserDto>
                {
                    Success = true,
                    Message = "Usuario creado exitosamente",
                    Data = userDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserDto>
                {
                    Success = false,
                    Message = "Error al crear usuario",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username
            };
        }
    }
}
