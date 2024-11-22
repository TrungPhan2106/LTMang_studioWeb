using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioManagement.Models;
using System.Security.Claims;

namespace StudioManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyDbContext _context;

        public AccountController(MyDbContext context)
        {
            _context = context;
        }

        // Đăng ký
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                // Tạo UUID duy nhất cho User
                model.UserUUID = Guid.NewGuid().ToString();

                // Mã hóa mật khẩu
                var passwordHasher = new PasswordHasher<User>();
                model.Password = passwordHasher.HashPassword(model, model.Password);

                // Gán Role mặc định nếu cần thiết (ví dụ: member)
                if (model.RoleId == null)
                {
                    var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.rolename == "Member");
                    if (defaultRole != null)
                    {
                        model.RoleId = defaultRole.Id;
                    }
                }

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // Đăng nhập
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tìm kiếm người dùng theo email
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    // Kiểm tra mật khẩu
                    var passwordHasher = new PasswordHasher<User>();
                    var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

                    if (result == PasswordVerificationResult.Success)
                    {
                        // Tạo danh sách claims
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.Role?.rolename ?? "member") // Gắn role trực tiếp
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                        // Phân hướng dựa theo vai trò
                        if (user.Role != null && user.Role.rolename == "employee")
                        {
                            return RedirectToAction("ViewProfile", "Employee"); // Trang của Employee
                        }
                        else
                        {
                            return RedirectToAction("ViewProfile", "Member"); // Trang của Member
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Mật khẩu không đúng.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email không tồn tại.");
                }
            }
            return View(model);
        }

        // Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    // Tạo token và gửi email tại đây
                    user.ResetToken = Guid.NewGuid().ToString();
                    user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ConfirmForgotPassword");
                }
                ModelState.AddModelError(string.Empty, "Email không tồn tại.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        // Thiết lập lại mật khẩu
        [HttpGet]
        public IActionResult ResetPassword(string token) => View(new ResetPasswordViewModel { Token = token });

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == model.Token && u.ResetTokenExpiration > DateTime.UtcNow);
                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    user.Password = passwordHasher.HashPassword(user, model.NewPassword);
                    user.ResetToken = null;
                    user.ResetTokenExpiration = null;
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Login");
                }
                ModelState.AddModelError(string.Empty, "Token không hợp lệ hoặc đã hết hạn.");
            }
            return View(model);
        }
    }
}
