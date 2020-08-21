using System;
using System.Threading.Tasks;
using Stokcar.Business.Models;

namespace Stokcar.Business.Intefaces
{
    public interface IEnderecoRepository : IRepository<Endereco>
    {
        Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId);
    }
}