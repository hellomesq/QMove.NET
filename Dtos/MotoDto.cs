namespace MotoMonitoramento.Dtos
{
    public class MotoDto
    {
        public string Placa { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SetorNome { get; set; } = string.Empty; // usuário informa o nome do setor
    }

    public class MotoResponseDto
    {
        public int Id { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? SetorId { get; set; }
        public string? SetorNome { get; set; }
    }
}
