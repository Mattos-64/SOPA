using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SOPA.Core.Models;

namespace SOPA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Ferramenta oficial do Identity para gerenciar usuários no banco
        private readonly UserManager<Voluntario> _userManager;

        public AuthController(UserManager<Voluntario> userManager)
        {
            _userManager = userManager;
        }

        // 1. ENDPOINT DE REGISTRO (api/auth/registrar)
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Mapeia o DTO para a nossa entidade real do Banco de Dados
            var novoVoluntario = new Voluntario
            {
                UserName = model.Email, // O Identity usa o UserName como identificador (vamos usar o Email)
                Email = model.Email,
                NomeCompleto = model.NomeCompleto,
                CPF = model.CPF,
                Contato1 = model.Contato1,
                Contato2 = model.Contato2,
                CEP = model.CEP,
                Endereco = model.Endereco,
                TenantId = 1 // Padrão do projeto SOPA por enquanto
            };

            // O CreateAsync já criptografa a senha automaticamente antes de salvar!
            var resultado = await _userManager.CreateAsync(novoVoluntario, model.Senha);

            if (resultado.Succeeded)
            {
                return Ok(new { Mensagem = "Voluntário registrado com sucesso!" });
            }

            // Se der erro (ex: senha fraca ou email já existe), devolve a lista de erros para o front
            return BadRequest(resultado.Errors);
        }

        // 2. ENDPOINT DE LOGIN (api/auth/login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Busca o usuário pelo e-mail enviado
            var voluntario = await _userManager.FindByEmailAsync(model.Email);

            // Verifica se o usuário existe e se a senha está correta
            if (voluntario != null && await _userManager.CheckPasswordAsync(voluntario, model.Senha))
            {
                // Se deu certo, gera o Token JWT passando os dados dele escondidos dentro
                var token = GerarTokenJwt(voluntario);

                return Ok(new LoginRespostaDto
                {
                    Sucesso = true,
                    Token = token,
                    Mensagem = "Login efetuado com sucesso!"
                });
            }

            // Se falhar, retorna erro de não autorizado de forma genérica por segurança
            return Unauthorized(new LoginRespostaDto
            {
                Sucesso = false,
                Mensagem = "E-mail ou senha inválidos."
            });
        }

        // MÉTODO AUXILIAR: Constrói o Token JWT criptografado
        private string GerarTokenJwt(Voluntario voluntario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // ATENÇÃO: Essa chave DEVE ser exatamente igual à chave que colocamos no Program.cs!
            var chaveSecreta = "SOPA_Chave_Secreta_Muito_Longa_E_Segura_Com_Mais_De_32_Caracteres_2026";
            var key = Encoding.ASCII.GetBytes(chaveSecreta);

            // As "Claims" são as informações que vão carimbadas dentro do QR Code do Token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, voluntario.Id.ToString()),
                    new Claim(ClaimTypes.Name, voluntario.NomeCompleto),
                    new Claim(ClaimTypes.Email, voluntario.Email ?? ""),
                    new Claim("TenantId", voluntario.TenantId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4), // O Token expira sozinho em 4 horas
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}