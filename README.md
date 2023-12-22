**Primeiro deve-se ter o banco configurado. Acesso root e o schema do banco criado.
No caso do MySql, usar o comando abaixo para criar o schema (o termo 'cadastro' pode ser ajustado para o nome que quiser, ele é o nome do banco de dados):**
```sql
CREATE SCHEMA `cadastro` ;
```

### Então agora abrir o Visual Studio Code.

***Criar o projeto (no caso aqui, uso WebApi) via terminal.***
```CSharp
dotnet new webapi -o MeuSistema
```

***Adicionar os pacotes abaixo.***
✅Dapper (para uso de SQL bruto no código)
```CSharp
dotnet add package Dapper
```

✅Plugin para conexão com MySql.
```CSharp
dotnet add package Pomelo.EntityFrameworkCore.MySql
```
✅Plugin para facilitar a parametrização das confiruações de acesso.
```CSharp
dotnet add package Microsoft.Extensions.Configuration.Json
```
✅Pacotes para uso do Entityframework (para não usar SQL Bruto)
```CSharp
dotnet add package Microsoft.EntityFrameworkCore -v 7.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design -v 7.0.2
```
✅Pacotes para uso da rotina de migrations
```CSharp
dotnet tool install -g dotnet-ef --version 7.0.2
```

Após, pode-se limpar o arquivo WeatherForecast.cs.

# Configurar acesso em appsettings.json
No arquivo appsettings.json. Criar uma estrutura semelhante a abaixo, informando seus dados de acesso.
```Json
"ConnectionStrings": {
      "MySql": "server=localhost; database=cadastro; user=usuario; password=senha"
  }
```

**Criamos a implementação da tabela Pessoa em uma classe, com a utilização de entidade base generica para armazenar informação comum a todas as tabelas (nesse caso, a Id).**
```CSharp
public abstract class EntidadeBase
{
    public int Id { get; set; }
}
```
```CSharp
public class Pessoa : EntidadeBase
{
    public string Primeiro_Nome { get; set; }
    public string Sobrenome { get; set; }
    public string Genero { get; set; }
    public string Endereco { get; set; }
}
```

# Usar SQL Bruto.
Para isso as tabelas já devem existir, e a manutenção das mesmas é feita pelo DBA. O sistema vai apenas manipular os DADOS.

*Utilizo uma estrutura generica com interface para criação das configurações das FABRICAS DE CONEXÃO, que serão as responsaveis pelo acesso ao banco. Com a estrutura generica, se consegue criar nova instancias sem precisar modificar o que já existe.

### Criando a interface (para uso na injeção de dependencia).
```CSharp
using System.Data;
public interface IConnectionFactory
{
    IDbConnection Connect();
    void Dispose();
}
```

### Implementando uma classe abstrata que implementa a interface (quem vai realmente implementar as rotinas são as classes concretas (para acesso a MySql, Oracle, etc).
```CSharp
using System.Data;

public abstract class ConnectionFactory : IConnectionFactory
{
    protected readonly IConfiguration configuration;
    protected IDbConnection connection;

    public ConnectionFactory(IConfiguration _configuration)
    {
        configuration = _configuration;
    }

    public abstract IDbConnection Connect();

    public void Dispose()
    {
        connection.Close();
    }
}
```

### Por fim, criando a classe concreta que vai acessar o MySql.
```CSharp
using System.Data;
using MySqlConnector;

public class MySqlFactory : ConnectionFactory
{
    public MySqlFactory(IConfiguration _configuration) : base(_configuration) { }

    public override IDbConnection Connect()
    {
        string connectionString = configuration.GetConnectionString("MySql"); //nome informado em appsetings na ConnectionString
        connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}
```

**Então para testes, criamos pelo MySqlWorkbench, uma tabela e a populamos com um registro.**
```SQL
USE CADASTRO;
CREATE TABLE IF NOT EXISTS Pessoa (
  Id bigint NOT NULL AUTO_INCREMENT,  
  Primeiro_Nome varchar(80) NOT NULL,
  Sobrenome varchar(80) NOT NULL,
  Genero varchar(10) NOT NULL,  
  Endereco varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
);
INSERT INTO PESSOA VALUES (NULL, 'Alexandre', 'Coletti', 'Masc', 'rua qualquer');
```
***Com a implementação dessa forma, se desejarmos criar um processo para conectar em um banco Oracle, só precisariamos criar a classe concreta para tal (no lugar de MySqlConnector).***

