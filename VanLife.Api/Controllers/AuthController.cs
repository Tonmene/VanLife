using Microsoft.AspNetCore.Mvc;
using VanLife.Api.Models;
using VanLife.Api.Services;

namespace VanLife.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request) => Ok(await authService.SignUp(request));

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) => Ok(await authService.Login(request));

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request) => Ok(await authService.ForgotPassword(request));

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request) => Ok(await authService.ResetPassword(request));
}

