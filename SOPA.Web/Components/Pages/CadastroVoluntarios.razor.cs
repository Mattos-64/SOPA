using Microsoft.AspNetCore.Components;
using SOPA.Core.Models;
using System.Net.Http.Json;

namespace SOPA.Web.Components.Pages
{
    public partial class CadastroVoluntarios : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        protected RegistroDto RegistroModel { get; set; } = new();
        protected string MensagemErro { get; set; } = string.Empty;
        protected string MensagemSucesso { get; set; } = string.Empty;
        protected bool IsCarregando { get; set; } = false;

        protected async Task HandleCadastro()
        {
            IsCarregando = true;
            MensagemErro = string.Empty;
            MensagemSucesso = string.Empty;

            try
            {
                var response = await Http.PostAsJsonAsync("api/auth/registrar", RegistroModel);

                if (response.IsSuccessStatusCode)
                {
                    MensagemSucesso = "Cadastro realizado! Redirecionando para o login...";
                    StateHasChanged();
                    await Task.Delay(2000);
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    var resultado = await response.Content.ReadFromJsonAsync<dynamic>();
                    MensagemErro = resultado?.mensagem ?? "Falha ao cadastrar. Verifique as regras de senha.";
                }
            }
            catch (Exception ex)
            {
                MensagemErro = $"Erro de conexão: {ex.Message}";
            }
            finally
            {
                IsCarregando = false;
            }
        }
    }
}