using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ERPSalesSystem.Models;
using System.Threading.Tasks;
using ERPSalesSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Security.Policy;
using System;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ERPContext _context;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ERPContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<IActionResult> UserList()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Create UserProfile for the new user
                var userProfile = new UserProfile
                {
                    UserId = user.Email,
                    User = user // I ve did this to code work, because of foreign key
               
                     
                };
                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
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
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult VerifyPasswordHash(string username, string password, [FromServices] ILogger<AccountController> logger)
    {
        var user = _userManager.FindByNameAsync(username).Result;
        if (user == null)
        {
            return Content("User not found.");
        }

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        logger.LogInformation($"Username: {username}\nEntered Password: {password}\nStored Password Hash: {user.PasswordHash}\nVerification Result: {verificationResult}");

        if (verificationResult == PasswordVerificationResult.Success)
        {
            return Content("Password verification succeeded.");
        }
        else
        {
            return Content("Password verification failed.");
        }
    }

public async Task<IActionResult> VerifyPassword(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return Content("User not found.");
        }

        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (verificationResult == PasswordVerificationResult.Success)
        {
            return Content("Password verification succeeded.");
        }
        else
        {
            return Content("Password verification failed.");
        }
    }

}

