using System.ComponentModel.DataAnnotations;

namespace MotoMonitoramento.Models
{
    public class Moto
    {
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public required string Placa { get; set; }

        [Required, MaxLength(20)]
        public required string Status { get; set; }

        public int? SetorId { get; set; }
        public Setor? Setor { get; set; }
    }
}
