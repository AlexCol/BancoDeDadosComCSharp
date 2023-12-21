
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
                            Endereco = @Endereco
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
        var comando = @"select Id, Primeiro_Nome, Sobrenome, Genero, Endereco from pessoa where id = @codigo";
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
            conexao.Execute(comando, new { codigo });
        }
    }

    public override void gravaNovoRegistro(Pessoa novoRegistro)
    {
        var comando = @"INSERT INTO PESSOA VALUES (NULL, @PrimeiroNome, @Sobrenome, @Genero, @Endereco);";
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