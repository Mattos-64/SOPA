using System.ComponentModel.DataAnnotations;

namespace SOPA.Core.Models
{
    public class Animal : BaseEntity
    {
        [Required(ErrorMessage = " O nome do animal é obrigatório !! ")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = " O nome do animal deve ter entre 2 e 100 caracteres ")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A espécie é obrigatória.")]
        public string Especie { get; set; } = string.Empty;

        [Required(ErrorMessage = "O porte é obrigatório.")]
        public string Porte { get; set; } = string.Empty;

        public int IdadeAproximada { get; set; }

        [Required(ErrorMessage = "O status do animal é obrigatório.")]
        public string Status { get; set; } = "Disponível para Adoção";

        public string? FotoUrl { get; set; }

        public string? Observacoes { get; set; }

        public int VoluntarioID { get; set; }

        public string? CadastradoPorNome { get; set; }
    }
}

