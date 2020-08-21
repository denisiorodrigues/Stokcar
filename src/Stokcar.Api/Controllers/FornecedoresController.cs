using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Stokcar.Api.ViewModel;
using Stokcar.Business.Intefaces;

namespace Stokcar.Api.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRespository;
        private readonly IMapper _mapper;

        //Injeção de dependência
        public FornecedoresController(IFornecedorRepository fornecedorRespository, IMapper mapper)
        {
            this._fornecedorRespository = fornecedorRespository;
            this._mapper = mapper;
        }
        
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
        {
            var fornecedor = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRespository.ObterTodos());

            return fornecedor;
        }
    }
}
