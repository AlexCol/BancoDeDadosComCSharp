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