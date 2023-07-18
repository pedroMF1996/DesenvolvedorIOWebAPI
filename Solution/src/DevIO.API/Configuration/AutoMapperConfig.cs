using AutoMapper;
using DevIO.API.ViewModels;
using DevIO.Business.Models;

namespace DevIO.API.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig() {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            CreateMap<ProdutoViewModel, Produto>();

            CreateMap<Produto, ProdutoViewModel>()
                .ForMember(dest => dest.NomeFornecedor, opt => opt.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}
