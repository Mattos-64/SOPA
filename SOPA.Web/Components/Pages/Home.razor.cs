using Microsoft.AspNetCore.Components;

namespace SOPA.Web.Pages
{
    public class HomeBase : ComponentBase
    {
        // Dados simulados para o cliente ver o visual com informações reais
        protected int TotalAnimais { get; set; } = 12; // Exemplo de contagem
        protected int TotalVoluntarios { get; set; } = 5;
        protected decimal SaldoCaixa { get; set; } = 1450.75m;

        protected override void OnInitialized()
        {
            // No futuro, buscaremos esses totais combinando as APIs
        }
    }
}