using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOPA.Core.Models
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Uso interno do banco (Performance)

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PublicId { get; set; } = Guid.NewGuid(); // Uso externo/URLs (Segurança)

        [Required]
        public int TenantId { get; set; } = 1; // Controle Multi-tenant para o futuro

        [Required]
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}