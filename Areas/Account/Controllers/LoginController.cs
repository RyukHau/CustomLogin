using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomLogin.Areas.Account.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace CustomLogin.Areas.Account.Controllers
{
    [Area("Account")]
    [AutoValidateAntiforgeryToken]
    public class LoginController : Controller
    {
        private readonly IConfiguration config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginController(IConfiguration config)
        {
            this.config = config;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel login)
        {
            if (ModelState.IsValid)
            {

                if (((login.LoginAccount == "test") && (login.LoginPassword == "test")) == false)
                {
                    ViewBag.errMsg = "Login Fail";

                    return View();
                }
                else
                {
                    Claim[] claims = new[] { new Claim("Account", login.LoginAccount) };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

                    double loginExpireMinute = this.config.GetValue<double>("LoginExpireMinute");
                    await HttpContext.SignInAsync(principal, new AuthenticationProperties() { IsPersistent = false, });

                    return Redirect("/Home/Index");
                }
            }
            else
            {
                ViewData["errMsg"] = "Please Input";

                return View();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Privacy");
        }
    }
}
