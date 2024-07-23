using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERPSalesSystem.Models;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
public class UserProfilesController : Controller
{
    private readonly ERPContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserProfilesController(ERPContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: UserProfiles/Edit
    public async Task<IActionResult> Edit()
    {
        var userId = _userManager.GetUserId(User);
        var userProfile = await _context.UserProfiles.SingleOrDefaultAsync(up => up.UserId == userId);
        if (userProfile == null)
        {
            return NotFound();
        }
        // Load countries from JSON file
        var countriesJson = await System.IO.File.ReadAllTextAsync("wwwroot/data/countries.json");
        var countryList = JsonSerializer.Deserialize<List<string>>(countriesJson);

        var countrySelectList = countryList.Select(c => new SelectListItem
        {
            Value = c,
            Text = c
        }).ToList();
        ViewBag.Countries = new SelectList(countrySelectList, "Value", "Text");
        return View(userProfile);
    }

    // POST: UserProfiles/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserProfile userProfile)
    {
        var userId = _userManager.GetUserId(User);
        userProfile.UserId = userId;

        if (ModelState.IsValid)
        {
            _context.Update(userProfile);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Your profile has been updated successfully!";
            return RedirectToAction("Index", "Home");
        }
        return View(userProfile);
    }
}
