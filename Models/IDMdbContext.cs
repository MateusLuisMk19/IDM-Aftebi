using IDM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IDM.Models
{
    public class IDMdbContext : IdentityDbContext<ColaboradorModel, IdentityRole<int>, int>
    {
        public DbSet<FornecedorModel> Fornecedores { get; set; }
        public DbSet<NivelModel> Niveis { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<EstoqueModel> Estoques { get; set; }
        public DbSet<RequisicaoModel> Requisicoes { get; set; }
        public DbSet<ColaboradorModel> Colaboradores { get; set; }
        public DbSet<AprovacaoModel> Aprovacoes { get; set; }

        public IDMdbContext(DbContextOptions<IDMdbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FornecedorModel>().ToTable("Fornecedor");
            modelBuilder.Entity<NivelModel>().ToTable("Nivel");

            modelBuilder.Entity<ProdutoModel>().ToTable("Produto");
            modelBuilder.Entity<EstoqueModel>().ToTable("Estoque");
            modelBuilder.Entity<EstoqueModel>().Property(e => e.DtEntrada).HasDefaultValueSql("datetime('now')")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<ColaboradorModel>().ToTable("Colaborador");

            modelBuilder.Entity<RequisicaoModel>().ToTable("Requisicao");
            modelBuilder.Entity<RequisicaoModel>().Property(e => e.DataRequisicao).HasDefaultValueSql("datetime('now')")
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<AprovacaoModel>().ToTable("Aprovacao");

        }
    }
}