using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SOPA.Core.Models
{
   public class Voluntario : IdentityUser<int>
    {
        [Required]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required]
        public string CPF { get; set; } = string.Empty;

        [Required]
        public string Contato1 {  get; set; } = string.Empty;

        public string Contato2 { get; set; } = string.Empty;

        public string CEP {  get; set; } = string.Empty;

        public string Endereco {  get; set; } = string.Empty;

        public int TenantId { get; set; } = 1;
    }
}
