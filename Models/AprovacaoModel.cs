using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDM.Models
{
    public class AprovacaoModel
    {
        [Key]
        public int IdAprovacao { get; set; }

        public int NumerodeAprovacao { get; set; }

        public bool isAprovado { get; set; }

        public int IdRequisicao {get; set;}

        [ForeignKey("IdRequisicao")]
        public RequisicaoModel Requisicao {get; set;}

        public ICollection<ColaboradorModel> Coordenador { get; set;}



    }
}