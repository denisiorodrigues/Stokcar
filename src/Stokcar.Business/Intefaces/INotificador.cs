using System.Collections.Generic;
using Stokcar.Business.Notificacoes;

namespace Stokcar.Business.Intefaces
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}