using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Stokcar.Api.ViewModel;
using Stokcar.Business.Intefaces;
using Stokcar.Business.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Stokcar.Api.Controllers
{
    [Route("api/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRespository;
        private readonly IEnderecoRepository _enderecoRespository;
        private readonly IProdutoRepository _produtoRespository;
        private readonly IMapper _mapper;
        private readonly INotificador _notificador;

        public ProdutosController(IEnderecoRepository enderecoRespository,
                                  IFornecedorRepository fornecedorRespository,
                                  IProdutoRepository produtoRepository,
                                  IMapper mapper, 
                                  INotificador notificador) : base(notificador)
        {
            _enderecoRespository = enderecoRespository;
            _fornecedorRespository = fornecedorRespository;
            _produtoRespository = produtoRepository;
            _mapper = mapper;
            _notificador = notificador;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRespository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);
            
            if (produtoViewModel == null) return NotFound();

            return produtoViewModel;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
            if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse();
            }

            produtoViewModel.Imagem = imagemNome;

            await _produtoRespository.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            var imagemDataByteArray = Convert.FromBase64String(arquivo);
            if (string.IsNullOrEmpty(arquivo))
            {
               NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imagemDataByteArray);

            return true;
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var produto = await ObterProduto(id);
            if (produto == null) return NotFound();
            await _produtoRespository.Remover(id);
            return CustomResponse(produto);
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRespository.ObterPorId(id));
        }
    }
}
