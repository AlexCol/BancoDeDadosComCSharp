using Microsoft.AspNetCore.Mvc;

namespace MeuSistema.Controllers;

[ApiController]
[Route("[controller]")]
public class PessoaController : ControllerBase
{
    private readonly ILogger<PessoaController> _logger;
    private readonly IMyService<Pessoa> _pessoaService;

    public PessoaController(ILogger<PessoaController> logger, IMyService<Pessoa> pessoaService)
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
        return Ok("Registro salvo.");
    }
    [HttpPut("{id}")]
    public IActionResult atualiza([FromBody] Pessoa pessoaNova, int id)
    {
        pessoaNova.Id = id;
        return Ok(_pessoaService.atualizaRegistro(pessoaNova));
    }
}
