﻿using DevIO.API.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace DevIO.API.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/[controller]")]
    public class TesteController : MainController
    {
        public TesteController(INotificador notificador, IUser appUser) : base(notificador, appUser)
        {
        }
        [HttpGet]
        public IActionResult Get()
        {
            return BadRequest("I Am the Danger");
            //return "Eu sou a versao 2";
        }
    }
}
