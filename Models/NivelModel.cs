using System.ComponentModel.DataAnnotations;

namespace IDM.Models
{
    public class NivelModel
    {
        [Key]
        public int IdNivel{get; set;}

        [Required]
        public int Classificacao{get; set;}

        [Required]
        public string Descricao{get; set;}
    }
}