## Da mesma forma que criamos uma forma geral para acesso a banco, vamos criar uma 'interface' para os comandos padrões do sistema. 
### De modo a facilitar o consumo da rotina, e permir a injeção de dependencia desses serviços.

*Primeiro criamos a Interface.*
```CSharp
public interface IMyService<T> //tipo generico T, de modo que possamos criar o serviço que processe todas as operacoes
{
    public T buscaPorCodigo(int codigo);
    public T atualizaRegistro(T registro);
    public void deletaRegistro(int codigo);
    public T gravaNovoRegistro(T novoRegistro);
}
```

*Então sua classe abstrata.*
```CSharp
public abstract class MyService<T> : IMyService<T>
{
    protected readonly IConnectionFactory connectionFactory;

    public MyService(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public abstract T atualizaRegistro(T registro);

    public abstract T buscaPorCodigo(int codigo);

    public abstract void deletaRegistro(int codigo);

    public abstract T gravaNovoRegistro(T novoRegistro);
}
```

**Então implementamos as classes de Serviço do repositório, para acesso ao banco.**
```CSharp
using Dapper;
class PessoaService : MyService<Pessoa>
{
    public PessoaService(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override Pessoa atualizaRegistro(Pessoa registro)
    {
        var comando = @"UPDATE PESSOA SET 
                            PRIMEIRO_NOME = @PrimeiroNome, 
                            SOBRENOME = @Sobrenome, 
                            GENERO = @Genero, 
                            ENDERECO = @Endereo
                        WHERE ID = @id;";
        using (var conexao = connectionFactory.Connect())
        {
            conexao.Execute(
                comando,
                new
                {
                    PrimeiroNome = registro.Primeiro_Nome,
                    registro.Sobrenome,
                    registro.Genero,
                    registro.Endereco,
                    registro.Id
                }
            );
        }
        return buscaPorCodigo(registro.Id);
    }

    public override Pessoa buscaPorCodigo(int codigo)
    {
        var comando = @"select Id, Primeiro_Nome, Sobrenome, Genero, Endereco nome from pessoa where id = @codigo";
        using (var conexao = connectionFactory.Connect())
        {
            return conexao.Query<Pessoa>(
                comando,
                new { codigo }
            ).First();
        }
    }

    public override void deletaRegistro(int codigo)
    {
        var comando = @"delete from pessoa where id = @codigo";
        using (var conexao = connectionFactory.Connect())
        {
            conexao.Execute(comando);
        }
    }

    public override void gravaNovoRegistro(Pessoa novoRegistro)
    {
        var comando = @"INSERT INTO PESSOA VALUES (NULL, @PrimeiroNome, @Sobrenome, @Genero, @Endereo);";
        using (var conexao = connectionFactory.Connect())
        {
            conexao.Execute(
                comando,
                new
                {
                    PrimeiroNome = novoRegistro.Primeiro_Nome,
                    novoRegistro.Sobrenome,
                    novoRegistro.Genero,
                    novoRegistro.Endereco
                }
            );
        }
    }
}
```

** Criamos então o controle (Crud basico)
```CSharp
using Microsoft.AspNetCore.Mvc;

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
```

**Por fim, parametrizar as injeções de dependencia em Program.cs**
```CSharp
builder.Services.AddScoped<IConnectionFactory, MySqlFactory>();
builder.Services.AddScoped<IMyService<Pessoa>, PessoaService>();
```
**ambos possuem comentários de qual sua função, cada nova tabela que se desejar criar, será necessário informar um novo builder.Services.AddScoped para registrar
a injeção de dependencia.**

# Para uso do EntityFrameWork

*Primeiro precisamos de uma classe para controle do banco de dados, que herde de DbContext (ou se deseja manipular acesso de usuários, precisa herdar de IdentityDbContext<IdentityUser>, nesse exemplo, uso apenas DbContext).

```CSharp
using Microsoft.EntityFrameworkCore;

namespace IWantApi.Infra.Data;
public class ApplicationDbContext : DbContext
{
    public DbSet<Pessoa> Pessoa { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        /*aqui controlamos propriedades dos campos da tabela*/
        modelBuilder.Entity<Pessoa>()
            .Property(p => p.Primeiro_Nome).IsRequired();
        modelBuilder.Entity<Pessoa>()
            .Property(p => p.Endereco).HasMaxLength(500);
    }

    //!aqui ficam as configurações 'default' dos campos
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>()
            .HaveMaxLength(100); //com isso digo que todos os campos do tipo string terão setados tamanho max de 100 se não for informado o contrário

    }
}
```

