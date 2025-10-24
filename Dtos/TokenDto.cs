namespace MotoMonitoramento.DTOs
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }

    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }
}
