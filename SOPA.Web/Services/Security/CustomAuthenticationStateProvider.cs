using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace SOPA.Web.Security
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        // Esse método é o que o Blazor chama automaticamente para saber se a pessoa está logada
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                LimparCabecalhoAutenticacao();
                return new AuthenticationState(_anonymous);
            }

            // Insere o token de forma automática em todas as requisições HTTP que fizermos
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        // Método chamado quando o usuário faz login com sucesso
        public async Task MarcarUsuarioComoAutenticado(string token)
        {
            await _localStorage.SetItemAsync("authToken", token);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
        }

        // Método chamado no Logout
        public async Task MarcarUsuarioComoDeslogado()
        {
            await _localStorage.RemoveItemAsync("authToken");
            LimparCabecalhoAutenticacao();

            var authState = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(authState);
        }

        private void LimparCabecalhoAutenticacao()
        {
            if (_http.DefaultRequestHeaders.Authorization != null)
            {
                _http.DefaultRequestHeaders.Authorization = null;
            }
        }

        // Função auxiliar que quebra o Token JWT para ler o Nome e ID do usuário logado
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
                }
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}