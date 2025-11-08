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

## Como rodar os testes
A solução possui dois tipos de testes:
- Unitários: testam classes e métodos isoladamente, sem depender de banco de dados ou servidor HTTP.
- Integração: simulam chamadas HTTP para a API usando WebApplicationFactory, testando fluxo completo.

1. Dentro do terminal, execute na raiz do projeto para rodar todos os testes
   ```bash
   dotnet test
   ```
2. Para rodar apenas os testes unitários
   ```bash
   dotnet test tests/MotoMonitoramento.Tests.Unit
   ```
3. Para rodar apenas os testes de integração
   ```bash
   dotnet test tests/MotoMonitoramento.Tests.Integration
   ```
Exemplos de testes implementados:
- LoginTests: login inválido retorna 401 Unauthorized
- UsuarioServiceTests: valida criação de usuário, alteração de senha, validação de e-mail e falha para email inválido

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

## Fluxo de uso 

#### Login
Para operações autenticadas, faça login para obter o token JWT:
```
POST /api/v{version}/Usuarios/login
{
  "email": "usuario@email.com",
  "senha": "senha123"
}
```
- Resposta: token JWT, usado no header Authorization: Bearer {token}

#### Cadastro de Usuário
Para gerenciar o pátio, tenha um usuário
```
POST /api/v{version}/Usuarios
{
  "nome": "João Silva",
  "email": "joao.silva@email.com",
  "senha": "senha123"
}
```

#### Cadastro de Setor
De acordo com o fluxo, cadastre primeiro o setor, para organizar o pátio
```
POST /api/v{version}/Setores
{
  "nome": "Setor A"
}
```

#### Cadastro de Motocicleta
Cadastre a moto e em qual setor ela inicialmente estará
```
POST /api/v{version}/Motos
{
  "placa": "ABC-1234",
  "setorId": 1
}
```

#### Consulta de Movimentações 
Caso queira fazer movimentações, informe o setor antigo e o novo
```
GET /api/v{version}/Movimentacoes/por-moto/1
```

#### Teste de ping
```
GET /api/v{version}/Teste/ping
```
- Resposta: API QMove funcionando!
  
## Endpoints 
<img width="488" height="481" alt="image" src="https://github.com/user-attachments/assets/8cadc374-febe-4bc9-8b5a-23496307a6df" />
<img width="455" height="261" alt="image" src="https://github.com/user-attachments/assets/be0d4dca-c728-4f6f-9c55-45c223323c2a" />
<img width="519" height="377" alt="image" src="https://github.com/user-attachments/assets/9d0b7457-a002-4453-b70e-0e1591c7eaca" />
<img width="703" height="272" alt="image" src="https://github.com/user-attachments/assets/f38da993-49f4-4177-9a49-15a841cc4a6a" />


