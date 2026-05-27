# Projeto Full-Stack Unificado: API .NET e Frontend Angular

Este projeto apresenta uma solução full-stack completa, onde o frontend desenvolvido em **Angular** está integrado ao backend, uma API robusta construída em **.NET**. Essa abordagem unificada simplifica a implantação e o gerenciamento, permitindo uma aplicação web moderna e eficiente.

## 📋 Pré-requisitos

Para configurar e executar este projeto em sua máquina local, certifique-se de ter os seguintes softwares instalados:

*   **[.NET SDK](https://dotnet.microsoft.com/download)**: Necessário para compilar e executar a API .NET.
*   **[Node.js](https://nodejs.org/en/download/)**: Essencial para o desenvolvimento do frontend Angular. Embora o frontend seja servido pelo backend em produção, o Node.js é necessário para construir o projeto Angular localmente.
*   **[MySQL Server](https://dev.mysql.com/downloads/mysql/)**: O banco de dados utilizado para persistência de dados da aplicação.

## 🚀 Repositório do Projeto

O projeto unificado está disponível no seguinte repositório:

*   **Repositório Principal:** [https://github.com/gabrielllbob/Sprint3](https://github.com/gabrielllbob/Sprint3)

    > **Nota:** O frontend Angular está agora integrado ao projeto .NET, residindo no diretório `wwwroot` do backend após a construção.

## ⚙️ Configuração e Execução Local

### Configuração do Banco de Dados

Por padrão, a API está configurada para se conectar a um servidor MySQL local na porta padrão. As credenciais e o nome do banco de dados são definidos via `user-secrets`.

### Passos para Iniciar o Projeto Localmente

1.  **Instalar a ferramenta `dotnet-ef` (se ainda não tiver):**

    ```bash
    dotnet tool install --global dotnet-ef
    ```

2.  **Configurar as variáveis de ambiente para o JWT e a string de conexão do banco de dados:**

    ```bash
    dotnet user-secrets set "JwtSettings:SecretKey" "GabrielLopesLimaPrecisaDeUmaBoaOportunidadeDeEmprego"
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=bancofraude;User=root;Password=cimatec;"
    ```

    > **Nota:** A `SecretKey` do JWT é crucial para a segurança da autenticação. Certifique-se de mantê-la segura e, em ambientes de produção, utilize um método mais robusto para gerenciamento de segredos.

3.  **Aplicar as migrações do banco de dados:**

    ```bash
    dotnet ef database update
    ```

4.  **Construir o Frontend Angular e publicá-lo no `wwwroot` (se ainda não estiver lá):**

    Navegue até o diretório do frontend (assumindo que está em uma subpasta, por exemplo, `Frontend/`) e execute:

    ```bash
    cd Frontend/
    npm install
    npx ng build --configuration production
    # Copie os arquivos gerados (geralmente em 'dist/frontend-name') para o diretório 'wwwroot' do projeto .NET
    # Exemplo (ajuste os caminhos conforme a estrutura do seu projeto):
    # cp -r dist/frontend-name/* ../wwwroot/
    cd ..
    ```

    > **Importante:** Certifique-se de que os arquivos estáticos do Angular estejam no diretório `wwwroot` do projeto .NET para que a aplicação seja servida corretamente.

5.  **Executar a Aplicação Full-Stack:**

    ```bash
    dotnet run --launch-profile https
    ```

    A aplicação estará disponível em `https://localhost:7151`.

### Usuário Administrador Padrão

Após a execução das migrações do banco de dados, um usuário administrador padrão é criado automaticamente. Utilize as seguintes credenciais para o primeiro acesso:

*   **CPF/Login:** `00000000000`
*   **Senha:** `123456`

    > **Atenção:** Por questões de segurança, é altamente recomendável alterar a senha deste usuário após o primeiro login em um ambiente de produção.

### Documentação da API (Swagger)

Após iniciar a aplicação, você pode acessar a documentação interativa do Swagger nos seguintes endereços:

*   [http://localhost:5168/swagger/index.html](http://localhost:5168/swagger/index.html)
*   [https://localhost:7151/swagger/index.html](https://localhost:7151/swagger/index.html)

## ☁️ Hospedagem e Implantação (Render)

Este projeto está configurado para ser implantado na plataforma [Render](https://render.com/). O Render simplifica a hospedagem de aplicações full-stack, automatizando o processo de build e deploy.

### Link da Aplicação Publicada

*   **Aplicação Online:** [https://projetofinalbackend-z0no.onrender.com/](https://projetofinalbackend-z0no.onrender.com/)

*   **Swagger Online:** [https://projetofinalbackend-z0no.onrender.com/swagger/](https://projetofinalbackend-z0no.onrender.com/swagger/)

---
