using DevIO.API.Controllers;
using DevIO.API.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Configuration
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(INotificador notificador, 
                              SignInManager<IdentityUser> signInManager, 
                              UserManager<IdentityUser> userManager) : base(notificador)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("/novaconta")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUserViewModel)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var user = new IdentityUser()
            {
                UserName = registerUserViewModel.Email,
                Email = registerUserViewModel.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUserViewModel.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return CustomResponse(registerUserViewModel);
            }
            else
                foreach (var error in result.Errors)
                {
                    NotificarErro(error.Description);
                }

            return CustomResponse(registerUserViewModel);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUserViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUserViewModel.Email, loginUserViewModel.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return CustomResponse();
            }

            if (result.IsLockedOut)
            {
                NotificarErro("Usuario temporariamente travado por tentativas invalidas");
                return CustomResponse();
            }

            NotificarErro("Usuario ou senha incorretos");

            return CustomResponse();
        }
    }
}
