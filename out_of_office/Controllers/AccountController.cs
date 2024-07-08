using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using out_of_office.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace out_of_office.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var role = model.Role ?? RoleModel.Employee;
                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                    else
                    {
                        return BadRequest("Invalid role");
                    }

                    return Ok(new { Message = "User created successfully!" });
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Invalid data");
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = RoleModel.Administrator)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                var result = await _userManager.AddToRoleAsync(user, model.Role);
                if (result.Succeeded)
                {
                    return Ok("Role assigned successfully");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            else
            {
                return BadRequest("Invalid role");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null.");
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Email cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Password cannot be null or empty.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var secretKey = _configuration["JWT:Secret"];
                if (string.IsNullOrEmpty(secretKey))
                {
                    throw new InvalidOperationException("JWT Secret Key is not configured.");
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost("create-role")]
        [Authorize(Roles = RoleModel.Administrator)]
        public async Task<IActionResult> CreateRole([FromBody] RoleModel model)
        {
            var roleExist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
                if (roleResult.Succeeded)
                {
                    return Ok(new { Message = "Role created successfully!" });
                }
                return BadRequest(roleResult.Errors);
            }
            return BadRequest("Role already exists");
        }
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Optional role for registration
    }

    public class AssignRoleModel
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RoleModel
    {
        public string RoleName { get; set; }
        public const string Employee = "Employee";
        public const string HRManager = "HRManager";
        public const string ProjectManager = "ProjectManager";
        public const string Administrator = "Administrator";
    }
}
