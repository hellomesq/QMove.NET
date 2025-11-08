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
<img width="1436" height="493" alt="image" src="https://github.com/user-attachments/assets/ff08f91a-b493-4e2a-8d09-61c56a59d4b7" />
<img width="1466" height="399" alt="image" src="https://github.com/user-attachments/assets/b2a9ac31-c040-4c15-aee5-f8b45a1c9f52" />
<img width="1429" height="266" alt="image" src="https://github.com/user-attachments/assets/3efa8970-ae27-4b36-9cfa-b8bc913d6628" />
<img width="1446" height="285" alt="image" src="https://github.com/user-attachments/assets/cb517a34-318a-4e6b-b6d0-105179d70a92" />
