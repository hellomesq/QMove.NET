namespace MotoMonitoramento.DTOs
{
    public class SetorDto
    {
        public string Nome { get; set; } = string.Empty;
    }

    public class SetorResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}
