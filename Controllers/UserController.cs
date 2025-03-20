using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserManagement.Models;
using UserManagement.Services;
using BCrypt.Net;
using System.Security.Claims;

[Route("api/auth")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public UserController(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        if (string.IsNullOrEmpty(registerRequest.Email) || 
            string.IsNullOrEmpty(registerRequest.Password) || 
            string.IsNullOrEmpty(registerRequest.Name))
        {
            Console.WriteLine("Registration failed: Missing required fields.");
            return BadRequest(new { message = "All fields are required." });
        }

        var existingUser = await _userService.GetUserByEmailAsync(registerRequest.Email);
        if (existingUser != null)
        {
            Console.WriteLine($"Registration failed: Email already in use ({registerRequest.Email}).");
            return Conflict(new { message = "Email already in use." });
        }

        Console.WriteLine($"Hashing password for {registerRequest.Email}...");
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
        Console.WriteLine($"Generated Hash: {hashedPassword}");

        var newUser = new User
        {
            Email = registerRequest.Email,
            PasswordHash = hashedPassword,
            Name = registerRequest.Name
        };

        await _userService.CreateUserAsync(newUser);
        Console.WriteLine($"User registered successfully: {newUser.Email}");

        return Ok(new { message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        Console.WriteLine($" Login attempt: {loginRequest.Email}");

        if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
        {
            Console.WriteLine(" Missing email or password.");
            return BadRequest(new { message = "Email and password are required." });
        }

        var user = await _userService.GetUserByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            Console.WriteLine(" User not found.");
            return Unauthorized(new { message = "Invalid email or password." });
        }

        Console.WriteLine($" Stored Hash: {user.PasswordHash}");
        Console.WriteLine($" Entered Password: {loginRequest.Password}");

        if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
        {
            Console.WriteLine(" Password mismatch.");
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = _authService.GenerateJwtToken(user);
        Console.WriteLine($" Login successful. Token generated for {user.Email}");
        return Ok(new { Token = token });
    }

    [HttpGet("/api/users/me")]
    [Authorize]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"🔹 Fetching user data for ID: {userId}");

        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine(" Invalid token: No user ID found.");
            return Unauthorized(new { message = "Invalid token." });
        }

        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            Console.WriteLine(" User not found.");
            return NotFound(new { message = "User not found." });
        }

        Console.WriteLine($" User data retrieved: {user.Email}");
        return Ok(user);
    }

    [HttpDelete("reset-test-users")]
    public async Task<IActionResult> ResetTestUsers()
    {
        try
        {
            await _userService.DeleteUsersByEmailPatternAsync("@example.com"); 
            Console.WriteLine("Test users deleted successfully.");
            return Ok(new { message = "Test users deleted successfully." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting test users: {ex.Message}");
            return StatusCode(500, new { message = "Error deleting test users." });
        }
    }
}
