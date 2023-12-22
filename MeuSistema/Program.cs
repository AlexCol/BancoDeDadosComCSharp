using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
Nesta linha, você está registrando um serviço no contêiner de injeção de dependência. 
    O método AddScoped significa que uma única instância do serviço será criada para cada escopo de solicitação. 
    Em um contexto web, isso geralmente significa que uma instância é criada para cada solicitação HTTP.
IConnectionFactory é uma interface que representa uma fábrica de conexões. 
    Esta interface provavelmente tem métodos para criar e gerenciar conexões com um banco de dados.
MySqlFactory é uma implementação concreta de IConnectionFactory. 
    Isso significa que, sempre que algo solicitar uma instância de IConnectionFactory, o contêiner de injeção de dependência fornecerá uma instância de MySqlFactory.
*/
builder.Services.AddScoped<IConnectionFactory, MySqlFactory>();

/*
Aqui, você está registrando outro serviço no contêiner de injeção de dependência.
IMyService<Pessoa> é uma interface genérica que provavelmente representa um serviço para realizar operações específicas relacionadas a entidades do tipo Pessoa. 
    Isso pode incluir operações de CRUD (criação, leitura, atualização e exclusão).
PessoaService é uma implementação concreta de IMyService<Pessoa>. 
    Isso significa que, sempre que algo solicitar uma instância de IMyService<Pessoa>, o contêiner de injeção de dependência fornecerá uma instância de PessoaService.
    A injeção de dependência garantirá que, quando o ASP.NET Core precisar de um serviço IMyService<Pessoa>, 
    ele criará automaticamente uma instância de PessoaService e resolverá suas dependências, incluindo IConnectionFactory. 
    Assim, cada vez que você usar IMyService<Pessoa>, obterá uma instância de PessoaService que foi configurada para usar MySqlFactory para suas 
    operações de banco de dados.
*/
builder.Services.AddScoped<IMyService<Pessoa>, PessoaService>();


string conectionString = builder.Configuration["ConnectionStrings:MySql"];
builder.Services.AddMySql<ApplicationDbContext>(conectionString, ServerVersion.AutoDetect(conectionString));
builder.Services.AddScoped<IMyService2<Pessoa>, PessoaService2>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
