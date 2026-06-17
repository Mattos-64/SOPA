using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SOPA.Core.Models;
using SOPA.Api.Data;


var builder = WebApplication.CreateBuilder(args);

// Regista o contexto da base de dados usando a Connection String do appsettings
builder.Services.AddDbContext<SOPA.Api.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<Voluntario, IdentityRole<int>>(options =>
{
    // Aqui você pode customizar as regras de senha se quiser facilitar os testes
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false; // Desativado para facilitar
    options.Password.RequireNonAlphanumeric = false; // Desativado para facilitar
    options.Password.RequiredLength = 6; // Mínimo de 6 caracteres
    options.User.RequireUniqueEmail = true; // Garante que não haverá emails duplicados
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 3. CONFIGURAÇÃO DO JWT: Define como a API vai validar o token gerado
// IMPORTANTE: Use uma chave secreta longa e segura. Guardaremos uma string direto aqui para facilitar o ambiente de desenvolvimento.
var chaveSecreta = "SOPA_Chave_Secreta_Muito_Longa_E_Segura_Com_Mais_De_32_Caracteres_2026";
var key = Encoding.ASCII.GetBytes(chaveSecreta);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Define como true em produção
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Pode validar o domínio emissor se preferir
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Mantém o seu CORS que configuramos no passo anterior
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorCorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7096", "http://localhost:5071")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// 4. ATIVAÇÃO DOS MIDDLEWARES NA ORDEM CORRETA (A ordem aqui importa muito!)
app.UseCors("BlazorCorsPolicy");

app.UseHttpsRedirection();

// CRUCIAL: Adicione o Authentication ANTES do Authorization
app.UseAuthentication(); // <-- ADICIONE ESTA LINHA AQUI
app.UseAuthorization();

app.MapControllers();

app.Run();