using System.ComponentModel.DataAnnotations;

namespace MotoMonitoramento.Models
{
    public class Movimentacao
    {
        public int Id { get; set; }

        // FK da moto
        public int MotoId { get; set; }
        public Moto Moto { get; set; } = null!;

        // Setor anterior
        public int SetorAntigoId { get; set; }
        public Setor SetorAntigo { get; set; } = null!;

        // Setor novo
        public int SetorNovoId { get; set; }
        public Setor SetorNovo { get; set; } = null!;

        // Data e hora da movimentação
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
    }
}
