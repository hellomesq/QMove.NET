using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotoMonitoramento.Models
{
    public class Setor
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public required string Nome { get; set; }

        [JsonIgnore]
        public ICollection<Moto> Motos { get; set; } = new List<Moto>();
    }
}
