using AuthApi.Auth.Models;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AuthController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
    {
        var response = _accountService.Authenticate(model);
        return string.IsNullOrEmpty(response.JwtToken) ? Unauthorized() : Ok(response); 
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public ActionResult<UserResponse> Register(CreateUserRequest model)
    {
        var tokenResp = _accountService.Register(model);
        return tokenResp is null ? Ok(new { message = "Email already Exist" }) : Ok(tokenResp);
    }
}
