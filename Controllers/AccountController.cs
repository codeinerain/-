using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web.Data;
using Web.Models;
using Web.Models.Database;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Check(RegisterViewModels model)
        {
            if (ModelState.IsValid)
            {
                // Проверяем, не занят ли логин
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == model.Login);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                    return View("Register", model);
                }

                // Хешируем пароль
                string passwordHash = HashPassword(model.Password);

                // Создаём пользователя
                var user = new User
                {
                    Login = model.Login,
                    PasswordHash = passwordHash,
                    RegistrationDate = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var claims = new List<Claim>{
                     new Claim(ClaimTypes.Name, user.Login),
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                ViewBag.Message = $"Пользователь {model.Login} успешно зарегистрирован!";
                return View("Success");
            }

            return View("Register", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Хешируем введённый пароль
                string passwordHash = HashPassword(model.Password);

                // Ищем пользователя с таким логином и хешем пароля
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Login == model.Login && u.PasswordHash == passwordHash);

                if (user != null)
                {
                    var claims = new List<Claim>{
                        new Claim(ClaimTypes.Name, user.Login),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties{
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));
                    ViewBag.Message = $"Добро пожаловать, {user.Login}!";
                    return View("Success");
                }

                ModelState.AddModelError("", "Неверный логин или пароль");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }



        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}

