using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Pessoa> Pessoa { get; set; }
    public DbSet<Livro> Livro { get; set; }
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