using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using LaptopStore.API.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Core;
using LaptopStore.Business.Services;
using LaptopStore.Data.Entities;
using LaptopStore.Data.Contexts;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace LaptopStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        //them o day
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        //sua code o day them EmailSender thanh IEmailSender

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
        IEmailSender emailSender)
        //sua code o day them EmailSender thanh IEmailSender
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _emailSender = emailSender;
            _roleManager = roleManager; //them o day
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVm registerVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                Name = registerVm.Name,
                Email = registerVm.Email,
                UserName = registerVm.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerVm.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"User could not be created: {errors}");
            }
            if (await _roleManager.RoleExistsAsync("Customer"))
            {
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            //them o day

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Authentication",
                new { token, email = user.Email }, Request.Scheme);

            var emailMessage = $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.";
            await _emailSender.SendEmailAsync(user.Email, "Confirm your account", emailMessage);

            return Created(nameof(Register), $"User {registerVm.Email} created! A confirmation email has been sent.");
        }

        [HttpGet("get-role")]
        public async Task<IActionResult> GetRole(string token)
        {

            string uid = InspectTokenByClaimType(token, ClaimTypes.NameIdentifier);
            if (uid == null)
            {
                return BadRequest("Invalid Token or Token is Expired!");
            }
            var user = await _userManager.FindByIdAsync(uid);
            if (user == null)
            {
                return BadRequest("User không tồn tại.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid Email Confirmation Request");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Redirect("http://localhost:4200/login");
                //return Ok("Email confirmed successfully!");
            }
            else
            {
                return BadRequest("Email confirmation failed.");
            }
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
        {
            var user = await _userManager.FindByEmailAsync(loginVm.Email);

            if (user == null)
            {
                return BadRequest("Your account has been locked. Please try again later.");
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest("YOUR EMAIL NOT CONFIRM");
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                return BadRequest($"Your account has been locked. Please try again later. {user.LockoutEnd - DateTime.UtcNow}.");
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginVm.Password);
            if (!isPasswordValid)
            {
                user.AccessFailedCount++;

                if (user.AccessFailedCount >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(5);
                    await _userManager.UpdateAsync(user);

                    return BadRequest($"Your account has been locked. Please try again later. {user.LockoutEnd - DateTime.UtcNow}.");
                }

                await _userManager.UpdateAsync(user);
                return BadRequest($"Incorrect Password {user.AccessFailedCount}/5 times. After 5 wrong attempts, the account will be locked for 5 minutes..");
            }
            user.LastLoginDate = DateTime.UtcNow;
            user.IsActive = true;
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            var tokenValue = await GenerateJWTToken(user);
            return Ok(tokenValue);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVm forgotPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Email is required");
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if (user == null)
            {
                return Ok("If the email is registered, a reset link will be sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:4200/reset-password?token={token.Replace("+", "%2B")}&email={forgotPasswordVm.Email}";

            var emailMessage = $"Please reset your password by clicking <a href='{resetLink}'>here</a>.";
            await _emailSender.SendEmailAsync(forgotPasswordVm.Email, "Password Reset", emailMessage);

            return Ok($"If the email is registered, a reset link will be sent.");
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmailAsync([FromBody] SendEmailVm sendEmailVm)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
            emailMessage.To.Add(new MailboxAddress(sendEmailVm.ToEmail, sendEmailVm.ToEmail));
            emailMessage.Subject = sendEmailVm.Subject;
            emailMessage.Body = new TextPart("html") { Text = sendEmailVm.Message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

            return Ok("Email sent successfully.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVm resetPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Tìm người dùng qua email
            var user = await _userManager.FindByEmailAsync(resetPasswordVm.Email);
            if (user == null)
            {
                return BadRequest("Invalid request.");
            }
            // Log token nhận được từ client
            Console.WriteLine($"Token received: {resetPasswordVm.Token}");
            // Kiểm tra token và reset mật khẩu
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordVm.Token, resetPasswordVm.NewPassword);

            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            return Ok("Password has been reset successfully.");
        }

        private async Task<AuthResultVm> GenerateJWTToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            var response = new AuthResultVm
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = token.ValidTo
            };

            return response;
        }

        [NonAction]
        public static bool IsTokenExpired(JwtSecurityToken token)
        {
            return token.ValidTo < DateTime.UtcNow;
        }

        [NonAction]
        public static string InspectTokenByClaimType(string jwtToken, String type)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            if (token != null)
            {
                if (!IsTokenExpired(token))
                {
                    var claims = token.Claims;
                    var claimValue = claims.FirstOrDefault(c => string.Equals(c.Type, type, StringComparison.OrdinalIgnoreCase))?.Value;
                    return claimValue;
                }
            }
            return null;
        }
    }
}
