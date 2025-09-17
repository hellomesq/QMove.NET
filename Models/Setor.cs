using System.ComponentModel.DataAnnotations;

namespace MotoMonitoramento.Models
{
    public class Setor
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public required string Nome { get; set; }

        public ICollection<Moto> Motos { get; set; } = new List<Moto>();
    }
}
