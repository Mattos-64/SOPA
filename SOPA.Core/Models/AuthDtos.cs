using System.ComponentModel.DataAnnotations;

namespace SOPA.Core.Models; // Ajuste para o seu namespace

// Dados necessários para criar uma conta de Voluntário
public class RegistroDto
{
    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    public string CPF { get; set; } = string.Empty;

    [Required(ErrorMessage = "O contato é obrigatório.")]
    public string Contato1 { get; set; } = string.Empty;
    public string Contato2 { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

// Dados necessários para fazer o Login
public class LoginDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}

// O que a API vai devolver para o Blazor quando o login der certo
public class LoginRespostaDto
{
    public bool Sucesso { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
}