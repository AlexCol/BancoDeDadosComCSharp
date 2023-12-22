public interface IMyGenericService<T> where T : EntidadeBase
{
    public T buscaPorCodigo(int codigo);
    public T atualizaRegistro(T registro);
    public void deletaRegistro(int codigo);
    public void gravaNovoRegistro(T novoRegistro);

}