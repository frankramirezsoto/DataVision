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
        string GenerateJwtToken(User user);
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

                var token = GenerateJwtToken(user);
                var expiresAt = DateTime.UtcNow.AddHours(24);

                return new LoginResponse
                {
                    Success = true,
                    Token = token,
                    Message = "Autenticación exitosa",
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Role = user.Role,
                        CreatedAt = user.CreatedAt
                    },
                    ExpiresAt = expiresAt
                };
            }
            catch (Exception ex)
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
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Role = request.Role,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
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
                Username = user.Username,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-here-make-it-long";
            var issuer = jwtSettings["Issuer"] ?? "DataVisionAPI";
            var audience = jwtSettings["Audience"] ?? "DataVisionClient";

            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("userId", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
