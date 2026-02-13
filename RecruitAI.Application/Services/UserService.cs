using RecruitAI.Domain.Entities;
using RecruitAI.Infrastructure.Persistence;
using RecruitAI.Application.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace RecruitAI.Application.Services;

public class UserService
{
    private readonly RecruitAIDbContext _context;
    private readonly JwtTokenService _jwtTokenService;
    public UserService(RecruitAIDbContext context, JwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<SignupResponse> SignupAsync(SignupRequest request)
    {
        // Check if user already exists
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        // Validate role
        if (request.Role != "Candidate" && request.Role != "Recruiter")
            throw new InvalidOperationException("Role must be 'Candidate' or 'Recruiter'");

        // Hash password
        var passwordHash = HashPassword(request.Password);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Create profile based on role
        if (request.Role == "Candidate")
        {
            var candidate = new Candidate
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.Candidates.Add(candidate);
        }
        else if (request.Role == "Recruiter")
        {
            var recruiter = new Recruiter
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };
            _context.Recruiters.Add(recruiter);
        }

        await _context.SaveChangesAsync();

        return new SignupResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid email or password");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Role.ToString());

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

}
