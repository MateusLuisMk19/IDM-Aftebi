using System.ComponentModel.DataAnnotations;

namespace IDM.Models
{
    public class FornecedorModel
    {
        [Key]
        public int IdFornecedor {get; set;}

        [Required, MaxLength(50)]
        public string Nome {get; set;}

        [Required, MaxLength(100)]
        public string Email {get; set;}

        public string Telefone {get; set;}

        [Required, MaxLength(150)]
        public string Endereco {get; set;}
        
    }
}
