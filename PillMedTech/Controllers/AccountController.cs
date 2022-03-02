using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PillMedTech.Models;
using System;
using System.Threading.Tasks;

namespace PillMedTech.Controllers
{
  [Authorize]
  public class AccountController : Controller
  {
    private UserManager<IdentityUser> userManager;
    private SignInManager<IdentityUser> signInManager;
    private IPillMedTechRepository repository;
    private IHttpContextAccessor contextAcc;

    public AccountController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr, IPillMedTechRepository repo, IHttpContextAccessor ctxAcc)
    {
      userManager = userMgr;
      signInManager = signInMgr;
      repository = repo;
      contextAcc = ctxAcc;
    }

    [AllowAnonymous]
    public ViewResult Login(string returnUrl)
    {
      return View(new LoginModel
      {
        ReturnUrl = returnUrl
      });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
      if (ModelState.IsValid)
      {
        IdentityUser user = await userManager.FindByNameAsync(loginModel.UserName);

        if (user != null)
        {
          await signInManager.SignOutAsync();

          if ((await signInManager.PasswordSignInAsync(user,
                          loginModel.Password, false, false)).Succeeded)
          {
            if (await userManager.IsInRoleAsync(user, "Employee"))
            {
              repository.Log(DateTime.Now,
                            HttpContext.Connection.RemoteIpAddress.ToString(),
                            user.ToString(),
                            "Sucessfully logged in"); 
              return Redirect("/Employee/StartEmployee");
            }
            else if (await userManager.IsInRoleAsync(user, "HRStaff"))
            {
              repository.Log(DateTime.Now,
                           HttpContext.Connection.RemoteIpAddress.ToString(),
                           user.ToString(),
                           "Sucessfully logged in"); 
              return Redirect("/Admin/HRStaff");
            }
            else if (await userManager.IsInRoleAsync(user, "ITStaff"))
            {
             repository.Log(DateTime.Now,
                           HttpContext.Connection.RemoteIpAddress.ToString(),
                           user.ToString(),
                           "Sucessfully logged in"); 
              return Redirect("/Admin/ITStaff");
            }
            else
            {
              repository.Log(DateTime.Now,
                           HttpContext.Connection.RemoteIpAddress.ToString(),
                           user.ToString(),
                           "Tried to log in, unsuccessfully");
              return Redirect("/Home/Index");
            }
          }
        }
      }
      ModelState.AddModelError("", "Felaktigt användarnamn eller lösenord");
      repository.Log(DateTime.Now,
                           HttpContext.Connection.RemoteIpAddress.ToString(),
                           loginModel.UserName,
                           "Tried to log in, unsuccessfully");
        
      return View(loginModel);
    }

    public async Task<RedirectResult> Logout(string returnUrl = "/")
    {
      var user = contextAcc.HttpContext.User.Identity.Name;
      await signInManager.SignOutAsync();

      repository.Log(DateTime.Now,
          HttpContext.Connection.RemoteIpAddress.ToString(),
          user.ToString(),
          "Successfully logged out");
      

      return Redirect(returnUrl);
    }

    [AllowAnonymous]
    public ViewResult AccessDenied()
    {
      var user = contextAcc.HttpContext.User.Identity.Name;

      repository.Log(DateTime.Now,
          HttpContext.Connection.RemoteIpAddress.ToString(),
          user.ToString(),
          "Tried to reach unauthorized view");
      
      return View();
    }
  }
}
