using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotoMonitoramento.Models
{
    public class Moto
    {
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public required string Placa { get; set; }

        //FK paea setor
        public int? SetorId { get; set; }
        public Setor? Setor { get; set; }

        //Histórico de movimentações
        [JsonIgnore]
        public ICollection<Movimentacao> Movimentacoes { get; set; } = new List<Movimentacao>();
    }
}
