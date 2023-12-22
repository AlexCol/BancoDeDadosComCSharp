using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

public class MyGenericService<T> : IMyGenericService<T> where T : EntidadeBase
{

    protected readonly ApplicationDbContext _context;
    private DbSet<T> dataset;
    public MyGenericService(ApplicationDbContext context)
    {
        _context = context;
        dataset = context.Set<T>();
    }

    public T atualizaRegistro(T registro)
    {
        dataset.Update(registro);
        _context.SaveChanges();
        return registro;
    }

    public virtual T buscaPorCodigo(int codigo)
    {
        return dataset.First(p => p.Id == codigo);
    }

    public void deletaRegistro(int codigo)
    {
        T item = dataset.Where(p => p.Id == codigo).First();
        dataset.Remove(item);
        _context.SaveChanges();
    }

    public void gravaNovoRegistro(T novoRegistro)
    {
        dataset.Add(novoRegistro);
        _context.SaveChanges();
    }
}