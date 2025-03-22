namespace DTO;

public class AuthResultDto
{
    public bool IsAuthenticated { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
}