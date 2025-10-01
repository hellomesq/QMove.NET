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

## Testes
1. De acordo com o fluxo, cadastre primeiro o setor, para organizar o pátio
 ```
POST /api/setores
{
  "nome": "Disponível"
}
   ```
2. Agora, cadastre a moto e em qual setor ela inicialmente estará
 ```
POST /api/motos
{
  "placa": "ABC-1234",
  "setorId": 1
}
   ```
3. Caso queira fazer movimentações, informe o setor antigo e o novo
 ```
POST /api/movimentacoes/movimentacoes?motoId={id}&novoSetorId={id}
 ```
4. Para acessar o aplicativo e gerenciar o pátio, tenha um usuário
 ```
POST /api/usuarios
{
  "nome": "João Silva",
  "email": "joao.silva@email.com",
  "senha": "senha123"
}
 ```
<img width="1783" height="342" alt="image" src="https://github.com/user-attachments/assets/8b16634e-cc0a-45ed-b0a8-250227338a04" />
<img width="1792" height="494" alt="image" src="https://github.com/user-attachments/assets/17e49d42-6209-47ec-a85e-ecde34cdc60e" />
<img width="1795" height="355" alt="image" src="https://github.com/user-attachments/assets/4b46919e-86d8-4360-bf48-fc97a4ee3a75" />
<img width="1794" height="408" alt="image" src="https://github.com/user-attachments/assets/8d1ffb2d-0c9d-4739-83d0-babb7b50c3a4" />



