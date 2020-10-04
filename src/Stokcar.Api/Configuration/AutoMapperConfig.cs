using AutoMapper;
using Stokcar.Api.ViewModel;
using Stokcar.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stokcar.Api.Configuration
{
    //Tudo que resulver nessa classe, vai resolver pro AUTOMAPPER na hora de iniciar a aplicação
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            CreateMap<ProdutoViewModel, Produto>();
            CreateMap<ProdutoImagemViewModel, Produto>().ReverseMap();
            CreateMap<Produto, ProdutoViewModel>()
                .ForMember(dest => dest.NomeFornecedor, opt => opt.MapFrom(src => src.Fornecedor.Nome));

        }
    }
}
