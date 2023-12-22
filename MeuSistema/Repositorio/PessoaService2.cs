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