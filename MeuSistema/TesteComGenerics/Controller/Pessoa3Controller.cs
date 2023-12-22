using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Controllers;

[ApiController]
[Route("[controller]")]
public class Pessoa3Controller : ControllerBase
{
    private readonly ILogger<PessoaController> _logger;
    private readonly IMyGenericService<Pessoa> _pessoaService;

    public Pessoa3Controller(ILogger<PessoaController> logger, IMyGenericService<Pessoa> pessoaService)
    {
        _logger = logger;
        _pessoaService = pessoaService;
    }

    [HttpGet("{id}")]
    public IActionResult buscaPorId(int id)
    {
        return Ok(_pessoaService.buscaPorCodigo(id));
    }
    [HttpDelete("{id}")]
    public IActionResult deleta(int id)
    {
        _pessoaService.deletaRegistro(id);
        return Ok("Reggistro deletado.");
    }

    [HttpPost()]
    public IActionResult salvaNovo([FromBody] Pessoa pessoaNova)
    {
        _pessoaService.gravaNovoRegistro(pessoaNova);
        return Ok(pessoaNova);
    }
    [HttpPut("{id}")]
    public IActionResult atualiza([FromBody] Pessoa pessoaNova, int id)
    {
        pessoaNova.Id = id;
        Pessoa pessoa = _pessoaService.buscaPorCodigo(id);
        pessoa.Primeiro_Nome = pessoaNova.Primeiro_Nome;
        pessoa.Sobrenome = pessoaNova.Sobrenome;
        pessoa.Endereco = pessoaNova.Endereco;
        pessoa.Genero = pessoaNova.Genero;
        return Ok(_pessoaService.atualizaRegistro(pessoa));
    }
}
