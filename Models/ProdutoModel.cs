using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDM.Models
{
    public class ProdutoModel
    {
        [Key]
        public int IdProduto {get; set;}

        [Required, MaxLength(100)]
        public string Descricao {get; set;}

        [Required]
        public double Valor {get; set;}

        public int IdFornecedor{get; set;}

        [ForeignKey("IdFornecedor")]
        public FornecedorModel Fornecedor {get; set;}

        public int IdNivel{get; set;}

        [ForeignKey("IdNivel")]
        public NivelModel Nivel {get; set;}

        public ICollection<RequisicaoModel> Requisicoes {get; set;}
        
    }
}