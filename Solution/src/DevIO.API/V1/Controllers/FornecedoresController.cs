using AutoMapper;
using DevIO.API.Controllers;
using DevIO.API.Extensions;
using DevIO.API.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.V1.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fornecedores")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      IMapper mapper,
                                      IFornecedorService fornecedorService,
                                      INotificador notificador,
                                      IEnderecoRepository enderecoRepository,
                                      IUser appUser) : base(notificador, appUser)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {
            var fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

            return Ok(fornecedores);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = await ObterFornecedorProdutosEndereco(id);

            if (fornecedor == null)
            {
                return NotFound();
            }

            return Ok(fornecedor);
        }

        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<ActionResult<EnderecoViewModel>> ObterEnderecoPorId(Guid id)
        {
            var enderecoViewModel = _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
            if (enderecoViewModel == null) { return NotFound(); }
            return CustomResponse(enderecoViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);

            await _fornecedorService.Adicionar(fornecedor);

            return CustomResponse(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id)
            {
                NotificarErro("O Id informado nao corresponde a query");
                return CustomResponse(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("atualizar-endereco/{id:guid}")]
        public async Task<ActionResult<EnderecoViewModel>> AtualizarEnderecoPorId(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (enderecoViewModel.Id != id)
            {
                NotificarErro("O Id informado nao corresponde a query");
                return CustomResponse();
            }

            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var endereco = _mapper.Map<Endereco>(enderecoViewModel);

            await _fornecedorService.AtualizarEndereco(endereco);

            return CustomResponse(enderecoViewModel);
        }


        [ClaimsAuthorize("Fornecedor", "Remover")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Deletar(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var fornecedor = _mapper.Map<Fornecedor>(await ObterFornecedorEndereco(id));

            if (fornecedor == null)
            {
                return NotFound();
            }

            await _fornecedorService.Remover(fornecedor.Id);

            return CustomResponse();
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }
    }


}