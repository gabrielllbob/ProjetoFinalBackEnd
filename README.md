# Sprint3
Este projeto consiste em uma solução full-stack, com uma API construída em .NET e um frontend desenvolvido em Angular.

📋 Pré-requisitos
Certifique-se de ter instalado em sua máquina:

.NET SDK

Node.js (para o Angular)

MySQL Server

⚙️ Configuração do Banco de Dados
Por padrão, a API está configurada para conectar a um servidor MySQL local na porta padrão.

Caso seja necessário alterar as credenciais de conexão ao banco de dados:

Acesse o arquivo appsettings.json localizado na pasta do projeto backend.

Localize a seção de ConnectionStrings.

Altere o endereço, usuário e senha conforme o seu ambiente.

🚀 Como Rodar o Projeto
Backend (API)
Após configurar o banco de dados, navegue até a pasta da API e execute os comandos abaixo para preparar o banco e iniciar o servidor:

Bash
# Aplica as migrações ao banco de dados
dotnet ef database update

# Inicia a API
dotnet run
Frontend (Angular)
Navegue até a pasta do frontend e execute o comando para iniciar a aplicação:

Bash
# Inicia o servidor de desenvolvimento e abre o navegador
npx ng serve --open
🌐 Deploy
Status: O deploy/conexão via Vercel está em andamento (Work In Progress - WIP).

🛠 Tecnologias
Backend: .NET (C#), Entity Framework Core, MySQL.

Frontend: Angular, TypeScript.
