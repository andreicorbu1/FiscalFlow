using System.Security.Claims;
using FiscalFlow.Dto.Response;
using FiscalFlow.Model;
using FiscalFlow.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisterRequest = FiscalFlow.Dto.Request.RegisterRequest;

namespace FiscalFlow.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(IJwtService jwtService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _jwtService = jwtService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet("refresh-user-token")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var user = await _userManager.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)!.Value);

        if (user == null)
        {
            return BadRequest();
        }

        var jwt = _jwtService.CreateJwt(user);
        return Ok(new { token = jwt });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            return Unauthorized("Invalid username!");
        }

        if (user.EmailConfirmed == false)
        {
            return Unauthorized("Please confirm your email!");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, true);

        if (!result.Succeeded)
        {
            return Unauthorized("Invalid password!");
        }

        var response = user.ToUserResponse();
        response.JWT = _jwtService.CreateJwt(user);

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        if (await CheckEmailExistsAlready(registerRequest.Email))
        {
            return BadRequest(
                $"An existing account with the email {registerRequest.Email} already exists! Please try another one!");
        }

        var userToAdd = new AppUser
        {
            UserName = registerRequest.Email.ToLower(),
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Email = registerRequest.Email.ToLower(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(userToAdd, registerRequest.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Created();
    }

    private async Task<bool> CheckEmailExistsAlready(string email)
    {
        return await _userManager.Users.AnyAsync(user => user.Email == email.ToLower());
    }

}