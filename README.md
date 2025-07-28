# RH_Backend

Sistema de Recursos Humanos (RH) responsável por gerenciar funcionários, cargos, férias e histórico de alterações.

---

## ✅ Pré-Requisitos

Antes de rodar o sistema, certifique-se de que os seguintes componentes estão instalados:

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server(Versão Developer)](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)
- [Entity Framework Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet) (já incluído com o .NET SDK)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou outro editor compatível (ex: VS Code)

---

## 🚀 Instruções Para Rodar O Sistema Localmente

1. **Clone o repositório:**

   ```bash
   git clone https://github.com/GBRielGCLC/RH.git
   cd rh_backend

2. **Configure o `appsettings.json`:**

   Edite o arquivo `appsettings.Development.json` e atualize a `DefaultConnection` (caso necessário) com sua string de conexão local do SQL Server:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

   ⚠️ **Atenção:** essa conexão é recomendada **somente para ambiente de desenvolvimento**. Em produção, utilize autenticação segura com usuário/senha ou outros mecanismos apropriados (como conexões criptografadas e gerenciamento seguro de segredos).

3. **Restaure os pacotes NuGet:**

   ```bash
   dotnet restore
   ```

4. **Crie e atualize o banco de dados:**

   Execute os comandos abaixo para aplicar as migrations e criar o banco:

   ```bash
   dotnet ef database update
   ```

   Certifique-se de que o SQLServer foi baixado e o banco foi corretamente configurado.

5. **Rode a aplicação:**

   ```bash
   dotnet run
   ```

6. **Acesse a API via Swagger:**

   Com o sistema rodando, acesse no navegador:

   [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 🗃️ Considerações Sobre o Banco de Dados

- O sistema utiliza **SQL Server Management Studio 20** como sistema de gerenciamento de banco de dados.
- O **Entity Framework Core** é responsável pelo controle do *schema* e das *migrations*.

### Comandos úteis:

- **Criar uma nova migration:**

  ```bash
  dotnet ef migrations add NomeDaMigration
  ```

- **Recriar o banco completamente (útil em ambiente local):**

  ```bash
  dotnet ef database drop -f
  dotnet ef database update
  ```