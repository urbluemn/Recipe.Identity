using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Recipe.Identity.Models;
using Recipe.Identity.Models.Login;
using Recipe.Identity.Models.Register;
using Recipe.Identity.Models.Logout;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Recipe.Identity.Account;
using IdentityServer4.Models;
using Recipe.Identity.Extensions;
using IdentityServer4;
using IdentityModel;
using IdentityServer4.Extensions;

namespace Recipe.Identity.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IIdentityServerInteractionService interactionService) =>
            (_signInManager, _userManager, _interactionService) =
            (signInManager, userManager, interactionService);

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = await BuildLoginViewModelAsync(returnUrl);

            return View(vm);
        }
            //Old Login Get
            // var viewModel = new LoginInputModel
            // {
            //     ReturnUrl = returnUrl
            // };
            // return View(viewModel);

            // build a model so we know what to show on the login page

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel viewModel, string button)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(viewModel.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "Sign In" && context != null)
            {
                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interactionService.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", viewModel.ReturnUrl);
                }

                return Redirect(viewModel.ReturnUrl);
            }

            if (ModelState.IsValid)
            {
                // only set explicit expiration here if user chooses "remember me". 
                // otherwise we rely upon expiration configured in cookie middleware.
                AuthenticationProperties props = null;
                if (AccountOptions.AllowRememberLogin && viewModel.RememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    };
                }

                var user = await _userManager.FindByNameAsync(viewModel.Username);
                    if(user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found");
                        return View(viewModel);
                    }

                await _signInManager.SignInAsync(user, props);

                if (context != null)
                {
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", viewModel.ReturnUrl);
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(viewModel.ReturnUrl);
                }

                // request for a local page
                if (Url.IsLocalUrl(viewModel.ReturnUrl))
                {
                    return Redirect(viewModel.ReturnUrl);
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(viewModel);
            return View(vm);
        }
            //Old Login Post
            // if(!ModelState.IsValid)
            // {
            //     return View(viewModel);
            // }

            // var user = await _userManager.FindByNameAsync(viewModel.Username);
            // if(user == null)
            // {
            //     ModelState.AddModelError(string.Empty, "User not found");
            //     return View(viewModel);
            // }

            // var result = await _signInManager.PasswordSignInAsync(viewModel.Username,
            // viewModel.Password, viewModel.RememberLogin, false);
            // if(result.Succeeded)
            // {
            //     if(Url.IsLocalUrl(viewModel.ReturnUrl))
            //     {
            //         return Redirect(viewModel.ReturnUrl);
            //     }
            // }
            // ModelState.AddModelError(string.Empty, "Login error");
            // return View(viewModel);

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
        public async Task<IActionResult> Register(RegisterViewModel viewModel, string button)
        {

            if (button != "register")
            {
                return Redirect(viewModel.ReturnUrl);
            }

            if (ModelState.IsValid)
            {
                AuthenticationProperties props = null;
                if (AccountOptions.AllowRememberLogin && viewModel.RememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    };
                }

                var user = new AppUser
                {
                    UserName = viewModel.Username,
                    Email = viewModel.Email
                };

                await _userManager.CreateAsync(user, viewModel.Password);

                await _signInManager.SignInAsync(user, props);

                if (Url.IsLocalUrl(viewModel.ReturnUrl))
                {
                    return Redirect(viewModel.ReturnUrl);
                }
                else
                {
                    throw new Exception("Invalid return URL");
                }
            }
            ModelState.AddModelError(string.Empty, "Error occured");
            return View(viewModel);
        }

            //Old Register Post
            // if(!ModelState.IsValid)
            // {
            //     var errors = ModelState.Values.SelectMany(x=>x.Errors);
            //     foreach(var e in errors)
            //     {
            //         ViewBag.Message = e.ToString();
            //     }
            //     return View(viewModel);
            // }
            //     var user = new AppUser
            //     {
            //         UserName = viewModel.Username,
            //         Email = viewModel.Email
            //     };

            //     var result = await _userManager.CreateAsync(user, viewModel.Password);
            //     if(result.Succeeded)
            //     {
            //         if(Url.IsLocalUrl(viewModel.ReturnUrl))
            //         {
            //             await _signInManager.SignInAsync(user, viewModel.RememberLogin);
            //             return Redirect(viewModel.ReturnUrl);
            //         }
            //     }
            // ModelState.AddModelError(string.Empty, "Error occured");
            // return View(viewModel);
        //}

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interactionService.GetAuthorizationContextAsync(returnUrl);

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }
        // private async Task<RegisterViewModel> BuildRegisterViewModelAsync(string returnUrl)
        // {
        //     var context = await _interactionService.GetAuthorizationContextAsync(returnUrl);

        //     return new RegisterViewModel
        //     {
        //         AllowRememberLogin = AccountOptions.AllowRememberLogin,
        //         EnableLocalLogin = AccountOptions.AllowLocalLogin,
        //         ReturnUrl = returnUrl,
        //         Username = context?.LoginHint,
        //         Email = context?.DisplayMode
        //     };
        // }

        // private async Task<RegisterViewModel> BuildRegisterViewModelAsync(RegisterViewModel model)
        // {
        //     var vm = await BuildRegisterViewModelAsync(model.ReturnUrl);
        //     vm.Username = model.Username;
        //     vm.RememberLogin = model.RememberLogin;
        //     return vm;
        // }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interactionService.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interactionService.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };
            return vm;
        }
    }
}
