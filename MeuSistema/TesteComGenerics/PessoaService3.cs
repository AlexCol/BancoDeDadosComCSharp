class PessoaService3 : MyGenericService<Pessoa>
{
    public PessoaService3(ApplicationDbContext context) : base(context)
    { }
    public override Pessoa buscaPorCodigo(int codigo)
    {
        Pessoa pessoa = _context.Pessoa.First(p => p.Id == codigo);
        pessoa.Primeiro_Nome = "passou pela proria";
        return pessoa;
    }
}