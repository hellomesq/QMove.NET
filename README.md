# QMOVE

API RESTful para gerenciamento de motos em pátios, com funcionalidades completas de CRUD, utilizando ASP.NET Core, banco de dados Oracle via Entity Framework Core, e documentação automática com Swagger.

## Integrantes

- Hellen Marinho Cordeiro RM 558841
- Heloisa Alves de Mesquita RM 559145

## Justificativa da Arquitetura 

A Mottu enfrenta atualmente dificuldades para localizar rapidamente as motos em seu pátio, impactando a eficiência operacional. Para resolver esse problema, desenvolvemos uma API que possibilita:

- Cadastro de motos presentes no pátio;

- Gerenciamento de setores para organização física do espaço;

- Registro das movimentações por meio da leitura de QR Codes.

Com essa solução, é possível rastrear o percurso de cada moto dentro do pátio, garantindo maior controle e agilidade nas operações. 

## Como rodar a API

1. Clone o repositório:
   ```bash
   git clone https://github.com/hellomesq/QMove.NET
   cd QMove.NET
   ```

2. Configure a connection string do Oracle no arquivo `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "OracleConnection": "User Id=seu_usuario;Password=sua_senha;Data Source=seu_host:porta/seu_servico"
   }
   ```

3. Crie o banco de dados e aplique as migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. Execute a aplicação:
   ```bash
   dotnet run
   ```

5. Acesse a documentação Swagger para explorar e testar as rotas:
   ```
   http://localhost/swagger/index.html
   ```
