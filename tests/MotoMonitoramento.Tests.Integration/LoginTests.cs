using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MotoMonitoramento.DTOs;
using Xunit;

namespace MotoMonitoramento.Tests.Integration
{
    public class LoginTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public LoginTests()
        {
            // Cria o client apontando para a raiz do projeto
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseContentRoot(Directory.GetCurrentDirectory());
            });

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_Deve_Retornar_Unauthorized_Quando_Credenciais_Invalidas()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "email@invalido.com", Senha = "senhaerrada" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/usuarios/login", loginDto);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
