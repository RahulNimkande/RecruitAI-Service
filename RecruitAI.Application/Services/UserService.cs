using Microsoft.EntityFrameworkCore;
using RecruitAI.Application.DTOs;
using RecruitAI.Domain.Entities;
using RecruitAI.Infrastructure.Persistence;

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
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists");

        if (request.Role != "Candidate" && request.Role != "Recruiter")
            throw new InvalidOperationException("Role must be 'Candidate' or 'Recruiter'");

        var passwordHash = HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

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
        else
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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid email or password");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Role);

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
