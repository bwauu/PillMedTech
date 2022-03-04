using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PillMedTech.Models;
using System;
using System.Threading.Tasks;

namespace PillMedTech.Controllers
{
    /*
    
    1. Datalagring.
        1.1. Olika databaser som hanterar olika typer av data (ex. inloggningsuppgifter, loggar, datahanteringen etc).

    3. Autentisering.
        
        3.1. Jobba med IdentityUser och IdentityRole.
        3.2. Generellt felmeddelande vid inlogggning.
        3.3. Försök till åtkomst via url går automatiskt till login-sidan.
    4. Auktorisering
        4.1.  Begränsa behörighet (inte mer än man behöver tillgång till).
        4.1.1. Jobba med Authorize och Roles, samt AllowAnonymus.
        4.2. Försök till åtkomst via url när man är inloggad leder till AccessDenied
    7. Logging.
        7.1. Logga viktiga saker (inloggningar, vad som gör, felmeddelanden).

     */
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
