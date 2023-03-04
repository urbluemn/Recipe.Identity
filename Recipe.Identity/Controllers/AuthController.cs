using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recipe.Identity.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Recipe.Identity.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
        IIdentityServerInteractionService interactionService) =>
        (_signInManager, _userManager, _interactionService) =
        (signInManager, userManager, interactionService);

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.FindByNameAsync(viewModel.Username);
            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View(viewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(viewModel.Username,
            viewModel.Password, /*viewModel.RememberMe*/true, false);
            if(result.Succeeded)
            {
                // if(!string.IsNullOrEmpty(viewModel.ReturnUrl) && Url.IsLocalUrl(viewModel.ReturnUrl)){
                    return Redirect(viewModel.ReturnUrl);
                // }
                // else{
                //     var claims = new List<Claim>
                //     {
                //         new Claim(ClaimTypes.Name, viewModel.Username),
                //         new Claim(ClaimTypes.NameIdentifier, user.Id)
                //     };

                //     var Identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //     var principal = new ClaimsPrincipal(Identity);
                //     var props = new AuthenticationProperties{
                //         IsPersistent = true,
                //         ExpiresUtc = DateTime.UtcNow.AddDays(10)
                //     };
                //     HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();
                //     return RedirectToAction("UserProfile");
                // }
            }
            ModelState.AddModelError(string.Empty, "Login error");
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            var viewModel = new RegisterViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = new AppUser
            {
                UserName = viewModel.Username
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);
            if(result.Succeeded)
            {
                await _signInManager.SignInAsync(user, true);
                return Redirect(viewModel.ReturnUrl);
            }
            ModelState.AddModelError(string.Empty, "Error occureed");
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
    }
}