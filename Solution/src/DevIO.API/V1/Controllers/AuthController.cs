﻿using DevIO.API.Extensions;
using DevIO.API.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using DevIO.API.Controllers;

namespace DevIO.API.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        public AuthController(INotificador notificador,
                              SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings,
                              IUser appUser) : base(notificador, appUser)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("novaconta")]
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
                var token = await GerarJWT(user.Email);
                return CustomResponse(token);
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

            var result = await _signInManager.PasswordSignInAsync(loginUserViewModel.Email,
                                                                  loginUserViewModel.Password,
                                                                  isPersistent: false,
                                                                  lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var token = await GerarJWT(loginUserViewModel.Email);
                return CustomResponse(token);
            }

            if (result.IsLockedOut)
            {
                NotificarErro("Usuario temporariamente travado por tentativas invalidas");
                return CustomResponse();
            }

            NotificarErro("Usuario ou senha incorretos");

            return CustomResponse();
        }

        private async Task<LoginResponseViewModel> GerarJWT(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));

            foreach (var role in userRoles)
            {
                claims.Add(new Claim("role", role));
            }

            var identityClaims = new ClaimsIdentity(claims);

            var TokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var token = TokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = TokenHandler.WriteToken(token);

            var response = new LoginResponseViewModel
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UserToken = new UserTokenViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value })
                }
            };

            return response;
        }

        private static long ToUnixEpochDate(DateTime utcNow)
        {
            return (long)Math.Round((utcNow.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }
    }
}
