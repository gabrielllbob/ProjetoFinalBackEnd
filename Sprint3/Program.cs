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
builder.Services.AddScoped<JwtService>(); // 👈 Não esqueça de registrar seu novo serviço

// 2. Configurar Autenticação JWT (OBRIGATÓRIO ANTES DO BUILD)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SUA_CHAVE_SECRETA_SUPER_LONGA_DE_PELO_MENOS_32_CARACTERES")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 3. CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Middlewares (ORDEM IMPORTANTE!)
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");

app.UseAuthentication(); // 👈 Valida o token
app.UseAuthorization();  // 👈 Restringe o acesso
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Sprint3.Data.AppDbContext>();
    dbContext.Database.Migrate(); // Cria o banco e as tabelas automaticamente
}

app.Run();