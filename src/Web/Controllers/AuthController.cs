using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Application.Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterUserCommand());
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken cancellationToken = default)
        {
            //if (!ModelState.IsValid) return View(command);

            var cmd = new RegisterUserCommand
            {
                UserName = command.UserName,
                Email = command.Email,
                Password = command.Password,
                PhoneNumber = command.PhoneNumber,
                FirstName = command.FirstName,
                LastName = command.LastName
            };

            try
            {
                var userId = await _mediator.SendAsync<RegisterUserCommand, Guid>(cmd, cancellationToken);
                // For now, redirect to a simple confirmation page or login
                TempData["RegistrationSuccess"] = "Registration successful. Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                // Prefer mapping known exceptions to friendly messages
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(command);
            }
        }

        // Minimal Login page (form implementation in future steps)
        [HttpGet]
        public IActionResult Login()
        {
            // Show link to Register if needed
            ViewBag.RegistrationEnabled = true;
            return View(); // create simple view or reuse later
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _mediator.SendAsync<LoginUserCommand, LoginUserResponse>(command, cancellationToken);

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", $"Account locked until {result.LockoutEnd}.");
                    return View(command);
                }

                // Create Claims
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Name, result.FullName)
            };

                // Add role claims if you have them:
                // claims.Add(new Claim(ClaimTypes.Role, "ROLE X"));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = command.RememberMe,
                    ExpiresUtc = command.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(30)
                        : DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties
                );

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(command);
            }
        }
    }
}
