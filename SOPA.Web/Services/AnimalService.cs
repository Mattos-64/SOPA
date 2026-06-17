using System.Net.Http.Json;
using SOPA.Core.Models;

namespace SOPA.Web.Services;

public class AnimalService
{
    private readonly HttpClient _http;

    public AnimalService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Animal>> ObterAnimaisAsync()
    {
        return await _http.GetFromJsonAsync<List<Animal>>("api/Animais") ?? new List<Animal>();
    }

    public async Task CriarAnimalAsync(Animal animal)
    {
        await _http.PostAsJsonAsync("api/Animais", animal);
    }

    public async Task AtualizarAnimalAsync(int id, Animal animal)
    {
        await _http.PutAsJsonAsync($"api/Animais/{id}", animal);
    }

    public async Task DeletarAnimalAsync(int id)
    {
        await _http.DeleteAsync($"api/Animais/{id}");
    }
}