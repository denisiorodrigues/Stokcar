using System;
using System.Threading.Tasks;
using Stokcar.Business.Intefaces;
using Stokcar.Business.Models;
using Stokcar.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Stokcar.Data.Repository
{
    public class EnderecoRepository : Repository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(MeuDbContext context) : base(context) { }

        public async Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId)
        {
            return await Db.Enderecos.AsNoTracking()
                .FirstOrDefaultAsync(f => f.FornecedorId == fornecedorId);
        }
    }
}