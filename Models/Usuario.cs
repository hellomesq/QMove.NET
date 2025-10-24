using System.ComponentModel.DataAnnotations;

namespace MotoMonitoramento.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public required string Nome { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Senha { get; set; }

        // Refresh Token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
