namespace MotoMonitoramento.Dtos
{
    public class MotoDto
    {
        public int Id { get; set; }
        public string Placa { get; set; } = null!;
        public string? SetorNome { get; set; }
    }

    public class MotoResponseDto
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public int? SetorId { get; set; }
        public string? SetorNome { get; set; }
    }
}