## Então vamos criar uma 'interface' para os comandos padrões do sistema. 
### De modo a facilitar o consumo da rotina, e permir a injeção de dependencia desses serviços.
```CSharp
public interface IMyService2<T>
{
    public T buscaPorCodigo(int codigo);
    public T atualizaRegistro(T registro);
    public void deletaRegistro(int codigo);
    public void gravaNovoRegistro(T novoRegistro);

}
```
```CSharp
public abstract class MyService2<T> : IMyService2<T>
{
    protected readonly ApplicationDbContext _context;

    public MyService2(ApplicationDbContext context)
    {
        _context = context;
    }

    public abstract T atualizaRegistro(T registro);

    public abstract T buscaPorCodigo(int codigo);

    public abstract void deletaRegistro(int codigo);

    public abstract void gravaNovoRegistro(T novoRegistro);
}
```

*Então podemos criar nossos serviços*
Criando PessoaService2.cs

```CSharp
class PessoaService2 : MyService2<Pessoa>
{

    public PessoaService2(ApplicationDbContext context) : base(context)
    {
    }

    public override Pessoa atualizaRegistro(Pessoa registro)
    {
        _context.Pessoa.Update(registro);
        _context.SaveChanges();
        return registro;
    }

    public override Pessoa buscaPorCodigo(int codigo)
    {
        return _context.Pessoa.First(p => p.Id == codigo);
    }

    public override void deletaRegistro(int codigo)
    {
        Pessoa pessoa = _context.Pessoa.Where(p => p.Id == codigo).First();
        _context.Pessoa.Remove(pessoa);
        _context.SaveChanges();
    }

    public override void gravaNovoRegistro(Pessoa novoRegistro)
    {
        _context.Pessoa.Add(novoRegistro);
        _context.SaveChanges();
    }
}
```

*Então criamos nosso controlador para os acessos*
```CSharp
using Microsoft.AspNetCore.Mvc;

namespace MeuSistema.Controllers;

[ApiController]
[Route("[controller]")]
public class Pessoa2Controller : ControllerBase
{
    private readonly ILogger<PessoaController> _logger;
    private readonly IMyService2<Pessoa> _pessoaService;

    public Pessoa2Controller(ILogger<PessoaController> logger, IMyService2<Pessoa> pessoaService)
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

```

*Adicionamos então na classe Program as dependencias para injeção do dbcontex e nossa classe de serviço*
```CSharp
string conectionString = builder.Configuration["ConnectionStrings:MySql"];
builder.Services.AddMySql<ApplicationDbContext>(conectionString, ServerVersion.AutoDetect(conectionString));
builder.Services.AddScoped<IMyService2<Pessoa>, PessoaService2>();
```

*E por fim, chamamos a migration para criar a tabela em banco com nossas configurações em ApplicationDbContext*
```CSharp
dotnet ef migrations add 'minhaPrimeiraMigration'
dotnet ef database update --(realiza carga do migration criado pro banco de dados)
```

# Para testes
## Com Primeiro caso
- Para busca com get: https://localhost:7297/Pessoa/id
- Para deletar com delete: https://localhost:7297/Pessoa/id
- Para criar com post: https://localhost:7297/Pessoa (passando um json no body com os campos da pessoa - sem id)
- Para atualizar com put: https://localhost:7297/Pessoa (passando um json no body com os campos da pessoa - com id)

## Com Segundo caso
- Para busca com get: https://localhost:7297/Pessoa2/id
- Para deletar com delete: https://localhost:7297/Pessoa2/id
- Para criar com post: https://localhost:7297/Pessoa2 (passando um json no body com os campos da pessoa - sem id)
- Para atualizar com put: https://localhost:7297/Pessoa2 (passando um json no body com os campos da pessoa - com id)

### Lembrando que é um exemplo simples, não sendo tratado aqui erros caso itens não existam

# Adcionada pasta 'TesteComGenerics' 
*Nela testo a funcionalidade serviço generico para salvar em banco. De modo que precisa apenas de uma implementação para qualquer classe, com a possibildiade de criar uma classe proprio. Ex. classes Livro e Pessoa, somente pessoa tem uma implementação propria no caso de um dos serviços.*
- Fora o que tem na pasta TesteComGenerics, foi criado a Classe Livro em Dominio e implementado na Program.cs
```CShap
builder.Services.AddScoped<IMyGenericService<Pessoa>, PessoaService3>();
builder.Services.AddScoped(typeof(IMyGenericService<>), typeof(MyGenericService<>));
regras especificas
```

