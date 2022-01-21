using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDM.Models{

    public class RequisicaoModel
    {
        [Key]
        public int IdRequisicao {get; set;}

        public int Quantidade {get; set;}

        public DateTime? DataRequisicao {get;}

        public string ProdutoName {get; set;}

        public string ColabName {get; set;}

        public double ValorTotal {get; set;}

        public Estado EstadoReq {get; set;}

        public int IdColaborador {get; set;}

        [ForeignKey("IdColaborador")]
        public ColaboradorModel Colaborador {get; set;}

        public int IdProduto {get; set;}

        [ForeignKey("IdProduto")]
        public ProdutoModel Produto {get; set;}

        public ICollection<AprovacaoModel> Aprovacoes {get; set;}
        

    }
    public enum Estado
    {
        Pendente, Aprovado, Rejeitado, Sem_Estoque
    }
}
