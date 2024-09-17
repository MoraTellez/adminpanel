using adminpanel.Data;
using adminpanel.Models;
using adminpanel.Models.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace adminpanel.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(ILogger<AdminController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LogIn()
        {
            LogInDTO dto = new();
            return View(dto);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInDTO dto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email or password invalid.");
                    return View(dto);
                }

                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, true, false);

                if (result.Succeeded)
                {

                    if (user.Status == SD.Status.Blocked)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is blocked.");
                        return View(dto);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email or password invalid.");
                }
            }

            return View(dto);

        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("LogIn");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            RegisterDTO dto = new();
            return View(dto);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = dto.Name, Email = dto.Email, Registration = DateTime.Now, LastLogin = DateTime.Now };
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string ids, string method)
        {
            if (string.IsNullOrEmpty(ids) || string.IsNullOrEmpty(method))
            {
                return BadRequest("You must select at least one user.");
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var usersIds = ids.Split(',');

            List<ApplicationUser> users = new();

            try
            {

                users = await _userManager.Users.Where(u => usersIds.Contains(u.Id)).ToListAsync();

                if (users == null || !users.Any())
                {
                    return BadRequest("No users found.");
                }

                switch (method)
                {
                    case "Block":
                        await UpdateUserStatuses(users, SD.Status.Blocked);
                        break;
                    case "Active":
                        await UpdateUserStatuses(users, SD.Status.Active);
                        break;
                    case "Delete":
                        await DeleteUsers(users);
                        break;
                    default:
                        return BadRequest("Invalid method.");
                }
            }
            catch
            {

                return BadRequest("No users found.");
            }

            var result = await _userManager.Users.Select(user => new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                lastLogin = user.LastLogin.ToString("M/dd/yyy HH:mm:ss tt"),
                status = user.Status.ToString(),
                registration = user.Registration.ToString("M/dd/yyy HH:mm:ss tt")
            }).ToListAsync();

            var currentUser = result.FirstOrDefault(u => u.id == currentUserId);

            if (currentUser is null || currentUser.status == SD.Status.Blocked.ToString())
            {
                await _signInManager.SignOutAsync();

                return Json(new { redirect = true });
            }

            return Json(new { success = true, result });
        }

        private async Task UpdateUserStatuses(List<ApplicationUser> users, SD.Status status)
        {
            foreach (var user in users)
            {
                user.Status = status;
                await _userManager.UpdateAsync(user);
            }
        }

        private async Task DeleteUsers(List<ApplicationUser> users)
        {
            foreach (var user in users)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
