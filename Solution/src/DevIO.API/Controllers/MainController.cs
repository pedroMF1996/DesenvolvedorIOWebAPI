using DevIO.Business.Intefaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DevIO.API.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;

        protected MainController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!ModelState.IsValid) NotificarModelStateInvalida(ModelState);
            
            return CustomResponse();
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object? result = null)
        {
            if(OperacaoValida())
            {
                return Ok(new
                {
                    sucess = true, 
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificador.ObterNotificacoes().Select(m => m.Mensagem)
            });
        }

        protected void NotificarModelStateInvalida(ModelStateDictionary modelState) {
            var erros = ModelState.Values.SelectMany(x => x.Errors);

            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;

                NotificarErro(erroMsg);
            }
        }

        protected void NotificarErro(string erroMsg)
        {
            _notificador.Handle(new Notificacao(erroMsg));
        }
    }    
}