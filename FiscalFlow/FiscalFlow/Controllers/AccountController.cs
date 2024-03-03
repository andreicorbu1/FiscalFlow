using System.Security.Claims;
using System.Text;
using FiscalFlow.Dto.Dto;
using FiscalFlow.Dto.Request;
using FiscalFlow.Dto.Response;
using FiscalFlow.Model;
using FiscalFlow.Services.Interfaces;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RegisterRequest = FiscalFlow.Dto.Request.RegisterRequest;

namespace FiscalFlow.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly IAccountService _accountService;

    public AccountController(IJwtService jwtService,
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IConfiguration config,
        IAccountService accountService)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _emailService = emailService;
        _config = config;
        _accountService = accountService;
    }

    [Authorize]
    [HttpGet("refresh-user-token")]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        var user = await _accountService.FindByEmailAsync(User.FindFirst(ClaimTypes.Email)!.Value);

        if (user == null)
        {
            return BadRequest();
        }

        var response = user.ToUserResponse();
        response.JWT = _jwtService.CreateJwt(user);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _accountService.FindByEmailAsync(loginRequest.Email);
        if (user == null)
        {
            return Unauthorized("Invalid username!");
        }

        if (user.EmailConfirmed == false)
        {
            return Unauthorized("Please confirm your email!");
        }

        var result = await _accountService.CheckPasswordSignInAsync(user, loginRequest.Password, true);

        if (!result.Succeeded)
        {
            return Unauthorized("Invalid password!");
        }

        var response = user.ToUserResponse();
        response.JWT = _jwtService.CreateJwt(user);

        return Ok(response);
    }

    [HttpPut("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailRequest confirmEmailRequest)
    {
        var user = await _accountService.FindByEmailAsync(confirmEmailRequest.Email);
        if (user == null) return Unauthorized("This email has not been registered yet!");
        if (user.EmailConfirmed) return BadRequest("Your email has already been confirmed!");

        try
        {
            var result = await _accountService.ConfirmEmailAsync(user, confirmEmailRequest.Token);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    title = "Email confirmed",
                    message = "Your email address has been confirmed! You can login now."
                });
            }

            return BadRequest("Invalid token. Please try again!");
        }
        catch (Exception)
        {
            return BadRequest("Invalid token. Please try again!");
        }
    }

    [HttpPost("resend-email-confirmation-link/{email}")]
    public async Task<IActionResult> ResendEmailConfirmationAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");
        var user = await _accountService.FindByEmailAsync(email);

        if (user == null) return Unauthorized("This email has not been registered yet!");
        if (user.EmailConfirmed) return BadRequest("Your email has already been confirmed!");

        try
        {
            if (await _accountService.SendConfirmEmailAsync(user))
            {
                return Ok(new
                {
                    title = "Confirmation link sent",
                    message = "New confirmation link has been sent. Please check your inbox."
                });
            }
            return BadRequest("Failed to send email.Please contact admin!");
        }
        catch (Exception)
        {
            return BadRequest("Failed to send email. Please contact admin!");
        }
    }

    [HttpPost("forgot-password/{email}")]
    public async Task<IActionResult> ForgotPasswordAsync(string email)
    {
        if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");
        var user = await _accountService.FindByEmailAsync(email);

        if (user == null) return Unauthorized("This email has not been registered yet!");
        if (!user.EmailConfirmed) return BadRequest("Please confirm your email first!");
        try
        {
            if (await _accountService.SendForgotPasswordEmailAsync(user))
            {
                return Ok(new
                {
                    title = "Forgot password email sent",
                    message = "Please check your email inbox."
                });
            }

            return BadRequest("Failed to send email. Please contact admin!");
        }
        catch (Exception )
        {
            return BadRequest("Failed to send email. Please contact admin!");
        }
    }

    [HttpPut("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
    {
        var user = await _accountService.FindByEmailAsync(resetPasswordRequest.Email);
        if (user is null) return Unauthorized("This email address has not been registered yet!");
        if (!user.EmailConfirmed) return BadRequest("Please confirm your email first!");
        try
        {
            var result = await _accountService.ResetPasswordAsync(user, resetPasswordRequest);
            if (!result.Succeeded)
                return BadRequest("Invalid token! Please try again!");
            return Ok(new
            {
                title = "Password reset success",
                message = "Your password has been reset"
            });
        }
        catch (Exception )
        {
            return BadRequest("Invalid token! Please try again!");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        if (await _accountService.CheckEmailExistsAlready(registerRequest.Email))
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
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(userToAdd, registerRequest.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        try
        {
            if (await _accountService.SendConfirmEmailAsync(userToAdd))
            {
                return Created();
            }
            return BadRequest("Failed to send email. Please contact admin!");
        }
        catch (Exception e)
        {
            return BadRequest("Failed to send email. Please contact admin!");
        }
    }
}