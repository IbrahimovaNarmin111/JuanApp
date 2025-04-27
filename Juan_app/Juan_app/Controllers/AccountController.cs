using Juan_app.DAL;
using Juan_app.Helpers;
using Juan_app.Models;
using Juan_app.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Juan_app.Controllers
{

    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;

        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = new AppUser()
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                UserName = registerVM.Username,
                Email = registerVM.Email,
            };
            IdentityResult results = await _userManager.CreateAsync(user, registerVM.Password);
            if (!results.Succeeded)
            {
                foreach (var error in results.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    return View();
                }
            }
            await _userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(Index), "Home");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existUser = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if (existUser == null)
            {
                existUser = await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (existUser == null)
                {
                    ModelState.AddModelError(String.Empty, "Username ve ya Password sehvdir");
                    return View();
                }

            }
            var signInCheck = _signInManager.CheckPasswordSignInAsync(existUser, loginVM.Password, true).Result;
            if (signInCheck.IsLockedOut)
            {
                ModelState.AddModelError("", "Biraz sonra yeniden cehd edin");
                return View();
            }
            if (!signInCheck.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username ve ya Password sehvdir");
                return View();
            }
            await _signInManager.SignInAsync(existUser, loginVM.RememberMe);
            //if(ReturnUrl!= null) 
            //{
            //    return RedirectToAction(ReturnUrl);
            //}
            return RedirectToAction(nameof(Index), "Home");

        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
        public async Task<IActionResult> CreateRole()
        {
            foreach (var role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = role.ToString(),
                    });
                }
            }
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
