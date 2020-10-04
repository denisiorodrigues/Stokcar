using AutoMapper;
using Microsoft.AspNetCore.Http;
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
                return CustomResponse(ModelState);
            }

            produtoViewModel.Imagem = imagemNome;

            await _produtoRespository.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [HttpPost("Adicionar")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo(ProdutoImagemViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgPrefixo = Guid.NewGuid() + "_";
            if (!UploadArquivoAlternativo(produtoViewModel.ImagemUpload, imgPrefixo))
            {
                return CustomResponse(ModelState);
            }

            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload;

            await _produtoRespository.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
               NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var imagemDataByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imagemDataByteArray);

            return true;
        }

        private bool UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                arquivo.CopyToAsync(stream);
            }
            
            return true;
        }

        //Ilimitado
        [DisableRequestSizeLimit]
        //Limitando a 40BM
        //[RequestSizeLimit(40000000)] 
        [HttpPost("imagem")]
        public async Task<ActionResult> AdicionarImagem(IFormFile file)
        {
            return Ok(file);
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
