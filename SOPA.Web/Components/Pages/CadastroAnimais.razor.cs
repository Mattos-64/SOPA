using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SOPA.Core.Models;
using SOPA.Web.Services;

namespace SOPA.Web.Components.Pages;

public partial class CadastroAnimais : ComponentBase
{
    [Inject]
    protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    [Inject]
    public AnimalService AnimalService { get; set; } = default!;

    [Inject]
    public IWebAssemblyHostEnvironment Env { get; set; } = default!;

    private Animal novoAnimal = new();

    private List<Animal> animais = new();

    private bool exibirModal = false;

    private string especieSelecionada = "";

    private string especieEspecifica = "";

    protected override async Task OnInitializedAsync()
    {
        await AtualizarLista();
    }

    private void AbrirModalCadastro()
    {
        novoAnimal = new();
        especieSelecionada = "";
        especieEspecifica = "";
        exibirModal = true;
    }

    private void FecharModal()
    {
        exibirModal = false;
    }

    private async Task AtualizarLista()
    {
        animais = await AnimalService.ObterAnimaisAsync();
    }

    private async Task SalvarAnimal(EditContext editContext)
    {
        Console.WriteLine("=== BATEU NO MÉTODO SALVAR ===");

        if (especieSelecionada == "Outros")
        {
            novoAnimal.Especie = especieEspecifica;
        }
        else
        {
            novoAnimal.Especie = especieSelecionada;
        }

        bool formularioValido = editContext.Validate();
        Console.WriteLine($"Formulário está válido? {formularioValido}");

        if (!formularioValido)
        {
            Console.WriteLine("Bloqueado na validação! Verifique os campos obrigatórios.");
            return;
        }
        try
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var usuarioLogado = authState.User;

            if (usuarioLogado.Identity != null && usuarioLogado.Identity.IsAuthenticated)
            {
                var claimId = usuarioLogado.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (claimId != null && int.TryParse(claimId.Value, out int voluntarioId))
                {
                    novoAnimal.VoluntarioID = voluntarioId;
                }

                var claimTenant = usuarioLogado.FindFirst("TenantId");
                if (claimTenant != null && int.TryParse(claimTenant.Value, out int tenantId))
                {
                    novoAnimal.TenantId = tenantId;
                }
            }
            else
            {
                Console.WriteLine("Aviso: Tentativa de salvamento por usuário não autenticado.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao extrair dados do voluntário logado: {ex.Message}");
        }

        Console.WriteLine("Passou na validação! Enviando para o Service...");

        if (novoAnimal.Id == 0)
        {
            await AnimalService.CriarAnimalAsync(novoAnimal);
        }
        else
        {
            await AnimalService.AtualizarAnimalAsync(novoAnimal.Id, novoAnimal);
        }

        novoAnimal = new();
        especieSelecionada = "";
        especieEspecifica = "";
        exibirModal = false;
        await AtualizarLista();
    }

    private void EditarAnimal(Animal animal)
    {
        novoAnimal = new Animal
        {
            Id = animal.Id,
            Nome = animal.Nome,
            Especie = animal.Especie,
            Porte = animal.Porte,
            IdadeAproximada = animal.IdadeAproximada,
            Status = animal.Status,
            FotoUrl = animal.FotoUrl,
            Observacoes = animal.Observacoes,
            VoluntarioID = animal.VoluntarioID, 
            TenantId = animal.TenantId
        };

        if (animal.Especie == "Cachorro" || animal.Especie == "Gato" || animal.Especie == "Pássaro")
        {
            especieSelecionada = animal.Especie;
            especieEspecifica = "";
        }
        else
        {
            especieSelecionada = "Outros";
            especieEspecifica = animal.Especie;
        }

        exibirModal = true;
    }

    private async Task ExcluirAnimal(int id)
    {
        await AnimalService.DeletarAnimalAsync(id);
        await AtualizarLista();
    }
}