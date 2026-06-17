using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SOPA.Core.Models;
using SOPA.Web.Security;

namespace SOPA.Web.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        protected LoginDto LoginModel { get; set; } = new();
        protected string MensagemErro { get; set; } = string.Empty;
        protected bool IsCarregando { get; set; } = false;

        protected async Task HandleLogin()
        {
            IsCarregando = true;
            MensagemErro = string.Empty;

            try
            {
                var response = await Http.PostAsJsonAsync("api/auth/login", LoginModel);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<LoginRespostaDto>();

                    if (resultado != null && !string.IsNullOrEmpty(resultado.Token))
                    {
                        var customAuthStateProvider = (CustomAuthenticationStateProvider)AuthStateProvider;
                        await customAuthStateProvider.MarcarUsuarioComoAutenticado(resultado.Token);

                        Navigation.NavigateTo("/animais");
                    }
                }
                else
                {
                    MensagemErro = "Falha na autenticação. Verifique os dados.";
                }
            }
            catch (Exception ex)
            {
                MensagemErro = $"Erro ao conectar com o servidor: {ex.Message}";
            }
            finally
            {
                IsCarregando = false;
            }
        }
    }
}