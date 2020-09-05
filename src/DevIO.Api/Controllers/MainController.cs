using DevIO.Business.Interfaces;
using DevIO.Business.Notificacoes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace DevIO.Api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;
        public readonly IUser _appUser;

        public Guid UsuarioId { get; set; }
        public bool UsuarioAutenticado { get; set; }

        protected MainController(INotificador notificador, IUser appUser)
        {
            _notificador = notificador;
            _appUser = appUser;

            if (_appUser.IsAuthenticated())
            {
                UsuarioId = _appUser.GetUserId();
                UsuarioAutenticado = true;
            }
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida()) return Ok(new { success = true, data = result });

            return BadRequest(new { success = false, errors = _notificador.ObterNotificacoes().Select(m => m.Mensagem) });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);

            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var listaErros = modelState.Values.SelectMany(e => e.Errors);

            foreach (var e in listaErros)
            {
                var erro = e.Exception == null ? e.ErrorMessage : e.Exception.Message;

                NotificarErro(erro);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }
    }
}
