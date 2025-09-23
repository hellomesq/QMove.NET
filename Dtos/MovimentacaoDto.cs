namespace MotoMonitoramento.Dtos
{
    public class MovimentacaoDto
    {
        public int Id { get; set; }
        public string MotoPlaca { get; set; } = null!;
        public string SetorAntigo { get; set; } = null!;
        public string SetorNovo { get; set; } = null!;
        public DateTime DataHora { get; set; }
    }
}
