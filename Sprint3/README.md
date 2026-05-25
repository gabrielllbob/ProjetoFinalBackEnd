# Projeto Full-Stack: API .NET e Frontend Angular

Este projeto apresenta uma solução full-stack completa, composta por uma API robusta desenvolvida em **.NET** e um frontend dinâmico construído com **Angular**. A integração entre as duas partes permite uma aplicação web moderna e eficiente.

## 📋 Pré-requisitos

Para configurar e executar este projeto em sua máquina local, certifique-se de ter os seguintes softwares instalados:

*   **[.NET SDK](https://dotnet.microsoft.com/download)**: Necessário para compilar e executar a API .NET.
*   **[Node.js](https://nodejs.org/en/download/)**: Essencial para o desenvolvimento e execução do frontend Angular, incluindo o gerenciador de pacotes `npm`.
*   **[MySQL Server](https://dev.mysql.com/downloads/mysql/)**: O banco de dados utilizado para persistência de dados da aplicação.

## 🚀 Repositórios do Projeto

O projeto está dividido em dois repositórios distintos para a API e o Frontend:

*   **API (.NET)**: [https://github.com/gabrielllbob/Sprint3](https://github.com/gabrielllbob/Sprint3)
*   **Frontend (Angular)**: [https://github.com/gabrielllbob/Sprint3_front](https://github.com/gabrielllbob/Sprint3_front)

## ⚙️ Configuração e Execução da API (.NET)

### Configuração do Banco de Dados

Por padrão, a API está configurada para se conectar a um servidor MySQL local na porta padrão. As credenciais e o nome do banco de dados são definidos via `user-secrets`.

### Passos para Iniciar a API

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

4.  **Executar a API:**

    ```bash
    dotnet run --launch-profile https
    ```

    A API estará disponível em `https://localhost:7151`.

### Usuário Administrador Padrão

Após a execução das migrações do banco de dados, um usuário administrador padrão é criado automaticamente. Utilize as seguintes credenciais para o primeiro acesso:

*   **CPF/Login:** `00000000000`
*   **Senha:** `123456`

    > **Atenção:** Por questões de segurança, é altamente recomendável alterar a senha deste usuário após o primeiro login em um ambiente de produção.

### Documentação da API (Swagger)

Após iniciar a API, você pode acessar a documentação interativa do Swagger nos seguintes endereços:

*   [http://localhost:5168/swagger/index.html](http://localhost:5168/swagger/index.html)
*   [https://localhost:7151/swagger/index.html](https://localhost:7151/swagger/index.html)

## 🌐 Configuração e Execução do Frontend (Angular)

### Passos para Iniciar o Frontend

1.  **Instalar as dependências do projeto:**

    Navegue até o diretório do frontend e execute:

    ```bash
    npm install
    ```

2.  **Executar o servidor de desenvolvimento e abrir no navegador:**

    ```bash
    npx ng serve --open
    ```

    O aplicativo Angular será compilado e aberto automaticamente em seu navegador padrão, geralmente em `http://localhost:4200`.

## ☁️ Versão Publicada do Frontend

Uma versão do frontend também está disponível online, acessível através do seguinte link:

*   [https://sprint3-front-beta.vercel.app/](https://sprint3-front-beta.vercel.app/)

---
---

# Endpoints da API

## Auth

### Login
```http
POST /api/Auth/login
```

Realiza a autenticação do usuário e retorna um token JWT.

---

## Clientes

### Listar Clientes
```http
GET /api/Clientes
```

Retorna todos os clientes cadastrados.

### Criar Cliente
```http
POST /api/Clientes
```

Cria um novo cliente.

### Atualizar Cliente
```http
PUT /api/Clientes
```

Atualiza os dados de um cliente existente.

### Buscar Cliente por ID
```http
GET /api/Clientes/{id}
```

Retorna os dados de um cliente específico.

### Remover Cliente
```http
DELETE /api/Clientes/{id}
```

Remove um cliente do sistema.

---

## Contas

### Buscar Contas por CPF
```http
GET /api/Contas/minhas-contas/{cpf}
```

Retorna todas as contas vinculadas ao CPF informado.

### Criar Conta
```http
POST /api/Contas/criar
```

Cria uma nova conta bancária.

---

## Cartões

### Criar Cartão
```http
POST /api/Cartoes/criar
```

Realiza a criação de um novo cartão.

---

## Transações

### Realizar Transação
```http
POST /api/Transacoes/realizar
```

Executa uma transação bancária entre contas.

### Extrato da Conta
```http
GET /api/Transacoes/extrato/{contaId}
```

Retorna o extrato de transações da conta informada.