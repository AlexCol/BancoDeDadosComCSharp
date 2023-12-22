public abstract class MyService<T> : IMyService<T>
{
    protected readonly IConnectionFactory connectionFactory;
    protected readonly ApplicationDbContext _context;

    public MyService(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public abstract T atualizaRegistro(T registro);

    public abstract T buscaPorCodigo(int codigo);

    public abstract void deletaRegistro(int codigo);

    public abstract void gravaNovoRegistro(T novoRegistro);
}