using System;
using System.Threading.Tasks;
using Stokcar.Business.Models;

namespace Stokcar.Business.Intefaces
{
    public interface IProdutoService : IDisposable
    {
        Task Adicionar(Produto produto);
        Task Atualizar(Produto produto);
        Task Remover(Guid id);
    }
}