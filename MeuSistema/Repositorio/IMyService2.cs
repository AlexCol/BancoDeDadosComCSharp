public interface IMyService2<T>
{
    public T buscaPorCodigo(int codigo);
    public T atualizaRegistro(T registro);
    public void deletaRegistro(int codigo);
    public void gravaNovoRegistro(T novoRegistro);

}