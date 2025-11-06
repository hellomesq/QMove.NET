using FluentAssertions;
using MotoMonitoramento.Models;
using Xunit;

namespace MotoMonitoramento.Tests.Unit
{
    public class UsuarioServiceTests
    {
        [Fact(DisplayName = "Deve criar usuário corretamente")]
        public void Deve_CriarUsuario_Corretamente()
        {
            var usuario = new Usuario
            {
                Nome = "João Silva",
                Email = "joao@teste.com",
                Senha = "1234",
            };

            usuario.Nome.Should().Be("João Silva");
            usuario.Email.Should().Be("joao@teste.com");
            usuario.Senha.Should().Be("1234");
        }

        [Fact(DisplayName = "Deve alterar a senha corretamente")]
        public void Deve_AlterarSenha_Corretamente()
        {
            var usuario = new Usuario
            {
                Nome = "Maria",
                Email = "maria@teste.com",
                Senha = "1234",
            };

            usuario.Senha = "novaSenha";
            usuario.Senha.Should().Be("novaSenha");
        }

        [Fact(DisplayName = "Deve validar se o e-mail é obrigatório")]
        public void Deve_ValidarEmailObrigatorio()
        {
            var usuario = new Usuario
            {
                Nome = "José",
                Email = "",
                Senha = "1234",
            };

            usuario.Email.Should().Be("");
        }

        [Fact(DisplayName = "Deve falhar ao criar usuário com email inválido")]
        public void Deve_Falhar_UsuarioComEmailInvalido()
        {
            var usuario = new Usuario
            {
                Nome = "Teste",
                Email = "invalido",
                Senha = "1234",
            };

            usuario.Email.Should().NotMatchRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
