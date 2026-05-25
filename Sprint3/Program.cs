using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Repositories;
using Sprint3.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Conexão e DI
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<JwtService>();

// 2. Configurar Autenticação JWT (com chave do appsettings)
var jwtKey = builder.Configuration["JwtSettings:SecretKey"] 
             ?? throw new InvalidOperationException("A configuração 'JwtSettings:SecretKey' não foi encontrada. Configure-a no UserSecrets ou Variáveis de Ambiente.");

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 3. CORS
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => {
        policy.WithOrigins(allowedOrigins!) // Use the list from config
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials(); 
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Aplicar migrations ANTES de rodar (MELHORADO COM LOGS E PROTEÇÃO)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Console.WriteLine("--> Iniciando verificação de migrações pendentes...");
        var dbContext = services.GetRequiredService<AppDbContext>();
        
        // Executa a migração no Railway de forma segura
        dbContext.Database.Migrate();
        Console.WriteLine("--> Banco de dados atualizado com sucesso no Railway!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> ERRO CRÍTICO nas migrações automáticas: {ex.Message}");
    }
}

// 5. Middlewares (ORDEM CORRETA É CRÍTICA!)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
});

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

app.UseAuthentication(); // PRIMEIRO autenticação
app.UseAuthorization();  // DEPOIS autorização

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();