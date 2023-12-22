using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuSistema.Controllers;

[ApiController]
[Route("[controller]")]
public class LivroController : ControllerBase
{
    private readonly ILogger<PessoaController> _logger;
    private readonly IMyGenericService<Livro> _service;

    public LivroController(ILogger<PessoaController> logger, IMyGenericService<Livro> service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("{id}")]
    public IActionResult buscaPorId(int id)
    {
        return Ok(_service.buscaPorCodigo(id));
    }
    [HttpDelete("{id}")]
    public IActionResult deleta(int id)
    {
        _service.deletaRegistro(id);
        return Ok("Reggistro deletado.");
    }

    [HttpPost()]
    public IActionResult salvaNovo([FromBody] Livro novoRegistro)
    {
        _service.gravaNovoRegistro(novoRegistro);
        return Ok(novoRegistro);
    }
    [HttpPut("{id}")]
    public IActionResult atualiza([FromBody] Livro novoRegistro, int id)
    {
        novoRegistro.Id = id;
        Livro livro = _service.buscaPorCodigo(id);
        livro.Autor = novoRegistro.Autor;
        livro.Titulo = novoRegistro.Titulo;
        return Ok(_service.atualizaRegistro(livro));
    }
}
