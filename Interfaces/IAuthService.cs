using DTO;

namespace Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> AuthAsync(LoginDto loginDto);
}