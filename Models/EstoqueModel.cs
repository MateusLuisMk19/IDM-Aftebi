using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDM.Models
{
    public class EstoqueModel
    {
        [Key]
        public int IdEstoque {get; set;}

        [Required]
        public int Quantidade {get; set;}

        [ReadOnly(true)]
        public DateTime? DtEntrada{get;}
        

        public int IdProduto{get; set;}

        [ForeignKey("IdProduto")]
        public ProdutoModel Produto {get; set;}

        
    }
}